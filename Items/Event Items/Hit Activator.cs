using UnityEngine;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent]
	internal sealed class HitActivator : Activator, IDamageable
	{
		[Header("Hit Activator")]
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private ushort _biggerDamage;
		public float StunResistance => 0f;
		public bool Damage(ushort damage)
		{
			if (damage >= this._biggerDamage && this.Usable)
				this.Activation();
			return damage >= this._biggerDamage && this.Usable;
		}
	};
};
