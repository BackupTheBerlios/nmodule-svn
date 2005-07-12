//
// nm_ur.cs
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

namespace NModule.Core.Test {

	using System;
	using System.Reflection;
	using System.Collections;
	
	using NModule.Core.Loader;
	using NModule.Core;
	using NModule.Core.Module;
	using NModule.Dependency.Core;
	using NModule.Dependency.Parser;
	using NModule.Dependency.Resolver;
	
	using NUnit.Framework;
	
	[TestFixture]
	public class nm_ur {
	
		public nm_ur () {
		}
		
		// This is a huge section.  This is a mirror of the dependency resolution section.
		
		// ==
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_01 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			UnresolvedDependencyException e = null;
			// This will look for a specific version of nm-ur-01b
			try {
				_mc.LoadModule ("nm-ur-01a");
			} catch (UnresolvedDependencyException exc) {
				e = exc;
			}
			
			Assert.IsFalse (_mc.IsLoaded ("nm-ur-01a"), "nm-ur-01a is loaded, it should not be.");
			Assert.IsFalse (_mc.IsLoaded ("nm-ur-01b"), "nm-ur-01a is loaded, it should not be.");
			Assert.IsFalse (_mc.IsLoaded ("nm-ur-01c"), "nm-ur-01a is loaded, it should not be.");
			Assert.IsFalse (_mc.IsLoaded ("nm-ur-01d"), "nm-ur-01a is loaded, it should not be.");
			Assert.IsFalse (_mc.IsLoaded ("nm-ur-01e"), "nm-ur-01a is loaded, it should not be.");
			Assert.IsFalse (_mc.IsLoaded ("nm-ur-01f"), "nm-ur-01a is loaded, it should not be.");
			Assert.IsFalse (_mc.IsLoaded ("nm-ur-01g"), "nm-ur-01a is loaded, it should not be.");
			
			if (e != null)
				throw e;
		}
		
		// !=
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_02 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			// This will look for a specific version of nm-ur-01b
			_mc.LoadModule ("nm-ur-02a");
		}
		
		// <<
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_03 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			// This will look for a specific version of nm-ur-01b
			_mc.LoadModule ("nm-ur-03a");
		}
		
		// >>
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_04 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			// This will look for a specific version of nm-ur-01b
			_mc.LoadModule ("nm-ur-04a");
		}
		
		// <=
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_05 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			// This will look for a specific version of nm-ur-01b
			_mc.LoadModule ("nm-ur-05a");
		}
		
		// >=
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_06 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			// This will look for a specific version of nm-ur-01b
			_mc.LoadModule ("nm-ur-06a");
		}
		
		// ##
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_07 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			// This will look for a specific version of nm-ur-01b
			_mc.LoadModule ("nm-ur-07a");
		}
		
		// (&& (==) (>=))
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_08 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			_mc.LoadModule ("nm-ur-08a");
		}
		
		// (|| (>>) (<<))
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_09 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			_mc.LoadModule ("nm-ur-09a");
		}
		
		// (^^ (>>) (!=))
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_10 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			_mc.LoadModule ("nm-ur-10a");
		}
		
		// (!#)
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_11() {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			_mc.LoadModule ("nm-ur-11b");
			_mc.LoadModule ("nm-ur-11b");
			_mc.LoadModule ("nm-ur-11a");
		}
		
		// (>=)
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_12() {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			_mc.LoadModule ("nm-ur-12a");
		}
		
		// (&& (|| (==) (!=)) (?? (##)))
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_13() {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			_mc.LoadModule ("nm-ur-13a");
		}
		
		// (|| (&& (##) (##)) (==))
		[Test]
		[ExpectedException (typeof (UnresolvedDependencyException))]
		public void nm_ur_14() {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ur");
			
			_mc.LoadModule ("nm-ur-14a");
		}
	}
}
