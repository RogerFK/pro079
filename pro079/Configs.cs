namespace Pro079
{
	/// <summary>
	/// User defined configurations and language options
	/// </summary>
	public static class Configs
	{
		private static Pro079Plugin plugin = Pro079Plugin.Instance;
		//////////////////////////////////////////////////////
		//					CONFIG OPTIONS					//
		//////////////////////////////////////////////////////
		
		/// <summary>
		/// Bool to check if Pro-079 has been disabled through the config
		/// </summary>
		public static bool Enabled
		{
			get
			{
				return plugin.enable;
			}
		}
		/// <summary>
		/// Checks if the suicide command is enabled or not
		/// </summary>
		public static bool SuicideCommand
		{
			get
			{
				return plugin.suicide;
			}
		}
		/// <summary>
		/// Checks if ultimates are enabled.
		/// Useful if you're developing a ultimate that registers events to avoid causing unintentional lag.
		/// </summary>
		public static bool UltimatesEnabled
		{
			get
			{
				return plugin.ult;
			}
		}
		//////////////////////////////////////////////////////
		//				   LANGUAGE OPTIONS					//
		//////////////////////////////////////////////////////

		/// <summary>
		/// Gets the translated string for level. Something like: "level {number}"
		/// </summary>
		/// <param name="Level">The number of the level</param>
		/// <param name="Uppercase">If the first character should be uppercase</param>
		/// <returns>The translated level string</returns>
		public static string LevelString(int Level, bool Uppercase)
		{
			if(Uppercase || char.IsDigit(plugin.level[0])) return char.ToUpper(plugin.level[0])
					+ plugin.level.Substring(1).Replace("$lvl", Level.ToString());
			return plugin.level.Replace("$lvl", Level.ToString());
		}
		/// <summary>
		/// Gets the translated string for AP. Something like: "{number} AP"
		/// </summary>
		/// <param name="AP">The number of the AP</param>
		/// <param name="Uppercase">If the first character should be uppercase</param>
		/// <returns>The translated AP string</returns>
		public static string APString(int AP, bool Uppercase)
		{
			if (Uppercase || char.IsDigit(plugin.level[0])) return char.ToUpper(plugin.level[0])
					 + plugin.level.Substring(1).Replace("$ap", AP.ToString());
			return plugin.level.Replace("$ap", AP.ToString());
		}
		/// <summary>
		/// Gets the "Not enough AP (you need $min)" but translated
		/// </summary>
		/// <param name="MinAP">The minimum AP</param>
		/// <returns>The translated string</returns>
		public static string LowAP(int MinAP)
		{
			return Min(plugin.lowmana, MinAP);
		}
		/// <summary>
		/// Gets the "Your level is too low (you need $min)" but translated
		/// </summary>
		/// <param name="MinLevel">The required level</param>
		/// <returns>The translated string</returns>
		public static string LowLevel(int MinLevel)
		{
			return Min(plugin.level, MinLevel);
		}
		private static string Min(string str, int number)
		{
			return str.Replace("$min", number.ToString());
		}
		/// <summary>
		/// Returns the cooldown translated
		/// </summary>
		public static string CmdOnCooldown(int Cooldown)
		{
			return plugin.cooldown.Replace("$cd", Cooldown.ToString());
		}

		/// <summary>
		/// Translated "This command is disabled."
		/// </summary>
		public static string CommandDisabled
		{
			get
			{
				return plugin.disabled;
			}
		}
		/// <summary>
		/// Translated "Command succesfully launched".
		/// </summary>
		public static string CommandSuccess
		{
			get
			{
				return plugin.success;
			}
		}
		/// <summary>
		/// Translated "Ultimate succesfully used."
		/// </summary>
		public static string UltimateLaunched
		{
			get
			{
				return plugin.ultlaunched;
			}
		}
	}
}
