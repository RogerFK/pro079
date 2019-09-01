using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace LockdownUltimate
{
	[PluginDetails(
		author = "RogerFK",
		name = "Pro 079 Lockdown",
		description = "Lockdown ultimate for Pro-079.",
		id = "rogerfk.pro079.lockdown",
		version = "2.0",
		configPrefix = "p079_lockdown",
		langFile = "p079lockdown",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
		)]

	public class LockdownPlugin : Plugin
	{
		public override void OnDisable()
		{
			this.Info("Pro079 Lockdown disabled.");
		}
		public override void OnEnable()
		{
			this.Info("Pro079 Lockdown enabled");
		}
		[ConfigOption]
		public readonly bool enabled = true;
		[ConfigOption]
		public readonly int time = 60;
		[ConfigOption]
		public readonly string info = "makes humans unable to open doors that require a keycard, but SCPs can open any";
		[ConfigOption]
		public readonly int cooldown = 180;
		[ConfigOption]
		public readonly int cost = 50;
		public override void Register()
		{
			this.Info("Loading Pro-079 Chaos configs and registering the command...");
			Pro079Core.Pro079.Manager.RegisterUltimate(new LockdownUltimate(this));
		}
	}
}