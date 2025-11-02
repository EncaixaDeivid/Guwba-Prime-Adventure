using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using System;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Animator), typeof(SortingGroup))]
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CircleCollider2D)), RequireComponent(typeof(CinemachineImpulseSource))]
	public sealed class GuwbaAstralMarker : StateController, ILoader, IConnector
	{
		private static GuwbaAstralMarker _instance;
		private GuwbaCanvas _guwbaCanvas;
		private GuwbaDamager[] _guwbaDamagers;
		private Animator _animator;
		private Rigidbody2D _rigidbody;
		private BoxCollider2D _collider;
		private CinemachineImpulseSource _screenShaker;
		private InputController _inputController;
		private readonly Sender _sender = Sender.Create();
		private Vector2 _normalOffset = new();
		private Vector2 _normalSize = new();
		private Vector2 _originCast;
		private Vector2 _sizeCast;
		private Vector3 _jokerValue;
		private RaycastHit2D _bottomCast;
		private readonly int _isOn = Animator.StringToHash("IsOn");
		private readonly int _idle = Animator.StringToHash("Idle");
		private readonly int _walk = Animator.StringToHash("Walk");
		private readonly int _walkSpeed = Animator.StringToHash("WalkSpeed");
		private readonly int _jump = Animator.StringToHash("Jump");
		private readonly int _fall = Animator.StringToHash("Fall");
		private readonly int _dashSlide = Animator.StringToHash("DashSlide");
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
		private Vector2 Local => (Vector2)transform.position + _collider.offset;
		public static Vector2 Localization => _instance ? _instance.transform.position : Vector2.zero;
		public PathConnection PathConnection => PathConnection.Character;
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
			_guwbaCanvas = GetComponentInChildren<GuwbaCanvas>();
			_guwbaDamagers = GetComponentsInChildren<GuwbaDamager>();
			_inputController = new InputController();
			_inputController.Commands.Movement.started += Movement;
			_inputController.Commands.Movement.performed += Movement;
			_inputController.Commands.Movement.canceled += Movement;
			_inputController.Commands.AttackUse.started += AttackUse;
			_inputController.Commands.AttackUse.canceled += AttackUse;
			_inputController.Commands.Interaction.started += Interaction;
			SceneManager.sceneLoaded += SceneLoaded;
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
			SceneManager.sceneLoaded -= SceneLoaded;
			Sender.Exclude(this);
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			if (_guwbaCanvas.RootElement != null)
				_guwbaCanvas.RootElement.style.display = DisplayStyle.Flex;
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
			_guwbaCanvas.RootElement.style.display = DisplayStyle.None;
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
			EnableInputs();
			DontDestroyOnLoad(gameObject);
		}
		public IEnumerator Load()
		{
			if (!_instance || _instance != this)
				yield break;
			yield return _guwbaCanvas.StartUI();
			SaveController.Load(out SaveFile saveFile);
			(_guwbaCanvas.LifeText.text, _guwbaCanvas.CoinText.text) = ($"X {saveFile.lifes}", $"X {saveFile.coins}");
			(_vitality, _stunResistance) = ((short)_guwbaCanvas.Vitality.Length, (short)_guwbaCanvas.StunResistance.Length);
			foreach (GuwbaDamager guwbaDamager in _guwbaDamagers)
			{
				guwbaDamager.DamagerHurt += Hurt;
				guwbaDamager.DamagerStun += Stun;
				guwbaDamager.DamagerAttack += Attack;
			}
			transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (_turnLeft ? -1f : 1f), transform.localScale.y, transform.localScale.z);
			(_gravityScale, _normalOffset, _normalSize) = (_rigidbody.gravityScale, _collider.offset, _collider.size);
			SceneLoaded.Invoke(SceneManager.GetActiveScene(), LoadSceneMode.Single);
			yield return new WaitForEndOfFrame();
		}
		private UnityAction<Scene, LoadSceneMode> SceneLoaded => (scene, loadMode) =>
		{
			if (!_instance || _instance != this)
				return;
			if (scene.name.ContainsInvariantCultureIgnoreCase("Menu"))
			{
				Destroy(gameObject);
				return;
			}
			StartCoroutine(ResetPosition());
			IEnumerator ResetPosition()
			{
				while (SceneInitiator.IsInTrancision())
				{
					transform.position = Vector2.zero;
					yield return new WaitForEndOfFrame();
				}
			}
			if (scene.name == _hubbyWorldScene)
			{
				foreach (VisualElement vitality in _guwbaCanvas.Vitality)
					vitality.style.display = DisplayStyle.None;
				foreach (VisualElement recoverVitality in _guwbaCanvas.RecoverVitality)
					recoverVitality.style.display = DisplayStyle.None;
				foreach (VisualElement stunResistance in _guwbaCanvas.StunResistance)
					stunResistance.style.display = DisplayStyle.None;
				foreach (VisualElement bunnyHop in _guwbaCanvas.BunnyHop)
					bunnyHop.style.display = DisplayStyle.None;
				_guwbaCanvas.FallDamageText.style.display = DisplayStyle.None;
			}
		};
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
				if (!_isOnGround && movement.performed && !(SceneManager.GetActiveScene().name == _hubbyWorldScene))
					if (_bunnyHopBoost >= _guwbaCanvas.BunnyHop.Length)
						_bunnyHopBoost = (ushort)_guwbaCanvas.BunnyHop.Length;
					else
						_bunnyHopBoost += 1;
			}
			if (_isJumping && _rigidbody.linearVelocityY > 0f && movement.ReadValue<Vector2>().y < 0.25f)
			{
				(_isJumping, _lastJumpTime) = (false, 0f);
				_rigidbody.AddForceY(_rigidbody.linearVelocityY * _jumpCut * -_rigidbody.mass, ForceMode2D.Impulse);
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
					_jokerValue = new Vector2(transform.position.x + _normalOffset.x, transform.position.y + _normalOffset.y + _groundChecker);
					float dashLocation = transform.position.x;
					while (Physics2D.BoxCast(_jokerValue, _normalSize, 0f, transform.up, _groundChecker, _groundLayer) || Mathf.Abs(transform.position.x - dashLocation) < _dashDistance)
					{
						_rigidbody.linearVelocityX = _dashSpeed * _dashMovement;
						_originCast = new Vector2(Local.x + (_collider.bounds.extents.x + _groundChecker / 2f) * _dashMovement, Local.y);
						_sizeCast = new Vector2(_groundChecker, _collider.size.y - _groundChecker);
						if (Physics2D.BoxCast(_originCast, _sizeCast, 0f, transform.right * _dashMovement, _groundChecker, _groundLayer) || !_dashActive || !_isOnGround || _isJumping)
							break;
						_jokerValue = new Vector2(transform.position.x + _normalOffset.x, transform.position.y + _normalOffset.y + _groundChecker);
						yield return new WaitForFixedUpdate();
						yield return new WaitUntil(() => isActiveAndEnabled && !_animator.GetBool(_stun));
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
				yield return new WaitUntil(() => isActiveAndEnabled);
			}
			foreach (GuwbaDamager guwbaDamager in _guwbaDamagers)
				guwbaDamager.Alpha = 1f;
		}
		public Predicate<ushort> Hurt => damage =>
		{
			if (_invencibility || damage <= 0f)
				return false;
			_vitality -= (short)damage;
			for (ushort i = (ushort)_guwbaCanvas.Vitality.Length; i > (_vitality >= 0f ? _vitality : 0f); i--)
			{
				_guwbaCanvas.Vitality[i - 1].style.backgroundColor = new StyleColor(_guwbaCanvas.MissingColor);
				_guwbaCanvas.Vitality[i - 1].style.borderBottomColor = new StyleColor(_guwbaCanvas.MissingColor);
				_guwbaCanvas.Vitality[i - 1].style.borderLeftColor = new StyleColor(_guwbaCanvas.MissingColor);
				_guwbaCanvas.Vitality[i - 1].style.borderRightColor = new StyleColor(_guwbaCanvas.MissingColor);
				_guwbaCanvas.Vitality[i - 1].style.borderTopColor = new StyleColor(_guwbaCanvas.MissingColor);
			}
			_timerOfInvencibility = _invencibilityTime;
			_invencibility = true;
			StartCoroutine(VisualEffect());
			if (_vitality <= 0f)
			{
				SaveController.Load(out SaveFile saveFile);
				_guwbaCanvas.LifeText.text = $"X {saveFile.lifes -= 1}";
				SaveController.WriteSave(saveFile);
				StopAllCoroutines();
				foreach (GuwbaDamager guwbaDamager in _guwbaDamagers)
					guwbaDamager.Alpha = 1f;
				OnDisable();
				_animator.SetBool(_idle, false);
				_animator.SetBool(_walk, false);
				_animator.SetBool(_jump, false);
				_animator.SetBool(_fall, false);
				_animator.SetBool(_dashSlide, false);
				_animator.SetBool(_attackJump, false);
				_animator.SetBool(_attackSlide, false);
				_animator.SetBool(_stun, false);
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
			for (ushort i = (ushort)_guwbaCanvas.StunResistance.Length; i > (_stunResistance >= 0f ? _stunResistance : 0f); i--)
				_guwbaCanvas.StunResistance[i - 1].style.backgroundColor = new StyleColor(_guwbaCanvas.MissingColor);
			if (_stunResistance <= 0f)
			{
				_animator.SetBool(_stun, true);
				_animator.SetFloat(_isOn, 100f);
				_stunTimer = stunTime;
				_stunResistance = (short)_guwbaCanvas.StunResistance.Length;
				for (ushort i = 0; i < _stunResistance; i++)
					_guwbaCanvas.StunResistance[i].style.backgroundColor = new StyleColor(_guwbaCanvas.StunResistanceColor);
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
					if (_recoverVitality >= _guwbaCanvas.RecoverVitality.Length && _vitality < _guwbaCanvas.Vitality.Length)
					{
						_recoverVitality = 0;
						for (ushort i = 0; i < _guwbaCanvas.RecoverVitality.Length; i++)
							_guwbaCanvas.RecoverVitality[i].style.backgroundColor = new StyleColor(_guwbaCanvas.MissingColor);
						_vitality += 1;
						for (ushort i = 0; i < _vitality; i++)
						{
							_guwbaCanvas.Vitality[i].style.backgroundColor = new StyleColor(_guwbaCanvas.BackgroundColor);
							_guwbaCanvas.Vitality[i].style.borderBottomColor = new StyleColor(_guwbaCanvas.BorderColor);
							_guwbaCanvas.Vitality[i].style.borderLeftColor = new StyleColor(_guwbaCanvas.BorderColor);
							_guwbaCanvas.Vitality[i].style.borderRightColor = new StyleColor(_guwbaCanvas.BorderColor);
							_guwbaCanvas.Vitality[i].style.borderTopColor = new StyleColor(_guwbaCanvas.BorderColor);
						}
						_stunResistance += 1;
						for (ushort i = 0; i < _stunResistance; i++)
							_guwbaCanvas.StunResistance[i].style.backgroundColor = new StyleColor(_guwbaCanvas.StunResistanceColor);
					}
					else if (_recoverVitality < _guwbaCanvas.RecoverVitality.Length)
					{
						_recoverVitality += 1;
						for (ushort i = 0; i < _recoverVitality; i++)
							_guwbaCanvas.RecoverVitality[i].style.backgroundColor = new StyleColor(_guwbaCanvas.BorderColor);
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
					(_guwbaCanvas.FallDamageText.style.opacity, _guwbaCanvas.FallDamageText.text) = (0f, $"X 0");
			if (!_dashActive && !_isOnGround && _rigidbody.linearVelocityY != 0f && !_downStairs && (_lastGroundedTime > 0f || _lastJumpTime > 0f))
				(_lastGroundedTime, _lastJumpTime) = (_lastGroundedTime - Time.deltaTime, _lastJumpTime - Time.deltaTime);
			if (_attackDelay > 0f)
				_attackDelay -= Time.deltaTime;
		}
		private void FixedUpdate()
		{
			if (!_instance || _instance != this)
				return;
			_downStairs = false;
			float BunnyHop(float callBackValue) => _bunnyHopBoost > 0f ? _bunnyHopBoost * callBackValue : 1f;
			if (!_dashActive)
			{
				if (_isOnGround)
				{
					if (_movementAction == 0f && (_animator.GetBool(_fall) || Mathf.Abs(_rigidbody.linearVelocityX) <= 1e-3f))
						_animator.SetBool(_idle, true);
					else if (_animator.GetBool(_idle))
						_animator.SetBool(_idle, false);
					if (_movementAction != 0f && !_animator.GetBool(_walk))
						_animator.SetBool(_walk, true);
					else if (_movementAction == 0f && _animator.GetBool(_walk))
						_animator.SetBool(_walk, false);
					if (_animator.GetBool(_jump))
						_animator.SetBool(_jump, false);
					if (_animator.GetBool(_fall))
						_animator.SetBool(_fall, false);
					(_lastGroundedTime, _longJumping, _bunnyHopBoost) = (_jumpCoyoteTime, _isJumping = false, _lastJumpTime > 0f ? _bunnyHopBoost : (ushort)0f);
					if (_bunnyHopBoost <= 0f && _isHoping)
					{
						_isHoping = false;
						for (ushort i = 0; i < _guwbaCanvas.BunnyHop.Length; i++)
							_guwbaCanvas.BunnyHop[i].style.backgroundColor = new StyleColor(_guwbaCanvas.MissingColor);
					}
					if (_fallDamage > 0f && _bunnyHopBoost <= 0f && !(SceneManager.GetActiveScene().name == _hubbyWorldScene))
					{
						_screenShaker.GenerateImpulseWithForce(_fallDamage / _fallDamageDistance);
						Hurt.Invoke((ushort)Mathf.Floor(_fallDamage / _fallDamageDistance));
						(_fallStarted, _fallDamage) = (false, 0f);
						if (_invencibility && _fadeTimer <= 0f)
							_fadeTimer = _timeToFadeShow;
						else
							(_guwbaCanvas.FallDamageText.style.opacity, _guwbaCanvas.FallDamageText.text) = (0f, $"X 0");
					}
				}
				else if (Mathf.Abs(_rigidbody.linearVelocityY) > 1e-3f && !_downStairs)
				{
					if (_animator.GetBool(_idle))
						_animator.SetBool(_idle, false);
					if (_animator.GetBool(_walk))
						_animator.SetBool(_walk, false);
					if (_rigidbody.linearVelocityY > 0f && !_animator.GetBool(_jump))
						_animator.SetBool(_jump, true);
					else if (_rigidbody.linearVelocityY < 0f && _animator.GetBool(_jump))
						_animator.SetBool(_jump, false);
					if (_rigidbody.linearVelocityY < 0f && !_animator.GetBool(_fall))
						_animator.SetBool(_fall, true);
					else if (_rigidbody.linearVelocityY > 0f && _animator.GetBool(_fall))
						_animator.SetBool(_fall, false);
					if (_animator.GetBool(_attackJump) && _rigidbody.linearVelocityY < 0f)
						_animator.SetBool(_attackJump, false);
					if (_animator.GetBool(_fall))
					{
						if (_rigidbody.gravityScale < _fallGravityMultiply * _gravityScale)
							_rigidbody.gravityScale = _fallGravityMultiply * _gravityScale;
						if (_fallStarted)
						{
							_fallDamage = Mathf.Abs(transform.position.y - _fallStart);
							if (_fallDamage >= _fallDamageDistance * _fallDamageShowMultiply)
								(_guwbaCanvas.FallDamageText.style.opacity, _guwbaCanvas.FallDamageText.text) = (1f, $"X " + (_fallDamage / _fallDamageDistance).ToString("F1"));
							else if (!_invencibility)
								(_guwbaCanvas.FallDamageText.style.opacity, _guwbaCanvas.FallDamageText.text) = (0f, $"X 0");
						}
						else
							(_fallStarted, _fallStart, _fallDamage) = (true, transform.position.y, 0f);
					}
					else
					{
						if (!_invencibility)
							(_guwbaCanvas.FallDamageText.style.opacity, _guwbaCanvas.FallDamageText.text) = (0f, $"X 0");
						if (_rigidbody.gravityScale > _gravityScale)
							_rigidbody.gravityScale = _gravityScale;
						if (_fallDamage > 0f)
							_fallDamage = 0f;
					}
					if (_attackUsage)
						_rigidbody.linearVelocityY *= _attackVelocityCut;
				}
				if (!_isOnGround && _canDownStairs && _movementAction != 0f && _lastJumpTime <= 0f)
				{
					_originCast = new Vector2(Local.x - (_collider.bounds.extents.x - _groundChecker) * _movementAction, Local.y - _collider.bounds.extents.y);
					_bottomCast = Physics2D.Raycast(_originCast, -transform.up, 1f + _groundChecker, _groundLayer);
					if (_downStairs = _bottomCast)
						transform.position = new Vector2(transform.position.x, transform.position.y - _bottomCast.distance);
				}
				if (_isOnGround && _movementAction != 0f)
				{
					_originCast = new Vector2(Local.x + (_collider.bounds.extents.x + _groundChecker / 2f) * _movementAction, Local.y - 1f * _bottomCheckerOffset);
					_sizeCast = new Vector2(_groundChecker, 1f - _groundChecker);
					_bottomCast = Physics2D.BoxCast(_originCast, _sizeCast, 0f, transform.right * _movementAction, _groundChecker, _groundLayer);
					if (_bottomCast)
					{
						_originCast = new Vector2(Local.x + (_collider.bounds.extents.x + _groundChecker / 2f) * _movementAction, Local.y + 5e-1f);
						_sizeCast = new Vector2(_groundChecker, 1f * _topWallChecker - _groundChecker);
						if (!Physics2D.BoxCast(_originCast, _sizeCast, 0f, transform.right * _movementAction, _groundChecker, _groundLayer))
						{
							_originCast = new Vector2(Local.x + (_collider.bounds.extents.x + _groundChecker) * _movementAction, Local.y + _collider.bounds.extents.y);
							_sizeCast = new Vector2(Local.x + (_collider.bounds.extents.x + _groundChecker) * _movementAction, Local.y - _collider.bounds.extents.y);
							foreach (RaycastHit2D lineCast in Physics2D.LinecastAll(_originCast, _sizeCast, _groundLayer))
								if (lineCast.collider == _bottomCast.collider)
								{
									_jokerValue.x = Mathf.Abs(lineCast.point.y - (transform.position.y - _collider.bounds.extents.y));
									transform.position = new Vector2(transform.position.x + _groundChecker * _movementAction, transform.position.y + _jokerValue.x);
									_rigidbody.linearVelocityX = _movementSpeed * _movementAction;
									break;
								}
						}
					}
				}
				_jokerValue.x = _longJumping ? _dashSpeed : _movementSpeed + BunnyHop(_velocityBoost);
				_jokerValue.y = _jokerValue.x * _movementAction - _rigidbody.linearVelocityX;
				_jokerValue.z = (Mathf.Abs(_jokerValue.x * _movementAction) > 0f ? _acceleration : _decceleration) + BunnyHop(_potencyBoost);
				_rigidbody.AddForceX(Mathf.Pow(Mathf.Abs(_jokerValue.y) * _jokerValue.z, _velocityPower) * Mathf.Sign(_jokerValue.y) * _rigidbody.mass);
				if (_movementAction != 0f)
				{
					if (Mathf.Abs(_rigidbody.linearVelocityX) > 1e-3f)
					{
						_jokerValue.y = _rigidbody.linearVelocityX > 0f ? 1f : -1f;
						transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * _jokerValue.y, transform.localScale.y, transform.localScale.z);
					}
					else if (Mathf.Abs(_rigidbody.linearVelocityX) <= 1e-3f)
						transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * _movementAction, transform.localScale.y, transform.localScale.z);
					if (_isOnGround && !_dashActive)
						_animator.SetFloat(_walkSpeed, Mathf.Abs(_rigidbody.linearVelocityX) <= 1e-3f ? 1f : Mathf.Abs(_rigidbody.linearVelocityX) / _jokerValue.x);
				}
				if (_attackUsage)
					_rigidbody.linearVelocityX *= _attackVelocityCut;
				if (_isOnGround && _movementAction == 0f && Mathf.Abs(_rigidbody.linearVelocityX) > 1e-3f)
				{
					_jokerValue.y = Mathf.Min(Mathf.Abs(_rigidbody.linearVelocityX), Mathf.Abs(_frictionAmount)) * Mathf.Sign(_rigidbody.linearVelocityX);
					_rigidbody.AddForceX(-_jokerValue.y * _rigidbody.mass, ForceMode2D.Impulse);
					_animator.SetFloat(_walkSpeed, Mathf.Abs(_rigidbody.linearVelocityX) / _jokerValue.x);
				}
			}
			if (!_isJumping && _lastJumpTime > 0f && _lastGroundedTime > 0f)
			{
				if (_comboAttackBuffer)
					_animator.SetBool(_attackJump, true);
				_isJumping = true;
				_longJumping = _dashActive;
				_rigidbody.gravityScale = _gravityScale;
				_rigidbody.linearVelocityY = 0f;
				if (_bunnyHopBoost > 0f)
				{
					_isHoping = true;
					for (ushort i = 0; i < _bunnyHopBoost; i++)
						_guwbaCanvas.BunnyHop[i].style.backgroundColor = new StyleColor(_guwbaCanvas.BunnyHopColor);
				}
				_rigidbody.AddForceY((_jumpStrenght + BunnyHop(_jumpBoost)) * _rigidbody.mass, ForceMode2D.Impulse);
			}
			(_isOnGround, _canDownStairs) = (false, _isOnGround);
		}
		private void GroundCheck()
		{
			if (!_instance || _instance != this)
				return;
			_originCast = new Vector2(Local.x, Local.y + (_collider.bounds.extents.y + _groundChecker / 2f) * -transform.up.y);
			_isOnGround = Physics2D.BoxCast(_originCast, new Vector2(_collider.size.x - _groundChecker, _groundChecker), 0f, -transform.up, _groundChecker, _groundLayer);
		}
		private void OnCollisionEnter2D(Collision2D collision) => GroundCheck();
		private void OnCollisionStay2D(Collision2D collision) => GroundCheck();
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<ICollectable>(out var collectable))
			{
				collectable.Collect();
				SaveController.Load(out SaveFile saveFile);
				(_guwbaCanvas.LifeText.text, _guwbaCanvas.CoinText.text) = ($"X {saveFile.lifes}", $"X {saveFile.coins}");
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
					for (ushort i = 0; i < (_vitality = (short)_guwbaCanvas.Vitality.Length); i++)
					{
						_guwbaCanvas.Vitality[i].style.backgroundColor = new StyleColor(_guwbaCanvas.BackgroundColor);
						_guwbaCanvas.Vitality[i].style.borderBottomColor = new StyleColor(_guwbaCanvas.BorderColor);
						_guwbaCanvas.Vitality[i].style.borderLeftColor = new StyleColor(_guwbaCanvas.BorderColor);
						_guwbaCanvas.Vitality[i].style.borderRightColor = new StyleColor(_guwbaCanvas.BorderColor);
						_guwbaCanvas.Vitality[i].style.borderTopColor = new StyleColor(_guwbaCanvas.BorderColor);
					}
					for (ushort i = _recoverVitality = 0; i < _guwbaCanvas.RecoverVitality.Length; i++)
						_guwbaCanvas.RecoverVitality[i].style.backgroundColor = new StyleColor(_guwbaCanvas.MissingColor);
					for (ushort i = 0; i < (_stunResistance = (short)_guwbaCanvas.StunResistance.Length); i++)
						_guwbaCanvas.StunResistance[i].style.backgroundColor = new StyleColor(_guwbaCanvas.StunResistanceColor);
					for (ushort i = _bunnyHopBoost = 0; i < _guwbaCanvas.BunnyHop.Length; i++)
						_guwbaCanvas.BunnyHop[i].style.backgroundColor = new StyleColor(_guwbaCanvas.MissingColor);
					_animator.SetBool(_death, _isHoping = false);
					transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (_turnLeft ? -1f : 1f), transform.localScale.y, transform.localScale.z);
				}
				else if (!data.ToggleValue.Value && additionalData is Vector2 position)
					transform.position = position;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue && data.ToggleValue.Value)
			{
				(_timerOfInvencibility, _invencibility) = (_invencibilityTime, true);
				StartCoroutine(VisualEffect());
				OnEnable();
			}
		}
	};
};
