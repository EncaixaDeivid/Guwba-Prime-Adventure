using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Connection
{
	public sealed class Sender
	{
		private Sender()
		{
			this._toggleValue = null;
			this._indexValue = null;
			this._fromConnection = PathConnection.None;
			this._toWhereConnection = PathConnection.None;
			this._connectionState = ConnectionState.None;
			this._bossType = BossType.None;
		}
		private IConnector _connectionToIgnore;
		private bool? _toggleValue;
		private uint? _indexValue;
		private PathConnection _fromConnection;
		private PathConnection _toWhereConnection;
		private ConnectionState _connectionState;
		private BossType _bossType;
		private static readonly List<IConnector> _connectors = new();
		public static void Include(IConnector connector)
		{
			if (!_connectors.Contains(connector))
				_connectors.Add(connector);
		}
		public static void Exclude(IConnector connector)
		{
			if (_connectors.Contains(connector))
				_connectors.Remove(connector);
		}
		public static Sender Create() => new();
		public Sender SetObjectToIgnore(IConnector instanceToIgnore)
		{
			this._connectionToIgnore = instanceToIgnore;
			return this;
		}
		public Sender SetFromConnection(PathConnection fromConnection)
		{
			this._fromConnection = fromConnection;
			return this;
		}
		public Sender SetToWhereConnection(PathConnection toWhereConnection)
		{
			this._toWhereConnection = toWhereConnection;
			return this;
		}
		public Sender SetConnectionState(ConnectionState connectionState)
		{
			this._connectionState = connectionState;
			return this;
		}
		public Sender SetBossType(BossType bossType)
		{
			this._bossType = bossType;
			return this;
		}
		public Sender SetToggle(bool value)
		{
			this._toggleValue = value;
			return this;
		}
		public Sender SetIndex(int value)
		{
			uint indexValue = (uint)(value < 0f ? -value : value);
			this._indexValue = indexValue;
			return this;
		}
		public void Send()
		{
			DataConnection dataConnection = new(this._fromConnection, this._connectionState, this._bossType, this._toggleValue, this._indexValue);
			foreach (IConnector connector in _connectors)
			{
				bool isValid = this._toWhereConnection != PathConnection.None && connector.PathConnection == this._toWhereConnection;
				if (connector == this._connectionToIgnore || isValid)
					return;
				connector.Receive(dataConnection);
			};
		}
	};
};
