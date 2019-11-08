using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;
using Pro079Core;

namespace InfoCommand
{
	[PluginDetails(
		author = "RogerFK",
		name = "Pro 079 Info",
		description = "Info command for Pro-079.",
		id = "rogerfk.pro079.info",
		version = "2.0",
		configPrefix = "p079_info",
		langFile = "p079info",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
		)]

	public class InfoPlugin : Plugin
	{
		public override void OnDisable()
		{
			Info("Pro079 Info disabled.");
		}
		public override void OnEnable()
		{
			Info("Pro079 Info enabled.");
		}
		[ConfigOption]
		public readonly bool enable = true;
		[ConfigOption]
		public readonly int alive = 1;
		[ConfigOption]
		public readonly int decont = 2;
		[ConfigOption]
		public readonly int escaped = 2;
		[ConfigOption]
		public readonly int plebs = 2;
		[ConfigOption]
		public readonly int mtfci = 3;
		[ConfigOption]
		public readonly int gens = 1;
		[ConfigOption]
		public readonly int mtfest = 3;
		[ConfigOption]
		public readonly bool mtfop = true;
		[ConfigOption]
		public readonly bool longTime = true;
		[ConfigOption]
		public readonly string color = "red";

		[LangOption]
		public readonly string infocmd = "info";
		[LangOption]
		public readonly string decontdisabled = "Decontamination is disabled";
		[LangOption]
		public readonly string deconthappened = "LCZ is decontaminated";
		[LangOption]
		public readonly string decontbug = "should have happened";
		[LangOption]
		public readonly string mtfRespawn = "in $time";
		[LangOption]
		public readonly string mtfest0 = "between $(min)s and $(max)s";
		[LangOption]
		public readonly string mtfest1 = "less than $(max)";
		[LangOption]
		public readonly string mtfest2 = "are respawning / should have already respawned";
		[LangOption]
		public readonly string infomsg = "SCP alive: $scpalive\\nHumans alive: $humans | Next MTF/Chaos: $estMTF\\nTime until decontamination: $decont\\nEscaped Class Ds:  $cdesc | Escaped scientists:    $sciesc\\nAlive Class-Ds:    $cdalive | Alive chaos:           $cialive\\nAlive scientists:  $scialive | Alive MTFs:            $mtfalive";
		[LangOption]
		public readonly string lockeduntil = "Locked until level $lvl";
		[LangOption]
		public readonly string generators = "Generators:";
		[LangOption]
		public readonly string generatorin = "$room's generator";
		[LangOption]
		public readonly string activated = "is activated.";
		[LangOption]
		public readonly string hastablet = "has a tablet";
		[LangOption]
		public readonly string notablet = "doesn't have a tablet";
		[LangOption]
		public readonly string timeleft = "and has $secs remaining";
		[LangOption]
		public readonly string infoextrahelp = "Shows stuff about the facility";
		[LangOption]
		public readonly string minutes = "minute$";
		[LangOption]
		public readonly string seconds = "second$";
		[LangOption]
		public readonly string and = "and";
		// I think this thing fucks germans and others.
		[LangOption]
		public readonly string pluralSuffix = "s";

		public override void Register()
		{
			Info("Loading Pro-079 Tesla configs and registering the command...");

			InfoCommand reference = new InfoCommand(this);
			AddEventHandlers(reference);
			Pro079.Manager.RegisterCommand(reference);
		}
	}
}