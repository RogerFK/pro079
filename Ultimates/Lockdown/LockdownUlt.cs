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
	class LockdownUltimate : IEventHandlerDoorAccess, IUltimate079
	{
		private LockdownPlugin plugin;
		public bool CurrentlyRunning { get; private set; }

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
		private IEnumerable<float> Ult2Toggle(float v)
		{
			CurrentlyRunning = true;
			yield return MEC.Timing.WaitForSeconds();
			CurrentlyRunning = false;
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("attention all Personnel . doors lockdown finished");
		}
	}
}
