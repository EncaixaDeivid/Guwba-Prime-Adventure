namespace GwambaPrimeAdventure.Connection
{
	public enum PathConnection
	{
		None,
		System,
		Hud,
		Character,
		Enemy,
		Item,
		EventItem,
		Story
	};
	public enum StateForm
	{
		None,
		State,
		Event
	};
	public interface IConnector
	{
		public PathConnection PathConnection { get; }
		public void Receive(DataConnection data, object additionalData);
	};
};
