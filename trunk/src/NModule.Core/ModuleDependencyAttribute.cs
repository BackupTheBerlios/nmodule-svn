//
// ModuleDependencyAttribute.cs
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
	/// Holds a string representation of a modules dependency's.
	/// </summary>
	/// <remarks>
	/// This attribute is only valid on assembly targets.
	/// See <see href="depstring.html" /> for information on the format of
	/// dependency strings and a description of the dependency operators.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class ModuleDependencyAttribute : Attribute {
		protected string _dep_string;
		
		/// <summary>
		/// Creates a new <code>ModuleDependencyAttribute</code> object using the given dep string.
		/// </summary>
		/// <param name="dep_string">A string representing the module's dependencies.</param>
		public ModuleDependencyAttribute (string dep_string) {
			_dep_string = dep_string;
		}
		
		/// <summary>
		/// Returns the dependency string of a <code>ModuleDependencyAttribute</code> object.
		/// </summary>
		public string DepString {
			get {
				return _dep_string;
			}
		}
	}
}
