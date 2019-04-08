using scp4aiur;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace pro079
{
    internal class Pro79Handlers : IEventHandlerCallCommand, IEventHandlerSetRole, IEventHandlerSetConfig, IEventHandlerPlayerDie, IEventHandlerTeamRespawn
    {
        private readonly pro079 plugin;
        public Pro79Handlers(pro079 plugin)
        {
            this.plugin = plugin;
        }
        public static bool cooldownGenerator = false;
        public static bool cooldownCassieGeneral = false;
        public static bool infoCooldown = false;
        public static bool cooldownMTF = false;
        private static bool DeconBool { get; set; }
        private static float DeconTime { get; set; }
        private static int MinMTF { get; set; }
        private static int MaxMTF { get; set; }
        private static float LastMtfSpawn { get; set; }

        private static void MandaAyuda(Player player)
        {
            player.SendConsoleMessage("Estos son los comandos que tienes disponibles:\n" +
            ".079 - Muestra este mensaje de ayuda\n" +
            ".079 te - Desactiva la tesla de la habitación en la que estás durante 8 segundos (50 puntos)\n" +
            ".079 mtf <letra> <numero> <scp-vivos> - Lanza un mensaje sobre que ha llegado la MTF a la zona con un número que elijas de SCPs con vida (80 de energía, nivel 2)\n" +
            ".079 gen [1-5] - Manda el mensaje de que X generadores han sido activados, o manda con un 6 para fingir tu muerte (50 de energía, nivel 2)\n" +
            "Si mandas .079 gen 5, activarás la secuencia para fingir que estás siendo contenido. gen 6 finge el comando \".079 suicidio\"\n" +
            ".079 scp <###> <motivo> - Manda un mensaje de muerte de SCP con el número, el motivo puede ser: unknown, tesla, mtf, decont (50 de energía)\n" +
            ".079 info - Te dice la gente que queda viva, junto a los SCP y los clase D y científicos (5 de energía)\n" +
            ".079 suicidio - Sobrecarga los generadores para morir cuando quedes tú solo" +
            "\n.079 controles - Muestra ayuda sobre cómo jugar con SCP-079 en general y cosas a tener en cuenta"
            //+ ".079 cont106 - Manda el audio de recontención de SCP 106" //no implementado porque seguramente no funcione bien, habría que invocar el audio y luego hacer el callrpc
            , "white");
        }
		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			string command = ev.Command.ToLower();
			//this is pasted from PlayerPrefs
			if (command.StartsWith("079"))
			{
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
			// everything below this is completely hardcoded in Spanish
				if (ev.Player.TeamRole.Role == Role.SCP_079)
				{
					if (args.Length == 0)
					{
						MandaAyuda(ev.Player);
						ev.ReturnMessage = "";
					}
					else if (args.Length == 1)
					{
						switch (args[0])
						{
                            case "controles":
                                ev.Player.SendConsoleMessage("TAB (encima del Bloq. Mayus): abre el mapa donde estás.\n" +
                                    "Espacio: cambia tu modo de cámara entre el modo normal y el modo primera persona.\n" +
                                    "Teclas de movimiento: muévete a la cámara que indica arriba a la derecha\n" +
                                    "Para salir de la heavy containment zone, ve hacia el elevador y pulsa el recuadro blanco, o hacia el checkpoint y usa la W para moverte entre cámaras" +
                                    "\nAdicionalmente, este plugin te permite usar comandos como podrás haber comprobado usando .079");
                                ev.ReturnMessage = "Para más información y sugerencias, no dudes en entrar en nuestro Discord o preguntar a RogerFK (discord: RogerFK#3679)";
                                return;
							case "te":
								if (ev.Player.Scp079Data.AP >= 50)
								{
									bool noTesla = true;
									foreach (Smod2.API.TeslaGate tesla in PluginManager.Manager.Server.Map.GetTeslaGates())
									{
										if (Vector.Distance(ev.Player.Scp079Data.Camera, tesla.Position) < 8.0f)
										{
											if (tesla.TriggerDistance > 0)
											{
												ev.Player.Scp079Data.AP -= 50;
												ev.Player.Scp079Data.ShowGainExp(ExperienceType.USE_TESLAGATE);
												ev.Player.Scp079Data.Exp += (10.0f / (ev.Player.Scp079Data.Level + 1)); //ignore these
												Timing.Run(DisableTesla(tesla, tesla.TriggerDistance));
												noTesla = false;
												ev.ReturnMessage = "Tesla desactivada.";
												break;
											}
											else
											{
												ev.ReturnMessage = ("Esta Tesla está desactivada.");
											}
										}
									}
									if (noTesla == true)
									{
										ev.ReturnMessage = ("No estás cerca de una Tesla.");
									}
								}
								else
								{
									ev.ReturnMessage = ("No tienes suficiente energía (necesitas 50).");
								}
                                return;
                            case "suicidio":
                                List<Player> PCplayers = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
                                int pcs = PCplayers.Count;
                                if (PluginManager.Manager.Server.Round.Stats.SCPAlive + PluginManager.Manager.Server.Round.Stats.Zombies - pcs == 0)
                                {
                                    ev.ReturnMessage = ("No puedes suicidarte cuando hay más SCP vivos");
                                    return;
                                }
                                PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
                                Timing.Run(FakeKillPC());
                                ev.Player.Kill(DamageType.NUKE);
                                return;
                            case "mtf":
                                ev.ReturnMessage = ("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP - 80 de energía");
                                return;
                            case "scp":
                                ev.ReturnMessage = ("Uso: .079 scp (173/096/106/049/939) (unknown/tesla/mtf/decont) - 50 de energía");
                                return;
                            case "gen":
                                ev.ReturnMessage = ("Uso: .079 gen (1-5) - Sonará que hay 1 generador activado - 50 de energía");
                                return;
                            case "info":
                                if (ev.Player.Scp079Data.AP < 5 && !ev.Player.GetBypassMode())
                                {
                                    ev.ReturnMessage = "No tienes suficiente energía (necesitas 5).";
                                    return;
                                }
                                Timing.Run(CooldownInfo(2.5f));
                                if (infoCooldown)
                                {
                                    ev.ReturnMessage = "Tienes que esperar antes de volver a usar el comando info";
                                    return;
                                }
                                string humansAlive = "[Nivel 2]";
                                string tiempoDecont = "[Nivel 2]";
                                string ScientistsEscaped = "[Nivel 3]";
                                string ClassDEscaped = "[Nivel 3]";
                                string ClassDAlive = "[Nivel 2]";
                                string ScientistsAlive = "[Nivel 2]";
                                string MTFAlive = "[Nivel 3]";
                                string CiAlive = "[Nivel 3]";
                                string MTFtiempo = "[Nivel 3]";

                                if (ev.Player.Scp079Data.Level > 0 || ev.Player.GetBypassMode())
                                {
                                    humansAlive = (PluginManager.Manager.Server.Round.Stats.ClassDAlive + PluginManager.Manager.Server.Round.Stats.ScientistsAlive + PluginManager.Manager.Server.Round.Stats.CiAlive + PluginManager.Manager.Server.Round.Stats.NTFAlive).ToString();

                                    ClassDAlive = PluginManager.Manager.Server.Round.Stats.ClassDAlive.ToString("00");
                                    ScientistsAlive = PluginManager.Manager.Server.Round.Stats.ScientistsAlive.ToString("00");

                                    if (DeconBool == true)
                                    {
                                        tiempoDecont = "La descontaminación está desactivada";
                                    }
                                    else if (PluginManager.Manager.Server.Map.LCZDecontaminated == true)
                                    {
                                        tiempoDecont = "Ya ha ocurrido descontaminación";
                                    }
                                    else
                                    {
                                        tiempoDecont = (DeconTime - float.Parse(PluginManager.Manager.Server.Round.Duration.ToString()) / 60.0f).ToString("0.00");
                                    }
                                }
                                if (ev.Player.Scp079Data.Level > 1 || ev.Player.GetBypassMode())
                                {
                                    ClassDEscaped = PluginManager.Manager.Server.Round.Stats.ClassDEscaped.ToString("00");
                                    ScientistsEscaped = PluginManager.Manager.Server.Round.Stats.ScientistsEscaped.ToString("00");

                                    MTFAlive = PluginManager.Manager.Server.Round.Stats.NTFAlive.ToString("00");
                                    CiAlive = PluginManager.Manager.Server.Round.Stats.CiAlive.ToString("00");

                                    if (PluginManager.Manager.Server.Round.Duration - LastMtfSpawn < MinMTF)
                                    {
                                        MTFtiempo = "entre " + (MinMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0") + " segundos y " + (MaxMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0") + " segundos.";
                                    }
                                    else if (PluginManager.Manager.Server.Round.Duration - LastMtfSpawn < MaxMTF)
                                    {
                                        MTFtiempo = "menos de " + (MaxMTF - PluginManager.Manager.Server.Round.Duration + LastMtfSpawn).ToString("0");
                                    }
                                    else MTFtiempo = "están reapareciendo / van a reaparecer pronto.";
                                }
                                ev.Player.SendConsoleMessage(
                                "\nSCP vivos: " + PluginManager.Manager.Server.Round.Stats.SCPAlive +
                                "\nHumanos vivos: " + humansAlive + " | Siguientes MTF/Chaos: " + MTFtiempo +
                                "\nTiempo hasta la descontaminación: " + tiempoDecont +
                                "\nClase D escapados: " + ClassDEscaped + " | Científicos escapados: " + ScientistsEscaped +
                                "\nClase D vivos:     " + ClassDAlive + " | Chaos vivos:           " + CiAlive +
                                "\nCientíficos vivos: " + ScientistsAlive + " | MTF vivos:             " + MTFAlive
                                , "white");
                                ev.ReturnMessage = "Generadores:\n";
                                foreach (Generator generator in PluginManager.Manager.Server.Map.GetGenerators())
                                {
                                    ev.ReturnMessage = ev.ReturnMessage + "Generador de " + generator.Room.RoomType.ToString();
                                    if (generator.Engaged)
                                    {
                                        ev.ReturnMessage += " está activado.\n";
                                    }
                                    else
                                    {
                                        ev.ReturnMessage += (generator.HasTablet ? " tiene una tablet y le quedan " : " no tiene tablet y le quedan ") + generator.TimeLeft.ToString("0") + " segundos.\n";
                                    }
                                }
                                ev.Player.Scp079Data.AP -= 5;
                                ev.Player.Scp079Data.Exp += 5;
                                return;
                            default:
                                ev.ReturnMessage = ("Comando no reconocido. Usa .079 para ayuda");
                                return;
                        }
                    }
                    else if (args.Length > 1)
                    {
                        if (cooldownCassieGeneral && !ev.Player.GetBypassMode())
                        {
                            ev.ReturnMessage = ("Tienes que esperar antes de volver a usar un comando que requiera a cassie");
                            return;
                        }
                        switch (args[0])
                        {
                            case "mtf":

                                if (cooldownMTF && !ev.Player.GetBypassMode())
                                {
                                    ev.ReturnMessage = ("Tienes que esperar antes de volver a usar el comando MTF");
                                    return;
                                }
                                if (ev.Player.Scp079Data.Level >= 1 || ev.Player.GetBypassMode())
                                {
                                    if (ev.Player.Scp079Data.AP >= 80 || ev.Player.GetBypassMode())
                                    {
                                        if (args.Length == 4)
                                        {
                                            try
                                            {
                                                int.Parse(args[3]);
                                                int.Parse(args[2]);
                                                if (!char.IsLetter(args[1][0]))
                                                {
                                                    ev.ReturnMessage = ("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP - 80 de energía");
                                                    return;
                                                }
                                            }
                                            catch
                                            {
                                                ev.ReturnMessage = ("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP - 80 de energía");
                                                return;
                                            }
                                            ev.Player.Scp079Data.AP -= 80;
                                            PluginManager.Manager.Server.Map.AnnounceNtfEntrance(int.Parse(args[3]), int.Parse(args[2]), args[1][0]);
                                            ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
                                            ev.Player.Scp079Data.Exp += (2.8f * (ev.Player.Scp079Data.Level + 1));
                                            Timing.Run(CooldownCassie(20.0f));
                                            Timing.Run(CooldownMTF(60.0f));
                                            ev.ReturnMessage = "Comando lanzado.";
                                            return;
                                        }
                                        else
                                        {
                                            ev.ReturnMessage = ("Uso: .079 mtf (p) (5) (4), dirá que Papa-5 viene y quedan 4 SCP - 80 de energía");
                                        }

                                    }
                                    else
                                    {
                                        ev.ReturnMessage = ("No tienes suficiente energía (necesitas 80).");
                                    }
                                }
                                else
                                {
                                    ev.ReturnMessage = ("No tienes suficiente nivel");
                                }

                                return;
                            case "scp":
                                if (ev.Player.Scp079Data.AP < 50 && !ev.Player.GetBypassMode())
                                {
                                    ev.ReturnMessage = "No tienes suficiente energía (necesitas 50).";
                                    return;
                                }
                                if (args.Length >= 3)
                                {
                                    string[] scpList = new string[5]
                                    {
                                        "173", "096", "106", "049", "939"
                                    };
                                    if (!scpList.Contains(args[1]))
                                    {
                                        ev.ReturnMessage = ("Pon un SCP que exista - Uso: .079 scp (173/096/106/049/939) (unknown/tesla/mtf/decont)");
                                        return;
                                    }
                                    string scpNum = string.Join(" ", args[1].ToCharArray());
                                    switch (args[2])
                                    {
                                        case "mtf":
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
                                            string matador = "la MTF";
                                            if (dummy == null)
                                            {
                                                ev.Player.SendConsoleMessage("No hay MTFs vivos. Mandando como \"unknown\"", "red");
                                                matador = "\"Unknown\" (Chaos o Clase D)";
                                            }

                                            PluginManager.Manager.Server.Map.AnnounceScpKill(args[1], dummy);
                                            ev.ReturnMessage = "Comando lanzado (SCP " + args[1] + " matado por " + matador + ".";
                                            break;
                                        case "unknown":
                                            PluginManager.Manager.Server.Map.AnnounceScpKill(args[1], null);
                                            ev.ReturnMessage = "Comando lanzado (SCP " + args[1] + " matado por \"Unknown\".";
                                            break;
                                        case "tesla":
                                            PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Successfully Terminated by automatic security systems");
                                            ev.ReturnMessage = "Comando lanzado (SCP " + args[1] + " matado por Tesla.";
                                            break;
                                        case "decont":
                                            PluginManager.Manager.Server.Map.AnnounceCustomMessage("scp " + scpNum + " Lost in Decontamination Sequence");
                                            ev.ReturnMessage = "Comando lanzado (SCP " + args[1] + " matado por descontaminación.";
                                            break;
                                        default:
                                            ev.ReturnMessage = "Pon un método de morir que exista - Uso: .079 scp " + args[1] + " (unknown/tesla/mtf/decont)";
                                            return;
                                    }
                                }
                                else
                                {
                                    ev.ReturnMessage = ("Uso: .079 scp (173/096/106/049/939) (unknown/tesla/mtf/decont) - 50 de energía");
                                    return;
                                }
                                ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
                                ev.Player.Scp079Data.AP -= 50;
                                ev.Player.Scp079Data.Exp += (5.0f * (ev.Player.Scp079Data.Level + 1));
                                Timing.Run(CooldownCassie(20.0f));
                                return;
                            case "gen":
                                if (ev.Player.Scp079Data.Level < 1 && !ev.Player.GetBypassMode())
                                {
                                    ev.ReturnMessage = ("No tienes suficiente nivel (nivel 2 o superior)");
                                    return;
                                }
                                if (ev.Player.Scp079Data.AP < 50 && !ev.Player.GetBypassMode())
                                {
                                    ev.ReturnMessage = "No tienes suficiente energía (necesitas 50).";
                                    return;
                                }
                                if (cooldownGenerator && !ev.Player.GetBypassMode())
                                {
                                    ev.ReturnMessage = ("Tienes que esperar antes de volver a usar este comando");
                                    return;
                                }
                                switch (args[1])
                                {
                                    case "1":
                                        PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon1");
                                        ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
                                        ev.Player.Scp079Data.Exp += 20f;
                                        Timing.Run(CooldownGen(20.0f));
                                        Timing.Run(CooldownCassie(10.5f));
                                        ev.ReturnMessage = "Comando (generador 1) lanzado.";
                                        return;
                                    case "2":
                                        PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon2");
                                        ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
                                        ev.Player.Scp079Data.Exp += 20f;
                                        Timing.Run(CooldownGen(20.0f));
                                        Timing.Run(CooldownCassie(10.5f));
                                        ev.ReturnMessage = "Comando (generador 2) lanzado.";
                                        return;
                                    case "3":
                                        PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon3");
                                        ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
                                        ev.Player.Scp079Data.Exp += 20f;
                                        Timing.Run(CooldownGen(20.0f));
                                        Timing.Run(CooldownCassie(10.5f));
                                        ev.ReturnMessage = "Comando (generador 3) lanzado.";
                                        return;
                                    case "4":
                                        PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon4");
                                        ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
                                        ev.Player.Scp079Data.Exp += 20f;
                                        Timing.Run(CooldownGen(20.0f));
                                        Timing.Run(CooldownCassie(10.5f));
                                        ev.ReturnMessage = "Comando (generador 4) lanzado.";
                                        return;
                                    case "5":
                                        ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
                                        Timing.Run(Fingir5Gens());
                                        ev.Player.Scp079Data.Exp += 80f;
                                        Timing.Run(CooldownGen(180.0f));
                                        Timing.Run(CooldownCassie(10.5f));
                                        ev.ReturnMessage = ("Comando lanzado. Se reproducirá el mensaje de tu contención al completo, incluyendo cuando te matan y cuando se apagan/encienden las luces.");
                                        return;
                                    case "6":
                                        PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
                                        ev.Player.Scp079Data.ShowGainExp(ExperienceType.CHEAT);
                                        ev.Player.Scp079Data.Exp += 50f;
                                        Timing.Run(CooldownGen(120.0f));
                                        Timing.Run(CooldownCassie(10.5f));
                                        Timing.Run(FakeKillPC());
                                        ev.ReturnMessage = "Comando de falsear el suicidio mandado.";
                                        return;
                                    default:
                                        ev.ReturnMessage = ("Uso: .079 gen 1 - Sonará que hay 1 generador activado, del 1-5, pon 6 para mandar que has muerto (nivel 3)\nEl 5 falseará la sequencia completa de contención");
                                        return;
                                }
                            default:
                                MandaAyuda(ev.Player);
                                return;
                        }
                    }
                }
                else
                {
                    ev.ReturnMessage = ("¡No eres SCP-079!");
                }
            }
        }

        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            if (!plugin.GetConfigBool("p079_broadcast_enable")) return;

            ev.Player.PersonalClearBroadcasts();
            if (ev.Role == Role.SCP_079)
            {
                //ev.Player.PersonalBroadcast(20, "<color=#85ff4c>Presiona ñ para abrir la consola y usar comandos adicionales</color>", true);
                ev.Player.PersonalBroadcast(20, plugin.GetConfigString("p079_broadcast_msg"), true);
                MandaAyuda(ev.Player);
            }
        }

        public static IEnumerable<float> DisableTesla(Smod2.API.TeslaGate tesla, float current)
        {
            tesla.TriggerDistance = -1.0f;
            yield return 8.0f;
            tesla.TriggerDistance = current;
        }
        /* // no usado por estar OP pero podría ser usado uwu
		public static IEnumerable<float> OverCharge()
		{
			PluginManager.Manager.Server.Map.AnnounceCustomMessage("Warning All Security Personnel Unauthorized Use Of Error Error Error Error pitch_2 error error error error pitch_1 error");

			yield return 7.3f;

			foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
			{
				try { if (room.ZoneType == ZoneType.HCZ) room.FlickerLights(); }
				catch { }
			}

			foreach (Smod2.API.TeslaGate tesla in PluginManager.Manager.Server.Map.GetTeslaGates())
			{
				Timing.Run(DisableTesla(tesla, tesla.TriggerDistance));
			}
		}
		*/
        public static IEnumerable<float> FakeKillPC()
        {
            // falta cerrar puertas
            yield return 7.3f;

            foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(x => x.ZoneType == ZoneType.HCZ))
            {
                room.FlickerLights();
            }
            yield return 2.4f;
            PluginManager.Manager.Server.Map.AnnounceCustomMessage("SCP 0 7 9 Contained Successfully");
        }

        public static IEnumerable<float> Fingir5Gens()
        {
            PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon5");
            yield return 70.3f;
            PluginManager.Manager.Server.Map.AnnounceCustomMessage("Scp079Recon6");
            Timing.Run(FakeKillPC());
        }

        // These 4 functions could be replaced with one that takes a bool as a function parameter, but idk lol
        public static IEnumerable<float> CooldownMTF(float time)
        {
            cooldownMTF = true;
            yield return time;
            cooldownMTF = false;
        }

        public static IEnumerable<float> CooldownGen(float time)
        {
            cooldownGenerator = true;
            yield return time;
            cooldownGenerator = false;
        }

        public static IEnumerable<float> CooldownCassie(float time)
        {
            cooldownCassieGeneral = true;
            yield return time;
            cooldownCassieGeneral = false;
        }

        public static IEnumerable<float> CooldownInfo(float time)
        {
            infoCooldown = true;
            yield return time;
            infoCooldown = false;

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
                    MinMTF = (int) ev.Value;
                    return;
                case "maximum_MTF_time_to_spawn":
                    MaxMTF = (int) ev.Value;
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
                if (PluginManager.Manager.Server.Round.Stats.SCPAlive + PluginManager.Manager.Server.Round.Stats.Zombies - pcs == 0)
                {
                    foreach (Player player in PCplayers)
                    {
                        player.PersonalBroadcast(20, "Pulsa ñ y escribe \".079 suicidio\" para suicidarte.", false);
                    }
                }
            }
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            LastMtfSpawn = PluginManager.Manager.Server.Round.Duration;
        }
    }
}
