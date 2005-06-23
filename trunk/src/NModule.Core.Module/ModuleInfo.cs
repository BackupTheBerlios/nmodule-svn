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
		
		public ModuleInfo (Assembly _asm) {
			_name = _asm.GetName().Name;
			_version = DepVersion.VersionParse (_asm.GetName().Version);
			
			ModuleDependencyAttribute _depAttr = ((ModuleDependencyAttribute)_asm.GetCustomAttributes (typeof (ModuleDependencyAttribute)));
			
			if (_depAttr != null) {	
				DepLexer _lexer = new DepLexer (new StringReader (_depAttr.DepString));
				DepParser _parser = new DepParser (_lexer);
				
				// woot...lets do this!
				_dependencies = new DepNode ();
				
				_parser.expr (_dependencies);
			} else
				_depenencies = null;
				
			ModuleRoleAttribute _roleAttr = ((ModuleRoleAttribute)_asm.GetCustomAttributes (typeof (ModuleRoleAttribute));
			
			if (_roleAttr != null) {
				_roles = _roleAttr.Roles;
			} else
				_roles = null;
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
	}		
}
