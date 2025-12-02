namespace GwambaPrimeAdventure.Connection
{
	public interface IConnector
	{
		public MessagePath Path { get; }
		public void Receive(MessageData data);
	};
	public struct MessageData
	{
		public MessageFormat Format { get; internal set; }
		public object AdditionalData { get; internal set; }
		public bool? ToggleValue { get; internal set; }
		public ushort? NumberValue { get; internal set; }
	};
	public enum MessagePath
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
	public enum MessageFormat
	{
		None,
		State,
		Event
	};
};
