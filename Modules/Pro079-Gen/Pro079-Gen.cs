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
		langFile = "p079info",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
		)]

	public class Pro079 : Plugin, I079Command
	{
		public override void OnDisable()
		{
			this.Info("Pro079 Info disabled.");
		}
		public override void OnEnable()
		{
			this.Info("Pro079 Info enabled");
		}
		public override void Register()
		{
			this.Info("Loading Pro-079 MTF configs and registering the command...");

			// Command configs
			AddConfig(new ConfigSetting("p079_gen", true, true, "Enables/disables Pro-079's fake SCP death messages"));
			AddConfig(new ConfigSetting("p079_gen_cooldown", 60f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_gen_cost", 40, true, "Cost for the command"));
			AddConfig(new ConfigSetting("p079_gen_cost_blackout", 40, true, "Penalty for the Gen5 and Gen6 commands"));
			AddConfig(new ConfigSetting("p079_gen_level", 2, true, "Minimum level for 079 to fake a generator"));
			AddConfig(new ConfigSetting("p079_gen_level_blackout", 3, true, "Minimum level for 079 to fake gen 5 and gen 6"));
			AddConfig(new ConfigSetting("p079_gen_penalty", 60f, true, "For how long there will be a penalty after using gen 5 or gen 6"));

			// Cmds and Help strings
			AddTranslation(new LangSetting("genuse", "Usage: .079 gen (1-6) - Will announce there are X generator activated, or will fake your death if you ttype 6. 5 generators will fake your whole recontainment process. - $min AP", lang));
			AddTranslation(new LangSetting("gen5msg", "Success. Your recontainment procedure, including when lights are turned off and a message telling you died, will be played.", lang));
			AddTranslation(new LangSetting("gen6msg", "Fake death command launched.", lang));
		}
	}
}