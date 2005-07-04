//
// nm-ul.cs
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
	public class nm_ul {
	
		public nm_ul () {
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
	}
}