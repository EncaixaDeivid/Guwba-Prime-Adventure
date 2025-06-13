namespace GuwbaPrimeAdventure.Connection
{
	public enum PathConnection
	{
		None,
		Character,
		Enemy,
		Boss,
		Item,
		EventItem,
		Hud,
		Controller,
		Dialog
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
