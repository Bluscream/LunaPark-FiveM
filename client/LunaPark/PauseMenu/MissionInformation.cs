using System;
using System.Collections.Generic;

namespace LunaPark.PauseMenu
{
	public class MissionInformation
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public MissionLogo Logo { get; set; }

		public List<Tuple<string, string>> ValueList { get; set; }

		public MissionInformation(string name, IEnumerable<Tuple<string, string>> info)
		{
			Name = name;
			ValueList = new List<Tuple<string, string>>(info);
		}

		public MissionInformation(string name, string description, IEnumerable<Tuple<string, string>> info)
		{
			Name = name;
			Description = description;
			ValueList = new List<Tuple<string, string>>(info);
		}
	}
}
