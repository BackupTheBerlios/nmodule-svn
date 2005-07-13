//
// ModuleRoleAttribute.cs
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

 
namespace NModule.Core {
	using System;
	
	/// <summary>
	/// Holds a string representation of a modules roles.
	/// </summary>
	/// <remarks>
	/// This attribute is only valid on assembly targets.
	/// The roles should be a comma-seperated list of roles that
	/// the module provides facilities for.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class ModuleRoleAttribute : Attribute {
		protected string _roles;
		
		/// <summary>
		/// Creates a new <code>ModuleRoleAttribute</code> with the given roles.
		/// </summary>
		/// <param name="role">Comma-seperate list of roles the module provides facilities for.</param>
		public ModuleRoleAttribute (string role) {
			_roles = role;
		}
		
		/// <summary>
		/// Retrieves the list of roles from a <code>ModuleRoleAttribute</code> object.
		/// </summary>
		public string Roles {
			get {
				return _roles;
			}
		}
	}
}
