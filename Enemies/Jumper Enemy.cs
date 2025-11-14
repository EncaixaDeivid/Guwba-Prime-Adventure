using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using GwambaPrimeAdventure.Character;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class JumperEnemy : MovingEnemy, IConnector
	{
		private InputController _inputController;
		private Vector2 _targetPosition;
		private Vector2 _direction;
		private bool _isJumping = false;
		private bool _stopJump = false;
		private bool _follow = false;
		private ushort _sequentialJumpIndex = 0;
		private float[] _timedJumpTime;
		private float _jumpTime = 0f;
		private float _stopTime = 0f;
		[Header("Jumper Enemy")]
		[SerializeField, Tooltip("The jumper statitics of this enemy.")] private JumperStatistics _statistics;
		private new void Awake()
		{
			base.Awake();
			_sender.SetStateForm(StateForm.State);
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
			for (ushort i = 0; i < _statistics.JumpPointStructures.Length; i++)
			{
				Instantiate(_statistics.JumpPointStructures[i].JumpPointObject, _statistics.JumpPointStructures[i].Point, Quaternion.identity).GetTouch(i, index =>
				{
					StartCoroutine(WaitToHitSurface());
					IEnumerator WaitToHitSurface()
					{
						yield return new WaitUntil(() => GroundCheck() && !_detected && isActiveAndEnabled && !IsStunned);
						if (_stopJump)
							yield break;
						if (_statistics.JumpPointStructures[index].RemovalJumpCount-- <= 0f)
						{
							if (_statistics.JumpPointStructures[index].JumpStats.StopMove)
							{
								_sender.SetToggle(false);
								_sender.Send(PathConnection.Enemy);
							}
							_isJumping = true;
							Rigidbody.AddForceY(_statistics.JumpPointStructures[index].JumpStats.Strength * Rigidbody.mass, ForceMode2D.Impulse);
							if (_statistics.JumpPointStructures[index].JumpStats.Follow)
								FollowJump(_statistics.JumpPointStructures[index].JumpStats.OtherTarget, _statistics.JumpPointStructures[index].JumpStats.UseTarget);
							_statistics.JumpPointStructures[index].RemovalJumpCount = (short)_statistics.JumpPointStructures[index].JumpCount;
						}
					}
				});
				_statistics.JumpPointStructures[i].RemovalJumpCount = (short)_statistics.JumpPointStructures[i].JumpCount;
			}
		}
		private void FollowJump(Vector2 otherTarget, bool useTarget)
		{
			StartCoroutine(FollowTarget());
			IEnumerator FollowTarget()
			{
				yield return new WaitUntil(() => !GroundCheck() && isActiveAndEnabled && !IsStunned && !_stopJump);
				Rigidbody.linearVelocityX = 0f;
				float targetPosition = GwambaStateMarker.Localization.x;
				if (_statistics.RandomFollow)
					targetPosition = UnityEngine.Random.Range(-1, 1) >= 0f ? GwambaStateMarker.Localization.x : otherTarget.x;
				else if (useTarget)
					targetPosition = otherTarget.x;
				transform.TurnScaleX(_movementSide = (short)(targetPosition < transform.position.x ? -1f : 1f));
				float xStart = transform.position.x;
				float distance = Mathf.Abs(targetPosition - xStart);
				float remainingDistance = distance;
				while (!GroundCheck() && !_stopJump)
				{
					transform.position = new Vector2(Mathf.Lerp(xStart, targetPosition, 1f - remainingDistance / distance), transform.position.y);
					if (Mathf.Abs(targetPosition - transform.position.x) > _statistics.DistanceToTarget)
						remainingDistance -= _statistics.MovementSpeed * Time.fixedDeltaTime;
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => isActiveAndEnabled && !IsStunned);
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
			if (_statistics.TimedJumps[jumpIndex].StopMove)
			{
				_sender.SetToggle(false);
				_sender.Send(PathConnection.Enemy);
				Rigidbody.linearVelocityX = 0f;
			}
			_isJumping = true;
			Rigidbody.AddForceY(_statistics.TimedJumps[jumpIndex].Strength * Rigidbody.mass, ForceMode2D.Impulse);
			if (_statistics.TimedJumps[jumpIndex].Follow)
				FollowJump(_statistics.TimedJumps[jumpIndex].OtherTarget, _statistics.TimedJumps[jumpIndex].UseTarget);
			if (_statistics.SequentialTimmedJumps)
				_sequentialJumpIndex++;
			else
				_timedJumpTime[jumpIndex] = _statistics.TimedJumps[jumpIndex].TimeToExecute;
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
						if (_timedJumpTime[i] > 0f)
							if ((_timedJumpTime[i] -= Time.deltaTime) <= 0f)
								TimedJump(i);
		}
		private void FixedUpdate()
		{
			if (IsStunned)
				return;
			if (_isJumping && GroundCheck())
			{
				if (_follow)
					Rigidbody.linearVelocityX = 0f;
				_sender.SetToggle(!(_isJumping = _detected = _follow = false));
				_sender.Send(PathConnection.Enemy);
			}
			if (_stopWorking)
				return;
			if (GroundCheck())
			{
				if (!_detected && _statistics.LookPerception)
					if (_statistics.CircularDetection)
					{
						foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, _statistics.LookDistance, _statistics.Physics.TargetLayer))
							if (collider.TryGetComponent<IDestructible>(out _))
							{
								_targetPosition = collider.transform.position;
								BasicJump();
								return;
							}
					}
					else
					{
						_direction = Quaternion.AngleAxis(_statistics.DetectionAngle, Vector3.forward) * transform.right * (transform.localScale.x < 0f ? -1f : 1f);
						foreach (RaycastHit2D ray in Physics2D.RaycastAll(transform.position, _direction, _statistics.LookDistance, _statistics.Physics.TargetLayer))
							if (ray.collider.TryGetComponent<IDestructible>(out _))
							{
								_targetPosition = ray.collider.transform.position;
								BasicJump();
								return;
							}
					}
			}
			else
			{
				if (Mathf.Abs(_targetPosition.x - transform.position.x) > _statistics.DistanceToTarget)
					Rigidbody.linearVelocityX = _movementSide * _statistics.MovementSpeed;
			}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			if ((EnemyProvider[])additionalData != null)
				foreach (EnemyProvider enemy in (EnemyProvider[])additionalData)
					if (enemy != this)
						return;
			base.Receive(data, additionalData);
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				_stopJump = !data.ToggleValue.Value;
			else if (data.StateForm == StateForm.Action && _statistics.ReactToDamage)
			{
				if (_statistics.StopMoveReact)
				{
					_sender.SetToggle(false);
					_sender.Send(PathConnection.Enemy);
				}
				_isJumping = true;
				Rigidbody.AddForceY(_statistics.StrenghtReact * Rigidbody.mass, ForceMode2D.Impulse);
				if (_statistics.FollowReact)
					FollowJump(_statistics.OtherTarget, _statistics.UseTarget);
			}
		}
	};
};
