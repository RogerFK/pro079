using System.Collections.Generic;
using System.Linq;
using Pro079Core.API;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace BlackoutUltimate
{
	internal class BlackoutLogic : IEventHandlerWaitingForPlayers, IUltimate079
	{
		private readonly BlackoutUltimate plugin;
		public BlackoutLogic(BlackoutUltimate plugin)
		{
			this.plugin = plugin;
		}
		public string Name => "Blackout";

		public string Info => plugin.p0blackoutinfo.Replace("$", plugin.minutes != 1 ? "s" : string.Empty).Replace("{min}", plugin.minutes.ToString());

		public int Cooldown => plugin.cooldown;

		public int Cost => plugin.cost;

		public Room[] Rooms { get; private set; }

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			Rooms = PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(r => r.ZoneType != ZoneType.ENTRANCE).ToArray();
		}

		public string TriggerUltimate(string[] args, Player Player)
		{
			int p = (int)System.Environment.OSVersion.Platform;
			if ((p == 4) || (p == 6) || (p == 128)) MEC.Timing.RunCoroutine(ShutDownLights(), MEC.Segment.Update);
			else MEC.Timing.RunCoroutine(ShutDownLights(), 1);
			return Pro079Core.Configs.Instance.UltimateLaunched;
		}

		private IEnumerator<float> ShutDownLights()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("warning . malfunction detected on heavy containment zone . Scp079Recon6 . . . light systems Disengaged");
			yield return MEC.Timing.WaitForSeconds(12.1f);
			float start = PluginManager.Manager.Server.Round.Duration;
			while (start + plugin.minutes * 60f > PluginManager.Manager.Server.Round.Duration)
			{
				foreach (var room in Rooms)
				{
					room.FlickerLights(8f);
				}
				yield return MEC.Timing.WaitForSeconds(8f);
			}
		}
	}
}
