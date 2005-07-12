//
// ModuleInfo.cs
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

namespace NModule.Core.Module {
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	
	using NModule.Dependency.Core;
	using NModule.Dependency.Parser;
	using NModule.Core;
		
	public class ModuleInfo {
		// name
		protected string _name;
		
		// version
		protected DepVersion _version;
		
		// dependency stuff
		protected DepNode _dependencies;
		
		// roles
		protected string _roles;
		
		// owner
		protected Assembly _owner;
		
		public ModuleInfo (Assembly _asm) {
			_name = _asm.GetName ().Name;
			_version = DepVersion.VersionParse (_asm.GetName().Version.ToString ());
			
			ModuleDependencyAttribute _depAttr;
			ModuleRoleAttribute _roleAttr;
			
			try {
				_depAttr = ((ModuleDependencyAttribute)(_asm.GetCustomAttributes (typeof (ModuleDependencyAttribute), false)[0]));
				foreach (ModuleDependencyAttribute _attr in _asm.GetCustomAttributes (typeof (ModuleDependencyAttribute), false)) {
					
				}
			} catch (IndexOutOfRangeException) {
				_depAttr = null;
			}
			
			if (_depAttr != null) {	
				DepLexer _lexer = new DepLexer (new StringReader (_depAttr.DepString));
				DepParser _parser = new DepParser (_lexer);
				
				// woot...lets do this!
				_dependencies = new DepNode ();
				
				_parser.expr (_dependencies);
			} else
				_dependencies = null;
			
			try {
				_roleAttr = ((ModuleRoleAttribute)(_asm.GetCustomAttributes (typeof (ModuleRoleAttribute), false)[0]));
			} catch (IndexOutOfRangeException) {
				_roleAttr = null;
			}
			
			if (_roleAttr != null) {
				_roles = _roleAttr.Roles;
			} else
				throw new ModuleInfoException (string.Format ("The module {0} has no defined roles, and is not a valid NModule module.", _asm.GetName ().Name));
				
			_owner = _asm;
		}
		
		public string Name {
			get {
				return _name;
			}
		}
		
		public DepVersion Version {
			get {
				return _version;
			}
		}
		
		public DepNode Dependencies {
			get {
				return _dependencies;
			}
		}
		
		public string Roles {
			get {
				return _roles;
			}
		}
		
		public Assembly Owner {
			get {
				return _owner;
			}
		}
	}		
}
