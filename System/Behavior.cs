namespace GuwbaPrimeAdventure
{
	public interface IDestructible
	{
		public short Health { get; }
		public bool Hurt(ushort damage);
		public void Stun(ushort stunStength, float stunTime);
	};
	public interface IInteractable
	{
		public void Interaction();
	};
	public interface ICollectable
	{
		public void Collect();
	};
};
