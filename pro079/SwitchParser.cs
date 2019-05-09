using Smod2;

namespace pro079
{
	internal class SwitchParser
	{
		private static string[] cmds = new string[] {
			"teslacmd", // 1
			"teslascmd", // 2
			"mtfcmd", // 3
			"gencmd", // 4
			"scpcmd", // 5
			"infocmd", // 6
			"suicidecmd", // 7
			"ultcmd", // 8
			"chaoscmd", // 9
			"tipscmd" // 10
		};
		public static byte ParseArg(string arg, Plugin plugin)
		{
			for (byte i = 0; i < cmds.Length; i++)
			{
				if (plugin.GetTranslation(cmds[i]) == arg)
				{
					return (byte)(i + 1);
				}
			}
			return 0;
		}
	}
}