using Unity.Jobs;
namespace GuwbaPrimeAdventure.Connection
{
	internal struct SenderPoperties
	{
		internal IConnector ConnectionToIgnore { get; private set; }
		internal PathConnection ToWhereConnection { get; private set; }
		internal object AdditionalData { get; private set; }
		internal SenderPoperties(IConnector connectionToIgnore, PathConnection toWhereConnection, object additionalData)
		{
			this.ConnectionToIgnore = connectionToIgnore;
			this.ToWhereConnection = toWhereConnection;
			this.AdditionalData = additionalData;
		}
	};
	public struct DataConnection : IJob
	{
		private readonly SenderPoperties _senderPoperties;
		public StateForm StateForm { get; private set; }
		public bool? ToggleValue { get; private set; }
		public uint? IndexValue { get; private set; }
		internal DataConnection(SenderPoperties senderPoperties, StateForm stateForm, bool? toggleValue, uint? indexValue)
		{
			this._senderPoperties = senderPoperties;
			this.StateForm = stateForm;
			this.ToggleValue = toggleValue;
			this.IndexValue = indexValue;
		}
		public readonly void Execute()
		{
			foreach (IConnector connector in Sender.Connectors)
			{
				if (connector == this._senderPoperties.ConnectionToIgnore || connector.PathConnection != this._senderPoperties.ToWhereConnection)
					continue;
				connector.Receive(this, this._senderPoperties.AdditionalData);
			}
		}
	};
};
