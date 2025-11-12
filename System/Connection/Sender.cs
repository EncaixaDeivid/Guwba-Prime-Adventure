using System.Collections.Generic;
namespace GwambaPrimeAdventure.Connection
{
	public sealed class Sender
	{
		private Sender()
		{
			_stateForm = StateForm.None;
			_additionalData = null;
			_toggleValue = null;
			_numberValue = null;
		}
		private static readonly List<IConnector> _connectors = new();
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
		public void SetAdditionalData(object additionalData) => _additionalData = additionalData;
		public void SetStateForm(StateForm stateForm) => _stateForm = stateForm;
		public void SetToggle(bool value) => _toggleValue = value;
		public void SetNumber(int value) => _numberValue = (uint)(value < 0f ? -value : value);
		public void Send(PathConnection path)
		{
			foreach (IConnector connector in _connectors.ToArray())
				if (connector != null && connector.PathConnection == path)
					connector.Receive(new DataConnection(_stateForm, _toggleValue, _numberValue), _additionalData);
		}
	};
};
