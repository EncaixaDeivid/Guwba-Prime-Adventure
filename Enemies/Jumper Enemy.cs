using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using GwambaPrimeAdventure.Character;
using GwambaPrimeAdventure.Connection;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class JumperEnemy : MovingEnemy, IJumper, IConnector
	{
		private InputController _inputController;
		private Vector2 _targetPosition;
		private Vector2 _direction;
		private bool _isJumping = false;
		private bool _onJump = false;
		private bool _stopJump = false;
		private bool _follow = false;
		private ushort _sequentialJumpIndex = 0;
		private short[] _jumpCount;
		private float[] _timedJumpTime;
		private float _jumpTime = 0f;
		private float _stopTime = 0f;
		[Header("Jumper Enemy")]
		[SerializeField, Tooltip("The jumper statitics of this enemy.")] private JumperStatistics _statistics;
		private new void Awake()
		{
			base.Awake();
			_timedJumpTime = new float[_statistics.TimedJumps.Length];
			if (_statistics.UseInput)
			{
				_inputController = new InputController();
				_inputController.Commands.Jump.started += Jump;
				_inputController.Commands.Jump.Enable();
			}
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			StopAllCoroutines();
			if (_statistics.UseInput)
			{
				_inputController.Commands.Jump.started -= Jump;
				_inputController.Commands.Jump.Disable();
				_inputController.Dispose();
			}
			Sender.Exclude(this);
		}
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			for (ushort i = 0; i < _statistics.TimedJumps.Length; i++)
				_timedJumpTime[i] = _statistics.TimedJumps[i].TimeToExecute;
			_jumpCount = new short[_statistics.JumpPointStructures.Length];
			for (ushort i = 0; i < _statistics.JumpPointStructures.Length; i++)
			{
				Instantiate(_statistics.JumpPointStructures[i].JumpPointObject, _statistics.JumpPointStructures[i].Point, Quaternion.identity).GetTouch(this, i);
				_jumpCount[i] = (short)_statistics.JumpPointStructures[i].JumpCount;
			}
		}
		private void FollowJump(Vector2 otherTarget, bool useTarget, bool turnFollow)
		{
			StartCoroutine(FollowTarget());
			IEnumerator FollowTarget()
			{
				yield return new WaitUntil(() => !GroundCheck() && isActiveAndEnabled && !IsStunned && !_stopJump);
				bool validVelocity;
				while (!GroundCheck() && !_stopJump)
				{
					if (_statistics.RandomFollow)
						_targetPosition.x = UnityEngine.Random.Range(-1, 1) >= 0f ? GwambaStateMarker.Localization.x : otherTarget.x;
					else if (useTarget)
						_targetPosition.x = otherTarget.x;
					else
						_targetPosition.x = GwambaStateMarker.Localization.x;
					_movementSide = (short)(_targetPosition.x < transform.position.x ? -1f : 1f);
					if (turnFollow)
						transform.TurnScaleX(_movementSide);
					yield return new WaitUntil(() =>
					{
						validVelocity = isActiveAndEnabled && !IsStunned && Mathf.Abs(_targetPosition.x - transform.position.x) > _statistics.DistanceToTarget;
						return Mathf.Abs(Rigidbody.linearVelocityX = validVelocity ? _statistics.MovementSpeed * _movementSide : 0f) > 0f;
					});
					yield return new WaitForFixedUpdate();
				}
				Rigidbody.linearVelocityX = 0f;
			}
		}
		private Action<InputAction.CallbackContext> Jump => jump =>
		{
			if (isActiveAndEnabled && !IsStunned && _jumpTime <= 0f)
			{
				_jumpTime = _statistics.TimeToJump;
				_targetPosition = GwambaStateMarker.Localization;
				BasicJump();
			}
		};
		private void BasicJump()
		{
			_detected = true;
			if (_statistics.DetectionStop)
			{
				_sender.SetStateForm(StateForm.State);
				_sender.SetToggle(false);
				_sender.Send(PathConnection.Enemy);
				_stopTime = _statistics.StopTime;
				return;
			}
			_isJumping = true;
			Rigidbody.AddForceY(Rigidbody.mass * _statistics.JumpStrenght, ForceMode2D.Impulse);
			if (_follow = !_statistics.UnFollow)
				transform.TurnScaleX(_movementSide = (short)(_targetPosition.x < transform.position.x ? -1f : 1f));
		}
		private void TimedJump(ushort jumpIndex)
		{
			if (_timedJumpTime[jumpIndex] > 0f)
				if ((_timedJumpTime[jumpIndex] -= Time.deltaTime) <= 0f)
				{
					if (_statistics.TimedJumps[jumpIndex].StopMove)
					{
						_sender.SetStateForm(StateForm.State);
						_sender.SetToggle(false);
						_sender.Send(PathConnection.Enemy);
						Rigidbody.linearVelocityX = 0f;
					}
					_isJumping = true;
					Rigidbody.AddForceY(_statistics.TimedJumps[jumpIndex].Strength * Rigidbody.mass, ForceMode2D.Impulse);
					if (_statistics.TimedJumps[jumpIndex].Follow)
						FollowJump(_statistics.TimedJumps[jumpIndex].OtherTarget, _statistics.TimedJumps[jumpIndex].UseTarget, _statistics.TimedJumps[jumpIndex].TurnFollow);
					if (_statistics.SequentialTimmedJumps)
						_sequentialJumpIndex++;
					else
						_timedJumpTime[jumpIndex] = _statistics.TimedJumps[jumpIndex].TimeToExecute;
				}
		}
		private void Update()
		{
			if (IsStunned || _stopWorking)
				return;
			if (_stopTime > 0f)
				if ((_stopTime -= Time.deltaTime) <= 0f)
				{
					_isJumping = true;
					Rigidbody.AddForceY(Rigidbody.mass * _statistics.JumpStrenght, ForceMode2D.Impulse);
					if (_follow = !_statistics.UnFollow)
						transform.TurnScaleX(_movementSide = (short)(_targetPosition.x < transform.position.x ? -1f : 1f));
				}
			if (!GroundCheck() || _detected || _isJumping)
				return;
			if (_jumpTime > 0f)
				_jumpTime -= Time.deltaTime;
			if (!_isJumping)
				if (_statistics.SequentialTimmedJumps)
				{
					if (_sequentialJumpIndex >= _statistics.TimedJumps.Length - 1)
						if (_statistics.RepeatTimmedJumps)
							_sequentialJumpIndex = 0;
						else
							return;
					TimedJump(_sequentialJumpIndex);
				}
				else
					for (ushort i = 0; i < _timedJumpTime.Length; i++)
						TimedJump(i);
		}
		private void FixedUpdate()
		{
			if (IsStunned)
				return;
			if (!_onJump && _isJumping && !GroundCheck())
				_onJump = true;
			if (_onJump && GroundCheck())
			{
				if (_follow)
					Rigidbody.linearVelocityX = 0f;
				_sender.SetStateForm(StateForm.State);
				_sender.SetToggle(!(_onJump = _isJumping = _detected = _follow = false));
				_sender.Send(PathConnection.Enemy);
			}
			if (_stopWorking)
				return;
			if (GroundCheck())
			{
				if (!_detected && _statistics.LookPerception)
					if (_statistics.CircularDetection)
					{
						foreach (Collider2D collider in Physics2D.OverlapCircleAll((Vector2)transform.position + _collider.offset, _statistics.LookDistance, _statistics.Physics.TargetLayer))
							if (collider.TryGetComponent<IDestructible>(out _))
							{
								_targetPosition = collider.transform.position;
								BasicJump();
								return;
							}
					}
					else
					{
						_originCast = new Vector2(transform.position.x + _collider.offset.x + _collider.bounds.extents.x * _movementSide, transform.position.y + _collider.offset.y);
						_direction = Quaternion.AngleAxis(_statistics.DetectionAngle, Vector3.forward) * transform.right * (transform.localScale.x < 0f ? -1f : 1f);
						foreach (RaycastHit2D ray in Physics2D.RaycastAll(_originCast, _direction, _statistics.LookDistance, _statistics.Physics.TargetLayer))
							if (ray.collider.TryGetComponent<IDestructible>(out _))
							{
								_targetPosition = ray.collider.transform.position;
								BasicJump();
								return;
							}
					}
			}
			else if (_follow)
			{
				if (Mathf.Abs(_targetPosition.x - transform.position.x) > _statistics.DistanceToTarget)
					Rigidbody.linearVelocityX = _movementSide * _statistics.MovementSpeed;
			}
		}
		public void OnJump(ushort jumpIndex)
		{
			StartCoroutine(WaitToHitSurface());
			IEnumerator WaitToHitSurface()
			{
				yield return new WaitUntil(() => GroundCheck() && !_detected && isActiveAndEnabled && !IsStunned);
				if (_stopJump || _jumpTime > 0f)
					yield break;
				if (_jumpCount[jumpIndex]-- <= 0f)
				{
					if (_statistics.JumpPointStructures[jumpIndex].JumpStats.StopMove)
					{
						_sender.SetStateForm(StateForm.State);
						_sender.SetToggle(false);
						_sender.Send(PathConnection.Enemy);
					}
					_isJumping = true;
					Rigidbody.AddForceY(_statistics.JumpPointStructures[jumpIndex].JumpStats.Strength * Rigidbody.mass, ForceMode2D.Impulse);
					if (_statistics.JumpPointStructures[jumpIndex].JumpStats.Follow)
					{
						bool turnFollow = _statistics.JumpPointStructures[jumpIndex].JumpStats.TurnFollow;
						FollowJump(_statistics.JumpPointStructures[jumpIndex].JumpStats.OtherTarget, _statistics.JumpPointStructures[jumpIndex].JumpStats.UseTarget, turnFollow);
					}
					_jumpCount[jumpIndex] = (short)_statistics.JumpPointStructures[jumpIndex].JumpCount;
					_jumpTime = _statistics.TimeToJump;
				}
			}
		}
		public new void Receive(DataConnection data)
		{
			if (data.AdditionalData != null && data.AdditionalData is EnemyProvider[] && data.AdditionalData as EnemyProvider[] != null && (data.AdditionalData as EnemyProvider[]).Length > 0)
				foreach (EnemyProvider enemy in data.AdditionalData as EnemyProvider[])
					if (enemy && enemy == this)
					{
						base.Receive(data);
						if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
							_stopJump = !data.ToggleValue.Value;
						else if (data.StateForm == StateForm.Event && _statistics.ReactToDamage)
						{
							if (_statistics.StopMoveReact)
							{
								_sender.SetStateForm(StateForm.State);
								_sender.SetToggle(false);
								_sender.Send(PathConnection.Enemy);
							}
							_isJumping = true;
							Rigidbody.AddForceY(_statistics.StrenghtReact * Rigidbody.mass, ForceMode2D.Impulse);
							if (_statistics.FollowReact)
								FollowJump(_statistics.OtherTarget, _statistics.UseTarget, _statistics.TurnFollowReact);
						}
						return;
					}
		}
	};
};
