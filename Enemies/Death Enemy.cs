using UnityEngine;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class DeathEnemy : EnemyController, IDestructible
	{
		private bool _isDead = false;
		private bool _cancelSend = false;
		private float _deathTime = 0;
		[Header("Death Enemy")]
		[SerializeField, Tooltip("The enemy that this enemy will spawn in death.")] private EnemyController _childEnemy;
		[SerializeField, Tooltip("The projectile that this enemy will spawn in death.")] private EnemyProjectile _childProjectile;
		[SerializeField, Tooltip("If this enemy will die on touch.")] private bool _onTouch;
		[SerializeField, Tooltip("THe time to this enemy die.")] private float _timeToDie;
		private new void Awake()
		{
			base.Awake();
			this._sender.SetStateForm(StateForm.State);
			this._sender.SetToggle(false);
		}
		private new void Update()
		{
			base.Update();
			if (this._isDead)
			{
				if (this._cancelSend)
				{
					this._sender.Send(PathConnection.Enemy);
					this._cancelSend = false;
				}
				this._sender.Send(PathConnection.Enemy);
				this._deathTime += Time.deltaTime;
				if (this._deathTime >= this._timeToDie)
				{
					this._isDead = false;
					if (this._childEnemy)
						Instantiate(this._childEnemy, this.transform.position, Quaternion.identity);
					if (this._childProjectile)
						Instantiate(this._childProjectile, this.transform.position, Quaternion.identity);
				}
			}
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._onTouch && CentralizableGuwba.EqualObject(other.gameObject))
				this._isDead = this._cancelSend = true;
		}
		public new bool Hurt(ushort damage)
		{
			if (this.Health - (short)damage <= 0f)
			{
				this._isDead = this._cancelSend = true;
				return true;
			}
			return base.Hurt(damage);
		}
	};
};
