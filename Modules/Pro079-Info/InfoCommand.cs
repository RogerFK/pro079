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

namespace Pro079_Info
{
	class InfoCommand : IEventHandlerTeamRespawn, IEventHandlerSetConfig, I079Command
	{
		private int LastMtfSpawn;

		public string CallComand(string[] args, Player player)
		{
			if (!plugin.GetConfigBool("p079_info"))
			{
				ev.ReturnMessage = plugin.GetTranslation("disabled");
				return;
			}
			int level = ev.Player.Scp079Data.Level + 1;
			string humansAlive;
			string decontTime;
			string ScientistsEscaped;
			string ClassDEscaped;
			string ClassDAlive;
			string ScientistsAlive;
			string MTFAlive;
			string CiAlive;
			string estMTFtime;

			if (level < plugin.GetConfigInt("p079_info_alive")) humansAlive = '[' + FirstCharToUpper(plugin.GetTranslation("level")).Replace("$lvl", plugin.GetConfigInt("p079_info_alive").ToString()) + ']';
			else humansAlive = (PluginManager.Manager.Server.Round.Stats.ClassDAlive + PluginManager.Manager.Server.Round.Stats.ScientistsAlive + PluginManager.Manager.Server.Round.Stats.CiAlive + PluginManager.Manager.Server.Round.Stats.NTFAlive).ToString();

			if (level < plugin.GetConfigInt("p079_info_decont"))
			{
				decontTime = '[' + FirstCharToUpper(plugin.GetTranslation("level")).Replace("$lvl", plugin.GetConfigInt("p079_info_decont").ToString()) + ']';
			}
			else
			{
				if (DeconBool == true)
				{
					decontTime = plugin.GetTranslation("decontdisabled");
				}
				else if (PluginManager.Manager.Server.Map.LCZDecontaminated == true)
				{
					decontTime = plugin.GetTranslation("deconthappened");
				}
				else
				{
					float auxTime = (DeconTime - PluginManager.Manager.Server.Round.Duration / 60.0f);
					decontTime = auxTime > 0 ? auxTime.ToString("0.00") : plugin.GetTranslation("decontbug");
				}
			}
			if (level < plugin.GetConfigInt("p079_info_escaped"))
			{
				ScientistsEscaped = '[' + FirstCharToUpper(plugin.GetTranslation("level")).Replace("$lvl", plugin.GetConfigInt("p079_info_escaped").ToString()) + ']';
				ClassDEscaped = '[' + FirstCharToUpper(plugin.GetTranslation("level")).Replace("$lvl", plugin.GetConfigInt("p079_info_escaped").ToString()) + ']';
			}
			else
			{
				ClassDEscaped = PluginManager.Manager.Server.Round.Stats.ClassDEscaped.ToString("00");
				ScientistsEscaped = PluginManager.Manager.Server.Round.Stats.ScientistsEscaped.ToString("00");
			}

			if (level < plugin.GetConfigInt("p079_info_plebs"))
			{
				ClassDAlive = '[' + FirstCharToUpper(plugin.GetTranslation("level")).Replace("$lvl", plugin.GetConfigInt("p079_info_plebs").ToString()) + ']';
				ScientistsAlive = '[' + FirstCharToUpper(plugin.GetTranslation("level")).Replace("$lvl", plugin.GetConfigInt("p079_info_plebs").ToString()) + ']';
			}
			else
			{
				ClassDAlive = PluginManager.Manager.Server.Round.Stats.ClassDAlive.ToString("00");
				ScientistsAlive = PluginManager.Manager.Server.Round.Stats.ScientistsAlive.ToString("00");
			}
			if (level < plugin.GetConfigInt("p079_info_mtfci"))
			{
				MTFAlive = '[' + FirstCharToUpper(plugin.GetTranslation("level")).Replace("$lvl", plugin.GetConfigInt("p079_info_mtfci").ToString()) + ']';
				CiAlive = '[' + FirstCharToUpper(plugin.GetTranslation("level")).Replace("$lvl", plugin.GetConfigInt("p079_info_mtfci").ToString()) + ']';
			}
			else
			{
				MTFAlive = PluginManager.Manager.Server.Round.Stats.NTFAlive.ToString("00");
				CiAlive = PluginManager.Manager.Server.Round.Stats.CiAlive.ToString("00");
			}

			if (level < plugin.GetConfigInt("p079_info_mtfest") || ev.Player.GetBypassMode())
			{

				if (PluginManager.Manager.Server.Round.Duration - LastMtfSpawn < MinMTF)
				{
					//estMTFtime = "entre " + (MinMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0") + " segundos y " + (MaxMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0") + " segundos.";
					estMTFtime = plugin.GetTranslation("mtfest0").Replace("$(min)", (MinMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0")).Replace("$(max)", (MaxMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0"));
				}
				else if (PluginManager.Manager.Server.Round.Duration - LastMtfSpawn < MaxMTF)
				{
					//estMTFtime = "menos de " + (MaxMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0");
					estMTFtime = plugin.GetTranslation("mtfest1").Replace("$(max)", (MaxMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0"));
				}
				else
				{
					estMTFtime = plugin.GetTranslation("mtfest2");
				}
			}
			else
			{
				estMTFtime = '[' + FirstCharToUpper(plugin.GetTranslation("level")).Replace("$lvl", plugin.GetConfigInt("p079_info_mtfest").ToString()) + ']';

			}
			/* Old method, just as a reference for me.
			ev.Player.SendConsoleMessage(
			"\nSCP vivos: " + PluginManager.Manager.Server.Round.Stats.SCPAlive +
			"\nHumanos vivos: " + humansAlive + " | Siguientes MTF/Chaos: " + estMTFtime +
			"\nTiempo hasta la descontaminación: " + decontTime +
			"\nClase D escapados: " + ClassDEscaped + " | Científicos escapados: " + ScientistsEscaped +
			"\nClase D vivos:     " + ClassDAlive + " | Chaos vivos:           " + CiAlive +
			"\nCientíficos vivos: " + ScientistsAlive + " | MTF vivos:             " + MTFAlive
			, "white");*/
			// This bs below can be optimized by using a dictionary or two arrays with its own function, but for now it's staying like this.
			string infomsg = plugin.GetTranslation("infomsg")
				.Replace("$scpalive", PluginManager.Manager.Server.Round.Stats.SCPAlive.ToString("0"))
				.Replace("$humans", humansAlive).Replace("$estMTF", estMTFtime)
				.Replace("$decont", decontTime)
				.Replace("$cdesc", ClassDEscaped).Replace("$sciesc", ScientistsEscaped)
				.Replace("$cdalive", ClassDAlive).Replace("$cialive", CiAlive)
				.Replace("$scialive", ScientistsAlive).Replace("$mtfalive", MTFAlive);
			ev.Player.SendConsoleMessage(infomsg.Replace("\\n", Environment.NewLine), "white");
			if (level > plugin.GetConfigInt("p079_info_gens"))
			{
				ev.ReturnMessage = plugin.GetTranslation("generators");
				foreach (Generator generator in PluginManager.Manager.Server.Map.GetGenerators())
				{
					ev.ReturnMessage += plugin.GetTranslation("generatorin").Replace("$room", generator.Room.RoomType.ToString()) + ' ';
					if (generator.Engaged)
					{
						ev.ReturnMessage += plugin.GetTranslation("activated") + '\n';
					}
					else
					{
						ev.ReturnMessage += (generator.HasTablet ? plugin.GetTranslation("hastablet") : plugin.GetTranslation("notablet")) + ' ' + plugin.GetTranslation("timeleft").Replace("$sec", generator.TimeLeft.ToString("0")) + '\n';
					}
				}
			}
			else
			{
				ev.ReturnMessage = '[' + plugin.GetTranslation("lockeduntil").Replace("$lvl", plugin.GetConfigInt("p079_info_gens").ToString()) + ']';
			}

			return;
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
