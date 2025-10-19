using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class ShooterEnemy : EnemyProvider, IConnector, IDestructible
	{
		private Vector2 _targetDirection;
		private float _shootInterval = 0f;
		private float _timeStop = 0f;
		private bool _canShoot = false;
		private bool _isStopped = false;
		[Header("Shooter Enemy")]
		[SerializeField, Tooltip("The shooter statitics of this enemy.")] private ShooterStatistics _statistics;
		private new void Awake()
		{
			base.Awake();
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private void Verify()
		{
			bool hasTarget = false;
			if (_statistics.CircularDetection)
			{
				foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, _statistics.PerceptionDistance, _statistics.Physics.TargetLayer))
					if (collider.TryGetComponent<IDestructible>(out _))
					{
						if (Physics2D.Linecast(transform.position, collider.transform.position, _statistics.Physics.GroundLayer))
							continue;
						_targetDirection = (collider.transform.position - transform.position).normalized;
						hasTarget = true;
					}
			}
			else
			{
				Vector2 origin = new(transform.position.x + _collider.bounds.extents.x * (transform.localScale.x < 0f ? -1f : 1f), transform.position.y);
				Vector2 direction = Quaternion.AngleAxis(_statistics.RayAngleDirection, Vector3.forward) * Vector2.up;
				if (_statistics.TurnRay)
					direction *= transform.localScale.x < 0f ? -1f : 1f;
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(origin, direction, _statistics.PerceptionDistance, _statistics.Physics.TargetLayer))
					if (ray.collider.TryGetComponent<IDestructible>(out _))
						hasTarget = true;
			}
			if ((hasTarget || _statistics.ShootInfinity) && _shootInterval <= 0f)
			{
				_shootInterval = _statistics.IntervalToShoot;
				if (_statistics.InvencibleShoot)
				{
					_sender.SetStateForm(StateForm.Action);
					_sender.SetToggle(true);
					_sender.Send(PathConnection.Enemy);
				}
				if (_statistics.Stop)
				{
					_timeStop = _statistics.StopTime;
					_sender.SetStateForm(StateForm.State);
					_sender.SetToggle(!(_isStopped = _canShoot = true));
					_sender.Send(PathConnection.Enemy);
					if (_statistics.Paralyze)
						_rigidybody.Sleep();
				}
				else
					Shoot();
			}
		}
		private void Shoot()
		{
			foreach (EnemyProjectile projectile in _statistics.Projectiles)
				if (_statistics.PureInstance)
					Instantiate(projectile, transform.position, projectile.transform.rotation, transform);
				else
				{
					Vector2 position = transform.position;
					Quaternion rotation = Quaternion.AngleAxis(_statistics.RayAngleDirection, Vector3.forward);
					if (_statistics.CircularDetection)
						rotation = Quaternion.AngleAxis((Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg) - 90f, Vector3.forward);
					if (!_statistics.InstanceOnSelf)
						position += (Vector2)(rotation * Vector2.up);
					Instantiate(projectile, position, rotation, transform);
				}
			if (_statistics.InvencibleShoot)
			{
				_sender.SetStateForm(StateForm.Action);
				_sender.SetToggle(false);
				_sender.Send(PathConnection.Enemy);
			}
		}
		private void Update()
		{
			if (_stopWorking || IsStunned)
				return;
			if (_shootInterval > 0f && !_isStopped)
				_shootInterval -= Time.deltaTime;
			if (_timeStop > 0f)
				_timeStop -= Time.deltaTime;
			if (_statistics.Stop && _canShoot && _timeStop <= _statistics.StopTime / 2f)
			{
				_canShoot = false;
				Shoot();
			}
			if (_timeStop <= 0f && _isStopped)
			{
				_isStopped = false;
				_sender.SetStateForm(StateForm.State);
				_sender.SetToggle(true);
				_sender.Send(PathConnection.Enemy);
				if (_statistics.ReturnParalyze)
					_rigidybody.WakeUp();
			}
		}
		private void FixedUpdate() => Verify();
		public new bool Hurt(ushort damage)
		{
			if (_statistics.ShootDamaged)
				Shoot();
			return base.Hurt(damage);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if ((EnemyProvider[])additionalData != null)
				foreach (EnemyProvider enemy in (EnemyProvider[])additionalData)
					if (enemy == this && data.StateForm == StateForm.Action && _statistics.ReactToDamage)
						Shoot();
		}
	};
};
