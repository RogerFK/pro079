using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;
using Pro079Core.API;
using Pro079Core;

namespace InfoCommand
{
	class InfoCommand : IEventHandlerTeamRespawn, IEventHandlerSetConfig, ICommand079
	{
		private int LastMtfSpawn;
		private readonly InfoPlugin plugin;
		public InfoCommand(InfoPlugin plugin) => this.plugin = plugin;

		public bool OverrideDisable = false;
		private bool DeconBool;
		private float DeconTime;
		private int MinMTF;
		private int MaxMTF;

		public bool Disabled
		{
			get
			{
				return OverrideDisable ? OverrideDisable : !plugin.enabled;
			}
			set
			{
				OverrideDisable = value;
			}
		}

		public string Command => plugin.infocmd;

		public string ExtraArguments => string.Empty;

		public string HelpInfo => plugin.infoextrahelp;

		public bool Cassie => false;

		public int Cooldown => 0;

		public int MinLevel => 1;

		public int APCost => 5;

		public string CommandReady => string.Empty;

		public int CurrentCooldown
		{
			get => 0;
			set => CurrentCooldown = 0;
		}

		public string CallCommand(string[] args, Player player)
		{
			if (!plugin.enabled)
			{
				return Pro079.Configs.CommandDisabled;
			}
			int level = player.GetBypassMode() ? 5 : player.Scp079Data.Level + 1;
			string humansAlive;
			string decontTime;
			string ScientistsEscaped;
			string ClassDEscaped;
			string ClassDAlive;
			string ScientistsAlive;
			string MTFAlive;
			string CiAlive;
			string estMTFtime;

			humansAlive = (PluginManager.Manager.Server.Round.Stats.ClassDAlive + PluginManager.Manager.Server.Round.Stats.ScientistsAlive + PluginManager.Manager.Server.Round.Stats.CiAlive + PluginManager.Manager.Server.Round.Stats.NTFAlive).ToString();

			if (level < plugin.decont)
			{
				decontTime = '[' + Pro079.Configs.LevelString(plugin.decont, true) + ']';
			}
			else
			{
				if (DeconBool)
				{
					decontTime = plugin.decontdisabled;
				}
				else if (PluginManager.Manager.Server.Map.LCZDecontaminated)
				{
					decontTime = plugin.deconthappened;
				}
				else
				{
					float auxTime = (DeconTime - PluginManager.Manager.Server.Round.Duration / 60.0f);
					decontTime = auxTime > 0 ? auxTime.ToString("0.00") : plugin.decontbug;
				}
			}
			if (level < plugin.escaped)
			{
				ScientistsEscaped = '[' + Pro079.Configs.LevelString(plugin.escaped, true) + ']';
				ClassDEscaped = '[' + Pro079.Configs.LevelString(plugin.escaped, true) + ']';
			}
			else
			{
				ClassDEscaped = PluginManager.Manager.Server.Round.Stats.ClassDEscaped.ToString("00");
				ScientistsEscaped = PluginManager.Manager.Server.Round.Stats.ScientistsEscaped.ToString("00");
			}

			if (level < plugin.plebs)
			{
				ClassDAlive = '[' + Pro079.Configs.LevelString(plugin.plebs, true) + ']';
				ScientistsAlive = '[' + Pro079.Configs.LevelString(plugin.plebs, true) + ']';
			}
			else
			{
				ClassDAlive = PluginManager.Manager.Server.Round.Stats.ClassDAlive.ToString("00");
				ScientistsAlive = PluginManager.Manager.Server.Round.Stats.ScientistsAlive.ToString("00");
			}
			if (level < plugin.mtfci)
			{
				MTFAlive = '[' + Pro079.Configs.LevelString(plugin.mtfci, true) + ']';
				CiAlive = '[' + Pro079.Configs.LevelString(plugin.mtfci, true) + ']';
			}
			else
			{
				MTFAlive = PluginManager.Manager.Server.Round.Stats.NTFAlive.ToString("00");
				CiAlive = PluginManager.Manager.Server.Round.Stats.CiAlive.ToString("00");
			}
			if (level > plugin.mtfest)
			{

				if (PluginManager.Manager.Server.Round.Duration - LastMtfSpawn < MinMTF)
				{
					estMTFtime = plugin.mtfest0.Replace("$(min)", (MinMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0")).Replace("$(max)", (MaxMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0"));
				}
				else if (PluginManager.Manager.Server.Round.Duration - LastMtfSpawn < MaxMTF)
				{
					estMTFtime = plugin.mtfest1.Replace("$(max)", (MaxMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0"));
				}
				else
				{
					estMTFtime = plugin.mtfest2;
				}
			}
			else
			{
				estMTFtime = '[' + Pro079.Configs.LevelString(plugin.mtfest, true) + ']';

			}
			// This bs below can be optimized by using a dictionary or two arrays with its own function, but for now it's staying like this.
			string infomsg = plugin.infomsg
				.Replace("$scpalive", PluginManager.Manager.Server.Round.Stats.SCPAlive.ToString("0"))
				.Replace("$humans", humansAlive).Replace("$estMTF", estMTFtime)
				.Replace("$decont", decontTime)
				.Replace("$cdesc", ClassDEscaped).Replace("$sciesc", ScientistsEscaped)
				.Replace("$cdalive", ClassDAlive).Replace("$cialive", CiAlive)
				.Replace("$scialive", ScientistsAlive).Replace("$mtfalive", MTFAlive);
			player.SendConsoleMessage(infomsg.Replace("\\n", Environment.NewLine), "white");
			if (level > plugin.gens)
			{
				string ReturnMessage = plugin.generators;
				foreach (Generator generator in PluginManager.Manager.Server.Map.GetGenerators())
				{
					ReturnMessage += plugin.generatorin.Replace("$room", generator.Room.RoomType.ToString()) + ' ';
					if (generator.Engaged)
					{
						ReturnMessage += plugin.activated + '\n';
					}
					else
					{
						ReturnMessage += (generator.HasTablet ? plugin.hastablet : plugin.notablet) + ' ' + plugin.timeleft.Replace("$sec", generator.TimeLeft.ToString("0")) + '\n';
					}
				}
				return ReturnMessage;
			}
			else
			{
				return '[' + plugin.lockeduntil.Replace("$lvl", plugin.gens.ToString()) + ']';
			}
		}

		public void OnSetConfig(SetConfigEvent ev)
		{
			switch (ev.Key)
			{
				case "disable_decontamination":
					DeconBool = (bool)ev.Value;
					return;
				case "decontamination_time":
					DeconTime = (float)ev.Value;
					return;
				case "minimum_MTF_time_to_spawn":
					MinMTF = (int)ev.Value;
					return;
				case "maximum_MTF_time_to_spawn":
					MaxMTF = (int)ev.Value;
					return;
				default:
					return;
			}
		}
		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			LastMtfSpawn = PluginManager.Manager.Server.Round.Duration;
		}
	}
}
