//
// NModuleTests.cs
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
	public class NModuleTests {
	
		public NModuleTests () {
		}
		
		// nm-ld-01 - Loading with no dependencies, single dir search path.
		[Test]
		public void nm_ld_01 () {
			Console.WriteLine ("nm_ld_01");
			
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ld/");
			
			_mc.LoadModule ("nm-ld-01");
		}
		
		// nm-ld-02 - Loading with no dependencies, single dir search path, not found.
		[Test]
		[ExpectedException (typeof (ModuleNotFoundException))]
		public void nm_ld_02 () {
			Console.WriteLine ("nm_ld_02");
			
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ld");
			
			_mc.LoadModule ("nm-ld-02");
		}
		
		// nm-ld-03 - Loading with no dependencies, multiple dir search path, first dir.
		[Test]
		public void nm_ld_03 () {
			Console.WriteLine ("nm_ld_03");
			
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ld");
			_mc.SearchPath.Add ("data/nm-ld-2");
			_mc.SearchPath.Add ("data/nm-ld-3");
			
			_mc.LoadModule ("nm-ld-03");
		}
		
		// nm-ld-04 - Loading with no dependencies, multiple dir search path, middle dir.
		[Test]
		public void nm_ld_04 () {
			Console.WriteLine ("nm_ld_04");
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ld-2");
			_mc.SearchPath.Add ("data/nm-ld");
			_mc.SearchPath.Add ("data/nm-ld-3");
			
			_mc.LoadModule ("nm-ld-04");
		}
		
		// nm-ld-05 - Loading with no dependencies, multiple dir search path, last dir.
		[Test]
		public void nm_ld_05 () {
			Console.WriteLine ("nm_ld_05");
			
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ld-2");
			_mc.SearchPath.Add ("data/nm-ld-3");
			_mc.SearchPath.Add ("data/nm-ld");
			
			_mc.LoadModule ("nm-ld-05");
		}
		
		// nm-ld-06 - Loading with no dependencies, multiple dir search path, not found.
		[Test]
		[ExpectedException (typeof (ModuleNotFoundException))]
		public void nm_ld_06 () {
			Console.WriteLine ("nm_ld_06");
			
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ld");
			_mc.SearchPath.Add ("data/nm-ld-2");
			_mc.SearchPath.Add ("data/nm-ld-3");
			
			_mc.LoadModule ("nm-ld-06");
		}
		
		// nm-ul-01 - Unloading with no dependencies
		[Test]
		public void nm_ul_01 () {
			Console.WriteLine ("nm_ul_01");
			
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ul");
			
			_mc.LoadModule ("nm-ul-01");
			
			_mc.UnloadModule ("nm-ul-01");
		}
		
		// nm-ul-02 - Unloading with ref count > 1
		[Test]
		[ExpectedException (typeof (DomainStillReferencedException))]
		public void nm_ul_02 () {
			Console.WriteLine ("nm_ul_02");
			
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-ul");
			
			_mc.LoadModule ("nm-ul-02");
			
			_mc.LoadModule ("nm-ul-02");
			
			_mc.UnloadModule ("nm-ul-02");
		}
		
		// This is a huge section.
		
		/**
		 * Combinations to be tested:
		 *   Valid-combination operators: && || ^^ !! ??
		 *   Valid-singleton operators: == != << <= >> >= ##
		 *   (==)
		 *   (!=)
		 *   (<<)
		 *   (>>)
		 *   (<=)
		 *   (>=)
		 *   (##)
		 *   (&& (==) (>=))
		 *   (|| (>>) (<<))
		 *   (^^ (>>) (!=))
		 *   (!! (==))
		 *   (?? (>=))
		 *   (&& (|| (==) (!=)) (?? (##)))
		 *   (|| (&& (##) (##)) (!! (!=)))
		 **/
		[Test]
		public void nm_dr_01 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-01a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-01b"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-01d"));
			Assert.IsFalse (_mc.IsLoaded ("nm-dr-01c"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-01e"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-01f"));
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-01g"));
		}
		
		//[Test]
		public void nm_dr_02 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-02a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-02b"));
		}
		
		//[Test]
		public void nm_dr_03 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-03a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-03b"));
		}
		
		//[Test]
		public void nm_dr_04 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-04a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-04b"));
		}
		
		//[Test]
		public void nm_dr_05 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-05a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-05b"));
		}
		
		//[Test]
		public void nm_dr_06 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-06a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-06b"));
		}
		
		//[Test]
		public void nm_dr_07 () {
			ModuleController _mc = new ModuleController ();
			
			_mc.SearchPath.Add ("data/nm-dr");
			
			// This will look for a specific version of nm-dr-01b
			_mc.LoadModule ("nm-dr-07a");
			
			// If it worked, we should have a domain for nm-dr-01b
			Assert.IsTrue (_mc.IsLoaded ("nm-dr-07b"));
		}
	}
}
