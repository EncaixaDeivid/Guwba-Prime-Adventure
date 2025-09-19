namespace GuwbaPrimeAdventure.Connection
{
	public enum PathConnection
	{
		None,
		System,
		Hud,
		Guwba,
		Enemy,
		Boss,
		Item,
		EventItem,
		Story
	};
	public enum StateForm
	{
		None,
		State,
		Action
	};
	public interface IConnector
	{
		public PathConnection PathConnection { get; }
		public void Receive(DataConnection data, object additionalData);
	};
};
