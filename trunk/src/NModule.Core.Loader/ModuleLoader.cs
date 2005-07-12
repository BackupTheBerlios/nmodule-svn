//
// ModuleLoader.cs
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
using System.Runtime.InteropServices;
using System.Security.Policy;
using NModule.Dependency.Resolver;
using NModule.Core.Module;

namespace NModule.Core.Loader {
	// This class is simply the loader class, it just creates a new AppDomain,
	// and loads the appropriate assembly into it.  It can also load an assembly into
	// an existing app-domain (for example, grouped dependencies).  See the configuration
	// options for an example.
	public class ModuleLoader {

		protected static ArrayList loaded_modules;
		
		protected ArrayList _search_path;
		
		protected ModuleController _controller;
		
		public ModuleLoader (ArrayList search_path, ModuleController controller) {
			_search_path = search_path;
			_controller = controller;
		}
		
		// Loads the content of a file to a byte array. 
		protected byte[] LoadRawFile (string _filename) {
			FileStream _fs = new FileStream (_filename, FileMode.Open);
			byte[] _buffer = new byte [(int) _fs.Length];
			_fs.Read (_buffer, 0, _buffer.Length);
			_fs.Close ();
   
			return _buffer;
		}
		  
		public string SearchForModule (string _name) {
			foreach (string s in _search_path) {
				if (Directory.Exists (s)) {
					foreach (string f in Directory.GetFiles (s, "*.dll")) {
						string _f = f.Replace (s, "").Replace ("/", "");
						if (_f.Substring (0, _f.Length - 4) == _name) {
							return s + "/" + _f;
						}
					}
				}
			}
			
			return null;
		}
		
		/*
		 * We provide two method signatures for loading for convienence.  The first just takes the name of the module (minus the .dll extension),
		 * and attempts to load it.  The second takes a list of parents and the name.  The first incidentally just calls the second with an empty
		 * parents list.  The parents list is used for detecting circular dependencies.
		 */
		public AppDomain LoadModule (string _name, out ModuleInfo _info) {
			return LoadModule (null, _name, out _info, false);
		}
		
		public AppDomain LoadModule (ArrayList _parents, string _name, out ModuleInfo _info, bool checking) {
			return LoadModule (_parents, _name, out _info, checking, true);
		}
		
		public Assembly GetAssembly (AppDomain _domain, string _name) {
			foreach (Assembly _asm in _domain.GetAssemblies ()) {
				
				if (_asm.GetName ().Name == _name)
					return _asm;
			}
			return null;
		}

		public AppDomain LoadModule (ArrayList _parents, string _name, out ModuleInfo _info, bool checking, bool depcheck) {
			

			// Okay, this is tricky.  First, we have to load the module into a temp domain
			// to retrieve its module info.  Then, we have to attempt to resolve the dependencies.
			// This is going to be fun.  Heh.
			if (_parents == null)
				_parents = new ArrayList ();
			
			// Try to find the module on the search path.
			
			string _filename = SearchForModule (_name);
			
			if (_filename == null)
				throw new ModuleNotFoundException (string.Format ("The module {0} was not found along the module search path.", _name));
				
			// Okay, well, now we know the module exists at least in the file (we hope its a proper dll, but we'll see :).  Now we
			// need to create the temporary AppDomain and load it to get the info from it.
			AppDomainSetup _setup = new AppDomainSetup ();
			
			_setup.ApplicationBase = Directory.GetCurrentDirectory ();
			AppDomain _tempDomain = AppDomain.CreateDomain (_name, new Evidence (), _setup);
			
			byte[] _raw_bytes = LoadRawFile (_filename);
			
			// set up the search path
			
			// Let's there this son of a bitch up.
			_tempDomain.ClearPrivatePath ();
			_tempDomain.AppendPrivatePath (Directory.GetCurrentDirectory ());
			
			foreach (string s in _search_path) {
				_tempDomain.AppendPrivatePath (s);
			}
			
			// The throw here is mostly used from dep resolver calls, although it should also be caught by the immediate caller
			// (i.e. the application).
			
			try {
				_tempDomain.Load (_raw_bytes);
			} catch (BadImageFormatException e) {
				AppDomain.Unload (_tempDomain);
				throw new ModuleImageException (e.Message);
			}
			
			// Okay, now lets grab the module info from the assembly attributes.
			
			Assembly _asm = GetAssembly (_tempDomain, _name);
			
			
			try {
				_info = new ModuleInfo (_asm);
			} catch (ModuleInfoException e) {
				AppDomain.Unload (_tempDomain);
				throw new InvalidModuleException (e.Message);
			}
			
			// okay, now we've got the info, let's do some magic with the dependencies.
			// this will recursively load all of the appropriate assemblies as per the parsed
			// depedency tree.  It will take into account dependency operators, such as AND, OR
			// OPT (optional).  Very intelligent stuff.  Of course, if there are no depends,
			// this just simply returns.  This will of course continue updating the parents as needed
			// since each time a new module is loaded, the resolver is recursively called until
			// a module with no dependencies is found.  This is cool.  What this will do is call this method with
			// checking=true, which will cause it to just return if the module suceeds.  This way
			// we can ensure we don't load unneeded module Z that is a dependency of X which depends
			// on Y, because if Z suceeds but Y fails, we don't want X, Y, or Z to fail.  This way,
			// we can ensure the entire tree can be loaded first (this does take into account already
			// loaded assemblies).
			
			if (depcheck)
			{
				DepResolver _resolver = new DepResolver (_controller, _search_path);
			
				_parents.Add (_name);
				
				try {
					_resolver.Resolve (_parents, _info);
				} catch (Exception e) {
					AppDomain.Unload (_tempDomain);
					throw e;
				}
			}
			
			if (checking)
			{
				AppDomain.Unload (_tempDomain);
				return null;
			}
							
			// okay, they're good, lets load the suckers.
			// alright, we've got them all loaded, they exist in the assembly map.
			// now we create the *real* app domain.
						
			// We can't do any more with this.
			
			return _tempDomain;
		}
	}
}
