using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pro079.API;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;

namespace Pro079
{
	internal class Pro79Handlers : IEventHandlerCallCommand, IEventHandlerSetRole,
		IEventHandlerPlayerDie,	IEventHandlerWaitingForPlayers
	{
		private readonly Pro079 plugin;
		public Pro79Handlers(Pro079 plugin)
		{
			this.plugin = plugin;
		}
		private static IEnumerable<Room> rooms;
		
		public static FlickerableLight[] FlickerableLightsArray { get; private set; }
		public static IEnumerable<Door> DoorArray { get; private set; }

		private bool UltDoors = false;

		private List<string> Help;
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
		private void FetchExternalHelp()
		{
			Help = new List<string>(Pro079.Manager.Commands.Keys.Count);
			foreach (KeyValuePair<string, ICommand079> kvp in Pro079.Manager.Commands)
			{
				if (!kvp.Value.Disabled) Help.Add($"<b>.079 {kvp.Key}</b> - {kvp.Value.HelpInfo} {FormatEnergyLevel(kvp.Value.APCost, kvp.Value.MinLevel, plugin.energy, plugin.level)}");
			}
		}
		private string GetHelp()
		{
			string help = plugin.basicHelp;
			if (Help == null || Help.Count != Pro079.Manager.Commands.Keys.Count) FetchExternalHelp();
			foreach (string line in Help)
			{
				help += Environment.NewLine + line;
			}
			if (plugin.suicide) help += System.Environment.NewLine + $"<b>.079 {plugin.suicidecmd}</b> - " + plugin.suicidehelp;
			if (plugin.ult) help += System.Environment.NewLine + $"<b>.079 {plugin.ultcmd}</b> - " + plugin.ulthelp;
			if (plugin.tips) help += System.Environment.NewLine + $"<b>.079 {plugin.tipscmd}</b> - " + plugin.tipshelp;
			return help;
		}
		List<string> UltimateHelp;
		private void FetchUltimates()
		{
			UltimateHelp = new List<string>(Pro079.Manager.Ultimates.Keys.Count);
			foreach (KeyValuePair<string, IUltimate079> kvp in Pro079.Manager.Ultimates)
			{
				UltimateHelp.Add($"<b>.079 {kvp.Key}</b> - {kvp.Value.Info} {plugin.ultdata.Replace("$cd", kvp.Value.Cooldown.ToString()).Replace("$cost", kvp.Value.Cost.ToString())}");
			}
		}
		private string GetUltimates()
		{
			string help = plugin.ultusageFirstline;
			if (UltimateHelp == null || UltimateHelp.Count != Pro079.Manager.Ultimates.Keys.Count) FetchExternalHelp();
			foreach (string line in UltimateHelp)
			{
				help += Environment.NewLine + " - " + line;
			}
			return help;
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

		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			if (ev.Command.StartsWith("079"))
			{
				if (!plugin.GetConfigBool("p079_enable"))
				{
					return;
				}

				ev.ReturnMessage = plugin.GetTranslation("unknowncmd");
				// this block is pasted from PlayerPrefs https://github.com/probe4aiur/PlayerPreferences/
				MatchCollection collection = new Regex("[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'").Matches(ev.Command);
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
						ev.Player.SendConsoleMessage(GetHelp(), "white");
						ev.ReturnMessage = "<Pro-079 made by RogerFK#3679. Repository: github.com/RogerFK/pro079>";
					}
					else if (args.Length >= 1)
					{
						// Most unclear way to do the switch statement, but anyways it's the most optimized way to do it.
						if (args[0] == plugin.tipscmd)
						{
							if (!plugin.tips)
							{
								ev.ReturnMessage = plugin.disabled;
								return;
							}
							ev.Player.SendConsoleMessage(plugin.tipsMsg.Replace("\\n", "\n"), "white");
							ev.ReturnMessage = "<Made by RogerFK#3679>";
							return;
						}
						else if (args[0] == plugin.suicidecmd)
						{
							if (!plugin.suicide)
							{
								ev.ReturnMessage = plugin.disabled;
								return;
							}
							List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
							int pcs = PCplayers.Count;
							if (PluginManager.Manager.Server.Round.Stats.SCPAlive + PluginManager.Manager.Server.Round.Stats.Zombies - pcs != 0)
							{
								ev.ReturnMessage = plugin.cantsuicide;
								return;
							}
							PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
							int p = (int)System.Environment.OSVersion.Platform;
							if ((p == 4) || (p == 6) || (p == 128)) MEC.Timing.RunCoroutine(FakeKillPC(ev.Player), MEC.Segment.Update);
							else MEC.Timing.RunCoroutine(FakeKillPC(ev.Player), 1);
							return;
						}
						if(args[0] == plugin.ultcmd)
						{
							if(args.Length == 1)
							{
								ev.ReturnMessage = GetUltimates();
								return;
							}
							int ultcd = Pro079.Manager.UltimateCooldown - PluginManager.Manager.Server.Round.Duration;
							if (ultcd > 0)
							{
								plugin.ultdown.Replace("$cd", ultcd.ToString());
								return;
							}
							string ultName = string.Join(" ", args.Skip(1).ToArray());
							IUltimate079 ultimate = Pro079.Manager.GetUltimate(ultName);
							if(ultimate == null)
							{
								ev.ReturnMessage = plugin.ulterror;
								return;
							}
							ultimate.TriggerUltimate(args.Skip(1).ToArray(), ev.Player);
						}

						// When everything else wasn't caught, search for external commands //
						if (!Pro079.Manager.Commands.TryGetValue(args[0], out ICommand079 CommandHandler))
						{
							ev.ReturnMessage = plugin.unknowncmd;
							return;
						}
						if(ev.Player.Scp079Data.Level + 1 < CommandHandler.MinLevel)
						{
							ev.ReturnMessage = Pro079.Configs.LowLevel(CommandHandler.MinLevel);
							return;
						}
						else if(ev.Player.Scp079Data.AP < CommandHandler.APCost)
						{
							ev.ReturnMessage = Pro079.Configs.LowAP(CommandHandler.APCost);
							return;
						}
						int cooldown = CommandHandler.CurrentCooldown - PluginManager.Manager.Server.Round.Duration;
						if (cooldown > 0)
						{
							ev.ReturnMessage = Pro079.Configs.CmdOnCooldown(cooldown);
							return;
						}
						if (CommandHandler.Cassie)
						{
							int cassieCD = Pro079.Manager.CassieCooldown - PluginManager.Manager.Server.Round.Duration;
							if (cassieCD < 0)
							{
								ev.ReturnMessage = plugin.cassieOnCooldown.Replace("$cd", cassieCD.ToString()).Replace("$(cd)", cassieCD.ToString());
								return;
							}
							Pro079.Manager.CassieCooldown = PluginManager.Manager.Server.Round.Duration + plugin.cassieCooldown;
						}
						// A try-catch statement since some plugins will throw an exception, I'm sure of it.
						try
						{
							ev.ReturnMessage = CommandHandler.CallCommand(args.Skip(1).ToArray(), ev.Player);
							Pro079.Manager.SetOnCooldown(CommandHandler);
						}
						catch (Exception e)
						{
							plugin.Error($"Error with command {args[0]} and literally not my problem:\n" + e.ToString());
							ev.ReturnMessage = plugin.error + ": " + e.Message;
						}
						/*
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
						default:
							ev.ReturnMessage = plugin.GetTranslation("unknowncmd");
							return;*/
					}
				}
			}
			else
			{
				ev.ReturnMessage = plugin.GetTranslation("notscp079");
			}
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (!plugin.spawnBroadcast || !plugin.enable)
			{
				return;
			}

