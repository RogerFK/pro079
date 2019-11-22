using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace Pro079MTF
{
	[PluginDetails(
		author = "RogerFK",
		name = "Pro 079 MTF",
		description = "MTF command for Pro-079.",
		id = "rogerfk.pro079.mtf",
		version = "2.0",
		configPrefix = "p079_mtf",
		langFile = "p079mtf",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
		)]

	public class MTFPlugin : Plugin
	{
		public override void OnDisable()
		{
			Info("Pro079 MTF disabled.");
		}
		public override void OnEnable()
		{
			Info("Pro079 MTF enabled");
		}
		[ConfigOption]
		public readonly bool enable = true;
		[ConfigOption]
		public readonly int cooldown = 60;
		[ConfigOption]
		public readonly int level = 2;
		[ConfigOption]
		public readonly int cost = 70;
		[ConfigOption]
		public readonly int maxscp = 5;

		[LangOption]
		public readonly string mtfcmd = "mtf";
		[LangOption]
		public readonly string mtfuse = "Usage: .079 mtf (p) (5) (4), will say Papa-5 is coming and there are 4 SCP remaining - $min ap";
		[LangOption]
		public readonly string mtfmaxscp = "Maximum SCPs: $max";
		[LangOption]
		public readonly string mtfusage = "<character> <number> <alive-scps>";
		[LangOption]
		public readonly string mtfextendedHelp = "Announces that a new MTF squad arrived, with your own custom number of SCPs";
		[LangOption]
		public readonly string mtfready = "<b><color=\"blue\">MTF command ready!</color></b>";
		public override void Register()
		{
			Info("Loading Pro-079 MTF configs and registering the command...");
			Pro079Core.Pro079.Manager.RegisterCommand(new MTFCommand(this));
		}
	}
}