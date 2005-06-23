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
 
using System;
using System.Collections;
using System.Reflection;

namespace NModule.Core.Loader {
	public delegate ModuleRoleHandler (Assembly asm, Type basetype);
	
	// FIXME:  Document this class using NDoc tags.
	/*
	 * This class handles the roles used by the module loader.
	 * It represents a role given its name, base type, and the
	 * handler used to instantiate that role.  This role is opaque,
	 * and could easily be used as a value type, but I feel this is
	 * the best way to go to ensure future changes dont require semantic
	 * changes to the engine.
	 */
	public class ModuleRole {
		private Type _baseType;
		private string _roleName;
		private ModuleRoleHandler _handler;
		
		// Lets get this baby setup :)
		public ModuleRole (Type basetype, string name, ModuleRoleHandler handler) {
			_baseType = basetype;
			_roleName = name;
			_handler = handler;
		}
		
		public InitiateRole (Assembly asm, Type type) {
			handler (asm, type);
		}
		
		// all properties are read-only for the moment except the handler (which could conceivably change as other modules are loaded which may take the load
		// off the main engine)
		public Type BaseType {
			get {
				return _baseType;
			}
		}
		
		public string RoleName {
			get {
				return _roleName;
			}
		}
		
		public ModuleRoleHandler Handler {
			get {
				return _handler;
			}
			set {
				_handler = value;
			}
		}
	}
}