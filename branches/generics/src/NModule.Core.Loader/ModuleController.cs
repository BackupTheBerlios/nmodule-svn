//
// ModuleController.cs
//
// Author:
//     Michael Tindal <urilith@gentoo.org>
//
// Copyright (C) 2005 Michael Tindal and the individuals listed on
// the ChangeLog entries.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
 
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NModule.Dependency.Core;
using NModule.Dependency.Resolver;
using NModule.Core.Module;

namespace NModule.Core.Loader {
	/// <summary>
	/// A ModuleController object is the bridge between an application
	/// and its modules.  The controller manages the loading of modules, registration
	/// of roles, and other behind the scenes information needed for the operation of
	/// the module engine.
	/// </summary>
	/// <remarks>None.</remarks>
	/// <preliminary/>
	public class ModuleController {
#region Members
		/// <summary>
		/// This maintains a map of module names to their
		/// <see cref="System.AppDomain">AppDomain</see>.
		/// </summary>
		/// <remarks>None.</remarks>
		protected Hashtable _app_domain_map;

		/// <summary>
		/// Maintains a map of AppDomains to their
		/// reference counts.
		/// </summary>
		/// <remarks>None.</remarks>
		protected Hashtable _ref_counts;
		
		/// <summary>
		/// The list of directories used to search for
		/// modules.
		/// </summary>
		/// <remarks>None.</remarks>
		protected ArrayList _search_path;

		/// <summary>
		/// A list of registered roles.
		/// </summary>
		/// <remarks>None.</remarks>
		protected ArrayList _roles;
		
		/// <summary>
		/// Maintains a map of module names
		/// to their <see cref="ModuleInfo" />
		/// object.
		/// </summary>
		/// <remarks>None.</remarks>
		protected Hashtable _info_map;
		
		/// <summary>
		/// The loader object used to load
		/// modules.
		/// </summary>
		/// <remarks>None.</remarks>
		protected ModuleLoader _loader;
#endregion
	
		/// <summary>
		/// Creates a new ModuleController
		/// object and initializes the members.
		/// </summary>
		/// <remarks>None.</remarks>
		public ModuleController () {
			_app_domain_map = new Hashtable ();
			_ref_counts = new Hashtable ();
			_search_path = new ArrayList ();
			_roles = new ArrayList ();
			_loader = new ModuleLoader (_search_path, this);
			_info_map = new Hashtable ();
		}

#region Loading/Unloading
		/// <summary>
		/// Loads a module with the given name.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_name">The name of the module minus the .dll extension.</param>
		public void LoadModule (string _name) {
			LoadModule (null, _name);
		}
	
		/// <summary>
		/// Loads a module with the given name and parents.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_parents">The parent modules for the loaded module.</param>
		/// <param name="_name">The name of the module to load minus the .dll extension.</param>
		public void LoadModule (ArrayList _parents, string _name) {
			if (_name == null)
			{
				
			}
			
			if (_app_domain_map.ContainsKey (_name))
			{
				IncRef ((AppDomain)_app_domain_map[_name]);
				return; // Already loaded, no need to load it again.
			}
			
			ModuleInfo _info;
			
			AppDomain _domain = _loader.LoadModule (_parents, _name, out _info, false, true);
			
			// set up the map
			_app_domain_map.Add (_name, _domain);
			
			// increment the reference count on the domain.
			IncRef (_domain);
			
			// increment the reference count for all the dependencies recursively (i.e.
			// if module A depends on B which depends on C, B gets inc ref'd once, while C
			// gets inc ref'd twice, for both A and B).
			
			// Set up roles.
			CallRoleHandlers (_info);
			
			// Set up info map.
			_info_map.Add (_name, _info);
			
			// Entry handlers
			CallEntryHandler (_domain.GetAssemblies()[0]);
		}
		
		/// <summary>
		/// Checks to see whether a module is loaded or not.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_name">The name of the module to check.</param>
		/// <returns>Returns true if the module is loaded, false otherwise.</returns>
		public bool IsLoaded (string _name) {
			return _app_domain_map.ContainsKey (_name);
		}
		
