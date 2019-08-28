namespace pro079.API
{
	public interface ICommand079
	{
		string Command { get; }
		string Usage { get; }
		bool Cassie { get; }
		int Cooldown { get; }
		string CallCommand(string[] args, Smod2.API.Player Player);
	}
	public interface IUltimate079
	{
		string Name { get; }
		int Cooldown { get; }
		string TriggerUltimate(string[] args, Smod2.API.Player Player);
	}
}
