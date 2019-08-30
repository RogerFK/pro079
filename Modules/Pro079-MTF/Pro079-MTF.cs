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
			AddConfig(new ConfigSetting("p079_mtf", true, true, "Enables/disables Pro-079's fake MTF messages"));
			AddConfig(new ConfigSetting("p079_mtf_cooldown", 60f, true, "How many seconds the command will give a cooldown for itself"));
			AddConfig(new ConfigSetting("p079_mtf_level", 2, true, "Minimum level for 079 to fake a MTF message"));
			AddConfig(new ConfigSetting("p079_mtf_cost", 70, true, "AP cost for 079 to fake a MTF message"));
			AddConfig(new ConfigSetting("p079_mtf_maxscp", 5, true, "Max SCP input so the guy doesn't spam it, or spams it"));

			// Cmds and Help strings
			AddTranslation(new LangSetting("mtfuse", "Usage: .079 mtf (p) (5) (4), will say Papa-5 is coming and there are 4 SCP remaining - $min ap", lang));
			AddTranslation(new LangSetting("mtfmaxscp", "Maximum SCPs: $max", lang));

			//
			AddTranslation(new LangSetting("mtfhelp", "<b>.079 mtf <character> <number> <alive-scps></b> - Announces that a new MTF squad arrived, with your own custom number of SCPs", lang)); //Lanza un mensaje sobre que ha llegado la MTF a la zona con un número que elijas de SCPs con vida"
		}
	}
}