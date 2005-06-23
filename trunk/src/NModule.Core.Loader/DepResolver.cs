/**************************************************************************
 * Copyright (c) 2005 Michael Tindal and the individuals listed           *
 * on the ChangeLog entries.                                              *
 *                                                                        *
 * Permission is hereby granted, free of charge, to any person obtaining  *
 * a copy of this software and associated documentation files (the        *
 * "Software"), to deal in the Software without restriction, including    *
 * without limitation the rights to use, copy, modify, merge, publish,    *
 * distribute, sublicense, and/or sell copies of the Software, and to     *
 * permit persons to whom the Software is furnished to do so, subject to  *
 * the following conditions                                               *
 *                                                                        *
 * The above copyright notice and this permission notice shall be         *
 * included in all copies or substantial portions of the Software.        *
 *                                                                        *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        *
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     *
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND                  *
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE *
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION *
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION  *
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.        *
 **************************************************************************/

namespace NModule.Dependency.Resolver {
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using System.Text;
	
	using NModule.Dependency.Core;
	using NModule.Core.Loader;
	using NModule.Core.Module;
	
	public class DepResolver {
#region Members
		// ModuleController used for loading modules to sastify dependencies.
		protected ModuleController _controller;
		
		// Search Path for modules
		protected ArrayList _search_path;
#endregion

#region Constructor
		public DepResolver (ModuleController controller, ArrayList search_path) {
			_controller = controller;
			_search_path = search_path;
		}
#endregion

#region Internal Helper Functions
		protected string SearchForModule (string _name) {
			foreach (string s in _search_path) {
				if (Directory.Exists (s)) {
					foreach (string f in Directory.GetFiles (s, "*.dll")) {
						if (f.Substring (0, f.Length - 4) == _name) {
							return s + "/" + f;
						}
					}
				}
			}
			
			return null;
		}
		
		protected void OpResolve (DepNode _node, ArrayList _parents, ModuleInfo _info, bool checking) {
			bool _ret;
			DepOps _op = _node.DepOp;
			DepConstraint _constraint = _node.Constraint;
			if ((_op == DepOps.And) || (_op == DepOps.Not) || (_op == DepOps.Opt) || (_op == DepOps.Or) || (_op == DepOps.Xor)) {
				// combo-operators
				ArrayList _results = new ArrayList ();
				ArrayList _c = new ArrayList ();
				foreach (DepNode _child in _node.Children) {
					try {
						OpResolve (_child, _parents, _info, checking);
						_results.Add (true);
						_c.Add (_child.Constraint);
					} catch (Exception e) {
						_results.Add (false);
						_c.Add (_child.Constraint);
					}
				}
				
				switch (_op) {
					case DepOps.And:
						int r = 0;
						foreach (bool _result in _results) {
							if (!_result) {
								throw new UnresolvedDependencyException (
									string.Format("The following dependency for the module {0} could not be resolved: (AND operator)\n\t{1} ({2})",
										_info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version)
								);
							}
							r++;
						}
						break;
					case DepOps.Not:
						foreach (bool _result in _results) {
							if (_result)
								throw new UnresolvedDependencyException (
									string.Format("The following dependency for the module {0} could not be resolved: (NOT operator)\n\t{1} ({2})",
										_info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version)
								);
						}
						break;
					case DepOps.Opt: // This is optional so stuff is true regardless
						break;
					case DepOps.Or:
						_ret = false;
						ArrayList _urexc = new ArrayList ();
						r = 0;
						foreach (bool _result in _results) {
							if (_result)
								_ret = true;
							else {
								_urexc.Add (string.Format("{1} ({2})", _info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version));
							}
							r++;
						}
						
						if (!_ret) {
							StringBuilder _sb = new StringBuilder (
								string.Format("The following dependency for the module {0} could not be resolved: (OR operator)\n")
							);
							foreach (string _exc in _urexc) {
								_sb.Append(string.Format("\t{0}\n", _exc));
							}
							throw new UnresolvedDependencyException (_sb.ToString ());
						}
						break;
					case DepOps.Xor:
						bool _xt = true;
						bool _xf = true;
						ArrayList _xexc = new ArrayList ();
				
						r = 0;
						_ret = true;
						
						foreach (bool _result in _results) {
							if (_result) {
								_xf = false;
								_xexc.Add (string.Format("{1} ({2}) (True)", _info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version));
							}
							if (!_result) {
								_xt = false;
								_xexc.Add (string.Format("{1} ({2}) (False)", _info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version));
							}
							r++;
						}
						
						if (_xt || _xf)
							_ret = false;
							
						if (!_ret) {
							StringBuilder _sb = new StringBuilder (
								string.Format ("The following dependency for the module {0} could not be resolved: (XOR operator)\n")
							);
							foreach (string _exc in _xexc) {
								_sb.Append (string.Format("\t{0}\n", _exc));
							}
							throw new UnresolvedDependencyException (_sb.ToString ());
						}
						break;
				}
			} else {
				// single operators
				if (SearchForModule (_constraint.Name) == null)
					_ret = false;
		
				ModuleInfo _ninfo;
						
				ModuleLoader _loader = new ModuleLoader (_search_path, this);
						
				_loader.LoadModule (_parents, _constraint.Name, out _ninfo, true);
				
				if ((_op == DepOps.Equal) || (_op == DepOps.GreaterThan) || (_op == DepOps.GreaterThanEqual) || (_op == DepOps.LessThan)
					|| (_op == DepOps.LessThanEqual) || (_op == DepOps.NotEqual)) {
					if (!IsEmptyVersion (_constraint.Version)) {
						if (!VersionMatch (_constraint.Version, _ninfo.Version, _op)) {
							throw new UnresolvedDependencyException (
								string.Format("The following dependency for the module {0} could not be resolved: ({3} operator)\n\t{1} ({2})",
									_info.Name, _constraint.Name, _constraint.Version, OpToString (_op))
							);
						}
					}
					if (!checking) {
						_controller.LoadModule (_constraint.Name);
					}		
				}
				// we got this far, so obviously it loaded okay
			}		
		}
		
