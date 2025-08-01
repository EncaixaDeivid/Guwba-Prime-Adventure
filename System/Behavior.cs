using UnityEngine;
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
	public interface ICollectable
	{
		public void Collect();
	};
	public interface IImageComponents
	{
		public Sprite Image { get; }
		public Vector2 ImageOffset { get; }
	};
	public interface IImagePool
	{
		public void Pull();
		public void Push();
	};
};
