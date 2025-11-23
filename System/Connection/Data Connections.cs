namespace GwambaPrimeAdventure.Connection
{
	public interface IConnector
	{
		public PathConnection PathConnection { get; }
		public void Receive(DataConnection data);
	};
	public struct DataConnection
	{
		public StateForm StateForm { get; internal set; }
		public object AdditionalData { get; internal set; }
		public bool? ToggleValue { get; internal set; }
		public ushort? NumberValue { get; internal set; }
	};
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
};
