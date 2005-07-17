//
// DepNode.cs
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
	/// Represents a node in the dependency tree.
	/// </summary>
	/// <remarks>None.</remarks>
	public class DepNode {
		private DepConstraint _constraint;
		private DepOps _op;
		private DepNode _parent;
		private ArrayList _children;

		/// <summary>
		/// Creates a new DepNode object.
		/// </summary>
		/// <remarks>None.</remarks>
		public DepNode () {
			_parent = null;
			_children = new ArrayList ();
		}

		/// <summary>
		/// Creates a new DepNode object with the given parent.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="parent">Parent node for this node.</param>
		public DepNode (DepNode parent) {
			_parent = parent;
			_children = new ArrayList ();
		}

		/// <summary>
		/// Gets this nodes parent node.
		/// </summary>
		/// <remarks>None.</remarks>
		public DepNode Parent {
			get {
				return _parent;
			}
		}

		/// <summary>
		/// Gets the children of this node.
		/// </summary>
		/// <remarks>None.</remarks>
		public ArrayList Children {
			get {
				return _children;
			}
		}

		/// <summary>
		/// Creates a new child node of this node.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <returns>Returns the newly created child.</returns>
		public DepNode CreateNewChild () {
			DepNode child = new DepNode (this);
			_children.Add (child);
			return child;
		}

		/// <summary>
		/// Gets or sets the operator for this node.
		/// </summary>
		/// <remarks>None.</remarks>
		public DepOps DepOp {
			get {
				return _op;
			}
			set {
				_op = value;
			}
		}

		/// <summary>
		/// Gets or sets the constraint for this node.
		/// </summary>
		/// <remarks>None.</remarks>
		public DepConstraint Constraint {
			get {
				return _constraint;
			}
			set {
				_constraint = value;
			}
		}
	}
}
