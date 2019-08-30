using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace LockdownUltimate
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

	public class BlackoutUltimate : Plugin
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
			// Cmds and Help strings

			// Register command
			// new blabla
			AddTranslation(new LangSetting("chaoshelp", "<b>.079 chaos</b> - Announces the chaos comming"));
		}
	}
}