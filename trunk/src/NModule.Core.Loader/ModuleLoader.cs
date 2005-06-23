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
	// This class is simply the loader class, it just creates a new AppDomain,
	// and loads the appropriate assembly into it.  It can also load an assembly into
	// an existing app-domain (for example, grouped dependencies).  See the configuration
	// options for an example.
	public class ModuleLoader {
		public static AppDomain Load (string module_name) {
			AppDomain _myDomain = AppDomain.CreateDomain (module_name);
			
			return Load (_myDomain, module_name);
		}
		
		public static AppDomain Load (AppDomain domain, string module_name) {
			try {
				domain.Load (module_name);
			} catch (Exception e) {
				// Logger logger = GlobalSettings.Logger;
				// LogID id = logger.WriteLogMessage (LogType.Error, "Caught an exception trying to load {0}: ", module_name);
				// logger.WriteException (id, e.Message);
				// logger.WriteStackTrace (id, e.Message);
				// logger.CommitMessage (id);
				throw e;
			}
			
			return domain;
		}
	}
}
