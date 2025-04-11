namespace GuwbaPrimeAdventure.Connection
{
	public enum ConnectionObject
	{
		All,
		Enemy,
		Boss,
		Item,
		EventItem,
		Hud,
		Controller
	};
	public enum ConnectionState
	{
		None,
		Enable,
		Disable,
		Action
	};
	public interface IConnector
	{
		public ConnectionObject ConnectionObject { get; }
		public void Receive(DataConnection data);
	};
};
