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
	
	using NModule.Dependency.Core;
	using NModule.Core.Loader;
	using NModule.Core.Module;
	
	public class DepResolver {
#region Members
		// ModuleController used for loading modules to sastify dependencies.
		protected ModuleController	_controller;
		
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
						if (f.SubString (0, f.Length - 4) == _name) {
							return s + "/" + f;
						}
					}
				}
			}
			
			return null;
		}
		
		protected void OpResolve (DepNode _node, ArrayList _parents, ModuleInfo _info, bool checking) {
			DepOp _op = _node.DepOp;
			DepConstraint _constraint = _node.Constraint;
			if ((_op == DepOps.And) || (_op == DepOps.Not) || (_op == DepOps.Opt) || (_op == DepOps.Or) || (_op == DepOps.Xor)) {
				// combo-operators
				int c = 0;
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
								throw new UnresolvedDependencyInformation (
									string.Format("The following dependency for the module {0} could not be resolved: (AND operator)\n\t{1} ({2})",
										_info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version)
								);
							}
						}
						break;
					case DepOps.Not:
						foreach (bool _result in _results) {
							if (_result) {
								throw new UnresolvedDependencyInformation (
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
						
						foreach (bool _result in _results) {
							if (_result)
								_ret = true;
							else {
								_urexc.Add (string.Format("{1} ({2})", _info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version));
							}
						}
						
						if (!_ret) {
							StringBuilder _sb = new StringBuilder (
								string.Format("The following dependency for the module {0} could not be resolved: (OR operator)\n")
							);
							foreach (string _exc in _urexc) {
								sb += string.Format("\t{0}\n", _exc);
							}
							throw new UnresolvedDependencyInformation (sb.ToString ());
						}
						break;
					case DepOps.Xor:
						_xt = true;
						_xf = true;
						ArrayList _urexc = new ArrayList ();
				
						foreach (bool _result in _results) {
							if (_result) {
								_xf = false;
								_urexc.Add (string.Format("{1} ({2}) (True)", _info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version));
							}
							if (!_result) {
								_xt = false;
								_ur.Add (string.Format("{1} ({2}) (False)", _info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version));
							}
						}
						
						if (_xt || _xf)
							_ret = false;
							
						if (!_ret) {
							StringBuilder _sb = new StringBuilder (
								string.Format("The following dependency for the module {0} could not be resolved: (XOR operator)\n")
							);
							foreach (string _exc in _urexc) {
								sb += string.Format("\t{0}\n", _exc);
							}
							throw new UnresolvedDependencyInformation (sb.ToString ());
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
						if (!VersionMatch (_ninfo, _version, _op)) {
							throw new UnresolvedDependencyInformation (
								string.Format("The following dependency for the module {0} could not be resolved: ({3} operator)\n\t{1} ({2})",
									_info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version, OpToString (_op))
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
			
		protected void InternalResolve (ArrayList _parents, ModuleInfo _info, bool checking) {
			if (_info.Dependencies == null)
				return;
			
			foreach (DepNode _node in _info.Dependencies) {
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