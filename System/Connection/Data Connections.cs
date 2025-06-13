using Unity.Jobs;
namespace GuwbaPrimeAdventure.Connection
{
	public struct DataConnection : IJob
	{
		private readonly Sender _sender;
		public StateForm ConnectionState { get; private set; }
		public bool? ToggleValue { get; private set; }
		public uint? IndexValue { get; private set; }
		internal DataConnection(Sender sender, StateForm connectionState, bool? toggleValue, uint? indexValue)
		{
			this._sender = sender;
			this.ConnectionState = connectionState;
			this.ToggleValue = toggleValue;
			this.IndexValue = indexValue;
		}
		public readonly void Execute()
		{
			foreach (IConnector connector in Sender.Connectors)
			{
				if (connector == this._sender.ConnectionToIgnore || connector.PathConnection != this._sender.ToWhereConnection)
					continue;
				connector.Receive(this, this._sender.AdditionalData);
			}
		}
	};
};
