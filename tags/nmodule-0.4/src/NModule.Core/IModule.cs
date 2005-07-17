//
// IModule.cs
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

namespace NModule.Core {
	/// <summary>
	/// This represents a module entry point class if a plug-in needs one.
	/// </summary>
	/// <remarks>
	/// A module does not need a type that implements this interface, it is
	/// only used if the modules need to do some supplementary information on
	/// the controller, such as registering a new role, or retrieving a type
	/// from a module which it depends on.
	/// </remarks>
	/// <preliminary />
	public interface IModule {
		/// <summary>
		/// The entry point of the module.
		/// </summary>
		/// <remarks>
		/// The <paramref name="controller" /> will always be a ModuleController object.
		/// This method is called when a module is loaded.
		/// </remarks>
		/// <param name="controller">The module controller to be used for auxillary operations.</param>
		void ModuleEntry (object controller);
		
		/// <summary>
		/// The exit point of the module.
		/// </summary>
		/// <remarks>
		/// The <paramref name="controller" /> will always be a ModuleController object.
		/// This method is called when a module is unloaded.
		/// </remarks>
		/// <param name="controller">The module controller to be used for auxillary operations.</param>
		void ModuleExit (object controller);
	}
}
