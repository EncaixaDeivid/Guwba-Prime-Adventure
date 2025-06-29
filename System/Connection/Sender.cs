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
		public void SetAdditionalData(object additionalData) => this._additionalData = additionalData;
		public void SetToWhereConnection(PathConnection toWhereConnection) => this._toWhereConnection = toWhereConnection;
		public void SetStateForm(StateForm stateForm) => this._stateForm = stateForm;
		public void SetToggle(bool value) => this._toggleValue = value;
		public void SetNumber(int value)
		{
			uint indexValue = (uint)(value < 0f ? -value : value);
			this._numberValue = indexValue;
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
