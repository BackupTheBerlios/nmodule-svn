//
// DepVersion.cs
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
	
	public class DepVersion {
		private int _major;
		private int _minor;
		private int _build;
		private int _revision;
		
		public DepVersion (int major, int minor, int build, int revision) {
			_major = major;
			_minor = minor;
			_build = build;
			_revision = revision;
		}
		
		public DepVersion (int major, int minor, int build) {
			_major = major;
			_minor = minor;
			_build = build;
			_revision = -1;
		}
		
		public DepVersion (int major, int minor) {
			_major = major;
			_minor = minor;
			_build = -1;
			_revision = -1;
		}
		 
		public DepVersion () {
			_major = -1;
			_minor = -1;
			_build = -1;
			_revision = -1;
		}
	
		public int Major {
			get {
				return _major;
			}
			set {
				_major = value;
			}
		}
		
		public int Minor {
			get {
				return _minor;
			}
			set {
				_minor = value;
			}
		}
		
		public int Build {
			get {
				return _build;
			}
			set {
				_build = value;
			}
		}
		
		public int Revision {
			get {
				return _revision;
			}
			set {
				_revision = value;
			}
		}
		
		public static DepVersion VersionParse (string v) {
			// Here we go :)
			DepVersion ver = new DepVersion ();
			string[] vparts = v.Split ('.');
			ver.Major = Int32.Parse (vparts[0]);
			ver.Minor = Int32.Parse (vparts[1]);
			if (vparts.Length > 2)
				ver.Build = Int32.Parse(vparts[2]);
			if (vparts.Length > 3)
				ver.Revision = Int32.Parse(vparts[3]);
			return ver;
		}
	}
}
