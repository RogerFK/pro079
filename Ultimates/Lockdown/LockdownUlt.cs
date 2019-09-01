using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pro079Core.API;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace LockdownUltimate
{
	public class LockdownUltimate : IEventHandlerDoorAccess, IUltimate079
	{
		private LockdownPlugin plugin;
		public bool CurrentlyRunning { get; private set; }
		public string Name => "Lockdown";
		public string Info => plugin.info;
		public int Cooldown => plugin.cooldown;
		public int Cost => plugin.cost;
		public LockdownUltimate(LockdownPlugin plugin)
		{
			this.plugin = plugin;
		}
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			CurrentlyRunning = false;
		}
		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
			if (CurrentlyRunning == false || string.IsNullOrWhiteSpace(ev.Door.Permission))
			{
				return;
			}
			else
			{
				if (ev.Player.TeamRole.Team == Smod2.API.Team.SCP)
				{
					ev.Allow = true;
				}
				else
				{
					ev.Allow = false;
				}
			}
		}
		private IEnumerator<float> Ult2Toggle()
		{
			CurrentlyRunning = true;
			yield return MEC.Timing.WaitForSeconds(plugin.time);
			CurrentlyRunning = false;
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("attention all Personnel . doors lockdown finished");
		}

		public string TriggerUltimate(string[] args, Player Player)
		{
			int p = (int)System.Environment.OSVersion.Platform;
			if ((p == 4) || (p == 6) || (p == 128)) MEC.Timing.RunCoroutine(Ult2Toggle(), MEC.Segment.Update);
			else MEC.Timing.RunCoroutine(Ult2Toggle(), 1);
			return Pro079Core.Configs.Instance.UltimateLaunched;
		}
	}
}
