using System.Collections.Generic;
namespace GwambaPrimeAdventure.Connection
{
	public sealed class Sender
	{
		private Sender()
		{
			_dataConnection = new()
			{
				StateForm = StateForm.None,
				AdditionalData = null,
				ToggleValue = null,
				NumberValue = null
			};
		}
		private static readonly Dictionary<PathConnection, List<IConnector>> _connectors = new();
		private DataConnection _dataConnection;
		public static void Include(IConnector connector)
		{
			if (!_connectors.ContainsKey(connector.PathConnection))
				_connectors.Add(connector.PathConnection, new List<IConnector>() { connector });
			else if (!_connectors[connector.PathConnection].Contains(connector))
				_connectors[connector.PathConnection].Add(connector);
		}
		public static void Exclude(IConnector connector)
		{
			if (_connectors.ContainsKey(connector.PathConnection) && _connectors[connector.PathConnection].Contains(connector))
				_connectors[connector.PathConnection].Remove(connector);
		}
		public static Sender Create() => new();
		public void SetAdditionalData(object additionalData) => _dataConnection.AdditionalData = additionalData;
		public void SetStateForm(StateForm stateForm) => _dataConnection.StateForm = stateForm;
		public void SetToggle(bool value) => _dataConnection.ToggleValue = value;
		public void SetNumber(int value) => _dataConnection.NumberValue = (ushort)(value < 0f ? -value : value);
		public void Send(PathConnection path)
		{
			if (_connectors.ContainsKey(path))
				foreach (IConnector connector in _connectors[path].ToArray())
					if (connector != null && connector.PathConnection == path)
						connector.Receive(_dataConnection);
		}
	};
};
