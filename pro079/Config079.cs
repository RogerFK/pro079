namespace Pro079Core
{
	public class Configs
	{
		public static Configs Instance;
		private readonly Pro079 plugin;
		public Configs(Pro079 plugin)
		{
			this.plugin = plugin;
			Instance = this;
		}
		//////////////////////////////////////////////////////
		//					CONFIG OPTIONS					//
		//////////////////////////////////////////////////////

		/// <summary>
		/// Bool to check if Pro-079 has been disabled through the config
		/// </summary>
		public bool Enabled => plugin.enable;
		/// <summary>
		/// Checks if the suicide command is enabled or not
		/// </summary>
		public bool SuicideCommand => plugin.suicide;
		/// <summary>
		/// Checks if ultimates are enabled.
		/// Useful if you're developing a ultimate that registers events to avoid causing unintentional lag.
		/// </summary>
		public bool UltimatesEnabled => plugin.ult;
		//////////////////////////////////////////////////////
		//				   LANGUAGE OPTIONS					//
		//////////////////////////////////////////////////////

		/// <summary>
		/// Gets the translated string for level. Something like: "level {number}"
		/// </summary>
		/// <param name="Level">The number of the level</param>
		/// <param name="Uppercase">If the first character should be uppercase</param>
		/// <returns>The translated level string</returns>
		public string LevelString(int Level, bool Uppercase)
		{
			if (Uppercase || char.IsDigit(plugin.level[0]))
			{
				return char.ToUpper(plugin.level[0])
					+ plugin.level.Substring(1).Replace("$lvl", Level.ToString());
			}

			return plugin.level.Replace("$lvl", Level.ToString());
		}
		/// <summary>
		/// Gets the translated string for AP. Something like: "{number} AP"
		/// </summary>
		/// <param name="AP">The number of the AP</param>
		/// <param name="Uppercase">If the first character should be uppercase</param>
		/// <returns>The translated AP string</returns>
		public string APString(int AP, bool Uppercase)
		{
			if (Uppercase || char.IsDigit(plugin.level[0]))
			{
				return char.ToUpper(plugin.level[0])
					 + plugin.level.Substring(1).Replace("$ap", AP.ToString());
			}

			return plugin.level.Replace("$ap", AP.ToString());
		}
		/// <summary>
		/// Gets the "Not enough AP (you need $min)" but translated
		/// </summary>
		/// <param name="MinAP">The minimum AP</param>
		/// <returns>The translated string</returns>
		public string LowAP(int MinAP)
		{
			return Min(plugin.lowmana, MinAP);
		}
		/// <summary>
		/// Gets the "Your level is too low (you need $min)" but translated
		/// </summary>
		/// <param name="MinLevel">The required level</param>
		/// <returns>The translated string</returns>
		public string LowLevel(int MinLevel)
		{
			return Min(plugin.lowlevel, MinLevel);
		}
		private string Min(string str, int number)
		{
			return str.Replace("$min", number.ToString());
		}
		/// <summary>
		/// Returns the cooldown translated
		/// </summary>
		public string CmdOnCooldown(int Cooldown)
		{
			return plugin.cooldown.Replace("$cd", Cooldown.ToString());
		}

		/// <summary>
		/// Translated "This command is disabled."
		/// </summary>
		public string CommandDisabled => plugin.disabled;
		/// <summary>
		/// Translated "Command succesfully launched".
		/// </summary>
		public string CommandSuccess => plugin.success;
		/// <summary>
		/// Translated "Ultimate succesfully used."
		/// </summary>
		public string UltimateLaunched => plugin.ultlaunched;
	}
}
