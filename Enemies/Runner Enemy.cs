using UnityEngine;
using System.Collections;
using GwambaPrimeAdventure.Character;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class RunnerEnemy : MovingEnemy, ILoader, IConnector, IDestructible
	{
		private RaycastHit2D _blockCast;
		private bool _stopRunning = false;
		private bool _edgeCast = false;
		private bool _invencibility = false;
		private bool _canRetreat = true;
		private bool _retreat = false;
		private bool _runTowards = false;
		private ushort _runnedTimes = 0;
		private float _timeRun = 0F;
		private float _dashedTime = 0F;
		private float _dashTime = 0F;
		private float _retreatTime = 0F;
		private float _retreatLocation = 0F;
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
		public IEnumerator Load()
		{
			(_timeRun, _dashTime) = (_statistics.RunOfTime, _statistics.TimeToDash);
			yield return null;
		}
		private void InvencibleDash()
		{
			if (_statistics.InvencibleDash)
			{
				_sender.SetFormat(MessageFormat.Event);
				_sender.SetToggle(_isDashing);
				_sender.Send(MessagePath.Enemy);
			}
		}
		private void Update()
		{
			if (IsStunned)
				return;
			if (_statistics.DetectionStop && _stopRunning)
				if ((_stoppedTime -= Time.deltaTime) <= 0F)
				{
					(_retreatTime, _dashedTime, _isDashing) = (_statistics.TimeToRetreat, _statistics.TimeDashing, !(_stopWorking = _stopRunning = false));
					_sender.SetFormat(MessageFormat.State);
					_sender.SetToggle(_statistics.JumpDash);
					_sender.Send(MessagePath.Enemy);
					InvencibleDash();
				}
			if (_stopWorking)
				return;
			if (_statistics.TimedDash && !_isDashing)
				if ((_dashTime -= Time.deltaTime) <= 0F)
				{
					(_dashedTime, _isDashing) = (_statistics.TimeDashing, true);
					InvencibleDash();
				}
			if (_statistics.RunFromTarget)
			{
				if (_timeRun > 0F && !_isDashing)
				{
					_isDashing = true;
					InvencibleDash();
				}
				if ((_timeRun -= Time.deltaTime) <= 0F && _isDashing)
				{
					if (_statistics.RunTowardsAfter && _runnedTimes >= _statistics.TimesToRun)
						(_runnedTimes, _runTowards) = (0, true);
					else if (_statistics.RunTowardsAfter)
						_runnedTimes++;
					_isDashing = false;
					InvencibleDash();
				}
			}
			if (!_retreat && _retreatTime > 0F)
				if ((_retreatTime -= Time.deltaTime) <= 0F)
					_canRetreat = true;
			if (_isDashing)
				if ((_dashedTime -= Time.deltaTime) <= 0F)
				{
					_dashTime = _statistics.TimeToDash;
					_sender.SetFormat(MessageFormat.State);
					_sender.SetToggle(!(_detected = _isDashing = false));
					_sender.Send(MessagePath.Enemy);
					InvencibleDash();
				}
		}
		private new void FixedUpdate()
		{
			if (IsStunned)
				return;
			if (_statistics.DetectionStop && _detected && !_isDashing && OnGround && !_retreat)
				Rigidbody.linearVelocityX = 0F;
			if (_stopWorking)
				return;
			if (_statistics.LookPerception && !_detected)
			{
				_originCast = new Vector2(transform.position.x + _collider.offset.x + _collider.bounds.extents.x * _movementSide, transform.position.y + _collider.offset.y);
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(_originCast, transform.right * _movementSide, _statistics.LookDistance, WorldBuild.CHARACTER_MASK))
					if (ray.collider.TryGetComponent<IDestructible>(out _))
					{
						_detected = true;
						break;
					}
			}
			_originCast = (Vector2)transform.position + _collider.offset;
			_originCast.x += (_collider.bounds.extents.x + WorldBuild.SNAP_LENGTH / 2f) * ((_retreat ? -1F : 1F) * _movementSide * transform.right).x;
			_sizeCast = new Vector2(WorldBuild.SNAP_LENGTH, _collider.bounds.size.y - WorldBuild.SNAP_LENGTH);
			_blockCast = Physics2D.BoxCast(_originCast, _sizeCast, 0F, transform.right * _movementSide, WorldBuild.SNAP_LENGTH, WorldBuild.SCENE_MASK);
			if (_statistics.RunFromTarget && _timeRun <= 0F && _detected)
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
					(_stoppedTime, _stopWorking, Rigidbody.linearVelocityX) = (_statistics.StopTime, _stopRunning = true, 0F);
				else if (_statistics.EventRetreat)
				{
					_retreatTime = _statistics.TimeToRetreat;
					_sender.SetFormat(MessageFormat.State);
					_sender.SetToggle(true);
					_sender.Send(MessagePath.Enemy);
					_sender.SetFormat(MessageFormat.Event);
					_sender.SetNumber(_statistics.EventIndex);
					_sender.Send(MessagePath.Enemy);
				}
				else
				{
					(_retreatTime, _dashedTime, _isDashing) = (_statistics.TimeToRetreat, _statistics.TimeDashing, !(_stopWorking = _stopRunning = false));
					_sender.SetFormat(MessageFormat.State);
					_sender.SetToggle(_statistics.JumpDash);
					_sender.Send(MessagePath.Enemy);
					InvencibleDash();
				}
			}
			_originCast = (Vector2)transform.position + _collider.offset;
			_originCast += new Vector2(_collider.bounds.extents.x * ((_retreat ? -1F : 1F) * _movementSide * transform.right).x, _collider.bounds.extents.y * -transform.up.y);
			_edgeCast = !Physics2D.Raycast(_originCast, -transform.up, WorldBuild.SNAP_LENGTH, WorldBuild.SCENE_MASK);
			if (OnGround && !_statistics.TurnOffEdge && _edgeCast || _blockCast && _blockCast.collider.CanContact(_collider))
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
				_sender.SetFormat(MessageFormat.State);
				_sender.SetToggle(false);
				_sender.Send(MessagePath.Enemy);
				return;
			}
			else if (_detected && !_isDashing)
			{
				(_dashedTime, _isDashing) = (_statistics.TimeDashing, !(_stopWorking = _stopRunning = false));
				_sender.SetFormat(MessageFormat.State);
				_sender.SetToggle(_statistics.JumpDash);
				_sender.Send(MessagePath.Enemy);
				InvencibleDash();
			}
			transform.TurnScaleX(_movementSide);
			Rigidbody.linearVelocityX = (transform.right * _movementSide).x * (_isDashing ? _statistics.DashSpeed : _statistics.MovementSpeed);
			base.FixedUpdate();
		}
		public new bool Hurt(ushort damage)
		{
			if (_invencibility)
				return false;
			if (_statistics.ReactToDamage && _canRetreat)
			{
				(_stoppedTime, _stopWorking, _retreatLocation) = (0F, _stopRunning = _canRetreat = !(_invencibility = _retreat = true), transform.position.x);
				transform.TurnScaleX(_movementSide = (short)(GwambaStateMarker.Localization.x < transform.position.x ? -1F : 1F));
				_sender.SetFormat(MessageFormat.State);
				_sender.SetToggle(false);
				_sender.Send(MessagePath.Enemy);
				return false;
			}
			return base.Hurt(damage);
		}
		public new void Receive(MessageData message)
		{
			if (message.AdditionalData != null && message.AdditionalData is EnemyProvider[] && (message.AdditionalData as EnemyProvider[]).Length > 0)
				foreach (EnemyProvider enemy in message.AdditionalData as EnemyProvider[])
					if (enemy && enemy == this)
					{
						base.Receive(message);
						if (message.Format == MessageFormat.State && message.ToggleValue.HasValue && !message.ToggleValue.Value)
							Rigidbody.linearVelocityX = 0F;
						if (message.Format == MessageFormat.Event && _statistics.ReactToDamage && _canRetreat)
						{
							(_stoppedTime, _stopWorking, _retreatLocation) = (0F, _stopRunning = _canRetreat = !(_invencibility = _retreat = true), transform.position.x);
							transform.TurnScaleX(_movementSide = (short)(GwambaStateMarker.Localization.x < transform.position.x ? -1 : 1));
							_sender.SetFormat(MessageFormat.State);
							_sender.SetToggle(false);
							_sender.Send(MessagePath.Enemy);
						}
						return;
					}
		}
	};
};
