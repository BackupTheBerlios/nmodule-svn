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


