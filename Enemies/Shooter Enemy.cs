using UnityEngine;
using GwambaPrimeAdventure.Character;
using GwambaPrimeAdventure.Connection;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class ShooterEnemy : EnemyProvider, IConnector, IDestructible
	{
		private Vector2 _originCast;
		private Vector2 _directionCast;
		private Vector2 _targetDirection;
		private float _shootInterval = 0f;
		private float _timeStop = 0f;
		private bool _hasTarget = false;
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
		private void Shoot()
		{
			Quaternion rotation = Quaternion.identity;
			if (!_statistics.PureInstance)
				if (_statistics.CircularDetection)
					rotation = Quaternion.AngleAxis((Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg) - 90f, Vector3.forward);
				else
					rotation = Quaternion.AngleAxis(_statistics.RayAngleDirection * (_statistics.TurnRay ? (transform.localScale.x < 0f ? -1f : 1f) : 1f), Vector3.forward);
			foreach (Projectile projectile in _statistics.Projectiles)
				if (_statistics.PureInstance)
					Instantiate(projectile, _statistics.SpawnPoint, projectile.transform.rotation, new InstantiateParameters() { parent = transform, worldSpace = false }).transform.SetParent(null);
				else
					Instantiate(projectile, _statistics.SpawnPoint, rotation, new InstantiateParameters() { parent = transform, worldSpace = false }).transform.SetParent(null);
			if (_statistics.InvencibleShoot)
			{
				_sender.SetStateForm(StateForm.Event);
				_sender.SetToggle(true);
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
			{
				if ((_timeStop -= Time.deltaTime) <= _statistics.StopTime / 2f && _statistics.Stop && _canShoot)
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
				}
			}
		}
		private void FixedUpdate()
		{
			_hasTarget = false;
			if (_shootInterval <= 0f)
				if (_statistics.CircularDetection)
				{
					foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, _statistics.PerceptionDistance, WorldBuild.CharacterMask))
						if (collider.TryGetComponent<IDestructible>(out _))
							if (!Physics2D.Linecast(transform.position, collider.transform.position, WorldBuild.SceneMask))
							{
								_targetDirection = (collider.transform.position - transform.position).normalized;
								transform.TurnScaleX(transform.position.x < collider.transform.position.x);
								_hasTarget = true;
								break;
							}
				}
				else
				{
					_originCast = (Vector2)transform.position + _collider.offset;
					_originCast.x += _collider.bounds.extents.x * (transform.localScale.x < 0f ? -1f : 1f);
					_directionCast = Quaternion.AngleAxis(_statistics.RayAngleDirection, Vector3.forward) * Vector2.up;
					if (_statistics.TurnRay)
						_directionCast *= transform.localScale.x < 0f ? -1f : 1f;
					foreach (RaycastHit2D ray in Physics2D.RaycastAll(_originCast, _directionCast, _statistics.PerceptionDistance, WorldBuild.CharacterMask))
						if (ray.collider.TryGetComponent<IDestructible>(out _))
						{
							_hasTarget = true;
							break;
						}
				}
			if ((_hasTarget || _statistics.ShootInfinity) && _shootInterval <= 0f)
			{
				_shootInterval = _statistics.IntervalToShoot;
				if (_statistics.InvencibleShoot)
				{
					_sender.SetStateForm(StateForm.Event);
					_sender.SetToggle(false);
					_sender.Send(PathConnection.Enemy);
				}
				if (_statistics.Stop)
				{
					_timeStop = _statistics.StopTime;
					_sender.SetStateForm(StateForm.State);
					_sender.SetToggle(!(_isStopped = _canShoot = true));
					_sender.Send(PathConnection.Enemy);
					if (_statistics.Paralyze)
						Rigidbody.Sleep();
				}
				else
					Shoot();
			}
		}
		public new bool Hurt(ushort damage)
		{
			if (_statistics.ShootDamaged)
				Shoot();
			return base.Hurt(damage);
		}
		public void Receive(DataConnection data)
		{
			if (data.AdditionalData != null && data.AdditionalData is EnemyProvider[] && (data.AdditionalData as EnemyProvider[]).Length > 0)
				foreach (EnemyProvider enemy in data.AdditionalData as EnemyProvider[])
					if (enemy && enemy == this && data.StateForm == StateForm.Event && _statistics.ReactToDamage)
					{
						_targetDirection = (GwambaStateMarker.Localization - (Vector2)transform.position).normalized;
						transform.TurnScaleX(GwambaStateMarker.Localization.x < transform.position.x);
						Shoot();
						return;
					}
		}
	};
};
