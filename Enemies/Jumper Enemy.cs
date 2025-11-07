using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class JumperEnemy : MovingEnemy, IConnector
	{
		private InputController _inputController;
		Vector2 _direction;
		private bool _isJumping = false;
		private bool _stopJump = false;
		private float _jumpTime = 0f;
		[Header("Jumper Enemy")]
		[SerializeField, Tooltip("The jumper statitics of this enemy.")] private JumperStatistics _statistics;
		private void BasicJump(Vector2 target)
		{
			StartCoroutine(Jump());
			IEnumerator Jump()
			{
				_detected = true;
				if (_statistics.DetectionStop)
				{
					_sender.SetToggle(false);
					_sender.Send(PathConnection.Enemy);
					yield return new WaitTime(this, _statistics.StopTime);
					yield return new WaitUntil(() => isActiveAndEnabled && !IsStunned);
				}
				_isJumping = true;
				_rigidybody.AddForceY(_rigidybody.mass * _statistics.JumpStrenght, ForceMode2D.Impulse);
				if (_statistics.UnFollow)
					StartCoroutine(FollowSide());
				IEnumerator FollowSide()
				{
					yield return new WaitUntil(() => !GroundCheck() && isActiveAndEnabled && !IsStunned && !_stopJump);
					_movementSide = (short)(target.x >= transform.position.x ? 1f : -1f);
					transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * _movementSide, transform.localScale.y, transform.localScale.z);
					while (!GroundCheck())
					{
						if (Mathf.Abs(target.x - transform.position.x) > _statistics.DistanceToTarget)
							_rigidybody.linearVelocityX = _movementSide * _statistics.MovementSpeed;
						yield return new WaitForFixedUpdate();
						yield return new WaitUntil(() => isActiveAndEnabled && !IsStunned && !_stopJump);
					}
					_rigidybody.linearVelocityX = 0f;
				}
			}
		}
		private void FollowJump(Vector2 otherTarget, bool useTarget)
		{
			StartCoroutine(FollowTarget());
			IEnumerator FollowTarget()
			{
				yield return new WaitUntil(() => !GroundCheck() && isActiveAndEnabled && !IsStunned && !_stopJump);
				_rigidybody.linearVelocityX = 0f;
				float targetPosition = GuwbaAstralMarker.Localization.x;
				if (_statistics.RandomFollow)
					targetPosition = UnityEngine.Random.Range(-1, 1) >= 0f ? GuwbaAstralMarker.Localization.x : otherTarget.x;
				else if (useTarget)
					targetPosition = otherTarget.x;
				_movementSide = (short)(targetPosition > transform.position.x ? 1f : -1f);
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * _movementSide, transform.localScale.y, transform.localScale.z);
				float xStart = transform.position.x;
				float distance = Mathf.Abs(targetPosition - xStart);
				float remainingDistance = distance;
				while (!GroundCheck())
				{
					transform.position = new Vector2(Mathf.Lerp(xStart, targetPosition, 1f - remainingDistance / distance), transform.position.y);
					if (Mathf.Abs(targetPosition - transform.position.x) > _statistics.DistanceToTarget)
						remainingDistance -= _statistics.MovementSpeed * Time.deltaTime;
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => isActiveAndEnabled && !IsStunned && !_stopJump);
				}
				_rigidybody.linearVelocityX = 0f;
			}
		}
		private new void Awake()
		{
			base.Awake();
			_sender.SetStateForm(StateForm.State);
			if (_statistics.UseInput)
			{
				_inputController = new InputController();
				_inputController.Commands.Jump.started += Jump;
				_inputController.Commands.Jump.Enable();
			}
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
							_rigidybody.AddForceY(_statistics.JumpPointStructures[index].JumpStats.Strength * _rigidybody.mass, ForceMode2D.Impulse);
							if (_statistics.JumpPointStructures[index].JumpStats.Follow)
								FollowJump(_statistics.JumpPointStructures[index].JumpStats.OtherTarget, _statistics.JumpPointStructures[index].JumpStats.UseTarget);
							_statistics.JumpPointStructures[index].RemovalJumpCount = (short)_statistics.JumpPointStructures[index].JumpCount;
						}
					}
				});
				_statistics.JumpPointStructures[i].RemovalJumpCount = (short)_statistics.JumpPointStructures[i].JumpCount;
			}
			if (_statistics.SequentialTimmedJumps)
			{
				StartCoroutine(SequentialJumps());
				IEnumerator SequentialJumps()
				{
					for (ushort index = 0; index < _statistics.TimedJumps.Length; index++)
						yield return TimedJump(_statistics.TimedJumps[index]);
					if (_statistics.RepeatTimmedJumps)
						StartCoroutine(SequentialJumps());
				}
			}
			else
				foreach (JumpStats jumpStats in _statistics.TimedJumps)
					StartCoroutine(TimedJump(jumpStats));
			IEnumerator TimedJump(JumpStats stats)
			{
				yield return new WaitUntil(() => GroundCheck() && !_detected && isActiveAndEnabled && !IsStunned && !_stopJump);
				yield return new WaitTime(this, stats.TimeToExecute);
				if (stats.StopMove)
				{
					_sender.SetToggle(false);
					_sender.Send(PathConnection.Enemy);
					_rigidybody.linearVelocityX = 0f;
				}
				_isJumping = true;
				_rigidybody.AddForceY(stats.Strength * _rigidybody.mass, ForceMode2D.Impulse);
				if (stats.Follow)
					FollowJump(stats.OtherTarget, stats.UseTarget);
				if (!_statistics.SequentialTimmedJumps)
					StartCoroutine(TimedJump(stats));
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
		private Action<InputAction.CallbackContext> Jump => jump =>
		{
			if (isActiveAndEnabled && !IsStunned && _jumpTime <= 0f)
			{
				_jumpTime = _statistics.TimeToJump;
				BasicJump(GuwbaAstralMarker.Localization);
			}
		};
		private void Update()
		{
			if (!IsStunned && _jumpTime > 0f)
				_jumpTime -= Time.deltaTime;
		}
		private void FixedUpdate()
		{
			if (IsStunned)
				return;
			if (_isJumping && GroundCheck())
			{
				_isJumping = false;
				_detected = false;
				_sender.SetToggle(true);
				_sender.Send(PathConnection.Enemy);
			}
			if (_stopWorking)
				return;
			if (!_detected && _statistics.LookPerception && GroundCheck())
				if (_statistics.CircularDetection)
				{
					foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, _statistics.LookDistance, _statistics.Physics.TargetLayer))
						if (collider.TryGetComponent<IDestructible>(out _))
						{
							BasicJump(collider.transform.position);
							return;
						}
				}
				else
				{
					_direction = Quaternion.AngleAxis(_statistics.DetectionAngle, Vector3.forward) * transform.right * (transform.localScale.x < 0f ? -1f : 1f);
					foreach (RaycastHit2D ray in Physics2D.RaycastAll(transform.position, _direction, _statistics.LookDistance, _statistics.Physics.TargetLayer))
						if (ray.collider.TryGetComponent<IDestructible>(out _))
						{
							BasicJump(ray.collider.transform.position);
							return;
						}
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
				_rigidybody.AddForceY(_statistics.StrenghtReact * _rigidybody.mass, ForceMode2D.Impulse);
				if (_statistics.FollowReact)
					FollowJump(_statistics.OtherTarget, _statistics.UseTarget);
			}
		}
	};
};
