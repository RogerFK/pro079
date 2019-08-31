using MEC;
using System.Collections.Generic;
using Pro079.API;
using Smod2.API;
using Smod2;
using System;
using System.Linq;

namespace Pro079
{
	/// <summary>
	/// Manager that contains all commands and useful functions
	/// </summary>
	public static class Manager
	{
		private static Pro079Plugin plugin = Pro079Plugin.Instance;
		private static int CassieCd = 0;
		/// <summary>
		/// The remaining seconds for CASSIE to be active.
		/// </summary>
		public static int CassieCooldown
		{
			// The logic demands a high IQ or a lot of knowledge in C# to be understood.
			set
			{
				CassieCd = PluginManager.Manager.Server.Round.Duration + value;
			}
			get
			{
				int cd = CassieCd - PluginManager.Manager.Server.Round.Duration;
				return cd <= 0 ? 0 : cd;
			}
		}
		/// <summary>
		/// Dictionary with all the Commands and their respective handlers
		/// </summary>
		public static readonly Dictionary<string, ICommand079> Commands = new Dictionary<string, ICommand079>();
		/// <summary>
		/// Function used to register the current command. Doesn't register EventHandlers, so be aware of that.
		/// </summary>
		/// <param name="CommandHandler">The class that implements ICommand079</param>
		/// <returns></returns>
		public static string RegisterCommand(ICommand079 CommandHandler)
		{
			if(CommandHandler == null || string.IsNullOrEmpty(CommandHandler.Command))
			{
				return "Trying to register a \"null\" or an empty Command is not allowed!";
			}
			if (Commands.ContainsKey(CommandHandler.Command))
			{
				return "You can't register the same command twice!";
			}
			Commands.Add(CommandHandler.Command, CommandHandler);
			return "Command succesfully added";
		}
		/// <summary>
		/// Dictionary with all the Ultimates and their respective handlers
		/// </summary>
		public static readonly Dictionary<string, IUltimate079> Ultimates = new Dictionary<string, IUltimate079>();
		/// <summary>
		/// Function used to register the current command. Doesn't register EventHandlers, so be aware of that.
		/// </summary>
		/// <param name="UltimateHandler">The class that implements ICommand079</param>
		/// <returns></returns>
		public static string RegisterUltimate(IUltimate079 UltimateHandler)
		{
			if (UltimateHandler == null || string.IsNullOrEmpty(UltimateHandler.Name))
			{
				return "Trying to register a \"null\" or an empty name is not allowed!";
			}
			if (Ultimates.ContainsKey(UltimateHandler.Name))
			{
				return "An ultimate called " + UltimateHandler.Name + " was already added!";
			}
			Ultimates.Add(UltimateHandler.Name, UltimateHandler);
			return "Ultimate succesfully added";
		}

		/// <summary>
		/// Properly sets the cooldown for the given command, and delays a broadcast to tell the user when it's ready if the property <see cref="CommandReady"/> has been set.
		/// </summary>
		/// <param name="Command"></param>
		public static void SetOnCooldown(ICommand079 Command)
		{
			Command.CurrentCooldown = PluginManager.Manager.Server.Round.Duration + Command.Cooldown;

			if (!string.IsNullOrEmpty(Command.CommandReady))
			{
				int p = (int) System.Environment.OSVersion.Platform;
				if ((p == 4) || (p == 6) || (p == 128)) MEC.Timing.RunCoroutine(DelayMessage(Command.CommandReady, Command.Cooldown), MEC.Segment.Update);
				else MEC.Timing.RunCoroutine(DelayMessage(Command.CommandReady, Command.Cooldown), 1);
			}
		}
		private static int UltCooldown = 0;
		/// <summary>
		/// Remaining seconds for the ultimates to be ready, in seconds. 0 means it has no cooldown
		/// </summary>
		public static int UltimateCooldown
		{
			// The logic demands a high IQ or a lot of knowledge in C# to be understood.
			set
			{
				UltCooldown = PluginManager.Manager.Server.Round.Duration + value;
			}
			get
			{
				int cd = UltCooldown - PluginManager.Manager.Server.Round.Duration;
				return cd <= 0 ? 0 : cd; 
			}
		}
		public static void SetOnCooldown(IUltimate079 Ultimate)
		{
			UltimateCooldown = Ultimate.Cooldown + PluginManager.Manager.Server.Round.Duration;

			if (!string.IsNullOrEmpty(plugin.ultready) || plugin.ultready == "disable" || plugin.ultready == "disabled" || plugin.ultready == "none" || plugin.ultready == "null")
			{
				int p = (int)System.Environment.OSVersion.Platform;
				if ((p == 4) || (p == 6) || (p == 128)) MEC.Timing.RunCoroutine(DelayMessage(plugin.ultready, Ultimate.Cooldown), MEC.Segment.Update);
				else MEC.Timing.RunCoroutine(DelayMessage(plugin.ultready, Ultimate.Cooldown), 1); 
			}
		}
		private static IEnumerator<float> DelayMessage(string message, int delay)
		{
			yield return MEC.Timing.WaitForSeconds(delay);
			var pcs = PluginManager.Manager.Server.GetPlayers(Role.SCP_079);
			foreach (Player pc in pcs) pc.PersonalBroadcast(6, message, false);
		}
		/// <summary>
		/// Properly gives the player XP. Must be done per-command.
		/// </summary>
		/// <param name="player">The player to give XP to</param>
		/// <param name="XP">The amount of XP</param>
		/// <param name="xptype">The experience type you want to show the player (optional)</param>
		public static void GiveExp(Player player, float XP, ExperienceType xptype = (ExperienceType)(-1))
		{
			player.Scp079Data.Exp += XP;
			if (xptype != (ExperienceType)(-1)) player.Scp079Data.ShowGainExp(xptype);
		}
		/// <summary>
		/// Drains the AP from a player. Will never go below 0. Negative amounts probably add AP instead of draining it.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		public static void DrainAP(Player player, float amount)
		{
			if (player.Scp079Data.AP < amount) player.Scp079Data.AP = 0;
			else player.Scp079Data.AP -= amount;
		}
		/// <summary>
		/// Gets the ultimate based on the name or based on how it starts.
		/// </summary>
		/// <param name="Name"></param>
		/// <returns></returns>
		public static IUltimate079 GetUltimate(string Name)
		{
			if (Ultimates.TryGetValue(Name, out IUltimate079 ultimate))
			{
				return ultimate;
			}
			string name = Ultimates.Keys.OrderBy(x => x.Length).FirstOrDefault(x => x.StartsWith(Name));
			if(name == null)
			{
				return null;
			}
			return Ultimates[name];
		}
	}
}
