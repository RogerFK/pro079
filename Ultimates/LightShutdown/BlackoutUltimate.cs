using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2;
using Smod2.EventHandlers;
using Smod2.Events;
using Pro079Core.API;
using Smod2.API;

namespace BlackoutUltimate
{
	class BlackoutLogic : IEventHandlerWaitingForPlayers, IUltimate079
	{
		private BlackoutUltimate plugin;
		public BlackoutLogic(BlackoutUltimate plugin)
		{
			this.plugin = plugin;
		}
		public FlickerableLight[] FlickerableLightsArray { get; private set; }

		public string Name => "Blackout";

		public string Info => plugin.info.Replace("$", plugin.minutes != 1 ? "s" : string.Empty);

		public int Cooldown => plugin.cooldown;

		public int Cost => plugin.cost;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			FlickerableLightsArray = UnityEngine.Object.FindObjectsOfType<FlickerableLight>();
		}

		public string TriggerUltimate(string[] args, Player Player)
		{
			int p = (int)System.Environment.OSVersion.Platform;
			if ((p == 4) || (p == 6) || (p == 128)) MEC.Timing.RunCoroutine(ShamelessTimingRunLights(), MEC.Segment.Update);
			else MEC.Timing.RunCoroutine(ShamelessTimingRunLights(), 1);
			return Pro079Core.Configs.Instance.UltimateLaunched;
		}

		private IEnumerator<float> ShamelessTimingRunLights()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("warning . malfunction detected on heavy containment zone . Scp079Recon6 . . . light systems Disengaged");
			yield return MEC.Timing.WaitForSeconds(12.1f);
			float start = PluginManager.Manager.Server.Round.Duration;
			while (start + plugin.minutes * 60f > PluginManager.Manager.Server.Round.Duration)
			{
				foreach (FlickerableLight light in FlickerableLightsArray)
				{
					light.EnableFlickering(60f - (PluginManager.Manager.Server.Round.Duration - start));
				}
				yield return MEC.Timing.WaitForSeconds(10f);
			}
		}
	}
}
