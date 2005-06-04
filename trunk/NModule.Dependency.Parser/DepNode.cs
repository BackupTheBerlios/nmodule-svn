using System;
using System.Collections;

namespace NModule.Dependency.Parser
{

	public class DepNode
	{
		private DepConstraint _constraint;
		private DepOps _op;
		private DepNode _parent;
		private ArrayList _children;

		public DepNode()
		{
			_parent = null;
			_children = new ArrayList();
		}

		public DepNode(DepNode parent)
		{
			_parent = parent;
			_children = new ArrayList();
		}

		public DepNode Parent
		{
			get
			{
				return _parent;
			}
		}

		public ArrayList Children
		{
			get
			{
				return _children;
			}
		}

		public DepNode CreateNewChild()
		{
			DepNode child = new DepNode(this);
			_children.Add(child);
			return child;
		}

		public DepOps DepOp
		{
			get
			{
				return _op;
			}
			set
			{
				_op = value;
			}
		}

		public DepConstraint Constraint
		{
			get
			{
				return _constraint;
			}
			set
			{
				_constraint = value;
			}
		}
	}
}
