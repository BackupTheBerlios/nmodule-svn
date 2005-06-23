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
using C5;
using NModule.Dependency.Resolver;

namespace NModule.Core.Loader {
	// This class is the heart and sole of the NModule architecture.
	// It manages dependencies, keeps ref counts on appdomains,
	// and so on.
	public class ModuleController {
		/***
		 * We need to keep several bits of information about each module to ensure
		 * that dependencies are resolved, and we're not unloaded needed appdomains.
		 * 1)  We keep a map of assemblies and the roles theyve registered.  This is extra
		 *     information since the handler is used.
		 * 2)  We keep a map of assemblies belonging to each AppDomain.  If assemblies are
		 *     loaded as group, then each AppDomain can contain more than one assembly.  This
		 *     information isn't used if we do 1-to-1 mappings.
		 * 3)  We keep a refcount on each appdomain.  Every time an appdomain (actually an assembly
		 *     within that appdomain) is referenced by another module by a dependency or by using
		 *     the IncRef call from the module, we increment the refcount.  Whenever the depending
		 *     module is unloaded, or DecRef is called, we decrement the refcount.  The appdomain
		 *     can only be unloaded when its refcount is 0.
		 ***/

		// AppDomain -> assembly maps.
		protected HashDictionary<string, AppDomain> _app_domain_map;

		// Reference Counts
		protected HashDictionary<AppDomain,int> _ref_counts;

		// Module Search Path
		protected ArrayList<string> _search_path;

		// Recognized Roles
		protected ArrayList<ModuleRole> _roles;
		
		// Dependency Resolver
		protected DepResolver _resolver;
		
		public ModuleController () {
			_app_domain_map = new HashDictionary<AppDomain,ArrayList<Assembly>> ();
			_ref_counts = new HashDictionary<AppDomain,int> ();
			_search_path = new ArrayList<string> ();
			_roles = new ArrayList<ModuleRole> ();
			_resolver = new DepResolver (this, _search_path);
		}

		// -->> Loading/Unloading <<--

		protected string SearchForModule (string _name) {
			foreach (string s in _search_path) {
				if (Directory.Exists (s)) {
					foreach (string f in Directory.GetFiles (s, "*.dll")) {
						if (f.SubString (0, f.Length - 4) == _name) {
							return s + "/" + f;
						}
					}
				}
			}
			
			return null;
		}
		
		// Loads the content of a file to a byte array. 
		protected byte[] LoadRawFile (string _filename) {
			FileStream _fs = new FileStream (_filename, FileMode.Open);
			byte[] _buffer = new byte [(int) _fs.Length];
			fs.Read (_buffer, 0, _buffer.Length);
			fs.Close ();
   
			return _buffer;
		}
		   
		/*
		 * We provide two method signatures for loading for convienence.  The first just takes the name of the module (minus the .dll extension),
		 * and attempts to load it.  The second takes a list of parents and the name.  The first incidentally just calls the second with an empty
		 * parents list.  The parents list is used for detecting circular dependencies.
		 */
		public void LoadModule (string _name) {
			LoadModule (null, _name);
		}
		
		public void LoadModule (ArrayList<string> _parents, string _name, bool checking=false) {
			// Okay, this is tricky.  First, we have to load the module into a temp domain
			// to retrieve its module info.  Then, we have to attempt to resolve the dependencies.
			// This is going to be fun.  Heh.
			
			if (_app_domain_map.HasKey (_name))
				return; // Already loaded, no need to load it again.
				
			if (_parents == null)
				_parents = new ArrayList<string> ();
				
			// This is technically a parent of any depending module.
			_parents.Add (_name);
			
			// Try to find the module on the search path.
			string _filename = SearchForModule (_name);
			
			if (_filename == null)
				throw new ModuleNotFoundException (string.Format ("The module {0} was not found along the module search path.", _name));
				
			// Okay, well, now we know the module exists at least in the file (we hope its a proper dll, but we'll see :).  Now we
			// need to create the temporary AppDomain and load it to get the info from it.
			AppDomain _tempDomain = AppDomain.Create ("_temp_" + _name);
			
			byte[] _raw_bytes = LoadRawFile (_filename);
			
			// The throw here is mostly used from dep resolver calls, although it should also be caught by the immediate caller
			// (i.e. the application).
			try {
				_tempDomain.Load (_raw_bytes);
			} catch (BadImageFormatException e) {
				throw ModuleImageException (e.Message);
			}
			
			// Okay, now lets grab the module info from the assembly attributes.
			Assembly _asm = _tempDomain.GetAssemblies ()[0];
			
			try {
				ModuleInfo _info = new ModuleInfo (_asm);
			} catch (ModuleInfoException e) {
				throw InvalidModuleException (e.Message);
			}
			
			// unload the temp domain since its unneeded now.
			
			_tempDomain.Unload ();
			
			// okay, now we've got the info, let's do some magic with the dependencies.
			// this will recursively load all of the appropriate assemblies as per the parsed
			// depedency tree.  It will take into account dependency operators, such as AND, OR
			// OPT (optional).  Very intelligent stuff.  Of course, if there are no depends,
			// this just simply returns.  This will of course continue updating the parents as needed
			// since each time a new module is loaded, the resolver is recursively called until
			// a module is found.  This is cool.  What this will do is call this method with
			// checking=true, which will cause it to just return if the module suceeds.  This way
			// we can ensure we don't load unneeded module Z that is a dependency of X which depends
			// on Y, because if Z suceeds but Y fails, we don't want X, Y, or Z to fail.  This way,
			// we can ensure the entire tree can be loaded first (this does take into account already
			// loaded assemblies).
			_resolver.ResolveCheck (_parents, _info);
		
			if (checking)
				return;
							
			// okay, they're good, lets load the suckers.
			_resolver.Resolve (_parents, _info);
			
			// alright, we've got them all loaded, they exist in the assembly map.
			// now we create the *real* app domain.
			AppDomain _domain = AppDomain.Create (_name);
			
			// let's load this assembly into the real app domain.
			_domain.Load (_raw_bytes);
			
			// set up the map
			_app_domain_map.Add (_name, _domain);
			
			// increment the reference count on the domain.
			IncRef (_domain);
			
			// increment the reference count for all the dependencies recursively (i.e.
			// if module A depends on B which depends on C, B gets inc ref'd once, while C
			// gets inc ref'd twice).
			_resolver.IncRefs (_info);
			
			// FIXME:  Add role handling here.
		}
	}
}		
		