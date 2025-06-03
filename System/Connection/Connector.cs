namespace GuwbaPrimeAdventure.Connection
{
	public enum PathConnection
	{
		None,
		Enemy,
		Boss,
		Item,
		EventItem,
		Hud,
		Controller,
		Dialog
	};
	public enum ConnectionState
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