		protected string OpToString (DepOps _op) {
			switch (_op)
			{
				case DepOps.And:
					return "&&";
				case DepOps.Equal:
					return "==";
				case DepOps.GreaterThan:
					return ">>";
				case DepOps.GreaterThanEqual:
					return ">=";
				case DepOps.LessThan:
					return "<<";
				case DepOps.LessThanEqual:
					return "<=";
				case DepOps.Loaded:
					return "##";
				case DepOps.Not:
					return "!!";
				case DepOps.NotEqual:
					return "!=";
				case DepOps.Opt:
					return "??";
				case DepOps.Or:
					return "||";
				case DepOps.Xor:
					return "^^";
			}
			
			return "";
		}
		
		protected bool IsEmptyVersion (DepVersion _ver) {
			return ((_ver.Major == -1) && (_ver.Minor == -1) && (_ver.Build == -1) && (_ver.Revision == -1));
		}
		
		protected bool VersionMatch (DepVersion _dver, DepVersion _ver, DepOps _op) {
			bool mjgt = false, mngt = false, bgt = false, rgt = false;
			bool mjeq = false, mneq = false, beq = false, req = false;
			bool mjlt = false, mnlt = false, blt = false, rlt = false;
			bool mji = false, mni = false, bi = false, ri = false;
			
			if (_dver.Major == -1) {
				mji = true;
			} else if (_dver.Major > _ver.Major) {
				mjgt = true;
			} else if (_dver.Major == _ver.Major) {
				mjeq = true;
			} else {
				mjlt = true;
			}
			
			if (_dver.Minor == -1) {
				mni = true;
			} else if (_dver.Minor > _ver.Minor) {
				mngt = true;
			} else if (_dver.Minor == _ver.Minor) {
				mneq = true;
			} else {
				mnlt = true;
			}
			
			if (_dver.Build == -1) {
				bi = true;
			} else if (_dver.Build > _ver.Build) {
				bgt = true;
			} else if (_dver.Build == _ver.Build) {
				beq = true;
			} else {
				blt = true;
			}
			
			if (_dver.Revision == -1) {
				ri = true;
			} else if (_dver.Revision > _ver.Revision) {
				rgt = true;
			} else if (_dver.Revision == _ver.Revision) {
				req = true;
			} else {
				rlt = true;
			}

			if (mji)
				return true;
									
			if ((_op == DepOps.Equal) || (_op == DepOps.GreaterThanEqual) || (_op == DepOps.LessThanEqual)) {					
					if (mjeq && mni)
						return true;
						
					if (mjeq && mneq && bi)
						return true;
						
					if (mjeq && mneq && beq && ri)
						return true;
					
					if (mjeq && mneq && beq && req)
						return true;
			}
			
			if (_op == DepOps.NotEqual) {
				if (!mjeq && mni)
					return true;
					
				if (!mjeq && !mneq && bi)
					return true;
					
				if (!mjeq && !mneq && !beq && ri)
					return true;
					
				if (!mjeq && !mneq && !beq && !req)
					return true;
			}
			
			if (_op == DepOps.GreaterThan) {
				if (mjlt)
					return false;
					
				if (mjgt)
					return true;
				
				if (mjeq && mnlt)
					return false;
					
				if (mngt)
					return true;
					
				if (mjeq && mneq && blt)
					return false;
					
				if (bgt)
					return true;
					
				if (mjeq && mneq && beq && rlt)
					return false;
					
				if (rgt)
					return true;
			}
			
			if (_op == DepOps.LessThan) {
				if (mjgt)
					return false;
					
				if (mjlt)
					return true;
				
				if (mjeq && mngt)
					return false;
					
				if (mnlt)
					return true;
					
				if (mjeq && mneq && bgt)
					return false;
					
				if (blt)
					return true;
					
				if (mjeq && mneq && beq && rgt)
					return false;
					
				if (rlt)
					return true;
			}
			
			if (_op == DepOps.Loaded)
				return true;
				
			
			return false;
		}
				
		protected void InternalResolve (ArrayList _parents, ModuleInfo _info, bool checking) {
			if (_info.Dependencies == null)
				return;
			
			DepNode _node = _info.Dependencies;
			
			foreach (string _parent in _parents) {
				if (_node.Constraint.Name == _parent) {
					throw new CircularDependencyException (
						string.Format("The module {0} is depending on {1} which is depending on {0}, causing a circular dependency.",
							_parent, _info.Name, _parent)
					);
				}
			}
			OpResolve (_node, _parents, _info, checking);			
		}
#endregion

#region Resolvers
		public void ResolveCheck (ArrayList _parents, ModuleInfo _info) {
			InternalResolve (_parents, _info, true);
		}
		
		public void Resolve (ArrayList _parents, ModuleInfo _info) {
			InternalResolve (_parents, _info, false);
		}
#endregion
	}
}