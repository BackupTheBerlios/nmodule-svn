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
 * the following conditions:                                              *
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
using System.IO;
using NModule.Dependency.Parser;
using antlr.collections;

namespace NModule
{
	public class NModuleTest
	{
		public static void PrintTree(DepNode root, int indent)
		{
			for(int i = 0; i < indent; i++)
				Console.Write("  ");
	
			Console.Write("op = {0}, ", root.DepOp.ToString());

			if (root.Constraint != null)
			{
				Console.Write("name = {0}", root.Constraint.Name);
				if (root.Constraint.Version != null)
				{
					DepVersion ver = root.Constraint.Version;
					Console.WriteLine(", version = {0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Patch);
				}
				else
					Console.WriteLine();
			}
			else
				Console.WriteLine();

			foreach(DepNode child in root.Children)
				PrintTree(child, indent + 1);
		}

	       	public static void Main(string[] args)
	        {
			foreach(string file in args)
			{
				DepNode root = new DepNode();

				Console.WriteLine("==== Testing Input from {0} ====", file);
				DepLexer lexer = new DepLexer(new FileStream(file, FileMode.Open));
		                DepParser parser = new DepParser(lexer);
	                	parser.expr(root);
                		PrintTree(root, 0);
			}
		
	        }
	}
}


