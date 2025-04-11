using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Connection
{
	public sealed class Sender
	{
		private Sender()
		{
			this._toggleValue = null;
			this._indexValue = null;
			this._connectionObject = ConnectionObject.All;
			this._connectionState = ConnectionState.None;
		}
		private IConnector _connectionToIgnore;
		private bool? _toggleValue;
		private uint? _indexValue;
		private ConnectionObject _connectionObject;
		private ConnectionState _connectionState;
		private static readonly List<IConnector> _connectors = new();
		public static void Implement(IConnector connector)
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
		public Sender SetConnectionObject(ConnectionObject connectionObject)
		{
			this._connectionObject = connectionObject;
			return this;
		}
		public Sender SetConnectionState(ConnectionState connectionState)
		{
			this._connectionState = connectionState;
			return this;
		}
		public Sender SetToggle(bool value)
		{
			this._toggleValue = value;
			return this;
		}
		public Sender SetIndex(int value)
		{
			this._indexValue = (uint)(value < 0f ? -value : value);
			return this;
		}
		public void Send()
		{
			DataConnection dataConnection = new(this._connectionState, this._toggleValue, this._indexValue);
			foreach (IConnector connector in _connectors)
			{
				bool isValid = this._connectionObject == ConnectionObject.All && connector.ConnectionObject == this._connectionObject;
				if (connector == this._connectionToIgnore || isValid)
					return;
				connector.Receive(dataConnection);
			};
		}
	};
};
