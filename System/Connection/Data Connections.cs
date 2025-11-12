namespace GwambaPrimeAdventure.Connection
{
	public struct DataConnection
	{
		public StateForm StateForm { get; private set; }
		public bool? ToggleValue { get; private set; }
		public uint? NumberValue { get; private set; }
		internal DataConnection(StateForm stateForm, bool? toggleValue, uint? numberValue)
		{
			StateForm = stateForm;
			ToggleValue = toggleValue;
			NumberValue = numberValue;
		}
	};
};
