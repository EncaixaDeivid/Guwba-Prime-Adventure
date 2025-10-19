using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Unity.Cinemachine;
using System;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Animator), typeof(SortingGroup))]
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CircleCollider2D)), RequireComponent(typeof(CinemachineImpulseSource))]
	public sealed class GuwbaAstralMarker : StateController, IConnector
	{
		private static GuwbaAstralMarker _instance;
		private GuwbaVisualizer _guwbaVisualizer;
		private GuwbaDamager[] _guwbaDamagers;
		private Animator _animator;
		private Rigidbody2D _rigidbody;
		private BoxCollider2D _collider;
		private CinemachineImpulseSource _screenShaker;
		private InputController _inputController;
		private readonly Sender _sender = Sender.Create();
		private Vector2 _normalOffset = new();
		private Vector2 _normalSize = new();
		private readonly int _isOn = Animator.StringToHash("IsOn");
		private readonly int _idle = Animator.StringToHash("Idle");
		private readonly int _walk = Animator.StringToHash("Walk");
		private readonly int _walkSpeed = Animator.StringToHash("WalkSpeed");
		private readonly int _dashSlide = Animator.StringToHash("DashSlide");
		private readonly int _jump = Animator.StringToHash("Jump");
		private readonly int _fall = Animator.StringToHash("Fall");
		private readonly int _attack = Animator.StringToHash("Attack");
		private readonly int _attackCombo = Animator.StringToHash("AttackCombo");
		private readonly int _attackJump = Animator.StringToHash("AttackJump");
		private readonly int _attackSlide = Animator.StringToHash("AttackSlide");
		private readonly int _stun = Animator.StringToHash("Stun");
		private readonly int _death = Animator.StringToHash("Death");
		private short _vitality;
		private short _stunResistance;
		private ushort _recoverVitality = 0;
		private ushort _bunnyHopBoost = 0;
		private float _timerOfInvencibility = 0f;
		private float _stunTimer = 0f;
		private float _fadeTimer = 0f;
		private float _gravityScale = 0f;
		private float _movementAction = 0f;
		private float _dashMovement = 0f;
		private float _guardDashMovement = 0f;
		private float _lastGroundedTime = 0f;
		private float _lastJumpTime = 0f;
		private float _fallStart = 0f;
		private float _fallDamage = 0f;
		private float _attackDelay = 0f;
		private bool _isOnGround = false;
		private bool _canDownStairs = false;
		private bool _downStairs = false;
		private bool _isJumping = false;
		private bool _longJumping = false;
		private bool _isHoping = false;
		private bool _dashActive = false;
		private bool _fallStarted = false;
		private bool _invencibility = false;
		[Header("Control Statistics")]
		[SerializeField, Tooltip("The scene of the hubby world.")] private SceneField _hubbyWorldScene;
		[SerializeField, Tooltip("The layer mask that Guwba identifies the ground.")] private LayerMask _groundLayer;
		[SerializeField, Tooltip("The layer mask that Guwba identifies a interactive object.")] private LayerMask _InteractionLayer;
		[SerializeField, Tooltip("Size of collider for checking the ground.")] private float _groundChecker;
		[SerializeField, Tooltip("Size of top part of the wall collider to climb stairs.")] private float _topWallChecker;
		[SerializeField, Tooltip("Offset of bottom part of the wall collider to climb stairs.")] private float _bottomCheckerOffset;
		[SerializeField, Tooltip("The amount of gravity to multiply on the fall.")] private float _fallGravityMultiply;
		[SerializeField, Tooltip("The amount of fall's distance to take damage.")] private float _fallDamageDistance;
		[SerializeField, Tooltip("The amount of time to fade the show of fall's damage.")] private float _timeToFadeShow;
		[SerializeField, Range(0f, 1f), Tooltip("The amount of fall's distance to start show the fall damage.")] private float _fallDamageShowMultiply;
		[SerializeField, Range(0f, 1f), Tooltip("The amount of velocity to cut during the attack.")] private float _attackVelocityCut;
		[SerializeField, Tooltip("The amount of time that Guwba gets invencible.")] private float _invencibilityTime;
		[SerializeField, Tooltip("The value applied to visual when a hit is taken.")] private float _invencibilityValue;
		[SerializeField, Tooltip("The amount of time that the has to stay before fade.")] private float _timeStep;
		[SerializeField, Tooltip("The amount of time taht Guwba will be stunned after recover.")] private float _stunnedTime;
		[Header("Movement")]
		[SerializeField, Tooltip("The amount of speed that Guwba moves yourself.")] private float _movementSpeed;
		[SerializeField, Tooltip("The amount of acceleration Guwba will apply to the Movement.")] private float _acceleration;
		[SerializeField, Tooltip("The amount of decceleration Guwba will apply to the Movement.")] private float _decceleration;
		[SerializeField, Tooltip("The amount of power the velocity Guwba will apply to the Movement.")] private float _velocityPower;
		[SerializeField, Tooltip("The amount of friction Guwba will apply to the end of Movement.")] private float _frictionAmount;
		[SerializeField, Tooltip("The amount of speed in both dashes.")] private float _dashSpeed;
		[SerializeField, Tooltip("The amount of distance Guwba will go in both dashes.")] private float _dashDistance;
		[SerializeField, Tooltip("The amount of max speed to increase on the bunny hop.")] private float _velocityBoost;
		[SerializeField, Tooltip("The amount of acceleration/decceleration to increase on the bunny hop.")] private float _potencyBoost;
		[SerializeField, Tooltip("If Guwba will look firstly to the left.")] private bool _turnLeft;
		[Header("Jump")]
		[SerializeField, Tooltip("The amount of strenght that Guwba can Jump.")] private float _jumpStrenght;
		[SerializeField, Tooltip("The amount of strenght that will be added on the bunny hop.")] private float _jumpBoost;
		[SerializeField, Tooltip("The amount of time that Guwba can Jump before thouching ground.")] private float _jumpBufferTime;
		[SerializeField, Tooltip("The amount of time that Guwba can Jump when get out of the ground.")] private float _jumpCoyoteTime;
		[SerializeField, Range(0f, 1f), Tooltip("The amount of cut that Guwba's jump will suffer at up.")] private float _jumpCut;
		[Header("Attack")]
		[SerializeField, Tooltip("The amount of time to stop the game when hit is given.")] private float _hitStopTime;
		[SerializeField, Tooltip("The amount of time to slow the game when hit is given.")] private float _hitSlowTime;
		[SerializeField, Tooltip("The amount of time the attack will be inactive after attack's hit.")] private float _delayAfterAttack;
		[SerializeField, Tooltip("If Guwba is attacking in the moment.")] private bool _attackUsage;
		[SerializeField, Tooltip("The buffer moment that Guwba have to execute a combo attack.")] private bool _comboAttackBuffer;
		public static Vector2 Localization => _instance ? _instance.transform.position : Vector2.zero;
		public PathConnection PathConnection => PathConnection.Character;
		public static bool Attacked => _instance ? _instance._attackUsage : false;
		public static bool Hurted => _instance ? _instance._invencibility : false;
		public static bool Stunned => _instance ? _instance._stunTimer > 0f : false;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			_animator = GetComponent<Animator>();
			_rigidbody = GetComponent<Rigidbody2D>();
			_collider = GetComponent<BoxCollider2D>();
			_screenShaker = GetComponent<CinemachineImpulseSource>();
			_guwbaVisualizer = GetComponentInChildren<GuwbaVisualizer>();
			_guwbaDamagers = GetComponentsInChildren<GuwbaDamager>();
			_inputController = new InputController();
			_inputController.Commands.Movement.started += Movement;
			_inputController.Commands.Movement.performed += Movement;
			_inputController.Commands.Movement.canceled += Movement;
			_inputController.Commands.AttackUse.started += AttackUse;
			_inputController.Commands.AttackUse.canceled += AttackUse;
			_inputController.Commands.Interaction.started += Interaction;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!_instance || _instance != this)
				return;
			StopAllCoroutines();
			foreach (GuwbaDamager guwbaDamager in _guwbaDamagers)
			{
				guwbaDamager.DamagerHurt -= Hurt;
				guwbaDamager.DamagerStun -= Stun;
				guwbaDamager.DamagerAttack -= Attack;
				guwbaDamager.Alpha = 1f;
			}
			_inputController.Commands.Movement.started -= Movement;
			_inputController.Commands.Movement.performed -= Movement;
			_inputController.Commands.Movement.canceled -= Movement;
			_inputController.Commands.AttackUse.started -= AttackUse;
			_inputController.Commands.AttackUse.canceled -= AttackUse;
			_inputController.Commands.Interaction.started -= Interaction;
			_inputController.Dispose();
			Sender.Exclude(this);
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			_guwbaVisualizer.RootElement.style.display = DisplayStyle.Flex;
			_animator.SetFloat(_isOn, 1f);
			_animator.SetFloat(_walkSpeed, 1f);
			EnableInputs();
			if (_dashActive)
				_dashMovement = _guardDashMovement;
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			_guwbaVisualizer.RootElement.style.display = DisplayStyle.None;
			_animator.SetFloat(_isOn, 0f);
			_animator.SetFloat(_walkSpeed, 0f);
			DisableInputs();
			if (_dashActive)
				(_guardDashMovement, _dashMovement) = (_dashMovement, 0f);
		}
		private void EnableInputs()
		{
			_inputController.Commands.Movement.Enable();
			_inputController.Commands.AttackUse.Enable();
			_inputController.Commands.Interaction.Enable();
			_rigidbody.WakeUp();
		}
		private void DisableInputs()
		{
			_inputController.Commands.Movement.Disable();
			_inputController.Commands.AttackUse.Disable();
			_inputController.Commands.Interaction.Disable();
			_rigidbody.Sleep();
			_movementAction = 0f;
		}
		private IEnumerator Start()
		{
			if (!_instance || _instance != this)
				yield break;
			DisableInputs();
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			SaveController.Load(out SaveFile saveFile);
			(_guwbaVisualizer.LifeText.text, _guwbaVisualizer.CoinText.text) = ($"X {saveFile.lifes}", $"X {saveFile.coins}");
			(_vitality, _stunResistance) = ((short)_guwbaVisualizer.Vitality.Length, (short)_guwbaVisualizer.StunResistance.Length);
			foreach (GuwbaDamager guwbaDamager in _guwbaDamagers)
			{
				guwbaDamager.DamagerHurt += Hurt;
				guwbaDamager.DamagerStun += Stun;
				guwbaDamager.DamagerAttack += Attack;
			}
			transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (_turnLeft ? -1f : 1f), transform.localScale.y, transform.localScale.z);
			_gravityScale = _rigidbody.gravityScale;
			_normalOffset = _collider.offset;
			_normalSize = _collider.size;
			if (gameObject.scene.name.Contains(_hubbyWorldScene))
			{
				foreach (VisualElement vitality in _guwbaVisualizer.Vitality)
					vitality.style.display = DisplayStyle.None;
				foreach (VisualElement recoverVitality in _guwbaVisualizer.RecoverVitality)
					recoverVitality.style.display = DisplayStyle.None;
				foreach (VisualElement stunResistance in _guwbaVisualizer.StunResistance)
					stunResistance.style.display = DisplayStyle.None;
				foreach (VisualElement bunnyHop in _guwbaVisualizer.BunnyHop)
					bunnyHop.style.display = DisplayStyle.None;
				_guwbaVisualizer.FallDamageText.style.display = DisplayStyle.None;
			}
			EnableInputs();
		}
		private Action<InputAction.CallbackContext> Movement => movement =>
		{
			if (!isActiveAndEnabled || _animator.GetBool(_stun))
				return;
			_movementAction = 0f;
			if (Mathf.Abs(movement.ReadValue<Vector2>().x) > 0.5f)
				if (movement.ReadValue<Vector2>().x > 0f)
					_movementAction = 1f;
				else if (movement.ReadValue<Vector2>().x < 0f)
					_movementAction = -1f;
			if (movement.ReadValue<Vector2>().y > 0.25f)
			{
				_lastJumpTime = _jumpBufferTime;
				if (!_isOnGround && movement.performed && !gameObject.scene.name.Contains(_hubbyWorldScene))
					if (_bunnyHopBoost >= _guwbaVisualizer.BunnyHop.Length)
						_bunnyHopBoost = (ushort)_guwbaVisualizer.BunnyHop.Length;
					else
						_bunnyHopBoost += 1;
			}
			if (_isJumping && _rigidbody.linearVelocityY > 0f && movement.ReadValue<Vector2>().y < 0.25f)
			{
				_isJumping = false;
				_rigidbody.AddForceY(_rigidbody.linearVelocityY * _jumpCut * -_rigidbody.mass, ForceMode2D.Impulse);
				_lastJumpTime = 0f;
			}
			if (_movementAction != 0f && movement.ReadValue<Vector2>().y < -0.25f && !_dashActive && _isOnGround && (!_attackUsage || _comboAttackBuffer))
			{
				StartCoroutine(Dash());
				IEnumerator Dash()
				{
					_animator.SetBool(_dashSlide, _dashActive = true);
					_animator.SetBool(_attackSlide, _comboAttackBuffer);
					_dashMovement = _movementAction;
					transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * _dashMovement, transform.localScale.y, transform.localScale.z);
					Vector2 position;
					Vector2 origin;
					Vector2 size;
					Vector2 wallOrigin;
					float dashLocation = transform.position.x;
					bool onWall = false;
					bool block = false;
					while (onWall || !(Mathf.Abs(transform.position.x - dashLocation) >= _dashDistance || !_dashActive || block || !_isOnGround || _isJumping))
					{
						position = (Vector2)transform.position + _collider.offset;
						origin = new Vector2(position.x + (_collider.bounds.extents.x + _groundChecker / 2f) * _dashMovement, position.y);
						size = new Vector2(_groundChecker, _collider.size.y - _groundChecker);
						block = Physics2D.BoxCast(origin, size, 0f, transform.right * _dashMovement, _groundChecker, _groundLayer);
						wallOrigin = new Vector2(transform.position.x + _normalOffset.x, transform.position.y + _normalOffset.y + _groundChecker);
						onWall = Physics2D.BoxCast(wallOrigin, _normalSize, 0f, transform.up, _groundChecker, _groundLayer);
						_rigidbody.linearVelocityX = _dashSpeed * _dashMovement;
						yield return new WaitForFixedUpdate();
						yield return new WaitUntil(() => isActiveAndEnabled || _animator.GetBool(_stun));
					}
					_animator.SetBool(_dashSlide, _dashActive = false);
					_animator.SetBool(_attackSlide, false);
				}
			}
		};
		private Action<InputAction.CallbackContext> AttackUse => attackUse =>
		{
			if ((_attackDelay > 0f && !_comboAttackBuffer) || _dashActive || !isActiveAndEnabled || _animator.GetBool(_stun))
				return;
			if (attackUse.started && !_attackUsage)
				_animator.SetTrigger(_attack);
			if (attackUse.canceled && _comboAttackBuffer)
				_animator.SetTrigger(_attackCombo);
		};
		private Action<InputAction.CallbackContext> Interaction => interaction =>
		{
			if (!_isOnGround || _movementAction != 0f || !isActiveAndEnabled || _animator.GetBool(_stun))
				return;
			foreach (Collider2D collider in Physics2D.OverlapBoxAll((Vector2)transform.position + _normalOffset, _normalSize, transform.eulerAngles.z, _InteractionLayer))
				if (collider.TryGetComponent<IInteractable>(out _))
				{
					foreach (IInteractable interactable in collider.GetComponents<IInteractable>())
						interactable.Interaction();
					return;
				}
		};
		private IEnumerator VisualEffect()
		{
			while (_invencibility)
			{
				foreach (GuwbaDamager guwbaDamager in _guwbaDamagers)
					guwbaDamager.Alpha = guwbaDamager.Alpha >= 1f ? _invencibilityValue : 1f;
				yield return new WaitTime(this, _timeStep);
			}
			foreach (GuwbaDamager guwbaDamager in _guwbaDamagers)
				guwbaDamager.Alpha = 1f;
		}
		public Predicate<ushort> Hurt => damage =>
		{
			if (_invencibility || damage <= 0f)
				return false;
			_vitality -= (short)damage;
			for (ushort i = (ushort)_guwbaVisualizer.Vitality.Length; i > (_vitality >= 0f ? _vitality : 0f); i--)
			{
				_guwbaVisualizer.Vitality[i - 1].style.backgroundColor = new StyleColor(_guwbaVisualizer.MissingColor);
				_guwbaVisualizer.Vitality[i - 1].style.borderBottomColor = new StyleColor(_guwbaVisualizer.MissingColor);
				_guwbaVisualizer.Vitality[i - 1].style.borderLeftColor = new StyleColor(_guwbaVisualizer.MissingColor);
				_guwbaVisualizer.Vitality[i - 1].style.borderRightColor = new StyleColor(_guwbaVisualizer.MissingColor);
				_guwbaVisualizer.Vitality[i - 1].style.borderTopColor = new StyleColor(_guwbaVisualizer.MissingColor);
			}
			_timerOfInvencibility = _invencibilityTime;
			_invencibility = true;
			StartCoroutine(VisualEffect());
			if (_vitality <= 0f)
			{
				SaveController.Load(out SaveFile saveFile);
				saveFile.lifes -= 1;
				_guwbaVisualizer.LifeText.text = $"X {saveFile.lifes}";
				SaveController.WriteSave(saveFile);
				StopAllCoroutines();
				foreach (GuwbaDamager guwbaDamager in _guwbaDamagers)
					guwbaDamager.Alpha = 1f;
				OnDisable();
				_animator.SetBool(_death, true);
				_rigidbody.gravityScale = _fallGravityMultiply * _gravityScale;
				_sender.SetToggle(false);
				_sender.SetStateForm(StateForm.State);
				_sender.Send(PathConnection.Hud);
				_sender.SetStateForm(StateForm.Action);
				_sender.Send(PathConnection.Hud);
				_sender.SetStateForm(StateForm.None);
				_sender.Send(PathConnection.Enemy);
				return true;
			}
			return true;
		};
		public UnityAction<ushort, float> Stun => (stunStrength, stunTime) =>
		{
			_stunResistance -= (short)stunStrength;
			for (ushort i = (ushort)_guwbaVisualizer.StunResistance.Length; i > (_stunResistance >= 0f ? _stunResistance : 0f); i--)
				_guwbaVisualizer.StunResistance[i - 1].style.backgroundColor = new StyleColor(_guwbaVisualizer.MissingColor);
			if (_stunResistance <= 0f)
			{
				_animator.SetBool(_stun, true);
				_animator.SetFloat(_isOn, 100f);
				_stunTimer = stunTime;
				_stunResistance = (short)_guwbaVisualizer.StunResistance.Length;
				for (ushort i = 0; i < _stunResistance; i++)
					_guwbaVisualizer.StunResistance[i].style.backgroundColor = new StyleColor(_guwbaVisualizer.StunResistanceColor);
				_dashActive = false;
				DisableInputs();
			}
		};
		private UnityAction<GuwbaDamager, IDestructible> Attack => (guwbaDamager, destructible) =>
		{
			if (destructible.Hurt(guwbaDamager.AttackDamage))
			{
				destructible.Stun(guwbaDamager.AttackDamage, guwbaDamager.StunTime);
				EffectsController.HitStop(_hitStopTime, _hitSlowTime);
				_attackDelay = _delayAfterAttack;
				for (ushort amount = 0; amount < (destructible.Health >= 0f ? guwbaDamager.AttackDamage : guwbaDamager.AttackDamage - Mathf.Abs(destructible.Health)); amount++)
					if (_recoverVitality >= _guwbaVisualizer.RecoverVitality.Length && _vitality < _guwbaVisualizer.Vitality.Length)
					{
						_recoverVitality = 0;
						for (ushort i = 0; i < _guwbaVisualizer.RecoverVitality.Length; i++)
							_guwbaVisualizer.RecoverVitality[i].style.backgroundColor = new StyleColor(_guwbaVisualizer.MissingColor);
						_vitality += 1;
						for (ushort i = 0; i < _vitality; i++)
						{
							_guwbaVisualizer.Vitality[i].style.backgroundColor = new StyleColor(_guwbaVisualizer.BackgroundColor);
							_guwbaVisualizer.Vitality[i].style.borderBottomColor = new StyleColor(_guwbaVisualizer.BorderColor);
							_guwbaVisualizer.Vitality[i].style.borderLeftColor = new StyleColor(_guwbaVisualizer.BorderColor);
							_guwbaVisualizer.Vitality[i].style.borderRightColor = new StyleColor(_guwbaVisualizer.BorderColor);
							_guwbaVisualizer.Vitality[i].style.borderTopColor = new StyleColor(_guwbaVisualizer.BorderColor);
						}
						_stunResistance += 1;
						for (ushort i = 0; i < _stunResistance; i++)
							_guwbaVisualizer.StunResistance[i].style.backgroundColor = new StyleColor(_guwbaVisualizer.StunResistanceColor);
					}
					else if (_recoverVitality < _guwbaVisualizer.RecoverVitality.Length)
					{
						_recoverVitality += 1;
						for (ushort i = 0; i < _recoverVitality; i++)
							_guwbaVisualizer.RecoverVitality[i].style.backgroundColor = new StyleColor(_guwbaVisualizer.BorderColor);
					}
			}
		};
		private void Update()
		{
			if (_invencibility)
				_invencibility = (_timerOfInvencibility -= Time.deltaTime) > 0f;
			if (_animator.GetBool(_stun))
				if ((_stunTimer -= Time.deltaTime) <= 0f)
				{
					_animator.SetBool(_stun, false);
					_animator.SetFloat(_isOn, 1f);
					EnableInputs();
				}
			if (_fadeTimer > 0f)
				if ((_fadeTimer -= Time.deltaTime) <= 0f)
					(_guwbaVisualizer.FallDamageText.style.opacity, _guwbaVisualizer.FallDamageText.text) = (0f, $"X 0");
			if (!_dashActive && !_isOnGround && _rigidbody.linearVelocityY != 0f && !_downStairs && (_lastGroundedTime > 0f || _lastJumpTime > 0f))
				(_lastGroundedTime, _lastJumpTime) = (_lastGroundedTime - Time.deltaTime, _lastJumpTime - Time.deltaTime);
			if (_attackDelay > 0f)
				_attackDelay -= Time.deltaTime;
			if (!_dashActive)
				if (_isOnGround)
				{
					_animator.SetBool(_idle, _movementAction == 0f);
					_animator.SetBool(_walk, _movementAction != 0f);
					_animator.SetBool(_jump, false);
					_animator.SetBool(_fall, false);
					_lastGroundedTime = _jumpCoyoteTime;
					_longJumping = _isJumping = !(_canDownStairs = true);
					_bunnyHopBoost = _lastJumpTime > 0f ? _bunnyHopBoost : (ushort)0f;
					if (_bunnyHopBoost <= 0f && _isHoping)
					{
						_isHoping = false;
						for (ushort i = 0; i < _guwbaVisualizer.BunnyHop.Length; i++)
							_guwbaVisualizer.BunnyHop[i].style.backgroundColor = new StyleColor(_guwbaVisualizer.MissingColor);
					}
					if (_fallDamage > 0f && _bunnyHopBoost <= 0f && !gameObject.scene.name.Contains(_hubbyWorldScene))
					{
						_screenShaker.GenerateImpulseWithForce(_fallDamage / _fallDamageDistance);
						Hurt.Invoke((ushort)Mathf.Floor(_fallDamage / _fallDamageDistance));
						(_fallStarted, _fallDamage) = (false, 0f);
						if (_invencibility && _fadeTimer <= 0f)
							_fadeTimer = _timeToFadeShow;
						else
							(_guwbaVisualizer.FallDamageText.style.opacity, _guwbaVisualizer.FallDamageText.text) = (0f, $"X 0");
					}
				}
				else if (Mathf.Abs(_rigidbody.linearVelocityY) >= 1e-3f && !_downStairs)
				{
					_animator.SetBool(_idle, false);
					_animator.SetBool(_walk, false);
					_animator.SetBool(_jump, _rigidbody.linearVelocityY > 0f);
					_animator.SetBool(_fall, _rigidbody.linearVelocityY < 0f);
					if (_animator.GetBool(_attackJump))
						_animator.SetBool(_attackJump, _rigidbody.linearVelocityY > 0f);
					if (_animator.GetBool(_fall))
					{
						_rigidbody.gravityScale = _fallGravityMultiply * _gravityScale;
						if (_fallStarted)
						{
							_fallDamage = Mathf.Abs(transform.position.y - _fallStart);
							if (_fallDamage >= _fallDamageDistance * _fallDamageShowMultiply)
								(_guwbaVisualizer.FallDamageText.style.opacity, _guwbaVisualizer.FallDamageText.text) = (1f, $"X " + (_fallDamage / _fallDamageDistance).ToString("F1"));
							else if (!_invencibility)
								(_guwbaVisualizer.FallDamageText.style.opacity, _guwbaVisualizer.FallDamageText.text) = (0f, $"X 0");
						}
						else
							(_fallStarted, _fallStart, _fallDamage) = (true, transform.position.y, 0f);
					}
					else
					{
						if (!_invencibility)
							(_guwbaVisualizer.FallDamageText.style.opacity, _guwbaVisualizer.FallDamageText.text) = (0f, $"X 0");
						(_rigidbody.gravityScale, _fallDamage) = (_gravityScale, 0f);
					}
					if (_attackUsage)
						_rigidbody.linearVelocityY *= _attackVelocityCut;
					_canDownStairs = false;
				}
		}
		private void FixedUpdate()
		{
			Vector2 position = (Vector2)transform.position + _collider.offset;
			_downStairs = false;
			if (!_isOnGround && _canDownStairs && _movementAction != 0f && _lastJumpTime <= 0f && !_dashActive)
			{
				Vector2 downRayOrigin = new(position.x - (_collider.bounds.extents.x - _groundChecker) * _movementAction, position.y - _collider.bounds.extents.y);
				RaycastHit2D downRay = Physics2D.Raycast(downRayOrigin, -transform.up, 1f + _groundChecker, _groundLayer);
				if (_downStairs = downRay)
					transform.position = new Vector2(transform.position.x, transform.position.y - downRay.distance);
			}
			float BunnyHop(float callBackValue) => _bunnyHopBoost > 0f ? _bunnyHopBoost * callBackValue : 1f;
			if (!_dashActive)
			{
				float speed = _longJumping ? _dashSpeed : _movementSpeed + BunnyHop(_velocityBoost);
				if (_isOnGround && _movementAction != 0f)
				{
					float stairsOriginX = (_collider.bounds.extents.x + _groundChecker / 2f) * _movementAction;
					Vector2 bottomOrigin = new(position.x + stairsOriginX, position.y - 1f * _bottomCheckerOffset);
					Vector2 bottomSize = new(_groundChecker, 1f - _groundChecker);
					RaycastHit2D bottomCast = Physics2D.BoxCast(bottomOrigin, bottomSize, 0f, transform.right * _movementAction, _groundChecker, _groundLayer);
					if (bottomCast)
					{
						Vector2 topOrigin = new(position.x + stairsOriginX, position.y + 1f * .5f);
						Vector2 topSize = new(_groundChecker, 1f * _topWallChecker - _groundChecker);
						if (!Physics2D.BoxCast(topOrigin, topSize, 0f, transform.right * _movementAction, _groundChecker, _groundLayer))
						{
							Vector2 lineStart = new(position.x + stairsOriginX + _groundChecker / 2f * _movementAction, position.y + _collider.bounds.extents.y);
							Vector2 lineEnd = new(position.x + stairsOriginX + _groundChecker / 2f * _movementAction, position.y - _collider.bounds.extents.y);
							RaycastHit2D lineWall = Physics2D.Linecast(lineStart, lineEnd, _groundLayer);
							if (lineWall.collider == bottomCast.collider)
							{
								float yDistance = position.y + (lineWall.point.y - (position.y - _collider.bounds.extents.y));
								transform.position = new Vector2(position.x + _groundChecker * _movementAction, yDistance);
								_rigidbody.linearVelocityX = _movementSpeed * _movementAction;
							}
						}
					}
				}
				if (_movementAction != 0f)
					transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * _movementAction, transform.localScale.y, transform.localScale.z);
				_animator.SetFloat(_walkSpeed, _movementAction != 0f && Mathf.Abs(_rigidbody.linearVelocityX) <= 1e-3f ? 1f : Mathf.Abs(_rigidbody.linearVelocityX) / speed);
				float speedDiferrence = speed * _movementAction - _rigidbody.linearVelocityX;
				float accelerationRate = (Mathf.Abs(speed * _movementAction) > 0f ? _acceleration : _decceleration) + BunnyHop(_potencyBoost);
				_rigidbody.AddForceX(Mathf.Pow(Mathf.Abs(speedDiferrence) * accelerationRate, _velocityPower) * Mathf.Sign(speedDiferrence) * _rigidbody.mass);
				if (_attackUsage)
					_rigidbody.linearVelocityX *= _attackVelocityCut;
			}
			if (_isOnGround && _movementAction == 0f && !_dashActive)
			{
				float frictionAmount = Mathf.Min(Mathf.Abs(_rigidbody.linearVelocityX), Mathf.Abs(_frictionAmount));
				frictionAmount *= Mathf.Sign(_rigidbody.linearVelocityX);
				_rigidbody.AddForceX(-frictionAmount * _rigidbody.mass, ForceMode2D.Impulse);
			}
			if (!_isJumping && _lastJumpTime > 0f && _lastGroundedTime > 0f)
			{
				_animator.SetBool(_attackJump, _comboAttackBuffer);
				_isJumping = true;
				_longJumping = _dashActive;
				_rigidbody.gravityScale = _gravityScale;
				_rigidbody.linearVelocityY = 0f;
				if (_bunnyHopBoost > 0f)
				{
					_isHoping = true;
					for (ushort i = 0; i < _bunnyHopBoost; i++)
						_guwbaVisualizer.BunnyHop[i].style.backgroundColor = new StyleColor(_guwbaVisualizer.BunnyHopColor);
				}
				_rigidbody.AddForceY((_jumpStrenght + BunnyHop(_jumpBoost)) * _rigidbody.mass, ForceMode2D.Impulse);
			}
			_isOnGround = false;
		}
		private void GroundCheck()
		{
			Vector2 position = (Vector2)transform.position + _collider.offset;
			Vector2 groundOrigin = new(position.x, position.y + (_collider.bounds.extents.y + _groundChecker / 2f) * -transform.up.y);
			_isOnGround = Physics2D.BoxCast(groundOrigin, new Vector2(_collider.size.x - _groundChecker, _groundChecker), 0f, -transform.up, _groundChecker, _groundLayer);
		}
		private void OnCollisionEnter2D(Collision2D collision) => GroundCheck();
		private void OnCollisionStay2D(Collision2D collision) => GroundCheck();
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<ICollectable>(out var collectable))
			{
				collectable.Collect();
				SaveController.Load(out SaveFile saveFile);
				(_guwbaVisualizer.LifeText.text, _guwbaVisualizer.CoinText.text) = ($"X {saveFile.lifes}", $"X {saveFile.coins}");
			}
		}
		public static bool EqualObject(params GameObject[] othersObjects)
		{
			if (_instance)
				foreach (GameObject other in othersObjects)
					if (other == _instance.gameObject)
						return true;
			return false;
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue)
				if (data.ToggleValue.Value)
				{
					for (ushort i = 0; i < (_vitality = (short)_guwbaVisualizer.Vitality.Length); i++)
					{
						_guwbaVisualizer.Vitality[i].style.backgroundColor = new StyleColor(_guwbaVisualizer.BackgroundColor);
						_guwbaVisualizer.Vitality[i].style.borderBottomColor = new StyleColor(_guwbaVisualizer.BorderColor);
						_guwbaVisualizer.Vitality[i].style.borderLeftColor = new StyleColor(_guwbaVisualizer.BorderColor);
						_guwbaVisualizer.Vitality[i].style.borderRightColor = new StyleColor(_guwbaVisualizer.BorderColor);
						_guwbaVisualizer.Vitality[i].style.borderTopColor = new StyleColor(_guwbaVisualizer.BorderColor);
					}
					for (ushort i = _recoverVitality = 0; i < _guwbaVisualizer.RecoverVitality.Length; i++)
						_guwbaVisualizer.RecoverVitality[i].style.backgroundColor = new StyleColor(_guwbaVisualizer.MissingColor);
					for (ushort i = 0; i < (_stunResistance = (short)_guwbaVisualizer.StunResistance.Length); i++)
						_guwbaVisualizer.StunResistance[i].style.backgroundColor = new StyleColor(_guwbaVisualizer.StunResistanceColor);
					for (ushort i = _bunnyHopBoost = 0; i < _guwbaVisualizer.BunnyHop.Length; i++)
						_guwbaVisualizer.BunnyHop[i].style.backgroundColor = new StyleColor(_guwbaVisualizer.MissingColor);
					_animator.SetBool(_death, _isHoping = false);
					transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (_turnLeft ? -1f : 1f), transform.localScale.y, transform.localScale.z);
				}
				else if (!data.ToggleValue.Value && (Vector2)additionalData != null)
					transform.position = (Vector2)additionalData;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue && data.ToggleValue.Value)
			{
				_timerOfInvencibility = _invencibilityTime;
				_invencibility = true;
				StartCoroutine(VisualEffect());
				OnEnable();
			}
		}
	};
};
