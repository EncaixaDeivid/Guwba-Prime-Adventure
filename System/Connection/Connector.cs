namespace GuwbaPrimeAdventure.Connection
{
	public enum PathConnection
	{
		None,
		System,
		Hud,
		Guwba,
		Enemy,
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
