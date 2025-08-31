using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class DeathEnemy : EnemyController, IDestructible
	{
		[Header("Death Enemy")]
		[SerializeField, Tooltip("The enemy that this enemy will spawn in death.")] private EnemyController _childEnemy;
		[SerializeField, Tooltip("The projectile that this enemy will spawn in death.")] private Projectile _childProjectile;
		[SerializeField, Tooltip("If this enemy will die on touch.")] private bool _onTouch;
		[SerializeField, Tooltip("If this enemy receives no type of damage.")] private float _deathTime;
		private new void Awake()
		{
			base.Awake();
			this._sender.SetStateForm(StateForm.State);
			this._sender.SetToggle(false);
		}
		private IEnumerator Death()
		{
			this._sender.Send();
			yield return new WaitTime(this, this._deathTime);
			if (this._childEnemy)
				Instantiate(this._childEnemy, this.transform.position, Quaternion.identity);
			if (this._childProjectile)
				Instantiate(this._childProjectile, this.transform.position, Quaternion.identity);
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._onTouch && CentralizableGuwba.EqualObject(other.gameObject))
				this.StartCoroutine(Death());
		}
		public new bool Damage(ushort damage)
		{
			if (this.Health - (short)damage <= 0f)
			{
				this.StartCoroutine(Death());
				return true;
			}
			return base.Damage(damage);
		}
	};
};
