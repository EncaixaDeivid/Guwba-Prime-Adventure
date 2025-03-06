using UnityEngine;
using System.Collections;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class WeakPointProp : BossController.BossProp, IDamageable
	{
		private bool _blockDamage = false;
		[Header("Weak Point"), SerializeField] private short _vitality;
		[SerializeField] private ushort _biggerDamage;
		[SerializeField] private float _timeToDamage;
		public bool Damage(ushort damage)
		{
			if (this._blockDamage)
				return false;
			if (damage >= this._biggerDamage)
			{
				this.StartCoroutine(Timer());
				IEnumerator Timer()
				{
					this._blockDamage = true;
					yield return new WaitTime(this, this._timeToDamage);
					this._blockDamage = false;
				}
				this._vitality -= (short)damage;
				if (this._indexReact)
					if (this._multipleReact)
						foreach (BossController bossController in this._bossesControllers)
							bossController.Index(this._indexEvent);
					else
						this._bossesControllers[this._bossIndex].Index(this._indexEvent);
				else if (!this._indexReact)
					if (this._multipleReact)
						foreach (BossController bossController in this._bossesControllers)
							bossController.ReactToDamage();
					else
						this._bossesControllers[this._bossIndex].ReactToDamage();
				if (this._vitality <= 0)
				{
					this._useDestructuion = true;
					Destroy(this.gameObject);
				}
				return true;
			}
			return false;
		}
	};
};