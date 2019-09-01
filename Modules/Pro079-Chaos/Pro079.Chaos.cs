using Pro079Core;
using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace ChaosCommand
{
	[PluginDetails(
		author = "RogerFK",
		name = "Pro 079 Chaos",
		description = "Chaos command for Pro-079.",
		id = "rogerfk.pro079.chaos",
		version = "2.0",
		configPrefix = "p079_chaos",
		langFile = "p079chaos",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
		)]

	public class ChaosPlugin : Plugin
	{
		public override void OnDisable()
		{
			Info("Pro079 Chaos disabled.");
		}
		public override void OnEnable()
		{
			Info("Pro079 Chaos enabled");
		}
		[ConfigOption("")]
		public readonly bool enable = true;
		[ConfigOption]
		public readonly int cooldown = 50;
		[LangOption]
		public readonly int cost = 50;
		[ConfigOption]
		public readonly int level = 2;
		[ConfigOption]
		public readonly string msg = "warning . chaosinsurgency detected in the surface";
		[LangOption]
		public readonly string chaoscmd = "chaos";
		[LangOption]
		public readonly string chaoshelp = "Fakes the chaos coming";
		[LangOption]
		public readonly string ready = "<b><color=\"green\">Chaos command ready</color></b>";

		public override void Register()
		{
			Info("Loading Pro-079 Chaos configs and registering the command...");
			Pro079.Manager.RegisterCommand(new ChaosCommand(this));
		}
	}
}