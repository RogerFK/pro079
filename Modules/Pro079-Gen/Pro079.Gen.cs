using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace GeneratorCommand
{
	[PluginDetails(
		author = "RogerFK",
		name = "Pro 079 Generator",
		description = "Generator command for Pro-079.",
		id = "rogerfk.pro079.gen",
		version = "2.0",
		configPrefix = "p079_gen",
		langFile = "p079gen",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
		)]

	public class GeneratorPlugin : Plugin
	{
		public override void OnDisable()
		{
			Info("Pro079 Generator disabled.");
		}
		public override void OnEnable()
		{
			Info("Pro079 Generator enabled");
		}

		[ConfigOption]
		public readonly bool enable = true;
		[ConfigOption]
		public readonly int cooldown = 60;
		[ConfigOption]
		public readonly int cost = 40;
		[ConfigOption]
		public readonly int costBlackout = 40;
		[ConfigOption]
		public readonly int level = 2;
		[ConfigOption]
		public readonly int levelBlackout = 3;
		[ConfigOption]
		public readonly int penalty = 60;

		[LangOption]
		public readonly string gencmd = "gen";
		[LangOption]
		public readonly string genuse = "Usage: .079 gen (1-6) - Will announce there are X generator activated, or will fake your death if you type 6. 5 generators will fake your whole recontainment process. - $min AP";
		[LangOption]
		public readonly string gen5msg = "Success. Your recontainment procedure, including when lights are turned off and a message telling you died, will be played.";
		[LangOption]
		public readonly string gen6msg = "Fake death command launched.";
		[LangOption]
		public readonly string genusage = "Announces that X generators are enabled, if it's 6 it will fake your suicide";
		[LangOption]
		public readonly string ready = "<b>Generator command ready</b>";

		public override void Register()
		{
			Info("Loading Pro-079 Generator configs and registering the command...");
			Pro079Core.Pro079.Manager.RegisterCommand(new GenCommand(this));
		}
	}
}