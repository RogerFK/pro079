using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace pro079tesla
{
	[PluginDetails(
		author = "RogerFK",
		name = "Pro 079 Tesla",
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
			this.Info("Loading Pro-079 Tesla configs and registering the command...");

			// Command configs
			AddConfig(new ConfigSetting("p079_info", true, true, "Enables/disables Pro-079's info functionality"));
			AddConfig(new ConfigSetting("p079_info_alive", 1, true, "Minimum level to display the info about how many people are alive"));
			AddConfig(new ConfigSetting("p079_info_decont", 2, true, "Minimum level to display how many Class D and Scientists are alive"));
			AddConfig(new ConfigSetting("p079_info_escaped", 2, true, "Minimum level to display the info about how many people escaped"));
			AddConfig(new ConfigSetting("p079_info_plebs", 2, true, "Minimum level to display how many Class D and Scientists are alive"));
			AddConfig(new ConfigSetting("p079_info_mtfci", 3, true, "Minimum level to display how many MTF and Chaos are alive"));
			AddConfig(new ConfigSetting("p079_info_gens", 1, true, "Minimum level to display info about the generators"));
			AddConfig(new ConfigSetting("p079_info_mtfest", 3, true, "Minimum level to display info about the time that MTF will come"));
			// Cmds and Help strings
			AddTranslation(new LangSetting("decontdisabled", "Decontamination is disabled", lang));
			AddTranslation(new LangSetting("deconthappened", "LCZ is decontaminated", lang));
			// This happens when the nuke goes off before decont, but I don't know how it works and how many minutes it adds, because I saw at one time -3 mins.
			AddTranslation(new LangSetting("decontbug", "should have happened", lang));
			AddTranslation(new LangSetting("mtfest0", "between $(min)s and $(max)s", lang));
			AddTranslation(new LangSetting("mtfest1", "less than $(max)", lang));
			AddTranslation(new LangSetting("mtfest2", "are respawning / should have already respawned", lang));
			//SCP alive: $scpalive\\nHumans alive: $humans | N N N N N N N : $estMTF\\nTiempo hasta la descontami: $decont\\nClase D escapados: $cdesc | Científicos escapados: $sciesc\\nClase D vivos:     $cdalive | Chaos vivos:           $cialive\\nCientíficos vivos: $scialive | MTF vivos:             $mtfalive
			AddTranslation(new LangSetting("infomsg", "SCP alive: $scpalive\\nHumans alive: $humans | Next MTF/Chaos: $estMTF\\nTime until decontamination: $decont\\nEscaped Class Ds:  $cdesc | Escaped scientists:    $sciesc\\nAlive Class-Ds:    $cdalive | Alive chaos:           $cialive\\nAlive scientists:  $scialive | Alive MTFs:            $mtfalive", lang));

			AddTranslation(new LangSetting("lockeduntil", "Locked until level $lvl", lang));
			AddTranslation(new LangSetting("generators", "Generators:", lang));

			AddTranslation(new LangSetting("generatorin", "$room's generator", lang));
			AddTranslation(new LangSetting("activated", "is activated.", lang));
			AddTranslation(new LangSetting("hastablet", "has a tablet", lang));
			AddTranslation(new LangSetting("notablet", "doesn't have a tablet", lang));

			AddTranslation(new LangSetting("timeleft", "and has $secs remaining", lang));
		}
	}
}