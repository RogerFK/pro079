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
			this.Info("Loading Pro-079 Chaos configs and registering the command...");

			// Command configs
			AddConfig(new ConfigSetting("p079_chaos", false, true, "Enables/disables Pro-079's fake Chaos messages"));
			AddConfig(new ConfigSetting("p079_chaos_cooldown", 50f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_chaos_level", 2, true, "Minimum level for 079 to fake a Chaos message"));
			AddConfig(new ConfigSetting("p079_chaos_cost", 70, true, "AP cost for 079 to fake a Chaos message"));
			AddConfig(new ConfigSetting("p079_chaos_msg", "warning . chaosinsurgency detected in the surface", true, "Message for the chaos command"));

			// Cmds and Help strings
			
		}
	}
}