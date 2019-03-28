using Smod2.Commands;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.API;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Threading.Tasks;
using Smod2;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using scp4aiur;
using Smod2.EventSystem.Events;

namespace pro079
{
	class ComandoManeador : IEventHandlerCallCommand, IEventHandlerSetRole, IEventHandlerPlayerDie
	{
		private readonly pro079 plugin;
		public static Smod2.API.Player ordenador;
		public ComandoManeador(pro079 plugin)
		{
			this.plugin = plugin;
		}
		public static bool intercomEnable = true;
		private void MandaAyuda(Player player)
		{
			player.SendConsoleMessage("Estos son los comandos que tienes disponibles:\n" +
			".079 - Muestra este mensaje de ayuda\n" +
			".079 te - Desactiva la tesla de la habitación en la que estás durante 8 segundos (50 puntos)\n" +
			".079 mtf <letra> <numero> <scp-vivos> - Lanza un mensaje sobre que ha llegado la MTF a la zona con un número falso de SCPs con vida, o el actual si no (100 puntos, nivel 2)\n" +
			".079 intercom - Manda un mensaje por la intercom o ponte en el lugar del actual hablante (cooldown: 180 segundos) (125 puntos)\n" +
			".079 scp <###> <motivo> - Manda un mensaje de muerte de SCP con el número, el motivo puede ser: unknown, tesla, mtf, decont\n" +
			".079 suicidio [nuke] - Sobrecarga los generadores para morir cuando quedes tú solo. Si pones \"suicidio nuke\" en nivel 4 o 5, activarás la bomba" +
			".079 gen [1-5] - Manda el mensaje de cuántos generadores han sido activados",
			"white");
		}

		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			string command = ev.Command.ToLower();

			if (command.StartsWith("079"))
			{
				MatchCollection collection = new Regex("[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'").Matches(command);
				string[] args = new string[collection.Count - 1];

				for (int i = 1; i < collection.Count; i++)
				{
					// If it's wrapped in quotes, 
					if (collection[i].Value[0] == '\"' && collection[i].Value[collection[i].Value.Length - 1] == '\"')
					{
						args[i - 1] = collection[i].Value.Substring(1, collection[i].Value.Length - 2);
					}
					else
					{
						args[i - 1] = collection[i].Value;
					}
				}
				if (ev.Player.TeamRole.Role == Role.SCP_079)
				{
					if (args.Length == 0)
					{
						MandaAyuda(ev.Player);
					}
					else if (args.Length == 1)
					{
						switch (args[0])
						{
							case "te":
								if (ev.Player.Scp079Data.AP >= 50)
								{
									foreach (Smod2.API.TeslaGate tesla in PluginManager.Manager.Server.Map.GetTeslaGates())
									{
										if (Vector.Distance(ev.Player.Scp079Data.Camera, tesla.Position) < 8.0f)
										{
											ev.Player.Scp079Data.AP = ev.Player.Scp079Data.AP - 50;
											Timing.Run(DisableTesla(tesla, tesla.TriggerDistance));
											break;
										}
										else ev.Player.SendConsoleMessage("No estás cerca de una Tesla.", "white");
										plugin.Info("x: " + ev.Player.Scp079Data.Camera.x + ", y: " + ev.Player.Scp079Data.Camera.y + ", z: " + ev.Player.Scp079Data.Camera.z);
									}
								}
								else ev.Player.SendConsoleMessage("No tienes suficiente energía (necesitas 50).", "white");
								return;
							case "intercom":
								if (!intercomEnable)
								{
									ev.Player.SendConsoleMessage("Espera antes de volver a usar la intercom.", "white");
									return;
								}
								if (PluginManager.Manager.Server.Map.GetIntercomSpeaker() == ev.Player)
								{
									PluginManager.Manager.Server.Map.SetIntercomSpeaker(null);
								}
								else if (ev.Player.Scp079Data.AP >= 125)
								{
									ev.Player.Scp079Data.AP = ev.Player.Scp079Data.AP - 125;
									PluginManager.Manager.Server.Map.SetIntercomSpeaker(ev.Player);
									DisableIntercom(180f);
								}
								else ev.Player.SendConsoleMessage("No tienes suficiente energía.", "white");
								return;
							case "suicidio":
								if ((PluginManager.Manager.Server.Round.Stats.Zombies + PluginManager.Manager.Server.Round.Stats.SCPAlive) != 1)
								{
									ev.Player.SendConsoleMessage("No puedes suicidarte cuando hay más SCP vivos", "white");
									return;
								}
								PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
								KillPC(ev.Player);
								return;
							case "mtf":
								ev.Player.SendConsoleMessage("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP");
								return;
							case "scp":
								ev.Player.SendConsoleMessage("Uso: .079 scp (173) (unknown)");
								return;
							case "gen":
								ev.Player.SendConsoleMessage("Uso: .079 gen (1) - Sonará que hay 1 generador activado");
								return;
							default:
								ev.Player.SendConsoleMessage("Comando no reconocido. Usa .079 para ayuda", "white");
								return;

						}
					}
					else if (args.Length > 2)
					{
						switch (args[0])
						{
							case "mtf":
								if (ev.Player.Scp079Data.AP >= 80)
								{
									if (ev.Player.Scp079Data.Level >= 1)
									{
										if (args.Length >= 5)
										{

											if (int.Parse(args[1]) < 0 || int.Parse(args[1]) > 20 ||
												int.Parse(args[2]) < 0 || int.Parse(args[2]) > 20 ||
												!char.IsLetter(args[3][0]))
											{
												ev.Player.SendConsoleMessage("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP");
											}
											else
											{
												ev.Player.Scp079Data.AP -= 80;
												PluginManager.Manager.Server.Map.AnnounceNtfEntrance(int.Parse(args[3]), int.Parse(args[2]), args[1][0]);
											}
										}
										else
										{
											ev.Player.SendConsoleMessage("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP");
										}
									}
									else ev.Player.SendConsoleMessage("No tienes suficiente nivel", "white");
								}
								else ev.Player.SendConsoleMessage("No tienes suficiente energía.", "white");
								return;

							case "scp":
								if (args.Length >= 3)
								{
									if (args[2] == "mtf")
									{
										Player dummy = null;
										List<Role> mtf = new List<Role>
										{
											Role.FACILITY_GUARD, Role.NTF_CADET, Role.NTF_LIEUTENANT, Role.NTF_SCIENTIST, Role.NTF_COMMANDER
										};

										foreach (Player player in PluginManager.Manager.Server.GetPlayers())
										{
											if (mtf.Contains(player.TeamRole.Role))
											{
												dummy = player;
												break;
											}
										}
										if (dummy == null) ev.Player.SendConsoleMessage("No hay MTFs vivos. Mandando como \"unknown\"", "white");
										PluginManager.Manager.Server.Map.AnnounceScpKill(args[1], dummy);
									}
									else if (args[2] == "unknown")
									{
										PluginManager.Manager.Server.Map.AnnounceScpKill(args[1], null);
									}
									else if (args[2] == "tesla")
									{
										string[] cutre = new string[20];
										for (int i = 0; i < args[1].Length; i++)
										{
											cutre[i] = args[1][i].ToString();
										}
										string scpNum = String.Join(" ", cutre);
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Successfully Terminated by automatic security systems");
									}
									else if (args[2] == "decont")
									{
										string[] cutre = new string[20];
										for (int i = 0; i< args[1].Length; i++)
										{
											cutre[i] = args[1][i].ToString();
										}
										string scpNum = String.Join(" ", cutre);
										PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Lost in Decontamination Sequence ");
									}
								}
								else ev.Player.SendConsoleMessage("Uso: .079 scp (173) (unknown)");
								return;
							case "suicidio":
								if (args[1] == "nuke" && ev.Player.Scp079Data.Level >= 3)
								{
									PluginManager.Manager.Server.Map.StartWarhead();
								}
								else ev.Player.SendConsoleMessage("Veo que quieres tirar la nuke. Sería \".079 suicidio nuke\"", "white");
								return;
							case "gen":
								if(int.Parse(args[1]) < 1 || int.Parse(args[1]) > 5)
								{
									ev.Player.SendConsoleMessage("Uso: .079 gen (1) - Sonará que hay 1 generador activado");
									return;
								}
								PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon" + args[1]);
								return;
							default:
								MandaAyuda(ev.Player);
								return;
						}
					}
				}
				else
				{
					ev.Player.SendConsoleMessage("¡No eres SCP-079!", "red");
				}
			}
		}

