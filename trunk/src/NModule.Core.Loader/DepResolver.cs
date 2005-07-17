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
	
	/// <summary>
	/// The DepResolver class handles resolving
	/// dependencies given a dependency tree, and loads needed
	/// modules or throws exceptions.  For a description of
	/// search paths see <see href="searching.html">this</see>.
	/// </summary>
	/// <remarks>None.</remarks>
	/// <preliminary/>
	public class DepResolver {
#region Members
		/// <summary>
		/// A <see cref="ModuleController"/> object that is used for loading dependencies.
		/// </summary>
		/// <remarks>None.</remarks>
		protected ModuleController _controller;
		
		/// <summary>
		/// A list of directories to search for modules.
		/// </summary>
		protected ArrayList _search_path;
#endregion

#region Constructor
		/// <summary>
		/// Creates a new DepResolver class with the the given
		/// <see cref="ModuleController" /> object and search path.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="controller">The <see cref="ModuleController"/> object used to load modules.</param>
		/// <param name="search_path">The initial search path.</param>
		public DepResolver (ModuleController controller, ArrayList search_path) {
			_controller = controller;
			_search_path = search_path;
		}
#endregion

#region Internal Helper Functions
		/// <summary>
		/// Searches for a given module along the search path.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_name">The name of the module to search for, without the .dll extension.</param>
		/// <returns>Returns a string giving the full path to the module or null if the module was not found.</returns>
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
		
		/// <summary>
		/// Recursively resolves dependencies given a dependency tree.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_node">The root node of the tree to resolve.</param>
		/// <param name="_parents">The parent modules to use when checking for circular dependencies.</param>
		/// <param name="_info">The <see cref="ModuleInfo" /> object representing the module to resolve dependencies for.</param>
		/// <param name="opt">Set to true if this pass is optional, otherwise set to false.</param>
		/// <exception cref="UnresolvedDependencyException">Thrown if the given dependency cannot be resolved and is not optional.</exception>
		/// <exception cref="CircularDependencyException">Thrown if two modules depend on each other.</exception>
		protected void OpResolve (DepNode _node, ArrayList _parents, ModuleInfo _info, bool checking, bool opt) {
			bool _ret;
			DepOps _op = _node.DepOp;
			
			// This is a large process.  I've chosen to use recursion here because it makes sense from a tree
			// point of view.  Our first step is to check our operator to see if its one of the "combination"
			// operators.  If it is, we simply recurse through its children and record their results in a
			// table (true if they suceeded, false if they failed).  Since we're not sure what kind of logic
			// we need (and, or, xor) we can't really make a decision at this point.  When we go through
			// the children, they simply go through this process to until we reach a bottom node, which would
			// be one of the "single" operators.  They are dealt with in the second part of this function.
			if ((_op == DepOps.And) || (_op == DepOps.Opt) || (_op == DepOps.Or) || (_op == DepOps.Xor)) {
				// This is our list of results for the children nodes.
				ArrayList _results = new ArrayList ();
				
				// This is a list of constraints for the children nodes so we can output sensible information.
				ArrayList _c = new ArrayList ();
				
				foreach (DepNode _child in _node.Children) {
					// Try to resolve the child node, if it suceeds, store
					// true into the results table, if an exception is thrown
					// store false into the results table.  Store the constraints
					// into the constraints table regardless of result so the indexes
					// will be identical.
					try {
						OpResolve (_child, _parents, _info, checking, _op == DepOps.Opt ? true : false);
						_results.Add (true);
						_c.Add (_child.Constraint);
					} catch (Exception e) {
						_results.Add (false);
						_c.Add (_child.Constraint);
					}
				}
				
				// This switch statement is where we go through the results table and apply
				// the operator logic to the table to figure out if the results sastify
				// the given operator.
				switch (_op) {
					case DepOps.And:
						int r = 0;
						
						foreach (bool _result in _results) {
							if (!_result) {
								if (!opt) {
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
							if (!opt)
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
							if (!opt) {
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
					if (!opt) {
								throw new UnresolvedDependencyException (
									string.Format("The following dependency for the module {0} could not be resolved: ({3} operator)\n\t{1} ({2})",
										_info.Name, _constraint.Name, _constraint.Version.ToString(), OpToString (_op))
								);
					}
				}
				
				if (_op == DepOps.Equal || _op == DepOps.GreaterThan || _op == DepOps.GreaterThanEqual || _op == DepOps.LessThan || _op == DepOps.LessThanEqual || _op == DepOps.NotEqual) {
					if (!IsEmptyVersion (_constraint.Version)) {
						if (!VersionMatch (_ninfo.Version, _constraint.Version, _op)) {
							if (!opt) {
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
						if (!opt) {
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
							if (!opt) {
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
		
		/// <summary>
		/// Converts a given <see cref="DepOps" /> object into a string.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_op">The operator to convert to a string.</param>
		/// <returns>A string value that matches the operator in the <see href="depstring.html">dependency operator table</see>.</returns>
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
		
		/// <summary>
		/// Determines if a given version is empty or not.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_ver">A <see cref="DepVersion" /> object to check.</param>
		/// <returns>Returns true if the version given is empty (ie -1:-1:-1:-1) or false otherwise.</returns>
		protected bool IsEmptyVersion (DepVersion _ver) {
			if (_ver == null)
				return true;
				
			return ((_ver.Major == -1) && (_ver.Minor == -1) && (_ver.Build == -1) && (_ver.Revision == -1));
		}
		
		/// <summary>
		/// Checks if the version given matches the constraint
		/// based on the given operator.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_ver">The version to compare to the constraint.</param>
		/// <param name="_dver">The constraint version.</param>
		/// <param name="_op">The operator to use to determine if the match is sucessful.</param>
		/// <returns>Returns true is the version matches the constraint version with the given operator, otherwise returns false.</returns>
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
		
		/// <summary>
		/// Helper function that is the actual resolving function.
		/// See <see cref="ResolveCheck" /> and <see cref="Resolve" /> for
		/// the API to call this method.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_parents">The list of parent modules to use for checking for circular dependencies.</param>
		/// <param name="_info">The <see cref="ModuleInfo" /> object representing the current module.</param>
		/// <param name="checking">A bool to determine whether or not we are just checking, or actually loading.</param>
		protected void InternalResolve (ArrayList _parents, ModuleInfo _info, bool checking) {
			if (_info.Dependencies == null)
				return;
			
			if (_parents == null)
				_parents = new ArrayList ();
			
			OpResolve (_info.Dependencies, _parents, _info, checking, false);		
		}
#endregion

#region Resolvers
		/// <summary>
		/// Performs a resolution check without actually loading anything.
		/// This is useful to determine if the dependencies can be resolved
		/// before actually loading anything.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_parents">The list of parent modules to use for checking for circular dependencies.</param>
		/// <param name="_info">A <see cref="ModuleInfo" /> object representing the current module.</param>
		public void ResolveCheck (ArrayList _parents, ModuleInfo _info) {
			InternalResolve (_parents, _info, true);
		}
		
		/// <summary>
		/// Performs a resolution check and loads the modules needed
		/// to sastify the dependencies.
		/// </summary>
		/// <remarks>None.</remarks>
		/// <param name="_parents">The list of parent modules to use for checking for circular dependencies.</param>
		/// <param name="_info">A <see cref="ModuleInfo" /> object representing the current module.</param>
		public void Resolve (ArrayList _parents, ModuleInfo _info) {
			InternalResolve (_parents, _info, false);
		}
#endregion
	}
}
