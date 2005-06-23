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

namespace NModule.Dependency.Core {
	public class DepConstraint {
		private DepVersion _version;
		private string _name;

		public DepConstraint () {
			_name = "";
			_version = new DepVersion (-1, -1, -1, -1);
		}

		public DepVersion Version {
			get {
				return _version;
			}
			set {
				_version = value;
			}
		}

		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}

		public string VersionTmp {
			set {
				_version = DepVersion.VersionParse(value);
			}
		}
	}
}