		/// <summary>
		/// Recursively decrements reference counts on modules
		/// listed in the given dependency tree.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_x">The root of the tree to recurse.</param>
		protected void DecRefs (DepNode _x) {
			if (_x == null)
				return;

			foreach (DepNode _d in _x.Children) {
				DecRefs (_d);
			}
			DecRef ((AppDomain)_app_domain_map[_x.Constraint.Name]);
		}
		
		/// <summary>
		/// Unloads a module.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_name">The name of the module to unload.</param>
		/// <exception cref="DomainStillReferencedException">Thrown when the domain holding the module is still referenced.</exception>
		public void UnloadModule (string _name) {
			// This is fun stuff.  We can't unload a module any of the following conditions fail:
			//  1) The module must be a top-level node in the dep map, i.e no other modules can
			//  be depending on it.
			//  2) The reference count on the appdomain must be 1, which means the only thing
			//  using this appdomain is the module inside of it.
			
			if (!_app_domain_map.ContainsKey (_name))
				return; // suckers not loaded, why are we unloading it?
			
			ModuleInfo _info = (ModuleInfo)_info_map[_name];
			
			AppDomain _domain = (AppDomain)_app_domain_map[_name];
			

			if (((int)_ref_counts[_domain]) > 1) {
				throw new DomainStillReferencedException (string.Format ("The domain holding the module {0} cannot be unloaded because it is still being referenced.", _name));
			}
			
			// okay, everything's good.  This will remove the domain from the reference list since its reference count is now 0.
			DecRef (_domain);
			
			DepNode _root = _info.Dependencies;
						
			DecRefs (_root);
			
			// okay, lets remove the domain map association
			_app_domain_map.Remove (_name);
			
			// the info map needs to go too
			_info_map.Remove (_name);
			
			// Let people know theyre module has been unloaded.
			CallRoleUnregisterHandlers (_info);
			
			// Exit handlers
			CallExitHandler (_domain.GetAssemblies()[0]);
			
			// And finally, unload the domain.
			AppDomain.Unload (_domain);
		}
#endregion

#region Domain Reference Counts
		/// <summary>
		/// Increments the reference count on a given module.
		/// </summary>
		/// <remarks>
		/// See <see href="refcount.html">Reference Counts</see> for an explanation of reference counts.
		/// </remarks>
		/// <param name="_name">The name of the module to incremement the reference count on.</param>
		public void IncRef (string _name) {			
			IncRef ((AppDomain)_app_domain_map[_name]);
		}
		
		/// <summary>
		/// Increments the reference count on a given domain.
		/// </summary>
		/// <remarks>
		/// See <see href="refcount.html">Reference Counts</see> for an explanation of reference counts.
		/// </remarks>
		/// <param name="_domain">The domain to increment the reference count on.</param>
		protected void IncRef (AppDomain _domain) {
			if (!_ref_counts.Contains (_domain)) {
				_ref_counts.Add (_domain, 0);
			}
			
			_ref_counts[_domain] = ((int)_ref_counts[_domain]) + 1;
		}

		/// <summary>
		/// Decrements the reference count on a given module.
		/// </summary>
		/// <remarks>
		/// See <see href="refcount.html">Reference Counts</see> for an explanation of reference counts.
		/// </remarks>
		/// <param name="_name">The name of the module to decremement the reference count on.</param>
		public void DecRef (string _name) {
			DecRef ((AppDomain)_app_domain_map[_name]);
		}
		
