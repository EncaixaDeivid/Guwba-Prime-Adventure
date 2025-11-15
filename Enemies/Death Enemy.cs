using UnityEngine;
using GwambaPrimeAdventure.Connection;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
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
			_sender.SetStateForm(StateForm.State);
			_sender.SetToggle(false);
		}
		private void Update()
		{
			if (_isDead && !IsStunned)
			{
				_deathTime -= Time.deltaTime;
				if (_deathTime <= 0f)
				{
					_isDead = false;
					if (_statistics.ChildEnemy)
						Instantiate(_statistics.ChildEnemy, transform.position, Quaternion.identity);
					if (_statistics.ChildProjectile)
						Instantiate(_statistics.ChildProjectile, transform.position, Quaternion.identity);
					Destroy(gameObject);
				}
			}
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_statistics.OnTouch && other.TryGetComponent<IDestructible>(out _))
			{
				_sender.Send(PathConnection.Enemy);
				_deathTime = _statistics.TimeToDie;
				_isDead = true;
			}
		}
		public new bool Hurt(ushort damage)
		{
			if (Health - (short)damage <= 0f)
			{
				_sender.Send(PathConnection.Enemy);
				_deathTime = _statistics.TimeToDie;
				return _isDead = true;
			}
			return base.Hurt(damage);
		}
	};
};
