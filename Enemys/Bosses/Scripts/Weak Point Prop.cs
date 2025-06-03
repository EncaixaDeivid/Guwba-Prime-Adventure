using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class WeakPointProp : BossProp, IDamageable
	{
		private readonly Sender _sender = Sender.Create();
		private bool _blockDamage = false;
		[Header("Weak Point")]
		[SerializeField, Tooltip("The vitality of this prop.")] private short _vitality;
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private ushort _biggerDamage;
		[SerializeField, Tooltip("The amount of time to wait after damaging the prop again.")] private float _timeToDamage;
		public ushort Health => (ushort)this._vitality;
		private new void Awake()
		{
			base.Awake();
			this._sender.SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Action);
			this._sender.SetAdditionalData(BossType.All);
		}
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
					this._sender.SetIndex(this._indexEvent).Send();
				else
					this._sender.Send();
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
