using UnityEngine;
namespace GuwbaPrimeAdventure
{
	public interface IDestructible
	{
		public bool Damage(ushort damage);
		public void Stun(float stunStength, float stunTime);
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
