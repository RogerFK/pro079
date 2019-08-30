using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2;
using Smod2.EventHandlers;
using Smod2.Events;

namespace LockdownUltimate
{
	class BlackoutLogic : IEventHandlerWaitingForPlayers, IUltimate079
	{
		private BlackoutUltimate plugin;
		public BlackoutLogic(BlackoutUltimate plugin)
		{
			this.plugin = plugin;
		}
		public FlickerableLight[] FlickerableLightsArray { get; private set; }
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			FlickerableLightsArray = UnityEngine.Object.FindObjectsOfType<FlickerableLight>();
		}

		private IEnumerator<float> ShamelessTimingRunLights()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("warning . malfunction detected on heavy containment zone . Scp079Recon6 . . . light systems Disengaged");
			yield return MEC.Timing.WaitForSeconds(12.1f);
			float start = PluginManager.Manager.Server.Round.Duration;
			while (start + 30f > PluginManager.Manager.Server.Round.Duration)
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
