//
// InvalidModuleException.cs
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
 
namespace NModule.Core.Loader {
	using System;
	
	/// <summary>
	/// Exception thrown when an assembly is loaded that does
	/// match the definition of a valid module.
	/// </summary>
	/// <remarks>None.</remarks>
	/// <preliminary />
	public class InvalidModuleException : Exception {
		/// <summary>
		/// Creates a new InvalidModuleException object.
		/// </summary>
		/// <remarks>None.</remarks>
		public InvalidModuleException ( ) : base ( ) { }
		
		/// <summary>
		/// Creates a new InvalidModuleException object with the given message.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_msg">The message to be given when the execption is thrown.</param>
		public InvalidModuleException (string _msg) : base (_msg) { }
		
		/// <summary>
		/// Creates a new InvalidModuleException object with the given message and inner exception.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_msg">The message to be given when the exception is thrown.</param>
		/// <param name="_exc">The inner exception of this exception.</param>
		public InvalidModuleException (string _msg, Exception _exc) : base (_msg, _exc) { }
	}
}