		public static IEnumerable<float> DisableTesla(Smod2.API.TeslaGate tesla, float current)
		{
			tesla.TriggerDistance = -1.0f;
			yield return 8.0f;
			tesla.TriggerDistance = current;
		}
		public static IEnumerable<float> KillPC(Player pc)
		{
			yield return 5.3f;

			foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
			{
				room.FlickerLights();
			}
			pc.Kill(DamageType.NONE);
		}
		public static IEnumerable<float> DisableIntercom(float time)
		{
			intercomEnable = false;
			yield return time;
			intercomEnable = true;
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if(ev.Player.TeamRole.Role == Role.SCP_079)
			{
				DelayBroadcastPC(ev.Player);
			}
		}

		public static IEnumerable<float> DelayBroadcastPC(Player pc)
		{
			yield return 1.0f;
			
			if(pc.TeamRole.Role == Role.SCP_079)
			{
				ordenador = pc;
				pc.PersonalBroadcast(20, "Presiona Ñ para abrir la consola y usar comandos adicionales como desactivar teslas, fingir la muerte de un SCP...", true);
				pc.SendConsoleMessage("Estos son los comandos que tienes disponibles:\n" +
				".079 - Muestra este mensaje de ayuda\n" +
				".079 te - Desactiva la tesla de la habitación en la que estás durante 8 segundos (50 puntos)\n" +
				".079 mtf <letra> <numero> <scp-vivos> - Lanza un mensaje sobre que ha llegado la MTF a la zona con un número falso de SCPs con vida, o el actual si no (100 puntos)\n" +
				".079 intercom - Manda un mensaje por la intercom o ponte en el lugar del actual hablante (cooldown: 180 segundos) (125 puntos)\n" +
				".079 scp <###> <motivo> - Manda un mensaje de muerte de SCP con el número, el motivo puede ser: unknown, tesla, mtf, decont\n" +
				".079 suicidio [nuke] - Sobrecarga los generadores para morir cuando quedes tú solo. Si pones \"suicidio nuke\" en nivel 4 o 5, activarás la bomba sin que pueda ser desactivada",
				"white");
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if ((PluginManager.Manager.Server.Round.Stats.Zombies + PluginManager.Manager.Server.Round.Stats.SCPAlive) == 1)
			{
				ordenador.PersonalBroadcast(20, "Solo quedas tú. Puedes presionar la Ñ y poner .079 suicidio [nuke] para morir.", true);
			}
		}
	}
}
