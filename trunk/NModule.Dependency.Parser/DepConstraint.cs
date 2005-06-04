using System;
using System.Collections;

namespace NModule.Dependency.Parser
{
	public class DepConstraint
	{
		private DepVersion _version;
		private string _name;

		public DepConstraint()
		{
			_name = "";
			_version = null;
		}

		public DepVersion Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public string VersionTmp
		{
			set
			{
				_version = VersionParse(value);
			}
		}

		protected DepVersion VersionParse(string v)
		{
			// Here we go :)
			DepVersion ver = new DepVersion();
			string[] vparts = v.Split('.');
			ver.Major = Int32.Parse(vparts[0]);
			ver.Minor = Int32.Parse(vparts[1]);
			if (vparts.Length > 2)
				ver.Build = Int32.Parse(vparts[2]);
			if (vparts.Length > 3)
				ver.Build = Int32.Parse(vparts[3]);
			return ver;
		}
	}
}

