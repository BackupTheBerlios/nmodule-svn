//
// DepResolver.cs
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
						if (f.Substring (0, f.Length - 4).IndexOf (_name) != -1) {
							return s + "/" + f;
						}
					}
				}
			}
			
			return null;
		}
		
		protected void OpResolve (DepNode _node, ArrayList _parents, ModuleInfo _info, bool checking, DepOps _parentop) {
			Console.WriteLine ("-->> OpResolve called with:");
			Console.WriteLine ("---->> _node = {0}", _node == null ? "null" : "not null");
			Console.WriteLine ("---->> _node.DepOp = {0}", OpToString (_node.DepOp));
			Console.WriteLine ("---->> _parents = {0}", _parents == null ? "null" : "not null");
			Console.WriteLine ("---->> _info = {0}", _info == null ? "null" : "not null");
			Console.WriteLine ("---->> checking = {0}", checking);
			bool _ret;
			DepOps _op = _node.DepOp;
			
			if ((_op == DepOps.And) || (_op == DepOps.Not) || (_op == DepOps.Opt) || (_op == DepOps.Or) || (_op == DepOps.Xor)) {
				// combo-operators
				ArrayList _results = new ArrayList ();
				ArrayList _c = new ArrayList ();
				foreach (DepNode _child in _node.Children) {
					//try {
						OpResolve (_child, _parents, _info, checking, _op);
						_results.Add (true);
						_c.Add (_child.Constraint);
					//} catch (Exception e) {
					//	_results.Add (false);
					//	_c.Add (_child.Constraint);
					//}
				}
				
				switch (_op) {
					case DepOps.And:
						int r = 0;
						if (_results == null) {
							Console.WriteLine ("_results is NULL!");
						}
						
						foreach (bool _result in _results) {
							if (!_result) {
								if (_parentop != DepOps.Null && _parentop != DepOps.Opt) {
									throw new UnresolvedDependencyException (
										string.Format("The following dependency for the module {0} could not be resolved: (AND operator)\n\t{1} ({2})",
											_info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version)
									);
								}
							}
							r++;
						}
						break;
					case DepOps.Not:
						foreach (bool _result in _results) {
							if (_result) {
								if (_parentop != DepOps.Null && _parentop != DepOps.Opt) {
									throw new UnresolvedDependencyException (
										string.Format("The following dependency for the module {0} could not be resolved: (NOT operator)\n\t{1} ({2})",
											_info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version)
									);
								}
							}
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
							if (_parentop != DepOps.Null && _parentop != DepOps.Opt)
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
						
						if (_xt && _xf)
							_ret = false;
						
						if (!_xt && _xf)
							_ret = false;
							
						if (!_ret) {
							StringBuilder _sb = new StringBuilder (
								string.Format ("The following dependency for the module {0} could not be resolved: (XOR operator)\n")
							);
							foreach (string _exc in _xexc) {
								_sb.Append (string.Format("\t{0}\n", _exc));
							}
							if (_parentop != DepOps.Null && _parentop != DepOps.Opt) {
								throw new UnresolvedDependencyException (_sb.ToString ());
							}
						}
						break;
				}
			} else {
				DepConstraint _constraint = _node.Constraint;
				
				// single operators
				if (SearchForModule (_constraint.Name) == null)
					_ret = false;
		
				ModuleInfo _ninfo;
						
				ModuleLoader _loader = new ModuleLoader (_search_path, _controller);
						
				_loader.LoadModule (_parents, _constraint.Name, out _ninfo, true, true);
				
				bool result = true;
				
				Console.WriteLine ("Checking {0}'s version:  Empty {1}", _constraint.Name, IsEmptyVersion (_constraint.Version));
				Console.WriteLine ("Checking ({0} {1} {2}) Result: {3}", OpToString (_op), _ninfo.Version.ToString(), _constraint.Version.ToString(), VersionMatch (_ninfo.Version, _constraint.Version, _op));
					
				if (!IsEmptyVersion (_constraint.Version)) {
					if (!VersionMatch (_ninfo.Version, _constraint.Version, _op)) {
						result = false;
					}
				}

				if (!result) {
					if (_parentop != DepOps.Null && _parentop != DepOps.Opt) {
						throw new UnresolvedDependencyException (
								string.Format("The following dependency for the module {0} could not be resolved: ({3} operator)\n\t{1} ({2})",
									_info.Name, _constraint.Name, _constraint.Version, OpToString (_op))
							);
					}
					else
						return;			
				}
					
				if (_controller == null)
					Console.WriteLine ("_controller is NULL!");
					
				if (!checking) {
					_controller.LoadModule (_constraint.Name);
				}
					
				Console.WriteLine ("Checking {0} for {1} (Result: {2})", OpToString (_op), _info.Name, result);
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
			if (_ver == null)
				return true;
				
			return ((_ver.Major == -1) && (_ver.Minor == -1) && (_ver.Build == -1) && (_ver.Revision == -1));
		}
		
		protected bool VersionMatch (DepVersion _ver, DepVersion _dver, DepOps _op) {
			Version _mver, _drver;

			if (_dver.Major == -1)
				return true;
				
			_mver = new Version (_dver.Major != -1 ? _ver.Major : 0,
				_dver.Minor != -1 ? _ver.Minor : 0,
				_dver.Build != -1 ? _ver.Build : 0,
				_dver.Revision != -1 ? _ver.Revision : 0);

			_drver = new Version (_dver.Major != -1 ? _dver.Major : 0,
				_dver.Minor != -1 ? _dver.Minor : 0,
				_dver.Build != -1 ? _dver.Build : 0,
				_dver.Revision != -1 ? _dver.Revision : 0);
			
			int vres = _mver.CompareTo (_drver);
			
			switch (_op) {
				case DepOps.Equal:
					return vres == 0;
				case DepOps.GreaterThan:
					return vres > 0;
				case DepOps.GreaterThanEqual:
					return vres > 0 || vres == 0;
				case DepOps.LessThan:
					return vres < 0;
				case DepOps.LessThanEqual:
					return vres < 0 || vres == 0;
				case DepOps.NotEqual:
					return vres != 0;
			}
			
			return false;				
		}
			
		protected void CheckCircularDeps (DepNode _x, ArrayList _parents) {
			if (_parents == null)
				return;
				
			if (_x == null)
				return;
				
			foreach (DepNode _d in _x.Children) {
				CheckCircularDeps (_d, _parents);
			}
			
			if (_x.Constraint != null) {
				foreach (string _parent in _parents) {
					Console.WriteLine ("CIRCULAR DEPENDENCY CHECK:  {0} {1}", _x.Constraint.Name, _parent);
					if (_x.Constraint.Name == _parent) {
						throw new CircularDependencyException (string.Format ("{0} depends on {1}, but {1} is depending on {0}.", _x.Constraint.Name, _parent));
					}
				}
			}
		}
		
		protected void InternalResolve (ArrayList _parents, ModuleInfo _info, bool checking) {
			if (_info.Dependencies == null)
				return;
			
			if (_parents == null)
				_parents = new ArrayList ();
			
			if (checking) {
				foreach (DepNode _c in _info.Dependencies.Children) {
					CheckCircularDeps (_c, _parents);
				}
			}
			
			Console.WriteLine (">>>>> Beginning OpResolve loop for {0}...", _info.Name);
			
			OpResolve (_info.Dependencies, _parents, _info, checking, DepOps.Null);		
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
