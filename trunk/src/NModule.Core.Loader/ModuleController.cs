/**************************************************************************
 * Copyright (c) 2005 Michael Tindal and the individuals listed           *
 * on the ChangeLog entries.                                              *
 *                                                                        *
 * Permission is hereby granted, free of charge, to any person obtaining  *
 * a copy of this software and associated documentation files (the        *
 * "Software"), to deal in the Software without restriction, including    *
 * without limitation the rights to use, copy, modify, merge, publish,    *
 * distribute, sublicense, and/or sell copies of the Software, and to     *
 * permit persons to whom the Software is furnished to do so, subject to  *
 * the following conditions                                               *
 *                                                                        *
 * The above copyright notice and this permission notice shall be         *
 * included in all copies or substantial portions of the Software.        *
 *                                                                        *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        *
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     *
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND                  *
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE *
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION *
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION  *
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.        *
 **************************************************************************/
 
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NModule.Dependency.Resolver;

namespace NModule.Core.Loader {
	// This class is the heart and sole of the NModule architecture.
	// It manages dependencies, keeps ref counts on appdomains,
	// and so on.
	public class ModuleController {
#region Members
		// AppDomain -> assembly maps.
		protected Hashtable _app_domain_map;

		// Reference Counts
		protected Hashtable _ref_counts;

		// Module Search Path
		protected ArrayList _search_path;

		// Recognized Roles
		protected ArrayList _roles;
		
		// Information Map
		protected Hashtable _info_map;
		
		// Dependency Resolver
		protected DepResolver _resolver;
		
		// Module Loader
		protected ModuleLoader _loader;
#endregion
	
		public ModuleController () {
			_app_domain_map = new Hashtable ();
			_ref_counts = new Hashtable ();
			_search_path = new ArrayList ();
			_roles = new ArrayList ();
			_resolver = new DepResolver (this, _search_path);
			_loader = new ModuleLoader (_search_path, _resolver);
			_info_map = new Hashtable ();
		}

#region Loading/Unloading
		public void LoadModule (string _name) {
			LoadModule (null, _name);
		}
		
		public void LoadModule (ArrayList _parents, string _name) {
			ModuleInfo _info;
			
			AppDomain _domain = _loader.LoadModule (_parents, _name, out _info);
			
			// set up the map
			_app_domain_map.Add (_name, _domain);
			
			// increment the reference count on the domain.
			IncRef (_domain);
			
			// increment the reference count for all the dependencies recursively (i.e.
			// if module A depends on B which depends on C, B gets inc ref'd once, while C
			// gets inc ref'd twice, for both A and B).
			_resolver.IncRefs (_info);
			
			// Set up roles.
			CallRoleHandlers (_info);
			
			// Set up info map.
			_info_map.Add (_name, _info);
			
			// Entry handlers
			CallEntryHandler (_domain.GetAssemblies()[0]);
		}
		
		public void UnloadModule (string _name) {
			// This is fun stuff.  We can't unload a module any of the following conditions fail:
			//  1) The module must be a top-level node in the dep map, i.e no other modules can
			//  be depending on it.
			//  2) The reference count on the appdomain must be 1, which means the only thing
			//  using this appdomain is the module inside of it.
			
			if (!_domain_map.Contains (_name))
				return; // suckers not loaded, why are we unloading it?
			
			ModuleInfo _info = (ModuleInfo)_info_map[_name];
			
			AppDomain _domain = (AppDomain)_domain_map[_name];
			if (((int)_ref_counts[_domain]) > 1) {
				throw new DomainStillReferencedException (string.Format ("The domain holding the module {0} cannot be unloaded because it is still being referenced.", _name));
			}
			
			// okay, everything's good.  This will remove the domain from the reference list since its reference count is now 0.
			DecRef (_domain);
			
			// okay, lets remove the domain map association
			_domain_map.Remove (_name);
			
			// the info map needs to go too
			_info_map.Remove (_name);
			
			// Let people know theyre module has been unloaded.
			CallRoleUnregisterHandlers (_info);
			
			// Exit handlers
			CallExitHandler (_domain.GetAssemblies()[0]);
			
			// And finally, unload the domain.
			_domain.Unload ();
		}
#endregion

#region Domain Reference Counts
		protected void IncRef (AppDomain _domain) {
			if (!_ref_counts.Contains (_domain)) {
				_ref_counts.Add (_domain, 1);
			}
			
			_ref_counts[_domain] = ((int)_ref_counts[_domain]) + 1;
		}
		
		protected void DecRef (AppDomain _domain) {
			if (!_ref_counts.Contains (_domain)) {
				return;
				
			_ref_counts[_domain] = ((int)_ref_counts[_domain]) - 1;
			
			if (((int)_ref_counts[_domain]) == 0) {
				_ref_counts.Remove (_domain); // no references, this suckers getting unloaded.
			}
		}
#endregion

#region Role registration
		public void RegisterNewRole (string name, Type type, RoleRegisterHandler reg, RoleUnregisterHandler unreg) {
			ModuleRole _role = new ModuleRole (name, type, reg, unreg);
			
			_roles.Add (_role);
		}
#endregion

#region Role Handlers
		public void CallRoleHandlers (ModuleInfo _info) {
			foreach (ModuleRoleAttribute _attr in _info.ModuleRoleAttributes) {
				string _myRole = _attr.Role;
				
				foreach (ModuleRole _role in _roles) {
					if (_role.Name == _myRole) {
						Assembly _asm = _info.Owner;
						
						Type _type = null;
						
						foreach (Type __type in _asm.GetTypes ()) {
							if (__type.IsSubclassOf (_role.BaseType)) {
								_type = __type;
								break;
							}
						}
						
						if (_type == null) {
							continue; // don't have a type for this role.
						}
						
						_role.RegistrationHandler (_asm, _type);
					}
				}
			}
		}
		
		public void CallRoleUnregisterHandlers (ModuleInfo _info) {
			foreach (ModuleRoleAttribute _attr in _info.ModuleRoleAttributes) {
				string _myRole = _attr.Role;
				
				foreach (ModuleRole _role in _roles) {
					if (_role.Name == _myRole) {
						Assembly _asm = _info.Owner;
						
						Type _type = null;
						
						foreach (Type __type in _asm.GetTypes ()) {
							if (__type.IsSubclassOf (_role.BaseType)) {
								_type = __type;
								break;
							}
						}
						
						if (_type == null) {
							continue; // don't have a type for this role.
						}
						
						_role.UnregistrationHandler (_asm, _type);
					}
				}
			}
		}
#endregion 
	}
}		