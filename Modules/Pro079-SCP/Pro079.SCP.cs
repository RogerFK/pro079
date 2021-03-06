﻿using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace SCPCommand
{
	[PluginDetails(
		author = "RogerFK",
		name = "Pro 079 SCP",
		description = "SCP command for Pro-079.",
		id = "rogerfk.pro079.scp",
		version = "2.0",
		configPrefix = "p079_scp",
		langFile = "p079scp",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
		)]

	public class SCPPlugin : Plugin
	{
		public override void OnDisable()
		{
			Info("Pro079 SCP command disabled.");
		}
		public override void OnEnable()
		{
			Info("Pro079 SCP command enabled");
		}
		[ConfigOption]
		public readonly bool enable = true;
		[ConfigOption]
		public readonly int cooldown = 30;
		[ConfigOption]
		public readonly int cost = 40;
		[ConfigOption]
		public readonly int level = 1;
		[ConfigOption]
		public readonly string[] list = new string[] { "173", "096", "106", "049", "939" };
		[LangOption]
		public readonly string scpextrainfo = "<###> <reason>";
		[LangOption]
		public readonly string scpusage = "Fakes an SCP(173, 096...) death, the reason can be: unknown, tesla, mtf, decon";
		[LangOption]
		public readonly string scpuse = "Usage: .079 scp (173/096/106/049/939) (unknown/tesla/mtf/decont) - $min AP";
		[LangOption]
		public readonly string scpexist = "Type a SCP that exists";
		[LangOption]
		public readonly string scpway = "Type a method that exists";
		[LangOption]
		public readonly string scpnomtfleft = "No MTF's alive. Sending as \"unknown\"";
		[LangOption]
		public readonly string scpcmd = "scp";
		[LangOption]
		public readonly string scpready = "<b><color=\"red\">SCP command ready</color></b>";

		public override void Register()
		{
			Info("Loading Pro-079 SCP command configs and registering the command...");

			Pro079Core.Pro079.Manager.RegisterCommand(new SCPCommand(this));
		}
	}
}