		/// <summary>
		/// Decrements the reference count on a given domain.
		/// </summary>
		/// <remarks>
		/// See <see href="refcount.html">Reference Counts</see> for an explanation of reference counts.
		/// </remarks>
		/// <param name="_domain">The domain to decrement the reference count on.</param>
		protected void DecRef (AppDomain _domain) {
			if (!_ref_counts.Contains (_domain))
				return;
				
			_ref_counts[_domain] = ((int)_ref_counts[_domain]) - 1;
			
			if (((int)_ref_counts[_domain]) == 0) {
				_ref_counts.Remove (_domain); // no references, this suckers getting unloaded.
			}
		}
#endregion

#region Role registration
		/// <summary>
		/// Registers a new role.
		/// </summary>
		/// <remarks>
		/// Registering a new role will go through already loaded modules to see if any of them
		/// fulfill the new role.  See <see href="roles.html">Roles</see> for an explanation of module roles.
		/// </remarks>
		/// <param name="_name">The name of the new role.</param>
		/// <param name="_type">The base type of the new role.</param>
		/// <param name="_reg">The registration handler for the new role.</param>
		/// <param name="_unreg">The unregistration handler for the new role.</param>
		public void RegisterNewRole (string _name, Type _type, RoleRegisterHandler _reg, RoleUnregisterHandler _unreg) {
			ModuleRole _role = new ModuleRole (_name, _type, _reg, _unreg);
			
			_roles.Add (_role);

			foreach (string _key in _info_map.Keys) {
				CallRoleHandlers ((ModuleInfo)_info_map[_key], _role);
			}
		}

		/// <summary>
		/// Unregisters a role.
		/// </summary>
		/// <remarks>
		/// See <see href="roles.html">Roles</see> for an explanation of module roles.
		/// </remarks>
		/// <param name="_name">The name of the role to unregister.</param>
		public void UnregisterRole (string _name) {
			ModuleRole _role = null;
			foreach (ModuleRole _r in _roles) {
				if (_r.RoleName == _name)
					_role = _r;
			}

			if (_role == null)
				return;
			
			_roles.Remove (_role);
			foreach (string _key in _info_map.Keys) {
				CallRoleUnregisterHandlers ((ModuleInfo)_info_map[_key], _role);
			}
		}
#endregion

#region Role Handlers
		/// <summary>
		/// Calls all of the role registration handlers for the given
		/// <see cref="ModuleInfo" /> object.
		/// </summary>
		/// <remarks>
		/// See <see href="roles.html">Roles</see> for an explanation of module roles.
		/// </remarks>
		/// <param name="_info">The <see cref="ModuleInfo"/> object to determine which role handlers to call.</param>
		protected void CallRoleHandlers (ModuleInfo _info) {
			foreach (ModuleRole _role in _roles) {
				CallRoleHandlers (_info, _role);
			}
		}
		
		/// <summary>
		/// Calls the role registration handler for the given role if the
		/// given <see cref="ModuleInfo" /> object fulfills the role.
		/// </summary>
		/// <remarks>
		/// See <see href="roles.html">Roles</see> for an explanation of module roles.
		/// </remarks>
		/// <param name="_info">The <see cref="ModuleInfo"/> object to determine if the role handler should be called.</param>
		/// <param name="_role">The role to fulfill.</param>
		protected void CallRoleHandlers (ModuleInfo _info, ModuleRole _role) {
			if (_info.Roles == null)
				return;
			
			foreach (string _myRole in _info.Roles.Split(',')) {
				if (_role.RoleName == _myRole) {
					Assembly _asm = _info.Owner;
						
					Type _type = null;
						
					foreach (Type __type in _asm.GetTypes ()) {
							
						if (_role.BaseType.IsClass) {
							if (__type.IsSubclassOf (_role.BaseType)) {
								_type = __type;
								break;
							}
						} else if (_role.BaseType.IsInterface) {
							if (__type.GetInterface (_role.BaseType.ToString ()) != null) {
								_type = __type;
								break;
							}
						}
					}
						
					if (_type == null) {
						continue; // don't have a type for this role.
					}
						
					_role.RegistrationHandler (_asm, _type);
				}
			}
		}
		
		/// <summary>
		/// Calls all of the role unregistration handlers for the given
		/// <see cref="ModuleInfo" /> object.
		/// </summary>
		/// <remarks>
		/// See <see href="roles.html">Roles</see> for an explanation of module roles.
		/// </remarks>
		/// <param name="_info">The <see cref="ModuleInfo"/> object to determine which role handlers to call.</param>
		protected void CallRoleUnregisterHandlers (ModuleInfo _info) {
			foreach (ModuleRole _role in _roles) {
				CallRoleUnregisterHandlers (_info, _role);
			}
		}
		
