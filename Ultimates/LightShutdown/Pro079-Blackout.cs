using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace BlackoutUltimate
{
	[PluginDetails(
		author = "RogerFK",
		name = "Pro 079 Blackout",
		description = "Blackout ultimate for Pro-079.",
		id = "rogerfk.pro079.blackout",
		version = "2.0",
		configPrefix = "p079_blackout",
		langFile = "p079blackout",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
		)]

	public class BlackoutUltimate : Plugin
	{
		public override void OnDisable()
		{
			Info("Pro079 Blackout disabled.");
		}
		public override void OnEnable()
		{
			Info("Pro079 Blackout enabled");
		}
		[ConfigOption]
		public readonly bool enable = true;
		[ConfigOption]
		public readonly int cooldown = 180;
		[ConfigOption]
		public readonly int minutes = 1;
		[ConfigOption]
		public readonly int cost = 50;
		[LangOption]
		public readonly string info = "Shuts the facility down for {min} minute$";

		public override void Register()
		{
			BlackoutLogic reference = new BlackoutLogic(this);
			AddEventHandlers(reference);
			Pro079Core.Manager.Instance.RegisterUltimate(reference);
			Info("Registed Pro-079 Blackout.");
		}
	}
}