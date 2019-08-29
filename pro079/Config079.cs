namespace pro079
{
	public class Config079
	{
		private readonly Pro079 plugin;

		public Config079(Pro079 plugin)
		{
			this.plugin = plugin;
		}
		//////////////////////////////////////////////////////
		//					CONFIG OPTIONS					//
		//////////////////////////////////////////////////////
		
		/// <summary>
		/// Bool to check if Pro-079 has been disabled through the config
		/// </summary>
		public bool Enabled
		{
			get
			{
				return plugin.enable;
			}
		}
		/// <summary>
		/// Checks if the suicide command is enabled or not
		/// </summary>
		public bool SuicideCommand
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
		public bool UltimatesEnabled
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
		/// Default user-defined string for when you/the user wants to disable the command.
		/// </summary>
		public string CommandDisabled
		{
			get
			{
				return plugin.disabled;
			}
		}

	}

}
