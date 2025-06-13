using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Connection
{
	public sealed class Sender
	{
		private Sender()
		{
			this._toWhereConnection = PathConnection.None;
			this._stateForm = StateForm.None;
			this.additionalData = null;
			this._toggleValue = null;
			this._indexValue = null;
		}
		private static readonly List<IConnector> _connectors = new();
		private IConnector _connectionToIgnore;
		private PathConnection _toWhereConnection;
		private StateForm _stateForm;
		private object additionalData;
		private bool? _toggleValue;
		private uint? _indexValue;
		internal static IReadOnlyList<IConnector> Connectors => _connectors.AsReadOnly();
		internal IConnector ConnectionToIgnore => this._connectionToIgnore;
		internal PathConnection ToWhereConnection => this._toWhereConnection;
		internal object AdditionalData => this.additionalData;
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
		public Sender SetAdditionalData(object additionalData)
		{
			this.additionalData = additionalData;
			return this;
		}
		public Sender SetToWhereConnection(PathConnection toWhereConnection)
		{
			this._toWhereConnection = toWhereConnection;
			return this;
		}
		public Sender SetStateForm(StateForm stateForm)
		{
			this._stateForm = stateForm;
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
		public void Send() => new DataConnection(this, this._stateForm, this._toggleValue, this._indexValue).Execute();
	};
};
