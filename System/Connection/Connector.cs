namespace GuwbaPrimeAdventure.Connection
{
	public enum PathConnection
	{
		None,
		System,
		Guwba,
		Enemy,
		Boss,
		Item,
		EventItem,
		Hud
	};
	public enum StateForm
	{
		None,
		State,
		Enable,
		Disable,
		Action
	};
	public interface IConnector
	{
		public PathConnection PathConnection { get; }
		public void Receive(DataConnection data, object additionalData);
	};
};
