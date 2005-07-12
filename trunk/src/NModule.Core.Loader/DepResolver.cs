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
						if (f.Substring (0, f.Length - 4) == _name) {
							return s + "/" + f;
						}
					}
				}
			}
			
			return null;
		}
		
		protected string ReportUnresolvedException (DepOps _op, DepConstraint _constraint) {
			// message should look like << Elfblade.Core 1.0
			// or ## Elfblade.Net.Base
			return string.Format ("{0} {1} {2}", OpToString (_op), _constraint.Name, IsEmptyVersion (_constraint.Version) ? "" : _constraint.Version.ToString ());
		}
		
		protected void OpResolve (DepNode _node, ArrayList _parents, ModuleInfo _info, bool checking, DepOps _parentop) {
			bool _ret;
			DepOps _op = _node.DepOp;
			
			if ((_op == DepOps.And) || (_op == DepOps.Opt) || (_op == DepOps.Or) || (_op == DepOps.Xor)) {
				// combo-operators
				ArrayList _results = new ArrayList ();
				ArrayList _c = new ArrayList ();
				foreach (DepNode _child in _node.Children) {
					try {
						OpResolve (_child, _parents, _info, checking, _op);
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
								if (_parentop != DepOps.Opt) {
									if (_c[r] != null) {
										throw new UnresolvedDependencyException (
											string.Format("The following dependency for the module {0} could not be resolved: ({3} operator)\n\t{1} ({2})",
												_info.Name, ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version.ToString(), OpToString (DepOps.And))
										);
									} else {
										throw new UnresolvedDependencyException (
											string.Format("The following dependency for the module {0} could not be resolved: ({1} operator)",
												_info.Name, OpToString (DepOps.And))
										);
									}
								}
							}
							r++;
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
								if (_c[r] != null)
									_urexc.Add (string.Format("{0} ({1})", ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version.ToString()));
							}
							r++;
						}
						
						if (!_ret) {
							StringBuilder _sb = new StringBuilder (
								string.Format("The following dependency for the module {0} could not be resolved: (OR operator)\n", _info.Name)
							);
							foreach (string _exc in _urexc) {
								_sb.Append(string.Format("\t{0}\n", _exc));
							}
							if (_parentop != DepOps.Opt)
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
								if (_c[r] != null)
									_xexc.Add (string.Format("{0} ({1}) (True)", ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version.ToString()));				
							}
							if (!_result) {
								_xt = false;
								if (_c[r] != null)
									_xexc.Add (string.Format("{0} ({1}) (False)", ((DepConstraint)_c[r]).Name, ((DepConstraint)_c[r]).Version.ToString()));
							}
							r++;
						}
						
						// If one of these is still true, that means all of the other results are true or false.
						if (_xt || _xf)
							_ret = false;;
							
						if (!_ret) {
							StringBuilder _sb = new StringBuilder (
								string.Format ("The following dependency for the module {0} could not be resolved: (XOR operator)\n", _info.Name)
							);
							foreach (string _exc in _xexc) {
								_sb.Append (string.Format("\t{0}\n", _exc));
							}
							if (_parentop != DepOps.Opt) {
								throw new UnresolvedDependencyException (_sb.ToString ());
							}
						}
						break;
				}
			} else {
				DepConstraint _constraint = _node.Constraint;
				
				foreach (string _parent in _parents) {
					if (_parent == _constraint.Name) {
						throw new CircularDependencyException (string.Format ("{0} and {1} have a circular dependency on each other", _parent, _info.Name));
					}
				}
				
				// single operators
				if (SearchForModule (_constraint.Name) == null)
					_ret = false;
		
				ModuleInfo _ninfo = null;
						
				ModuleLoader _loader = new ModuleLoader (_search_path, _controller);
				
				try {
					if (!_parents.Contains (_info.Name))
						_parents.Add (_info.Name);
						
					_loader.LoadModule (_parents, _constraint.Name, out _ninfo, true, true);
				} catch (CircularDependencyException ce) {
					throw ce;
				} catch (Exception) {
					if (_parentop != DepOps.Opt) {
								throw new UnresolvedDependencyException (
									string.Format("The following dependency for the module {0} could not be resolved: ({3} operator)\n\t{1} ({2})",
										_info.Name, _constraint.Name, _constraint.Version.ToString(), OpToString (_op))
								);
					}
				}
				
				if (_op == DepOps.Equal || _op == DepOps.GreaterThan || _op == DepOps.GreaterThanEqual || _op == DepOps.LessThan || _op == DepOps.LessThanEqual || _op == DepOps.NotEqual) {
					if (!IsEmptyVersion (_constraint.Version)) {
						if (!VersionMatch (_ninfo.Version, _constraint.Version, _op)) {
							if (_parentop != DepOps.Opt) {
								throw new UnresolvedDependencyException (
									string.Format("The following dependency for the module {0} could not be resolved: ({3} operator)\n\t{1} ({2})",
										_info.Name, _constraint.Name, _constraint.Version.ToString(), OpToString (_op))
								);
							}		
						}
					}
						
					if (!checking) {
						_controller.LoadModule (_constraint.Name);
						
						_parents.Sort ();
						
						ArrayList _seen = new ArrayList ();
						_seen.Add (_info.Name);
						_seen.Add (_constraint.Name);
						foreach (string _p in _parents) {
							if (!_seen.Contains (_p)) {
								_controller.IncRef (_constraint.Name);
								_seen.Add (_p);
							}
						}
					}
				}
					
				if (_op == DepOps.Loaded) {					
					if (_controller.Loader.SearchForModule (_constraint.Name) == null)
					{
						if (_parentop != DepOps.Opt) {
							throw new UnresolvedDependencyException (
										string.Format("The following dependency for the module {0} could not be resolved: ({3} operator)\n\t{1} ({2})",
											_info.Name, _constraint.Name, _constraint.Version.ToString(), OpToString (_op))
									);
						}
					}
					
					if (!checking) {
						_controller.LoadModule (_constraint.Name);
						
						_parents.Sort ();
						
						ArrayList _seen = new ArrayList ();
						_seen.Add (_info.Name);
						_seen.Add (_constraint.Name);
						foreach (string _p in _parents) {
							if (!_seen.Contains (_p)) {
								_controller.IncRef (_constraint.Name);
								_seen.Add (_p);
							}
						}
					}
				}
				
				if (_op == DepOps.NotLoaded) {
					if (_controller.IsLoaded (_constraint.Name)) {
						if (_controller.RefCount (_constraint.Name) > 1) {
							if (_parentop != DepOps.Opt) {
								throw new UnresolvedDependencyException (
										string.Format("The following dependency for the module {0} could not be resolved: ({3} operator)\n\t{1} ({2})",
											_info.Name, _constraint.Name, _constraint.Version.ToString(), OpToString (_op))
									);
							}
						}
						if (!checking) {
							_controller.UnloadModule (_constraint.Name);
						}
					}
				}
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
				case DepOps.NotLoaded:
					return "!#";
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
				case DepOps.Loaded:
					return true;
			}
			
			return false;				
		}
		
		protected void InternalResolve (ArrayList _parents, ModuleInfo _info, bool checking) {
			if (_info.Dependencies == null)
				return;
			
			if (_parents == null)
				_parents = new ArrayList ();
			
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
