//
// DepConstraint.cs
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

namespace NModule.Dependency.Core {
	using System;
	using System.Collections;
	
	/// <summary>
	/// Represents a dependency constraint.
	/// </summary>
	/// <remarks>None.</remarks>
	public class DepConstraint {
		private DepVersion _version;
		private string _name;

		/// <summary>
		/// Creates a new DepConstraint object.
		/// </summary>
		/// <remarks>None.</remarks>
		public DepConstraint () {
			_name = "";
			_version = new DepVersion (-1, -1, -1, -1);
		}

		/// <summary>
		/// Gets or sets the needed version.
		/// </summary>
		/// <remarks>None.</remarks>
		public DepVersion Version {
			get {
				return _version;
			}
			set {
				_version = value;
			}
		}

		/// <summary>
		/// Gets or sets the needed module name.
		/// </summary>
		/// <remarks>None.</remarks>
		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}

		/// <summary>
		/// Sets the version based on a string representation of it.
		/// </summary>
		/// <remarks>None.</remarks>
		public void SetVersion (string _ver) {
			_version = DepVersion.VersionParse (_ver);
		}
	}
}
