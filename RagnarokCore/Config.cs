using Exiled.API.Interfaces;
using System.ComponentModel;
using System.Collections.Generic;

namespace RagnarokCore
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("Tags Multicolor")]
		public string[] Sequences { get; set; } = new string[]
		{
			"red",
			"orange",
			"yellow",
			"green",
			"blue_green",
			"magenta",
			"silver",
			"crimson"
		};
		public List<string> RoleRainbowTags { get; set; } = new List<string>
		{
			"owner"
		};
		public float ColorInterval { get; set; } = 0.5f;
		[Description("Cassie Respawn Chaos")]
		public string ChaosCassie { get; set; } = string.Empty;
		[Description("Caramelo Features")]
		public bool Check330Eating { get; set; } = true;
	}
}