			if (ev.Role == Role.SCP_079)
			{
				MEC.Timing.RunCoroutine(DelayMsg(ev.Player), 1);
			}
		}
		private IEnumerator<float> DelayMsg(Player player)
		{
			yield return 0.1f; // This value produces completely random outputs, but it's good enough for delaying the message a tiny bit so it doesn't overlap
			if (player.TeamRole.Role == Role.SCP_079)
			{
				player.PersonalClearBroadcasts();
				player.PersonalBroadcast(20, plugin.broadcastMsg, true);
				player.SendConsoleMessage(GetHelp(), "white");
			}
		}
		public static IEnumerator<float> FakeKillPC(Player player)
		{
			yield return MEC.Timing.WaitForSeconds(7.3f);
			foreach (FlickerableLight flickerableLight in FlickerableLightsArray)
			{
				Scp079Interactable component = flickerableLight.GetComponent<Scp079Interactable>();
				if (component == null || component.currentZonesAndRooms[0].currentZone == "HeavyRooms")
				{
					flickerableLight.EnableFlickering(10f);
				}
			}
			foreach (Door door in DoorArray)
			{
				Scp079Interactable component = door.GetComponent<Scp079Interactable>();
				if (component.currentZonesAndRooms[0].currentZone == "HeavyRooms" && door.isOpen && !door.locked)
				{
					door.ChangeState(true);
				}
			}
			yield return MEC.Timing.WaitForSeconds(11f);
			if(player != null) player.ChangeRole(Role.SPECTATOR);
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("SCP 0 7 9 ContainedSuccessfully"); // thanks to "El n*z* jud*o" (uh...) for helping me with this
		}

		public static IEnumerable<float> Fake5Gens()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon5");
			yield return MEC.Timing.WaitForSeconds(79.89f);
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
			int p = (int)System.Environment.OSVersion.Platform;
			if ((p == 4) || (p == 6) || (p == 128)) MEC.Timing.RunCoroutine(FakeKillPC(null), MEC.Segment.Update);
			else MEC.Timing.RunCoroutine(FakeKillPC(null), 1);
		}

		/* Cooldowns could be substituted with float like I did in Stalky 106 (https://github.com/RogerFK/stalky106),
		 * but it wouldn't matter anyways as there will always be
		 * a coroutine for the broadcast

		 * Also before you ask, no, you can't pass a bool as a reference in C#
		 * or else I don't know the proper way to do it.*/

		private IEnumerable<float> CooldownCassie(float time)
		{
			if (time > 4)
			{
				yield return (time);

				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(3, plugin.cassieready, false);
				}
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (ev.Player.TeamRole.Team == Smod2.API.Team.SCP && ev.Player.TeamRole.Role != Role.SCP_079)
			{
				List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
				int pcs = PCplayers.Count;
				if (pcs < 0) return;
				if (PluginManager.Manager.Server.Round.Stats.SCPAlive + PluginManager.Manager.Server.Round.Stats.Zombies - pcs <= 1)
				{
					MEC.Timing.RunCoroutine(DelayKysMessage(PCplayers), 1);
				}
			}
		}
		private IEnumerator<float> DelayKysMessage(List<Player> PCplayers)
		{
			if (string.IsNullOrEmpty(plugin.kys)) yield break;
			yield return 0.3f;
			if (PluginManager.Manager.Server.Round.Stats.SCPAlive + PluginManager.Manager.Server.Round.Stats.Zombies - PCplayers.Count == 0)
			{
				foreach (Player player in PCplayers)
				{
					player.PersonalBroadcast(20, plugin.kys, false);
				}
			}
		}
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			rooms = PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(x => x.ZoneType != ZoneType.ENTRANCE);
			FlickerableLightsArray = UnityEngine.Object.FindObjectsOfType<FlickerableLight>();
			DoorArray = UnityEngine.Object.FindObjectsOfType<Door>();
		}
	}
}
