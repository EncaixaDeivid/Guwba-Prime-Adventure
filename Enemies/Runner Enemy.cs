using UnityEngine;
using GwambaPrimeAdventure.Connection;
using GwambaPrimeAdventure.Character;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class RunnerEnemy : MovingEnemy, IConnector, IDestructible
	{
		private RaycastHit2D _blockCast;
		private bool _stopRunning = false;
		private bool _edgeCast = false;
		private bool _invencibility = false;
		private bool _canRetreat = true;
		private bool _retreat = false;
		private bool _runTowards = false;
		private ushort _runnedTimes = 0;
		private float _timeRun = 0f;
		private float _dashedTime = 0f;
		private float _dashTime = 0f;
		private float _retreatTime = 0f;
		private float _retreatLocation = 0f;
		[Header("Runner Enemy")]
		[SerializeField, Tooltip("The runner statitics of this enemy.")] private RunnerStatistics _statistics;
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
		private void Start() => (_timeRun, _dashTime) = (_statistics.RunOfTime, _statistics.TimeToDash);
		private void Update()
		{
			if (IsStunned)
				return;
			if (_statistics.DetectionStop && _stopRunning)
				if ((_stoppedTime -= Time.deltaTime) <= 0f)
				{
					(_retreatTime, _dashedTime, _isDashing) = (_statistics.TimeToRetreat, _statistics.TimeDashing, !(_stopWorking = _stopRunning = false));
					_sender.SetToggle(_statistics.JumpDash);
					_sender.Send(PathConnection.Enemy);
				}
			if (_stopWorking)
				return;
			if (_statistics.TimedDash && !_isDashing)
				if ((_dashTime -= Time.deltaTime) <= 0f)
					(_dashedTime, _isDashing) = (_statistics.TimeDashing, true);
			if (_statistics.RunFromTarget)
			{
				if (_timeRun > 0f)
					_isDashing = true;
				if ((_timeRun -= Time.deltaTime) <= 0f && _isDashing)
				{
					if (_statistics.RunTowardsAfter && _runnedTimes >= _statistics.TimesToRun)
						(_runnedTimes, _runTowards) = (0, true);
					else if (_statistics.RunTowardsAfter)
						_runnedTimes++;
					_isDashing = false;
				}
			}
			if (!_retreat && _retreatTime > 0f)
				if ((_retreatTime -= Time.deltaTime) <= 0f)
					_canRetreat = true;
			if (_isDashing)
				if ((_dashedTime -= Time.deltaTime) <= 0f)
				{
					_dashTime = _statistics.TimeToDash;
					_sender.SetToggle(!(_detected = _isDashing = false));
					_sender.Send(PathConnection.Enemy);
				}
		}
		private void FixedUpdate()
		{
			if (IsStunned)
				return;
			if (_statistics.DetectionStop && _detected && !_isDashing && GroundCheck() && !_retreat)
				Rigidbody.linearVelocityX = 0f;
			if (_stopWorking)
				return;
			if (_statistics.LookPerception && !_detected)
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(transform.position, transform.right * _movementSide, _statistics.LookDistance, _statistics.Physics.TargetLayer))
					if (ray.collider.TryGetComponent<IDestructible>(out _))
					{
						_detected = true;
						break;
					}
			_originCast = transform.position;
			_originCast += new Vector2((_collider.bounds.extents.x + WorldBuild.SNAPLENGTH / 2f) * ((_retreat ? -1f : 1f) * _movementSide * transform.right).x, 0f);
			_sizeCast = new Vector2(WorldBuild.SNAPLENGTH, _collider.bounds.size.y - WorldBuild.SNAPLENGTH);
			_blockCast = Physics2D.BoxCast(_originCast, _sizeCast, 0f, transform.right * _movementSide, WorldBuild.SNAPLENGTH, _statistics.Physics.GroundLayer);
			if (_statistics.RunFromTarget && _timeRun <= 0f && _detected)
			{
				_timeRun = _statistics.RunOfTime;
				if (_runTowards)
					_runTowards = false;
				else
					_movementSide *= -1;
			}
			void RetreatUse()
			{
				_invencibility = _retreat = false;
				if (_statistics.DetectionStop)
					(_stoppedTime, _stopWorking, Rigidbody.linearVelocityX) = (_statistics.StopTime, _stopRunning = true, 0f);
				else if (_statistics.EventRetreat)
				{
					_retreatTime = _statistics.TimeToRetreat;
					_sender.SetToggle(true);
					_sender.Send(PathConnection.Enemy);
					_sender.SetStateForm(StateForm.Event);
					_sender.SetNumber(_statistics.EventIndex);
					_sender.Send(PathConnection.Enemy);
					_sender.SetStateForm(StateForm.State);
				}
				else
				{
					(_retreatTime, _dashedTime, _isDashing) = (_statistics.TimeToRetreat, _statistics.TimeDashing, !(_stopWorking = _stopRunning = false));
					_sender.SetToggle(_statistics.JumpDash);
					_sender.Send(PathConnection.Enemy);
				}
			}
			_originCast = (Vector2)transform.position + _collider.offset;
			_originCast += new Vector2(_collider.bounds.extents.x + ((_retreat ? -1f : 1f) * _movementSide * transform.right).x, _collider.bounds.extents.y * -transform.up.y);
			_edgeCast = !Physics2D.Raycast(_originCast, -transform.up, WorldBuild.SNAPLENGTH, _statistics.Physics.GroundLayer);
			if (GroundCheck() && !_statistics.TurnOffEdge && _edgeCast || _blockCast && _blockCast.collider.CanContact(_collider))
				if (_retreat)
					RetreatUse();
				else
					_movementSide *= -1;
			if (_retreat)
			{
				Rigidbody.linearVelocityX = (transform.right * _movementSide).x * -_statistics.RetreatSpeed;
				if (Mathf.Abs(transform.position.x - _retreatLocation) >= _statistics.RetreatDistance)
					RetreatUse();
				return;
			}
			if (_statistics.DetectionStop && _detected && !_isDashing)
			{
				(_stoppedTime, _stopWorking) = (_statistics.StopTime, _stopRunning = true);
				_sender.SetToggle(false);
				_sender.Send(PathConnection.Enemy);
				return;
			}
			else if (_detected && !_isDashing)
			{
				(_dashedTime, _isDashing) = (_statistics.TimeDashing, !(_stopWorking = _stopRunning = false));
				_sender.SetToggle(_statistics.JumpDash);
				_sender.Send(PathConnection.Enemy);
			}
			transform.TurnScaleX(_movementSide);
			Rigidbody.linearVelocityX = (transform.right * _movementSide).x * (_isDashing ? _statistics.DashSpeed : _statistics.MovementSpeed);
		}
		public new bool Hurt(ushort damage)
		{
			if (_invencibility)
				return false;
			if (_statistics.ReactToDamage && _canRetreat)
			{
				(_stoppedTime, _stopWorking, _retreatLocation) = (0f, _stopRunning = _canRetreat = !(_invencibility = _retreat = true), transform.position.x);
				transform.TurnScaleX(_movementSide = (short)(GwambaStateMarker.Localization.x < transform.position.x ? -1f : 1f));
				_sender.SetToggle(false);
				_sender.Send(PathConnection.Enemy);
				return false;
			}
			return base.Hurt(damage);
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			if (additionalData != null && additionalData is EnemyProvider[] && additionalData as EnemyProvider[] != null && (additionalData as EnemyProvider[]).Length > 0)
				foreach (EnemyProvider enemy in additionalData as EnemyProvider[])
					if (enemy && enemy == this)
					{
						base.Receive(data, additionalData);
						if (data.StateForm == StateForm.State && data.ToggleValue.HasValue && !data.ToggleValue.Value)
							Rigidbody.linearVelocityX = 0f;
						return;
					}
		}
	};
};
