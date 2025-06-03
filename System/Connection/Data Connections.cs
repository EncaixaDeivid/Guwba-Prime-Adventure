namespace GuwbaPrimeAdventure.Connection
{
	public struct DataConnection
	{
		public ConnectionState ConnectionState { get; private set; }
		public bool? ToggleValue { get; private set; }
		public uint? IndexValue { get; private set; }
		internal DataConnection(ConnectionState connectionState, bool? toggleValue, uint? indexValue)
		{
			this.ConnectionState = connectionState;
			this.ToggleValue = toggleValue;
			this.IndexValue = indexValue;
		}
	};
};
