using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Connection
{
	public sealed class Sender
	{
		private Sender()
		{
			this._toWhereConnection = PathConnection.None;
			this._stateForm = StateForm.None;
			this._additionalData = null;
			this._toggleValue = null;
			this._numberValue = null;
		}
		private static readonly List<IConnector> _connectors = new();
		private PathConnection _toWhereConnection;
		private StateForm _stateForm;
		private object _additionalData;
		private bool? _toggleValue;
		private uint? _numberValue;
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
		public Sender SetAdditionalData(object additionalData)
		{
			this._additionalData = additionalData;
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
			this._numberValue = indexValue;
			return this;
		}
		public void Send()
		{
			DataConnection dataConnection = new(this._stateForm, this._toggleValue, this._numberValue);
			foreach (IConnector connector in _connectors)
				if (connector.PathConnection == this._toWhereConnection)
					connector.Receive(dataConnection, this._additionalData);
		}
	};
};
