using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Threading.Tasks;
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
		private bool _contunuosFollow = false;
		private bool _useTarget = false;
		private bool _turnFollow = false;
		private ushort _sequentialJumpIndex = 0;
		private short[] _jumpCount;
		private float[] _timedJumpTime;
		private float _jumpTime = 0f;
		private float _stopTime = 0f;
		private float _otherTarget = 0f;
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
		private new IEnumerator Start()
		{
			yield return base.Start();
			for (ushort i = 0; i < _statistics.TimedJumps.Length; i++)
				_timedJumpTime[i] = _statistics.TimedJumps[i].TimeToExecute;
			_jumpCount = new short[_statistics.JumpPointStructures.Length];
			for (ushort i = 0; i < _statistics.JumpPointStructures.Length; i++)
			{
				Instantiate(_statistics.JumpPointStructures[i].JumpPointObject, _statistics.JumpPointStructures[i].Point, Quaternion.identity).GetTouch(this, i);
				_jumpCount[i] = (short)_statistics.JumpPointStructures[i].JumpCount;
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
		private async void TimedJump(ushort jumpIndex)
		{
			if (_timedJumpTime[jumpIndex] > 0f)
				if ((_timedJumpTime[jumpIndex] -= Time.deltaTime) <= 0f)
				{
					Rigidbody.AddForceY(_statistics.TimedJumps[jumpIndex].Strength * Rigidbody.mass, ForceMode2D.Impulse);
					while (OnGround)
						await Task.Yield();
					_isJumping = true;
					_contunuosFollow = _follow = _statistics.TimedJumps[jumpIndex].Follow;
					_turnFollow = _statistics.TimedJumps[jumpIndex].TurnFollow;
					_useTarget = _statistics.TimedJumps[jumpIndex].UseTarget;
					_otherTarget = _statistics.TimedJumps[jumpIndex].OtherTarget;
					if (_statistics.SequentialTimmedJumps)
						_sequentialJumpIndex++;
					else
						_timedJumpTime[jumpIndex] = _statistics.TimedJumps[jumpIndex].TimeToExecute;
					if (_statistics.TimedJumps[jumpIndex].StopMove)
					{
						_sender.SetStateForm(StateForm.State);
						_sender.SetToggle(false);
						_sender.Send(PathConnection.Enemy);
						Rigidbody.linearVelocityX = 0f;
					}
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
			if (!OnGround || _detected || _isJumping)
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
		private new void FixedUpdate()
		{
			if (IsStunned)
				return;
			if (_onJump && OnGround)
			{
				if (_follow)
					Rigidbody.linearVelocityX = 0f;
				_sender.SetStateForm(StateForm.State);
				_sender.SetToggle(!(_onJump = _isJumping = _detected = _contunuosFollow = _follow = false));
				_sender.Send(PathConnection.Enemy);
			}
			else if (!_onJump && _isJumping && !OnGround)
				_onJump = true;
			if (_stopWorking)
				return;
			if (OnGround)
			{
				if (!_detected && _statistics.LookPerception)
					if (_statistics.CircularDetection)
					{
						foreach (Collider2D collider in Physics2D.OverlapCircleAll((Vector2)transform.position + _collider.offset, _statistics.LookDistance, WorldBuild.CharacterMask))
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
						foreach (RaycastHit2D ray in Physics2D.RaycastAll(_originCast, _direction, _statistics.LookDistance, WorldBuild.CharacterMask))
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
				if (_contunuosFollow)
				{
					if (_statistics.RandomFollow)
						_targetPosition.x = UnityEngine.Random.Range(-1, 1) >= 0f ? GwambaStateMarker.Localization.x : _otherTarget;
					else if (_useTarget)
						_targetPosition.x = _otherTarget;
					else
						_targetPosition.x = GwambaStateMarker.Localization.x;
					_movementSide = (short)(_targetPosition.x < transform.position.x ? -1f : 1f);
					if (_turnFollow)
						transform.TurnScaleX(_movementSide);
				}
				Rigidbody.linearVelocityX = Mathf.Abs(_targetPosition.x - transform.position.x) > _statistics.DistanceToTarget ? _movementSide * _statistics.MovementSpeed : 0f;
			}
			base.FixedUpdate();
		}
		public async void OnJump(ushort jumpIndex)
		{
			while (!OnGround || _detected || !isActiveAndEnabled || IsStunned)
				await Task.Yield();
			if (_stopJump || _jumpTime > 0f)
				return;
			if (_jumpCount[jumpIndex]-- <= 0f)
			{
				Rigidbody.AddForceY(_statistics.JumpPointStructures[jumpIndex].JumpStats.Strength * Rigidbody.mass, ForceMode2D.Impulse);
				while (OnGround)
					await Task.Yield();
				_isJumping = true;
				_contunuosFollow = _follow = _statistics.JumpPointStructures[jumpIndex].JumpStats.Follow;
				_turnFollow = _statistics.JumpPointStructures[jumpIndex].JumpStats.TurnFollow;
				_useTarget = _statistics.JumpPointStructures[jumpIndex].JumpStats.UseTarget;
				_otherTarget = _statistics.JumpPointStructures[jumpIndex].JumpStats.OtherTarget;
				_jumpCount[jumpIndex] = (short)_statistics.JumpPointStructures[jumpIndex].JumpCount;
				_jumpTime = _statistics.TimeToJump;
				if (_statistics.JumpPointStructures[jumpIndex].JumpStats.StopMove)
				{
					_sender.SetStateForm(StateForm.State);
					_sender.SetToggle(false);
					_sender.Send(PathConnection.Enemy);
					Rigidbody.linearVelocityX = 0f;
				}
			}
		}
		public new async void Receive(DataConnection data)
		{
			if (data.AdditionalData != null && data.AdditionalData is EnemyProvider[] && (data.AdditionalData as EnemyProvider[]).Length > 0)
				foreach (EnemyProvider enemy in data.AdditionalData as EnemyProvider[])
					if (enemy && enemy == this)
					{
						base.Receive(data);
						if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
							_stopJump = !data.ToggleValue.Value;
						else if (data.StateForm == StateForm.Event && _statistics.ReactToDamage)
						{
							Rigidbody.AddForceY(_statistics.StrenghtReact * Rigidbody.mass, ForceMode2D.Impulse);
							while (OnGround)
								await Task.Yield();
							_isJumping = true;
							_contunuosFollow = _follow = _statistics.FollowReact;
							_turnFollow = _statistics.TurnFollowReact;
							_useTarget = _statistics.UseTarget;
							_otherTarget = _statistics.OtherTarget;
							if (_statistics.StopMoveReact)
							{
								_sender.SetStateForm(StateForm.State);
								_sender.SetToggle(false);
								_sender.Send(PathConnection.Enemy);
							}
						}
						return;
					}
		}
	};
};