		/// <summary>
		/// Calls the role unregistration handler for the given role if the
		/// given <see cref="ModuleInfo" /> object fulfills the role.
		/// </summary>
		/// <remarks>
		/// See <see href="roles.html">Roles</see> for an explanation of module roles.
		/// </remarks>
		/// <param name="_info">The <see cref="ModuleInfo"/> object to determine if the role handler should be called.</param>
		/// <param name="_role">The role to fulfill.</param>
		protected void CallRoleUnregisterHandlers (ModuleInfo _info, ModuleRole _role) {
			if (_info.Roles != null)
				return;

			foreach (string _myRole in _info.Roles.Split(',')) {
				if (_role.RoleName == _myRole) {
					Assembly _asm = _info.Owner;
					
					Type _type = null;
						
					foreach (Type __type in _asm.GetTypes ()) {
						if (_role.BaseType.IsClass) {
							if (__type.IsSubclassOf (_role.BaseType)) {
								_type = __type;
								break;
							}
						} else if (_role.BaseType.IsInterface) {
							if (__type.GetInterface (_role.BaseType.ToString ()) != null) {
								_type = __type;
								break;
							}
						}
					}
						
					if (_type == null) {
						continue; // don't have a type for this role.
					}
						
					_role.UnregistrationHandler (_asm);
				}
			}
		}
#endregion

#region Entry/Exit Handlers
		/// <summary>
		/// Calls the modules entry handler.
		/// </summary>
		/// <remarks>
		/// See <see href="exhandler.html">Entry/Exit Handlers</see> for an explanation of handlers.
		/// </remarks>
		/// <param name="_asm">The assembly to call the entry handler for.</param>
		protected void CallEntryHandler (Assembly _asm) {
			foreach (Type _type in _asm.GetTypes ()) {
				if (_type.GetInterface (typeof (NModule.Core.IModule).ToString()) != null) {
					MethodInfo _method = _type.GetMethod ("ModuleEntry");
		
					if (_method != null)			
						_method.Invoke (null, BindingFlags.Static | BindingFlags.Public, null, (new object[] { this }), null);
				}
			}
		}
		
		/// <summary>
		/// Calls the modules exit handler.
		/// </summary>
		/// <remarks>
		/// See <see href="exhandler.html">Entry/Exit Handlers</see> for an explanation of handlers.
		/// </remarks>
		/// <param name="_asm">The assembly to call the exit handler for.</param>
		protected void CallExitHandler (Assembly _asm) {
			foreach (Type _type in _asm.GetTypes ()) {
				if (_type.GetInterface (typeof (NModule.Core.IModule).ToString()) != null) {
					MethodInfo _method = _type.GetMethod ("ModuleExit");
					
					if (_method != null)
						_method.Invoke (null, BindingFlags.Static | BindingFlags.Public, null, (new object[] { this }), null);
				}
			}
		}
#endregion

#region Properties
		/// <summary>
		/// Gets or sets the search path for this controller.
		/// </summary>
		/// <remarks>
		/// See <see href="searching.html">this</see> for information on search paths.
		/// </remarks>
		public ArrayList SearchPath {
			get {
				return _search_path;
			}
			set {
				_search_path = value;
			}
		}
		
		/// <summary>
		/// Gets the module loader used by this controller.
		/// </summary>
		/// <remarks>None.</remarks>
		public ModuleLoader Loader {
			get {
				return _loader;
			}
		}
		
		/// <summary>
		/// Gets the reference count on the given module.
		/// </summary>
		/// <remarks>
		/// See <see href="refcount.html">Reference Counts</see> for an explanation of reference counts.
		/// </remarks>
		/// <param name="_name">The name of the module to check.</param>
		/// <returns>The reference count on the given module.</returns>
		public int RefCount (string _name) {
			if (!_app_domain_map.Contains (_name))
				return -1;
		
			return ((int)_ref_counts[(AppDomain)_app_domain_map[_name]]);
		}
#endregion				
	}
}		
