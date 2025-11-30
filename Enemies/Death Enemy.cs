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
		private void Update()
		{
			if (_isDead)
				if ((_deathTime -= Time.deltaTime) <= 0f)
				{
					_isDead = false;
					if (_statistics.ChildEnemy)
						Instantiate(_statistics.ChildEnemy, _statistics.SpawnPoint, Quaternion.identity).transform.SetParent(null);
					if (_statistics.ChildProjectile)
						Instantiate(_statistics.ChildProjectile, _statistics.SpawnPoint, Quaternion.identity).transform.SetParent(null);
					Destroy(gameObject);
				}
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!_isDead && _statistics.OnTouch && other.TryGetComponent<IDestructible>(out _))
			{
				_sender.SetStateForm(StateForm.State);
				_sender.SetToggle(false);
				_sender.Send(PathConnection.Enemy);
				_deathTime = _statistics.TimeToDie;
				_isDead = true;
			}
		}
		public new bool Hurt(ushort damage)
		{
			if (_isDead)
				return false;
			if (Health - (short)damage <= 0f)
			{
				_sender.SetStateForm(StateForm.State);
				_sender.SetToggle(false);
				_sender.Send(PathConnection.Enemy);
				_deathTime = _statistics.TimeToDie;
				return _isDead = true;
			}
			return base.Hurt(damage);
		}
	};
};
