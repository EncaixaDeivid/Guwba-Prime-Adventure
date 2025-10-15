using UnityEngine;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class RunnerEnemy : MovingEnemy, IConnector
	{
		private bool _runTowards = false;
		private ushort _runnedTimes = 0;
		private float _timeRun = 0f;
		private float _dashedTime = 0f;
		private float _dashTime = 0f;
		[Header("Runner Enemy")]
		[SerializeField, Tooltip("The runner statitics of this enemy.")] private RunnerStatistics _statistics;
		private void Start() => _timeRun = _statistics.TimesToRun;
		private void Update()
		{
			if (IsStunned)
				return;
			if (_statistics.DetectionStop && _stopWorking)
			{
				_stoppedTime += Time.deltaTime;
				if (_stoppedTime >= _statistics.StopTime)
				{
					_stoppedTime = 0f;
					_stopWorking = false;
					_isDashing = true;
					_sender.SetToggle(_statistics.JumpDash);
					_sender.Send(PathConnection.Enemy);
				}
				return;
			}
			if (_stopWorking)
				return;
			if (_statistics.TimedDash && !_isDashing)
			{
				_dashTime += Time.deltaTime;
				if (_dashTime >= _statistics.TimeToDash)
				{
					_dashTime = 0f;
					_isDashing = true;
				}
			}
			if (_statistics.RunFromTarget)
			{
				if (_timeRun > 0f)
				{
					_timeRun -= Time.deltaTime;
					_isDashing = true;
				}
				if (_timeRun <= 0f && _isDashing)
				{
					if (_statistics.RunTowardsAfter && _runnedTimes >= _statistics.TimesToRun)
					{
						_runnedTimes = 0;
						_runTowards = true;
					}
					else if (_statistics.RunTowardsAfter)
						_runnedTimes++;
					_isDashing = false;
				}
			}
			if (_isDashing)
			{
				_dashedTime += Time.deltaTime;
				if (_dashedTime >= _statistics.TimeDashing)
				{
					_detected = false;
					_dashedTime = 0f;
					_isDashing = false;
					_sender.SetToggle(true);
					_sender.Send(PathConnection.Enemy);
				}
			}
		}
		private void FixedUpdate()
		{
			if (IsStunned)
				return;
			if (_statistics.DetectionStop && _detected && !_isDashing && SurfacePerception())
				_rigidybody.linearVelocityX = 0f;
			if (_stopWorking)
				return;
			if (_statistics.LookPerception && !_detected)
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(transform.position, transform.right * _movementSide, _statistics.LookDistance, _statistics.Physics.TargetLayer))
					if (ray.collider.TryGetComponent<IDestructible>(out _))
					{
						_detected = true;
						break;
					}
			float originX = (_collider.bounds.extents.x + _statistics.Physics.GroundChecker / 2f) * (transform.right * _movementSide).x;
			Vector2 origin = new(transform.position.x + originX, transform.position.y);
			Vector2 size = new(_statistics.Physics.GroundChecker, _collider.bounds.size.y - _statistics.Physics.GroundChecker);
			RaycastHit2D blockCast = Physics2D.BoxCast(origin, size, 0f, transform.right * _movementSide, _statistics.Physics.GroundChecker, _statistics.Physics.GroundLayer);
			if (_statistics.RunFromTarget && _timeRun <= 0f && _detected)
			{
				_timeRun = _statistics.RunOfTime;
				if (_runTowards)
					_runTowards = false;
				else
					_movementSide *= -1;
			}
			float xAxis = transform.position.x + _collider.bounds.extents.x * (transform.right * _movementSide).x;
			float yAxis = transform.position.y + _collider.bounds.extents.y * -transform.up.y;
			bool valid = !Physics2D.Raycast(new Vector2(xAxis, yAxis), -transform.up, _statistics.Physics.GroundChecker, _statistics.Physics.GroundLayer);
			if (SurfacePerception() && !_statistics.TurnOffEdge && valid || blockCast && blockCast.collider.CanContact(_collider))
				_movementSide *= -1;
			if (_statistics.DetectionStop && _detected && !_isDashing)
			{
				_stopWorking = true;
				_sender.SetToggle(false);
				_sender.Send(PathConnection.Enemy);
				return;
			}
			transform.localScale = new Vector3()
			{
				x = Mathf.Abs(transform.localScale.x) * _movementSide,
				y = transform.localScale.y,
				z = transform.localScale.z
			};
			_rigidybody.linearVelocityX = (transform.right * _movementSide).x * (_detected ? _statistics.DashSpeed : _statistics.MovementSpeed);
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			if ((EnemyProvider[])additionalData != null)
				foreach (EnemyProvider enemy in (EnemyProvider[])additionalData)
					if (enemy != this)
						return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue && !data.ToggleValue.Value)
				_rigidybody.linearVelocityX = 0f;
			else if (data.StateForm == StateForm.Action && _statistics.ReactToDamage)
			{
				Vector2 targetPosition = GuwbaCentralizer.Position;
				if (_statistics.UseOtherTarget)
					targetPosition = _statistics.OtherTarget;
				_movementSide = (short)(targetPosition.x < transform.position.x ? -1f : 1f);
				if (_statistics.DetectionStop)
				{
					_stopWorking = true;
					_sender.SetToggle(_statistics.JumpDash);
					_sender.Send(PathConnection.Enemy);
				}
			}
		}
	};
};
