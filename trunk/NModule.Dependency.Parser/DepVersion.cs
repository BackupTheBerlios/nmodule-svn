namespace NModule.Dependency.Parser
{
	public class DepVersion
	{
		public DepVersion()
		{
			Major = Minor = Build = Patch = 0;
		}

		public int Major;
		public int Minor;
		public int Build;
		public int Patch;
	}
}
