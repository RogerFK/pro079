using Smod2;

namespace ddd
{
	internal class SwitchParser
	{
		private static string[] cmds = new string[] {
			"newteslacmd", // 1
			"mtfcmd", // 2
			"gencmd", // 3
			"scpcmd", // 4
			"infocmd", // 5
			"suicidecmd", // 6
			"ultcmd", // 7
			"chaoscmd", // 8
			"tipscmd" // 9
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