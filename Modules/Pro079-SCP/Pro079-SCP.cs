using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace pro079mtf
{
	[PluginDetails(
		author = "RogerFK",
		name = "Pro 079 MTF",
		description = "Tesla command for Pro-079.",
		id = "rogerfk.pro079.tesla",
		version = "2.0",
		configPrefix = "p079_info",
		langFile = "p079scp",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
		)]

	public class Pro079 : Plugin, I079Command
	{
		public override void OnDisable()
		{
			this.Info("Pro079 SCP command disabled.");
		}
		public override void OnEnable()
		{
			this.Info("Pro079 SCP command enabled");
		}
		[LangOption]
		private readonly string usage = "Usage: .079 scp (173/096/106/049/939) (unknown/tesla/mtf/decont) - $min AP";
		public override void Register()
		{
			this.Info("Loading Pro-079 SCP command configs and registering the command...");
			// Command configs
			AddConfig(new ConfigSetting("p079_scp", true, true, "Enables/disables Pro-079's fake SCP death messages"));
			AddConfig(new ConfigSetting("p079_scp_cooldown", 30f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_scp_cost", 40, true, "AP cost for 079 to fake a SCP death message"));
			AddConfig(new ConfigSetting("p079_scp_level", 2, true, "Minimum level for 079 to fake a SCP death message"));
			AddConfig(new ConfigSetting("p079_scp_list", new string[] { "173", "096", "106", "049", "939" }, true, "List of allowed SCPs you can type in the .079 scp command"));

			// Cmds and Help strings
			AddTranslation(new LangSetting("scpuse", "Usage: .079 scp (173/096/106/049/939) (unknown/tesla/mtf/decont) - $min AP", lang));
			AddTranslation(new LangSetting("scpexist", "Type a SCP that exists", lang));
			AddTranslation(new LangSetting("scpway", "Type a method that exists", lang));
			AddTranslation(new LangSetting("nomtfleft", "No MTF's alive. Sending as \"unknown\"", lang));

			//
			AddTranslation(new LangSetting("scphelp", "<b>.079 scp <###> <reason></b> - Fakes an SCP (173, 096...) death, the reason can be: unknown, tesla, mtf, decont", lang));
		}
	}
}