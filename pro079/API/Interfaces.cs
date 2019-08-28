namespace pro079.API
{
	public interface ICommand079
	{
		string Command { get; }
		string HelpInfo { get; }
		bool Cassie { get; }
		int Cooldown { get; }
		string CommandReady { get; }
		int CurrentCooldown { set; get; }
		string CallCommand(string[] args, Smod2.API.Player Player);
		bool Disabled { set; get }
	}
	public interface IUltimate079
	{
		string Name { get; }
		int Cooldown { get; }
		string TriggerUltimate(string[] args, Smod2.API.Player Player);
	}
}
