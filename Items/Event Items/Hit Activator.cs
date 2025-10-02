using UnityEngine;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Collider2D))]
	internal sealed class HitActivator : Activator, IDestructible
	{
		[Header("Hit Activator")]
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private ushort _biggerDamage;
		public short Health => 0;
		public bool Hurt(ushort damage)
		{
			if (damage >= this._biggerDamage && this.Usable)
				this.Activation();
			return damage >= this._biggerDamage && this.Usable;
		}
		public void Stun(ushort stunStength, float stunTime) { }
	};
};
