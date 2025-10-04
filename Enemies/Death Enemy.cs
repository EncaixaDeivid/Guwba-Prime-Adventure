using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class DeathEnemy : EnemyController, IDestructible
	{
		private bool _isDead = false;
		private bool _cancelSend = false;
		private float _deathTime = 0f;
		[Header("Death Enemy")]
		[SerializeField, Tooltip("The death statitics of this enemy.")] private DeathStatistics _statistics;
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
				if (this._deathTime >= this._statistics.TimeToDie)
				{
					this._isDead = false;
					if (this._statistics.ChildEnemy)
						Instantiate(this._statistics.ChildEnemy, this.transform.position, Quaternion.identity);
					if (this._statistics.ChildProjectile)
						Instantiate(this._statistics.ChildProjectile, this.transform.position, Quaternion.identity);
					Destroy(this.gameObject);
				}
			}
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._statistics.OnTouch && other.TryGetComponent<IDestructible>(out _))
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
