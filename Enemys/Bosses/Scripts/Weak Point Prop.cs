using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class WeakPointProp : BossController.BossProp, IDamageable
	{
		private bool _blockDamage = false;
		[Header("Weak Point"), SerializeField] private short _vitality;
		[SerializeField] private ushort _biggerDamage;
		[SerializeField] private float _timeToDamage;
		public ushort Health => (ushort)this._vitality;
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
					Sender.Create().SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Action)
						.SetBossType(BossType.All).SetIndex(this._indexEvent).Send();
				else
					Sender.Create().SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Action)
						.SetBossType(BossType.All).Send();
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
