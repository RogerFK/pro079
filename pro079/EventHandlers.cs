using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using scp4aiur; // Will get removed whenever MEC works properly
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;

namespace pro079
{
	internal class Pro79Handlers : IEventHandlerCallCommand, IEventHandlerSetRole,
		IEventHandlerSetConfig, IEventHandlerPlayerDie, IEventHandlerTeamRespawn,
		IEventHandlerDoorAccess, IEventHandlerWaitingForPlayers
	{
		private readonly Pro079 plugin;
		private string helpFormatted;
		public Pro79Handlers(Pro079 plugin)
		{
			this.plugin = plugin;
		}
		private float ultDown;
		private float cooldownScp;
		private static IEnumerable<Room> rooms;
		private static bool DeconBool { get; set; }
		private static float DeconTime { get; set; }
		private static int MinMTF { get; set; }
		private static int MaxMTF { get; set; }
		private static float LastMtfSpawn { get; set; }
		private bool UltDoors = false;

		private string FormatEnergyLevel(int energy, int level, string energStr, string lvlStr)
		{
			string str;
			if (energy > 0)
			{
				str = " (" + energStr.Replace("$ap", energy.ToString())
					+ (level > 1 ? ", " + lvlStr.Replace("$lvl", level.ToString()) : "") + ')';
				return str;
			}
			if (energy <= 0 && level > 1)
			{
				str = " (" + FirstCharToUpper(lvlStr.Replace("$lvl", level.ToString())) + ')';
				return str;
			}
			return string.Empty;
		}

		// This thing below was pasted from here: https://www.c-sharpcorner.com/blogs/first-letter-in-uppercase-in-c-sharp1
		public static string FirstCharToUpper(string s)
		{
			// Check for empty string.
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			// Return char and concat substring.
			return char.ToUpper(s[0]) + s.Substring(1);
		}
		private string FormatHelp()
		{
			// Auxiliary stuff so you don't have to get the translations multiple times
			string help = plugin.GetTranslation("help");
			string lvlaux = plugin.GetTranslation("level"), energyaux = plugin.GetTranslation("energy");
			if (plugin.GetConfigBool("p079_tesla"))
			{
				help += '\n' + plugin.GetTranslation("newteslahelp")
				+ FormatEnergyLevel(plugin.GetConfigInt("p079_tesla_cost"), plugin.GetConfigInt("p079_tesla_level"), energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_mtf"))
			{
				help += '\n' + plugin.GetTranslation("mtfhelp")
				+ FormatEnergyLevel(plugin.GetConfigInt("p079_mtf_cost"), plugin.GetConfigInt("p079_mtf_level"), energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_chaos"))
			{
				help += '\n' + plugin.GetTranslation("chaoshelp")
				+ FormatEnergyLevel(plugin.GetConfigInt("p079_chaos_cost"), plugin.GetConfigInt("p079_chaos_level"), energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_gen"))
			{
				help += '\n' + plugin.GetTranslation("genhelp")
				+ FormatEnergyLevel(plugin.GetConfigInt("p079_gen_cost"), plugin.GetConfigInt("p079_gen_level"), energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_scp"))
			{
				help += '\n' + plugin.GetTranslation("scphelp")
				+ FormatEnergyLevel(plugin.GetConfigInt("p079_scp_cost"), plugin.GetConfigInt("p079_scp_level"), energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_suicide"))
			{
				help += '\n' + plugin.GetTranslation("suicidehelp");
			}
			if (plugin.GetConfigBool("p079_ult"))
			{
				help += '\n' + plugin.GetTranslation("ulthelp")
				+ FormatEnergyLevel(0, 4, energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_info"))
			{
				help += '\n' + plugin.GetTranslation("infohelp") + FormatEnergyLevel(0, 0, energyaux, lvlaux);
			}
			if (plugin.GetConfigBool("p079_tips"))
			{
				help += '\n' + plugin.GetTranslation("tipshelp") + FormatEnergyLevel(0, 0, energyaux, lvlaux);
			}
			return help;
		}

		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			string command = ev.Command.ToLower();
			if (command.StartsWith("079"))
			{
				if (!plugin.GetConfigBool("p079_enable"))
				{
					return;
				}

				ev.ReturnMessage = plugin.GetTranslation("unknowncmd");
				// this block is pasted from PlayerPrefs https://github.com/probe4aiur/PlayerPreferences/
				MatchCollection collection = new Regex("[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'").Matches(command);
				string[] args = new string[collection.Count - 1];

				for (int i = 1; i < collection.Count; i++)
				{
					// If the first char (0) and the last one (its length - 1) is ", that is the char (defined by ' ')
					// that is \" (the \ is to define it's not a quote for a string), then take the substring that
					// starts from the second character to the second last one. Pretty fucking clever.
					if (collection[i].Value[0] == '\"' && collection[i].Value[collection[i].Value.Length - 1] == '\"')
					{
						args[i - 1] = collection[i].Value.Substring(1, collection[i].Value.Length - 2);
					}
					else
					{
						args[i - 1] = collection[i].Value;
					}
				}
				// end of the paste thx
				if (ev.Player.TeamRole.Role == Role.SCP_079)
				{
					if (args.Length == 0)
					{
						ev.Player.SendConsoleMessage(helpFormatted, "white");
						ev.ReturnMessage = plugin.GetTranslation("bugwarn") + " <Made by RogerFK#3679>";
					}
					else if (args.Length > 0)
					{
						// Most unclear way to do the switch statement, but anyways it's the most optimized way to do it.
						switch (SwitchParser.ParseArg(args[0], plugin))
						{
							case 9: // tipscmd
								if (!plugin.GetConfigBool("p079_tips"))
								{
									ev.ReturnMessage = plugin.GetTranslation("disabled");
									return;
								}
								ev.Player.SendConsoleMessage(plugin.GetTranslation("tips").Replace("\\n", "\n"), "white");
								ev.ReturnMessage = "<Made by RogerFK#3679>";
								return;
							case 1: // teslacmd
								return;
							case 6: // suicidecmd
								if (!plugin.GetConfigBool("p079_suicide"))
								{
									ev.ReturnMessage = plugin.GetTranslation("disabled");
									return;
								}
								List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
								int pcs = PCplayers.Count;
								if (PluginManager.Manager.Server.Round.Stats.SCPAlive + PluginManager.Manager.Server.Round.Stats.Zombies - pcs != 0)
								{
									ev.ReturnMessage = plugin.GetTranslation("cantsuicide");
									return;
								}
								PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
								Timing.Run(FakeKillPC());
								ev.Player.Kill(DamageType.NUKE);
								return;
							case 2: // mtfcmd
								return;
							case 4: // scpcmd
								return;
							case 3: // gencmd
								return;
							case 5: // infocmd
								return;
							case 7: // ultcmd
								if (!plugin.GetConfigBool("p079_ult"))
								{
									ev.ReturnMessage = plugin.GetTranslation("disabled");
									return;
								}
								string ultUsage = plugin.GetTranslation("ultusage").Replace("\\n", "\n");
								// if it's our server, and by the way don't enter if you're a guiri
								if (PluginManager.Manager.Server.Name.ToLower().Contains("world in chaos"))
								{
									ultUsage += "3. ... ¡Añade tu idea aquí! Tan solo tienes que ponerlo en #sugerencias-debates o en #sugerencias (ve a #bots y pon ,suggest \"Tu idea\" en el Discord de World in Chaos.\n"
									+ "Adicionalmente, si estás baneado, muteado o cualquier cosa, puedes contactar directamente con RogerFK#3679";
								}

								if (args.Count() == 1)
								{
									if (ev.Player.Scp079Data.Level < 3 && !ev.Player.GetBypassMode())
									{
										ev.ReturnMessage = plugin.GetTranslation("ultlocked");
									}
									else
									{
										ev.ReturnMessage = ultUsage;
										return;
									}
								}
								else
								{
									if (ev.Player.Scp079Data.Level < 3 && !ev.Player.GetBypassMode())
									{
										ev.ReturnMessage = plugin.GetTranslation("ultlocked");
										return;
									}
									if (PluginManager.Manager.Server.Round.Duration < ultDown)
									{
										ev.ReturnMessage = plugin.GetTranslation("ultdown").Replace("$cd", (ultDown - PluginManager.Manager.Server.Round.Duration).ToString());
										return;
									}
									if (!int.TryParse(args[1], out int ult))
									{
										ev.ReturnMessage = ultUsage;
										return;
									}
									ev.ReturnMessage = plugin.GetTranslation("ultlaunched");
									switch (ult)
									{
										case 1:
											PluginManager.Manager.Server.Map.AnnounceCustomMessage("warning . malfunction detected on heavy containment zone . Scp079Recon6 . . . light systems Disengaged");
											Timing.Run(ShamelessTimingRunLights());
											ultDown = PluginManager.Manager.Server.Round.Duration + 180;
											Timing.Run(CooldownUlt(180f));
											return;
										case 2:
											PluginManager.Manager.Server.Map.AnnounceCustomMessage("warning facility control lost . starting security lockdown");
											Timing.Run(Ult2Toggle(30f));
											ultDown = PluginManager.Manager.Server.Round.Duration + 300;
											Timing.Run(CooldownUlt(300f));
											return;
										default:
											ev.ReturnMessage = ultUsage;
											return;
									}
								}
								return;
							case 8: // chaoscmd
								return;
							default:
								ev.ReturnMessage = plugin.GetTranslation("unknowncmd");
								return;
						}
					}
				}
				else
				{
					ev.ReturnMessage = plugin.GetTranslation("notscp079");
				}
			}
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (!plugin.GetConfigBool("p079_broadcast_enable"))
			{
				return;
			}

			if (ev.Role == Role.SCP_079)
			{
				ev.Player.PersonalClearBroadcasts();
				ev.Player.PersonalBroadcast(20, plugin.GetTranslation("broadcast_msg"), true);
				ev.Player.SendConsoleMessage(helpFormatted, "white");
			}
		}

		private IEnumerable<float> DisableTeslas(float time)
		{
			TeslaGate[] teslas = UnityEngine.Object.FindObjectsOfType<TeslaGate>();
			int length = teslas.Length;
			float[] distances = new float[teslas.Length];
			int i;

			for (i=0; i<length; i++)
			{
				distances[i] = teslas[i].sizeOfTrigger;
				teslas[i].sizeOfTrigger = -1f;
			}
			int remTime = plugin.GetConfigInt("p079_tesla_remaining");
			yield return time - remTime;
			foreach (Smod2.API.Player player in PluginManager.Manager.Server.GetPlayers())
			{
				string remainingStr = plugin.GetTranslation("teslarem");
				if (player.TeamRole.Role == Role.SCP_079)
				{
					for(i=remTime; i>0; i--)
					{
						player.PersonalBroadcast(1, Environment.NewLine + remainingStr.Replace("$sec", "<b>" + i.ToString() + "</b>"), false);
					}
					player.PersonalBroadcast(5, Environment.NewLine + plugin.GetTranslation("teslarenabled"), false);
				}
			}
			yield return remTime;

			for (i = 0; i < length; i++)
			{
				teslas[i].sizeOfTrigger = distances[i];
			}
		}

		public static IEnumerable<float> FakeKillPC()
		{
			// doesn't close doors but I'm not gonna do it lmao
			yield return (7.3f);

			foreach (Room room in rooms)
			{
				room.FlickerLights();
			}
			yield return (8f);
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("SCP 0 7 9 ContainedSuccessfully"); // thanks to "El n*z* jud*o" (uh...) for helping me with this
		}

		public static IEnumerable<float> Fake5Gens()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon5");
			yield return (79.89f); // this value is fucking shit actually
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
			Timing.Run(FakeKillPC());
		}

		/* Cooldowns could be substituted with float like I did in Stalky 106 (https://github.com/RogerFK/stalky106),
		 * but it wouldn't matter anyways as there will always be
		 * a coroutine for the broadcast 
		 
		 * Also before you ask, no, you can't pass a bool as a reference in C#
		 * or else I don't know the proper way to do it.*/
		private IEnumerable<float> CooldownScp(float v)
		{
			if (v > 4)
			{
				yield return (v);
				List<Player> pcs = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player pc in pcs)
				{
					pc.PersonalBroadcast(6, plugin.GetTranslation("scpready"), false);
				}
			}
		}
		private IEnumerable<float> CooldownUlt(float time)
		{
			if (time > 4)
			{
				yield return (time);
				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(3, plugin.GetTranslation("ultready"), false);
				}
			}
		}

		private IEnumerable<float> Ult2Toggle(float v)
		{
			UltDoors = true;
			yield return (v);
			UltDoors = false;
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("attention all Personnel . doors lockdown finished");
		}

		private IEnumerable<float> CooldownMTF(float time)
		{
			if (time > 4)
			{
				yield return (time);
				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(3, plugin.GetTranslation("mtfready"), false);
				}
			}
		}

		private IEnumerable<float> CooldownGen(float time)
		{
			if (time > 4)
			{
				yield return (time);

				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(3, plugin.GetTranslation("genready"), false);
				}
			}
		}

		private IEnumerable<float> CooldownCassie(float time)
		{
			if (time > 4)
			{
				yield return (time);

				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(3, plugin.GetTranslation("cassieready"), false);
				}
			}
		}

		/* pasted and slightly modified from here btw https://github.com/probe4aiur/Blackout
		 * tbh that's what I was programming myself but then I had to check how many seconds
		 * the lights were going to be turned off for, so I just actually copied that yield
		 * thingy I swear to god I'm not just a copy paster (and if I were I would still give proper credit)
		 */
		private IEnumerable<float> ShamelessTimingRunLights()
		{
			yield return (12.1f);
			float start = PluginManager.Manager.Server.Round.Duration;
			while (start + 60f > PluginManager.Manager.Server.Round.Duration)
			{
				foreach (Room room in rooms)
				{
					room.FlickerLights();
				}
				yield return (8f);
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

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (ev.Player.TeamRole.Team == Smod2.API.Team.SCP && ev.Player.TeamRole.Role != Role.SCP_079)
			{
				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				int pcs = PCplayers.Count;
				if (pcs < 0) return;
				if (PluginManager.Manager.Server.Round.Stats.SCPAlive + PluginManager.Manager.Server.Round.Stats.Zombies - pcs == 0)
				{
					string kys = plugin.GetTranslation("kys");
					foreach (Player player in PCplayers)
					{
						player.PersonalBroadcast(10, kys, false);
					}
				}
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			LastMtfSpawn = PluginManager.Manager.Server.Round.Duration;
		}

		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
			if (UltDoors == false || string.IsNullOrWhiteSpace(ev.Door.Permission))
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
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			rooms = PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(x => x.ZoneType != ZoneType.ENTRANCE);
			helpFormatted = FormatHelp();
			cooldownGenerator = 0f;
			cooldownCassieGeneral = 0f;
			cooldownMTF = 0f;
			ultDown = 0f;
			cooldownScp = 0f;
			DeconBool = false;
			UltDoors = false;
			cooldownChaos = 0f;
		}
	}
}
