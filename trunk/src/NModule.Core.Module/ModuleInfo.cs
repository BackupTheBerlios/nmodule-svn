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

	/// <summary>
	/// Represents information about a module.
	/// </summary>
	/// <remarks>None.</remarks>
	public class ModuleInfo {
		/// <summary>
		/// The name of the module.
		/// </summary>
		/// <remarks>None.</remarks>
		protected string _name;
		
		/// <summary>
		/// The module version.
		/// </summary>
		/// <remarks>None.</remarks>
		protected DepVersion _version;
		
		/// <summary>
		/// Modules generated dependency tree.
		/// </summary>
		/// <remarks>None.</remarks>
		protected DepNode _dependencies;
		
		/// <summary>
		/// Modules fulfilled roles.
		/// </summary>
		/// <remarks>See <see href="roles.html">Roles</see> for an explanation of roles.</remarks>
		protected string _roles;
		
		/// <summary>
		/// The owning assembly.
		/// </summary>
		/// <remarks>None.</remarks>
		protected Assembly _owner;
		
		/// <summary>
		/// Creates a new ModuleInfo belonging to the given assembly.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_asm">The owning assembly.</param>
		public ModuleInfo (Assembly _asm) {
			_name = _asm.GetName ().Name;
			_version = DepVersion.VersionParse (_asm.GetName().Version.ToString ());
			
			ModuleDependencyAttribute _depAttr;
			ModuleRoleAttribute _roleAttr;
			
			try {
				_depAttr = ((ModuleDependencyAttribute)(_asm.GetCustomAttributes (typeof (ModuleDependencyAttribute), false)[0]));
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
				_roles = null;
			
			_owner = _asm;
		}
		
		/// <summary>
		/// Gets the modules name.
		/// </summary>
		/// <remarks>None.</remarks>
		public string Name {
			get {
				return _name;
			}
		}
		
		/// <summary>
		/// Gets the modules version.
		/// </summary>
		/// <remarks>None.</remarks>
		public DepVersion Version {
			get {
				return _version;
			}
		}
		
		/// <summary>
		/// Gets the modules dependency tree.
		/// </summary>
		/// <remarks>None.</remarks>
		public DepNode Dependencies {
			get {
				return _dependencies;
			}
		}
		
		/// <summary>
		/// Gets the modules roles.
		/// </summary>
		/// <remarks>See <see href="roles.html">Roles</see> for more information on module roles.</remarks>
		public string Roles {
			get {
				return _roles;
			}
		}
		
		/// <summary>
		/// Gets the owning assembly.
		/// </summary>
		/// <remarks>None.</remarks>
		public Assembly Owner {
			get {
				return _owner;
			}
		}
	}		
}
