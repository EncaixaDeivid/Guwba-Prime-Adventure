namespace GuwbaPrimeAdventure.Connection
{
	public struct DataConnection
	{
		public StateForm StateForm { get; private set; }
		public bool? ToggleValue { get; private set; }
		public uint? IndexValue { get; private set; }
		internal DataConnection(StateForm stateForm, bool? toggleValue, uint? indexValue)
		{
			this.StateForm = stateForm;
			this.ToggleValue = toggleValue;
			this.IndexValue = indexValue;
		}
	};
};
