using UnityEngine;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent]
	internal sealed class HitActivator : Activator, IDamageable
	{
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private ushort _biggerDamage;
		public ushort Health => 0;
		public bool Damage(ushort damage)
		{
			if (damage >= this._biggerDamage)
				this.Activation();
			return damage >= this._biggerDamage;
		}
	};
};
