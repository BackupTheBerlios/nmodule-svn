//
// ModuleRole.cs
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
using System.Reflection;

namespace NModule.Core.Module {
	/// <summary>
	/// Handler used to register a new producer of the role.
	/// </summary>
	/// <remarks>See <see href="roles.html">Roles</see> for more information on roles.</remarks>
	public delegate void RoleRegisterHandler (Assembly asm, Type basetype);
	
	/// <summary>
	/// Handler used to unregister a producer of the role.
	/// </summary>
	/// <remarks>See <see href="roles.html">Roles</see> for more information on roles.</remarks>
	public delegate void RoleUnregisterHandler (Assembly asm);
	
	/// <summary>
	/// This represents a role that modules can fulfill.
	/// </summary>
	/// <remarks>See <see href="roles.html">Roles</see> for more information on roles.</remarks>
	public class ModuleRole {
		private Type _baseType;
		private string _roleName;
		private RoleRegisterHandler _regHandler;
		private RoleUnregisterHandler _unregHandler;
		
		/// <summary>
		/// Creates a new ModuleRole argument.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="name">The name of the role.</param>
		/// <param name="basetype">The basetype of the role.</param>
		/// <param name="regHandler">The registration handler for the role.</param>
		/// <param name="unregHandler">The unregistration handler for the role.</param>
		public ModuleRole (string name, Type basetype, RoleRegisterHandler regHandler, RoleUnregisterHandler unregHandler) {
			_baseType = basetype;
			_roleName = name;
			_regHandler = regHandler;
			_unregHandler = unregHandler;
		}
		
		/// <summary>
		/// Gets the base type of the role.
		/// </summary>
		/// <remarks>None.</remarks>
		public Type BaseType {
			get {
				return _baseType;
			}
		}
		
		/// <summary>
		/// Gets the name of the role.
		/// </summary>
		/// <remarks>None.</remarks>
		public string RoleName {
			get {
				return _roleName;
			}
		}
		
		/// <summary>
		/// Gets the registration handler for the role.
		/// </summary>
		/// <remarks>None.</remarks>
		public RoleRegisterHandler RegistrationHandler {
			get {
				return _regHandler;
			}
		}
		
		/// <summary>
		/// Gets the registration handler for the role.
		/// </summary>
		/// <remarks>None.</remarks>
		public RoleUnregisterHandler UnregistrationHandler {
			get {
				return _unregHandler;
			}
		} 
	}
}
