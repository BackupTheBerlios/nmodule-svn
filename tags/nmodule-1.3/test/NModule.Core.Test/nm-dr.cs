//
// nm-dr.cs
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
	public class nm_dr {
	
		public nm_dr () {
		}
		
		// This is a huge section.
		
		// ==
		[Test]
		public void nm_dr_01 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-01a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-01a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-01b"));
		}
		
		// !=
		[Test]
		public void nm_dr_02 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-02a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-02a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-02b"));
		}
		
		// <<
		[Test]
		public void nm_dr_03 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-03a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-03a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-03b"));
		}
		
		// >>
		[Test]
		public void nm_dr_04 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-04a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-04a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-04b"));
		}
		
		// <=
		[Test]
		public void nm_dr_05 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-05a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-05a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-05b"));
		}
		
		// >=
		[Test]
		public void nm_dr_06 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-06a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-06a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-06b"));
		}
		
		// ##
		[Test]
		public void nm_dr_07 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-07a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-07a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-07b"));
		}
		
		// (&& (==) (>=))
		[Test]
		public void nm_dr_08 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			_mc.LoadModule ("nm-dr-08a");
			
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-08a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-08b"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-08c"));
		}
		
		// (|| (>>) (<<))
		[Test]
		public void nm_dr_09 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			_mc.LoadModule ("nm-dr-09a");
			
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-09a"));
			Assert.IsFalse (_mc.IsLoaded ("nm-dr-09b"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-09c"));
		}
		
		// (^^ (>>) (!=))
		[Test]
		public void nm_dr_10 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			_mc.LoadModule ("nm-dr-10a");
			
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-10a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-10b"));
			Assert.IsFalse (_mc.IsLoaded ("nm-dr-10c"));
		}
		
		// (!#))
		[Test]
		public void nm_dr_11() {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			_mc.LoadModule ("nm-dr-11a");
			
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-11a"));
			Assert.IsFalse (_mc.IsLoaded ("nm-dr-11b"));
		}
		
		// (?? (>=))
		[Test]
		public void nm_dr_12() {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			_mc.LoadModule ("nm-dr-12a");
			
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-12a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-12b"));
		}
		
		// (&& (|| (==) (!=)) (?? (##)))
		[Test]
		public void nm_dr_13() {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			_mc.LoadModule ("nm-dr-13a");
			
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-13a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-13b"));
			Assert.IsFalse (_mc.IsLoaded ("nm-dr-13c"));
			Assert.IsFalse (_mc.IsLoaded ("nm-dr-13d"));
		}
		
		// (|| (&& (##) (##)) (==)))
		[Test]
		public void nm_dr_14() {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			_mc.LoadModule ("nm-dr-14a");
			
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-14a"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-14b"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-14c"));
			Assert.IsFalse (_mc.IsLoaded ("nm-dr-14d"));
		}

		public static void Main (string[] args) {
			nm_dr _t = new nm_dr();

			_t.nm_dr_14 ();
		}
	}
}
