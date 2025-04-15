namespace GuwbaPrimeAdventure.Connection
{
	public struct DataConnection
	{
		public PathConnection FromConnection { get; private set; }
		public ConnectionState ConnectionState { get; private set; }
		public BossType BossType { get; private set; }
		public bool? ToggleValue { get; private set; }
		public uint? IndexValue { get; private set; }
		internal DataConnection(
			PathConnection fromConnection,
			ConnectionState connectionState,
			BossType bossType,
			bool? toggleValue,
			uint? indexValue)
		{
			this.FromConnection = fromConnection;
			this.ConnectionState = connectionState;
			this.BossType = bossType;
			this.ToggleValue = toggleValue;
			this.IndexValue = indexValue;
		}
	};
};
