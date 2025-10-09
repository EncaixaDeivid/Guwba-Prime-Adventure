using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class DeathEnemy : EnemyProvider, IDestructible
	{
		private bool _isDead = false;
		private float _deathTime = 0f;
		[Header("Death Enemy")]
		[SerializeField, Tooltip("The death statitics of this enemy.")] private DeathStatistics _statistics;
		private new void Awake()
		{
			base.Awake();
			this._sender.SetStateForm(StateForm.State);
			this._sender.SetToggle(false);
		}
		private void Update()
		{
			if (this._isDead && !this.IsStunned)
			{
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
			{
				this._sender.Send(PathConnection.Enemy);
				this._isDead = true;
			}
		}
		public new bool Hurt(ushort damage)
		{
			if (this.Health - (short)damage <= 0f)
			{
				this._sender.Send(PathConnection.Enemy);
				return this._isDead = true;
			}
			return base.Hurt(damage);
		}
	};
};
