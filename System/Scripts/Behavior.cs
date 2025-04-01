namespace GuwbaPrimeAdventure
{
	public interface IDamageable
	{
		public bool Damage(ushort damage);
	};
	public interface IInteractable
	{
		public void Interaction();
	};
	public interface IGrabtable
	{
		public void Paralyze();
		public void Unparalyze();
	};
	public interface ICollectable
	{
		public void Collect();
	};
};
