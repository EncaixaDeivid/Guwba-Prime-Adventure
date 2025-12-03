using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Unity.Cinemachine;
using System;
using System.Collections;
using NaughtyAttributes;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Animator), typeof(SortingGroup)), SelectionBase]
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CircleCollider2D)), RequireComponent(typeof(CinemachineImpulseSource))]
	public sealed class GwambaStateMarker : StateController, ILoader, IConnector
	{
		private static GwambaStateMarker _instance;
		private GwambaCanvas _gwambaCanvas;
		private GwambaDamager[] _gwambaDamagers;
		private Animator _animator;
		private Rigidbody2D _rigidbody;
		private BoxCollider2D _collider;
		private CinemachineImpulseSource _screenShaker;
		private InputController _inputController;
		private readonly Sender _sender = Sender.Create();
		private IEnumerator _airJumpMethod;
		private IEnumerator _dashSlideMethod;
		private Vector2 _originCast;
		private Vector2 _sizeCast;
		private Vector3 _jokerValue;
		private RaycastHit2D _castHit;
		private readonly int IsOn = Animator.StringToHash(nameof(IsOn));
		private readonly int Idle = Animator.StringToHash(nameof(Idle));
		private readonly int Walk = Animator.StringToHash(nameof(Walk));
		private readonly int WalkSpeed = Animator.StringToHash(nameof(WalkSpeed));
		private readonly int Jump = Animator.StringToHash(nameof(Jump));
		private readonly int Fall = Animator.StringToHash(nameof(Fall));
		private readonly int AirJump = Animator.StringToHash(nameof(AirJump));
		private readonly int DashSlide = Animator.StringToHash(nameof(DashSlide));
		private readonly int Attack = Animator.StringToHash(nameof(Attack));
		private readonly int AttackCombo = Animator.StringToHash(nameof(AttackCombo));
		private readonly int AttackJump = Animator.StringToHash(nameof(AttackJump));
		private readonly int AttackAirJump = Animator.StringToHash(nameof(AttackAirJump));
		private readonly int AttackSlide = Animator.StringToHash(nameof(AttackSlide));
		private readonly int Stun = Animator.StringToHash(nameof(Stun));
		private readonly int Death = Animator.StringToHash(nameof(Death));
		private short _vitality;
		private short _stunResistance;
		private ushort _recoverVitality = 0;
		private ushort _bunnyHopBoost = 0;
		private float _timerOfInvencibility = 0f;
		private float _showInvencibilityTimer = 0f;
		private float _stunTimer = 0f;
		private float _fadeTimer = 0f;
		private float _gravityScale = 0f;
		private float _movementAction = 0f;
		private float _lastGroundedTime = 0f;
		private float _lastJumpTime = 0f;
		private float _fallStart = 0f;
		private float _fallDamage = 0f;
		private float _attackDelay = 0f;
		private bool _didStart = false;
		private bool _isOnGround = false;
		private bool _canDownStairs = false;
		private bool _downStairs = false;
		private bool _isJumping = false;
		private bool _canAirJump = true;
		private bool _longJumping = false;
		private bool _hopActive = false;
		private bool _isHoping = false;
		private bool _fallStarted = false;
		private bool _invencibility = false;
		[Space(WorldBuild.FIELD_SPACE_LENGTH * 2f)]
		[SerializeField, BoxGroup("Control"), Tooltip("The scene of the hubby world.")] private SceneField _hubbyWorldScene;
		[SerializeField, BoxGroup("Control"), Tooltip("The scene of the menu.")] private SceneField _menuScene;
		[SerializeField, BoxGroup("Control"), Tooltip("The sound to play when Gwamba gets hurt.")] private AudioClip _hurtSound;
		[SerializeField, BoxGroup("Control"), Tooltip("The sound to play when Gwamba gets stunned.")] private AudioClip _stunSound;
		[SerializeField, BoxGroup("Control"), Tooltip("The sound to play when Gwamba die.")] private AudioClip _deathSound;
		[SerializeField, BoxGroup("Control"), Tooltip("The velocity of the shake on the fall.")] private Vector2 _fallShake;
		[SerializeField, BoxGroup("Control"), Min(0f), Tooltip("The amount of time the fall screen shake will be applied.")] private float _fallShakeTime;
		[SerializeField, BoxGroup("Control"), Min(0f), Tooltip("Size of top part of the wall collider to climb stairs.")] private float _topWallChecker;
		[SerializeField, BoxGroup("Control"), Tooltip("Offset of bottom part of the wall collider to climb stairs.")] private float _bottomCheckerOffset;
		[SerializeField, BoxGroup("Control"), Min(0f), Tooltip("The amount of gravity to multiply on the fall.")] private float _fallGravityMultiply;
		[SerializeField, BoxGroup("Control"), Min(0f), Tooltip("The amount of fall's distance to take damage.")] private float _fallDamageDistance;
		[SerializeField, BoxGroup("Control"), Min(0f), Tooltip("The amount of time to fade the show of fall's damage.")] private float _timeToFadeShow;
		[SerializeField, BoxGroup("Control"), Range(0f, 1f), Tooltip("The amount of fall's distance to start show the fall damage.")] private float _fallDamageShowMultiply;
		[SerializeField, BoxGroup("Control"), Min(0f), Tooltip("The amount of time that Gwamba gets invencible.")] private float _invencibilityTime;
		[SerializeField, BoxGroup("Control"), Range(0f, 1f), Tooltip("The value applied to visual when a hit is taken.")] private float _invencibilityValue;
		[SerializeField, BoxGroup("Control"), Min(0f), Tooltip("The amount of time that Gwamba has to stay before fade.")] private float _timeStep;
		[SerializeField, BoxGroup("Control"), Min(0f), Tooltip("The amount of time taht Gwamba will be stunned after recover.")] private float _stunnedTime;
		[Space(WorldBuild.FIELD_SPACE_LENGTH * 2f)]
		[SerializeField, BoxGroup("Movement"), Tooltip("The sound to play when Gwamba steps while walking.")] private AudioClip _stepSound;
		[SerializeField, BoxGroup("Movement"), Tooltip("The sound to play when Gwamba executes the air jump.")] private AudioClip _airJumpSound;
		[SerializeField, BoxGroup("Movement"), Tooltip("The sound to play when Gwamba executes the dash slide.")] private AudioClip _dashSlideSound;
		[SerializeField, BoxGroup("Movement"), Min(0f), Tooltip("The amount of speed that Gwamba moves yourself.")] private float _movementSpeed;
		[SerializeField, BoxGroup("Movement"), Min(0f), Tooltip("The amount of acceleration Gwamba will apply to the movement.")] private float _acceleration;
		[SerializeField, BoxGroup("Movement"), Min(0f), Tooltip("The amount of decceleration Gwamba will apply to the movement.")] private float _decceleration;
		[SerializeField, BoxGroup("Movement"), Min(0f), Tooltip("The amount of power the velocity Gwamba will apply to the movement.")] private float _velocityPower;
		[SerializeField, BoxGroup("Movement"), Min(0f), Tooltip("The amount of friction Gwamba will apply to the end of movement.")] private float _frictionAmount;
		[SerializeField, BoxGroup("Movement"), Min(0f), Tooltip("The amount of speed that the dash will apply.")] private float _dashSpeed;
		[SerializeField, BoxGroup("Movement"), Min(0f), Tooltip("The amount of distance Gwamba will go in both dashes.")] private float _dashDistance;
		[SerializeField, BoxGroup("Movement"), Min(0f), Tooltip("The amount of max speed to increase on the bunny hop.")] private float _velocityBoost;
		[SerializeField, BoxGroup("Movement"), Min(0f), Tooltip("The amount of acceleration/decceleration to increase on the bunny hop.")] private float _potencyBoost;
		[SerializeField, BoxGroup("Movement"), Tooltip("If Gwamba will look firstly to the left.")] private bool _turnLeft;
		[Space(WorldBuild.FIELD_SPACE_LENGTH * 2f)]
		[SerializeField, BoxGroup("Jump"), Tooltip("The sound to play when Gwamba execute a jump.")] private AudioClip _jumpSound;
		[SerializeField, BoxGroup("Jump"), Min(0f), Tooltip("The amount of strenght that Gwamba can Jump.")] private float _jumpStrenght;
		[SerializeField, BoxGroup("Jump"), Min(0f), Tooltip("The amount of strenght that Gwamba can Jump on the air.")] private float _airJumpStrenght;
		[SerializeField, BoxGroup("Jump"), Min(0f), Tooltip("The amount of strenght that will be added on the bunny hop.")] private float _jumpBoost;
		[SerializeField, BoxGroup("Jump"), Min(0f), Tooltip("The amount of time that Gwamba can Jump before thouching ground.")] private float _jumpBufferTime;
		[SerializeField, BoxGroup("Jump"), Min(0f), Tooltip("The amount of time that Gwamba can Jump when get out of the ground.")] private float _jumpCoyoteTime;
		[SerializeField, BoxGroup("Jump"), Range(0f, 1f), Tooltip("The amount of cut that Gwamba's jump will suffer at up.")] private float _jumpCut;
		[Space(WorldBuild.FIELD_SPACE_LENGTH * 2f)]
		[SerializeField, BoxGroup("Attack"), Tooltip("The sound to play when Gwamba attack.")] private AudioClip _attackSound;
		[SerializeField, BoxGroup("Attack"), Tooltip("The sound to play when Gwamba damages something.")] private AudioClip _damageAttackSound;
		[SerializeField, BoxGroup("Attack"), Range(0f, 1f), Tooltip("The amount of velocity to cut during the attack.")] private float _attackVelocityCut;
		[SerializeField, BoxGroup("Attack"), Min(0f), Tooltip("The amount of time to stop the game when hit is given.")] private float _hitStopTime;
		[SerializeField, BoxGroup("Attack"), Min(0f), Tooltip("The amount of time to slow the game when hit is given.")] private float _hitSlowTime;
		[SerializeField, BoxGroup("Attack"), Min(0f), Tooltip("The amount of time the attack will be inactive after attack's hit.")] private float _delayAfterAttack;
		[SerializeField, BoxGroup("Attack"), Tooltip("If Gwamba is attacking in the moment.")] private bool _attackUsage;
		[SerializeField, BoxGroup("Attack"), Tooltip("The buffer moment that Gwamba have to execute a combo attack.")] private bool _comboAttackBuffer;
		private Vector2 Local => (Vector2)transform.position + _collider.offset;
		public static Vector2 Localization => _instance ? _instance.transform.position : Vector2.zero;
		public MessagePath Path => MessagePath.Character;
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
			_gwambaCanvas = GetComponentInChildren<GwambaCanvas>();
			_gwambaDamagers = GetComponentsInChildren<GwambaDamager>();
			_inputController = new InputController();
			_inputController.Commands.Movement.started += MovementInput;
			_inputController.Commands.Movement.performed += MovementInput;
			_inputController.Commands.Movement.canceled += MovementInput;
			_inputController.Commands.Jump.started += JumpInput;
			_inputController.Commands.Jump.canceled += JumpInput;
			_inputController.Commands.AttackUse.started += AttackUseInput;
			_inputController.Commands.AttackUse.canceled += AttackUseInput;
			_inputController.Commands.Interaction.started += InteractionInput;
			SceneManager.sceneLoaded += SceneLoaded;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!_instance || _instance != this)
				return;
			StopAllCoroutines();
			_airJumpMethod = null;
			_dashSlideMethod = null;
			foreach (GwambaDamager gwambaDamager in _gwambaDamagers)
			{
				gwambaDamager.DamagerHurt -= DamagerHurt;
				gwambaDamager.DamagerStun -= DamagerStun;
				gwambaDamager.DamagerAttack -= DamagerAttack;
				gwambaDamager.Alpha = 1f;
			}
			_inputController.Commands.Movement.started -= MovementInput;
			_inputController.Commands.Movement.performed -= MovementInput;
			_inputController.Commands.Movement.canceled -= MovementInput;
			_inputController.Commands.Jump.started -= JumpInput;
			_inputController.Commands.Jump.canceled -= JumpInput;
			_inputController.Commands.AttackUse.started -= AttackUseInput;
			_inputController.Commands.AttackUse.canceled -= AttackUseInput;
			_inputController.Commands.Interaction.started -= InteractionInput;
			_inputController.Dispose();
			SceneManager.sceneLoaded -= SceneLoaded;
			Sender.Exclude(this);
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			if (_gwambaCanvas.RootElement != null)
				_gwambaCanvas.RootElement.style.display = DisplayStyle.Flex;
			_animator.SetFloat(IsOn, 1f);
			_animator.SetFloat(WalkSpeed, 1f);
			EnableInputs();
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			_gwambaCanvas.RootElement.style.display = DisplayStyle.None;
			_animator.SetFloat(IsOn, 0f);
			_animator.SetFloat(WalkSpeed, 0f);
			DisableInputs();
		}
		private void EnableInputs()
		{
			_inputController.Commands.Movement.Enable();
			_inputController.Commands.Jump.Enable();
			_inputController.Commands.AttackUse.Enable();
			_inputController.Commands.Interaction.Enable();
			_rigidbody.WakeUp();
		}
		private void DisableInputs()
		{
			_inputController.Commands.Movement.Disable();
			_inputController.Commands.Jump.Disable();
			_inputController.Commands.AttackUse.Disable();
			_inputController.Commands.Interaction.Disable();
			_rigidbody.Sleep();
			_movementAction = 0f;
		}
		private IEnumerator Start()
		{
			if (!_instance || _instance != this)
				yield break;
			yield return StartCoroutine(StartLoad());
			_didStart = true;
			DontDestroyOnLoad(gameObject);
		}
		public IEnumerator StartLoad()
		{
			DisableInputs();
			yield return new WaitWhile(() =>
			{
				transform.position = Vector2.zero;
				return SceneInitiator.IsInTrancision();
			});
			if (_animator.GetBool(Death))
			{
				Reanimate();
				OnEnable();
			}
			else
				EnableInputs();
		}
		private void Reanimate()
		{
			for (ushort i = 0; i < (_vitality = (short)_gwambaCanvas.Vitality.Length); i++)
			{
				_gwambaCanvas.Vitality[i].style.backgroundColor = new StyleColor(_gwambaCanvas.BackgroundColor);
				_gwambaCanvas.Vitality[i].style.borderBottomColor = new StyleColor(_gwambaCanvas.BorderColor);
				_gwambaCanvas.Vitality[i].style.borderLeftColor = new StyleColor(_gwambaCanvas.BorderColor);
				_gwambaCanvas.Vitality[i].style.borderRightColor = new StyleColor(_gwambaCanvas.BorderColor);
				_gwambaCanvas.Vitality[i].style.borderTopColor = new StyleColor(_gwambaCanvas.BorderColor);
			}
			for (ushort i = _recoverVitality = 0; i < _gwambaCanvas.RecoverVitality.Length; i++)
				_gwambaCanvas.RecoverVitality[i].style.backgroundColor = new StyleColor(_gwambaCanvas.MissingColor);
			for (ushort i = 0; i < (_stunResistance = (short)_gwambaCanvas.StunResistance.Length); i++)
				_gwambaCanvas.StunResistance[i].style.backgroundColor = new StyleColor(_gwambaCanvas.StunResistanceColor);
			for (ushort i = _bunnyHopBoost = 0; i < _gwambaCanvas.BunnyHop.Length; i++)
				_gwambaCanvas.BunnyHop[i].style.backgroundColor = new StyleColor(_gwambaCanvas.MissingColor);
			_animator.SetBool(Death, _hopActive = _isHoping = false);
			transform.TurnScaleX(_turnLeft);
		}
		public IEnumerator Load()
		{
			if (!_instance || _instance != this)
				yield break;
			yield return _gwambaCanvas.StartUI();
			SaveController.Load(out SaveFile saveFile);
			(_gwambaCanvas.LifeText.text, _gwambaCanvas.CoinText.text) = ($"X {saveFile.Lifes}", $"X {saveFile.Coins}");
			(_vitality, _stunResistance) = ((short)_gwambaCanvas.Vitality.Length, (short)_gwambaCanvas.StunResistance.Length);
			foreach (GwambaDamager gwambaDamager in _gwambaDamagers)
			{
				gwambaDamager.DamagerHurt += DamagerHurt;
				gwambaDamager.DamagerStun += DamagerStun;
				gwambaDamager.DamagerAttack += DamagerAttack;
			}
			transform.TurnScaleX(_turnLeft);
			_gravityScale = _rigidbody.gravityScale;
			SceneLoaded.Invoke(SceneManager.GetActiveScene(), LoadSceneMode.Single);
			yield return null;
		}
		private UnityAction<Scene, LoadSceneMode> SceneLoaded => (scene, loadMode) =>
		{
			if (scene.name == _menuScene)
			{
				Destroy(gameObject);
				return;
			}
			if (_didStart)
				StartCoroutine(StartLoad());
			if (scene.name == _hubbyWorldScene)
			{
				foreach (VisualElement vitality in _gwambaCanvas.Vitality)
					vitality.style.display = DisplayStyle.None;
				foreach (VisualElement recoverVitality in _gwambaCanvas.RecoverVitality)
					recoverVitality.style.display = DisplayStyle.None;
				foreach (VisualElement stunResistance in _gwambaCanvas.StunResistance)
					stunResistance.style.display = DisplayStyle.None;
				foreach (VisualElement bunnyHop in _gwambaCanvas.BunnyHop)
					bunnyHop.style.display = DisplayStyle.None;
				_gwambaCanvas.FallDamageText.style.display = DisplayStyle.None;
			}
		};
		private Action<InputAction.CallbackContext> MovementInput => movement =>
		{
			if (!isActiveAndEnabled || _animator.GetBool(Stun))
				return;
			_movementAction = 0f;
			if (Mathf.Abs(movement.ReadValue<Vector2>().x) > 0.5f)
				if (movement.ReadValue<Vector2>().x > 0f)
					_movementAction = 1f;
				else if (movement.ReadValue<Vector2>().x < 0f)
					_movementAction = -1f;
			if (_movementAction != 0f && (!_attackUsage || _comboAttackBuffer))
				if (movement.ReadValue<Vector2>().y > 0.25f && !_isOnGround && _canAirJump)
				{
					_airJumpMethod = AirJumpMethod(_movementAction);
					_rigidbody.linearVelocity = Vector2.zero;
					_rigidbody.AddForceX((_airJumpStrenght + BunnyHop(_jumpBoost)) * _movementAction * _rigidbody.mass, ForceMode2D.Impulse);
					_rigidbody.AddForceY((_airJumpStrenght + BunnyHop(_jumpBoost)) * _rigidbody.mass, ForceMode2D.Impulse);
					IEnumerator AirJumpMethod(float dashMovement)
					{
						_animator.SetBool(AirJump, !(_canAirJump = false));
						_animator.SetBool(AttackAirJump, _comboAttackBuffer);
						transform.TurnScaleX(dashMovement);
						EffectsController.SoundEffect(_airJumpSound, transform.position);
						if (_comboAttackBuffer)
							EffectsController.SoundEffect(_attackSound, transform.position);
						while (!_isOnGround)
						{
							_originCast = new Vector2(Local.x + (_collider.bounds.extents.x + WorldBuild.SNAP_LENGTH / 2f) * dashMovement, Local.y);
							_sizeCast = new Vector2(WorldBuild.SNAP_LENGTH, _collider.size.y - WorldBuild.SNAP_LENGTH);
							_castHit = Physics2D.BoxCast(_originCast, _sizeCast, 0f, transform.right * dashMovement, WorldBuild.SNAP_LENGTH, WorldBuild.SceneMask);
							if (_castHit || _isJumping || _animator.GetBool(Stun) || _animator.GetBool(Death) || _airJumpMethod is null)
								break;
							_lastGroundedTime = _jumpCoyoteTime;
							yield return null;
						}
						_animator.SetBool(AirJump, (_airJumpMethod = null) is not null);
						_animator.SetBool(AttackAirJump, false);
					}
				}
				else if (movement.ReadValue<Vector2>().y < -0.25f && !_animator.GetBool(DashSlide) && _isOnGround)
				{
					_dashSlideMethod = DashSlideMethod(_movementAction);
					IEnumerator DashSlideMethod(float dashMovement)
					{
						_animator.SetBool(DashSlide, true);
						_animator.SetBool(AttackSlide, _comboAttackBuffer);
						transform.TurnScaleX(dashMovement);
						_jokerValue.z = transform.position.x;
						EffectsController.SoundEffect(_dashSlideSound, transform.position);
						if (_comboAttackBuffer)
							EffectsController.SoundEffect(_attackSound, transform.position);
						while (Mathf.Abs(transform.position.x - _jokerValue.z) < _dashDistance)
						{
							_originCast = new Vector2(Local.x + (_collider.bounds.extents.x + WorldBuild.SNAP_LENGTH / 2f) * dashMovement, Local.y);
							_sizeCast = new Vector2(WorldBuild.SNAP_LENGTH, _collider.size.y - WorldBuild.SNAP_LENGTH);
							_jokerValue = new Vector3(transform.right.x * dashMovement, transform.right.y * dashMovement, _jokerValue.z);
							_castHit = Physics2D.BoxCast(_originCast, _sizeCast, 0f, _jokerValue, WorldBuild.SNAP_LENGTH, WorldBuild.SceneMask);
							if (_castHit || !_isOnGround || _isJumping || _animator.GetBool(Stun) || _animator.GetBool(Death) || _dashSlideMethod is null)
								break;
							_rigidbody.linearVelocityX = _dashSpeed * dashMovement;
							yield return null;
						}
						_animator.SetBool(DashSlide, (_dashSlideMethod = null) is not null);
						_animator.SetBool(AttackSlide, false);
					}
			}
		};
		private void FootStepSound(float stepPositionX)
		{
			EffectsController.SoundEffect(_stepSound, new Vector2(transform.position.x + stepPositionX, transform.position.y - _collider.bounds.extents.y - WorldBuild.SNAP_LENGTH));
			EffectsController.SurfaceSound((Vector2)transform.position + new Vector2(transform.position.x + stepPositionX, transform.position.y - _collider.bounds.extents.y - WorldBuild.SNAP_LENGTH));
		}
		private Action<InputAction.CallbackContext> JumpInput => jump =>
		{
			if (jump.started)
			{
				_lastJumpTime = _jumpBufferTime;
				if (!_isOnGround && !_hopActive && SceneManager.GetActiveScene().name != _hubbyWorldScene)
				{
					_hopActive = true;
					if ((_bunnyHopBoost += 1) >= _gwambaCanvas.BunnyHop.Length)
						_bunnyHopBoost = (ushort)_gwambaCanvas.BunnyHop.Length;
				}
			}
			else if (jump.canceled && _isJumping && _rigidbody.linearVelocityY > 0f)
			{
				(_isJumping, _lastJumpTime) = (false, 0f);
				_rigidbody.AddForceY(_rigidbody.linearVelocityY * _jumpCut * -_rigidbody.mass, ForceMode2D.Impulse);
			}
		};
		private Action<InputAction.CallbackContext> AttackUseInput => attackUse =>
		{
			if ((_attackDelay > 0f && !_comboAttackBuffer) || _animator.GetBool(AirJump) || _animator.GetBool(DashSlide) || !isActiveAndEnabled || _animator.GetBool(Stun))
				return;
			if (attackUse.started && !_attackUsage)
				_animator.SetTrigger(Attack);
			if (attackUse.canceled && _comboAttackBuffer)
				_animator.SetTrigger(AttackCombo);
		};
		private void StartAttackSound() => EffectsController.SoundEffect(_attackSound, transform.position);
		private Action<InputAction.CallbackContext> InteractionInput => interaction =>
		{
			if (!_isOnGround || _movementAction != 0f || !isActiveAndEnabled || _animator.GetBool(AirJump) || _animator.GetBool(DashSlide) || _animator.GetBool(Stun))
				return;
			LayerMask mask = WorldBuild.SystemMask + WorldBuild.CharacterMask + WorldBuild.SceneMask + WorldBuild.ItemMask;
			foreach (Collider2D collider in Physics2D.OverlapBoxAll(Local, _collider.size, transform.eulerAngles.z, mask))
				if (collider.TryGetComponent<IInteractable>(out _))
				{
					foreach (IInteractable interactable in collider.GetComponents<IInteractable>())
						interactable.Interaction();
					return;
				}
		};
		public Predicate<ushort> DamagerHurt => damage =>
		{
			if (_invencibility || damage <= 0f)
				return false;
			EffectsController.SoundEffect(_hurtSound, transform.position);
			_vitality -= (short)damage;
			for (ushort i = (ushort)_gwambaCanvas.Vitality.Length; i > (_vitality >= 0f ? _vitality : 0f); i--)
			{
				_gwambaCanvas.Vitality[i - 1].style.backgroundColor = new StyleColor(_gwambaCanvas.MissingColor);
				_gwambaCanvas.Vitality[i - 1].style.borderBottomColor = new StyleColor(_gwambaCanvas.MissingColor);
				_gwambaCanvas.Vitality[i - 1].style.borderLeftColor = new StyleColor(_gwambaCanvas.MissingColor);
				_gwambaCanvas.Vitality[i - 1].style.borderRightColor = new StyleColor(_gwambaCanvas.MissingColor);
				_gwambaCanvas.Vitality[i - 1].style.borderTopColor = new StyleColor(_gwambaCanvas.MissingColor);
			}
			(_timerOfInvencibility, _invencibility) = (_invencibilityTime, true);
			if (_vitality <= 0f)
			{
				EffectsController.SoundEffect(_deathSound, transform.position);
				SaveController.Load(out SaveFile saveFile);
				_gwambaCanvas.LifeText.text = $"X {saveFile.Lifes -= 1}";
				SaveController.WriteSave(saveFile);
				_invencibility = false;
				foreach (GwambaDamager gwambaDamager in _gwambaDamagers)
					gwambaDamager.Alpha = 1f;
				OnDisable();
				_animator.SetBool(Idle, false);
				_animator.SetBool(Walk, false);
				_animator.SetBool(Jump, false);
				_animator.SetBool(Fall, false);
				_animator.SetBool(AttackJump, false);
				_animator.SetBool(Stun, false);
				_animator.SetBool(Death, true);
				_rigidbody.gravityScale = _fallGravityMultiply * _gravityScale;
				_sender.SetToggle(false);
				_sender.SetFormat(MessageFormat.State);
				_sender.Send(MessagePath.Hud);
				_sender.SetFormat(MessageFormat.Event);
				_sender.Send(MessagePath.Hud);
				_sender.SetFormat(MessageFormat.None);
				_sender.Send(MessagePath.Enemy);
				return true;
			}
			return true;
		};
		public UnityAction<ushort, float> DamagerStun => (stunStrength, stunTime) =>
		{
			_stunResistance -= (short)stunStrength;
			for (ushort i = (ushort)_gwambaCanvas.StunResistance.Length; i > (_stunResistance >= 0f ? _stunResistance : 0f); i--)
				_gwambaCanvas.StunResistance[i - 1].style.backgroundColor = new StyleColor(_gwambaCanvas.MissingColor);
			if (_stunResistance <= 0f && !_animator.GetBool(Death))
			{
				_animator.SetBool(Stun, !(_invencibility = false));
				foreach (GwambaDamager gwambaDamager in _gwambaDamagers)
					gwambaDamager.Alpha = 1f;
				_stunTimer = stunTime;
				for (ushort i = 0; i < (_stunResistance = (short)_gwambaCanvas.StunResistance.Length); i++)
					_gwambaCanvas.StunResistance[i].style.backgroundColor = new StyleColor(_gwambaCanvas.StunResistanceColor);
				EffectsController.SoundEffect(_stunSound, transform.position);
				DisableInputs();
			}
		};
		private UnityAction<GwambaDamager, IDestructible> DamagerAttack => (gwambaDamager, destructible) =>
		{
			if (destructible.Hurt(gwambaDamager.AttackDamage))
			{
				EffectsController.SoundEffect(_damageAttackSound, gwambaDamager.transform.position);
				destructible.Stun(gwambaDamager.AttackDamage, gwambaDamager.StunTime);
				_screenShaker.ImpulseDefinition.ImpulseDuration = gwambaDamager.AttackShakeTime;
				_screenShaker.GenerateImpulse(gwambaDamager.AttackShake);
				EffectsController.HitStop(_hitStopTime, _hitSlowTime);
				gwambaDamager.DamagerDamaged.Add(destructible);
				_attackDelay = _delayAfterAttack;
				for (ushort amount = 0; amount < (destructible.Health <= 0f ? gwambaDamager.AttackDamage + 1f : gwambaDamager.AttackDamage); amount++)
					if (_recoverVitality >= _gwambaCanvas.RecoverVitality.Length && _vitality < _gwambaCanvas.Vitality.Length)
					{
						_vitality += 1;
						for (ushort i = 0; i < _vitality; i++)
						{
							_gwambaCanvas.Vitality[i].style.backgroundColor = new StyleColor(_gwambaCanvas.BackgroundColor);
							_gwambaCanvas.Vitality[i].style.borderBottomColor = new StyleColor(_gwambaCanvas.BorderColor);
							_gwambaCanvas.Vitality[i].style.borderLeftColor = new StyleColor(_gwambaCanvas.BorderColor);
							_gwambaCanvas.Vitality[i].style.borderRightColor = new StyleColor(_gwambaCanvas.BorderColor);
							_gwambaCanvas.Vitality[i].style.borderTopColor = new StyleColor(_gwambaCanvas.BorderColor);
						}
						for (ushort i = _recoverVitality = 0; i < _gwambaCanvas.RecoverVitality.Length; i++)
							_gwambaCanvas.RecoverVitality[i].style.backgroundColor = new StyleColor(_gwambaCanvas.MissingColor);
						_stunResistance = (short)(_stunResistance < _gwambaCanvas.StunResistance.Length ? _stunResistance + 1f : _stunResistance);
						for (ushort i = 0; i < _stunResistance; i++)
							_gwambaCanvas.StunResistance[i].style.backgroundColor = new StyleColor(_gwambaCanvas.StunResistanceColor);
					}
					else if (_recoverVitality < _gwambaCanvas.RecoverVitality.Length)
					{
						_recoverVitality += 1;
						for (ushort i = 0; i < _recoverVitality; i++)
							_gwambaCanvas.RecoverVitality[i].style.backgroundColor = new StyleColor(_gwambaCanvas.BorderColor);
					}
			}
		};
		private void Update()
		{
			if (_invencibility)
			{
				_invencibility = (_timerOfInvencibility -= Time.deltaTime) > 0f;
				if (_invencibility && (_showInvencibilityTimer -= Time.deltaTime) <= 0f)
				{
					foreach (GwambaDamager gwambaDamager in _gwambaDamagers)
						gwambaDamager.Alpha = gwambaDamager.Alpha >= 1f ? _invencibilityValue : 1f;
					_showInvencibilityTimer = _timeStep;
				}
				if (!_invencibility)
					foreach (GwambaDamager gwambaDamager in _gwambaDamagers)
						gwambaDamager.Alpha = 1f;
			}
			if (_animator.GetBool(Stun))
				if ((_stunTimer -= Time.deltaTime) <= 0f)
				{
					_animator.SetBool(Stun, !(_invencibility = true));
					EnableInputs();
				}
			if (_fadeTimer > 0f)
				if ((_fadeTimer -= Time.deltaTime) <= 0f)
					(_gwambaCanvas.FallDamageText.style.opacity, _gwambaCanvas.FallDamageText.text) = (0f, $"X 0");
			if (!_animator.GetBool(DashSlide) && !_isOnGround && Mathf.Abs(_rigidbody.linearVelocityY) != 0f && !_downStairs && (_lastGroundedTime > 0f || _lastJumpTime > 0f))
				(_lastGroundedTime, _lastJumpTime) = (_lastGroundedTime - Time.deltaTime, _lastJumpTime - Time.deltaTime);
			if (_attackDelay > 0f)
				if ((_attackDelay -= Time.deltaTime) <= 0f)
					foreach (GwambaDamager gwambaDamager in _gwambaDamagers)
						gwambaDamager.DamagerDamaged.Clear();
		}
		private float BunnyHop(float callBackValue) => _bunnyHopBoost > 0f ? _bunnyHopBoost * callBackValue : 1f;
		private void FixedUpdate()
		{
			if (!_instance || _instance != this)
				return;
			if (_dashSlideMethod is not null)
				_dashSlideMethod.MoveNext();
			else
			{
				if (_isOnGround)
				{
					if (_movementAction == 0f || (_animator.GetBool(Fall) || Mathf.Abs(_rigidbody.linearVelocityX) <= 1e-3f))
						_animator.SetBool(Idle, true);
					else if (_animator.GetBool(Idle))
						_animator.SetBool(Idle, false);
					if (!_animator.GetBool(Walk) && _movementAction != 0f)
						_animator.SetBool(Walk, true);
					else if (_animator.GetBool(Walk) && _movementAction == 0f)
						_animator.SetBool(Walk, false);
					if (_animator.GetBool(Jump))
						_animator.SetBool(Jump, false);
					if (_animator.GetBool(Fall))
						_animator.SetBool(Fall, false);
					(_lastGroundedTime, _canAirJump, _bunnyHopBoost) = (_jumpCoyoteTime, !(_longJumping = _isJumping = false), _lastJumpTime > 0f ? _bunnyHopBoost : (ushort)0f);
					if (_bunnyHopBoost <= 0f && _isHoping)
					{
						_hopActive = _isHoping = false;
						for (ushort i = 0; i < _gwambaCanvas.BunnyHop.Length; i++)
							_gwambaCanvas.BunnyHop[i].style.backgroundColor = new StyleColor(_gwambaCanvas.MissingColor);
					}
					if (_fallDamage > 0f && _bunnyHopBoost <= 0f && SceneManager.GetActiveScene().name != _hubbyWorldScene)
					{
						_screenShaker.ImpulseDefinition.ImpulseDuration = _fallShakeTime;
						_screenShaker.GenerateImpulse(_fallDamage / _fallDamageDistance * _fallShake);
						DamagerHurt.Invoke((ushort)Mathf.FloorToInt(_fallDamage / _fallDamageDistance));
						(_fallStarted, _fallDamage) = (false, 0f);
						if (_invencibility && _fadeTimer <= 0f)
							_fadeTimer = _timeToFadeShow;
						else
							(_gwambaCanvas.FallDamageText.style.opacity, _gwambaCanvas.FallDamageText.text) = (0f, $"X 0");
					}
					if (_movementAction != 0f)
					{
						_originCast = new Vector2(Local.x + (_collider.bounds.extents.x + WorldBuild.SNAP_LENGTH / 2f) * _movementAction, Local.y - _bottomCheckerOffset);
						_sizeCast = new Vector2(WorldBuild.SNAP_LENGTH, 1f - WorldBuild.SNAP_LENGTH);
						if (_castHit = Physics2D.BoxCast(_originCast, _sizeCast, 0f, transform.right * _movementAction, WorldBuild.SNAP_LENGTH, WorldBuild.SceneMask))
						{
							_originCast = new Vector2(Local.x + (_collider.bounds.extents.x + WorldBuild.SNAP_LENGTH / 2f) * _movementAction, Local.y + 5e-1f);
							_sizeCast = new Vector2(WorldBuild.SNAP_LENGTH, _topWallChecker - WorldBuild.SNAP_LENGTH);
							if (!Physics2D.BoxCast(_originCast, _sizeCast, 0f, transform.right * _movementAction, WorldBuild.SNAP_LENGTH, WorldBuild.SceneMask))
							{
								_originCast = new Vector2(Local.x + (_collider.bounds.extents.x + WorldBuild.SNAP_LENGTH) * _movementAction, Local.y + _collider.bounds.extents.y);
								_sizeCast = new Vector2(Local.x + (_collider.bounds.extents.x + WorldBuild.SNAP_LENGTH) * _movementAction, Local.y - _collider.bounds.extents.y);
								foreach (RaycastHit2D lineCast in Physics2D.LinecastAll(_originCast, _sizeCast, WorldBuild.SceneMask))
									if (lineCast.collider == _castHit.collider)
									{
										_jokerValue.x = Mathf.Abs(lineCast.point.y - (transform.position.y - _collider.bounds.extents.y));
										transform.position = new Vector2(transform.position.x + WorldBuild.SNAP_LENGTH * _movementAction, transform.position.y + _jokerValue.x);
										_rigidbody.linearVelocityX = _movementSpeed * _movementAction;
										break;
									}
							}
						}
					}
					else if (Mathf.Abs(_rigidbody.linearVelocityX) > 1e-3f)
					{
						_jokerValue.y = Mathf.Min(Mathf.Abs(_rigidbody.linearVelocityX), Mathf.Abs(_frictionAmount)) * Mathf.Sign(_rigidbody.linearVelocityX);
						_rigidbody.AddForceX(-_jokerValue.y * _rigidbody.mass, ForceMode2D.Impulse);
						_animator.SetFloat(WalkSpeed, Mathf.Abs(_rigidbody.linearVelocityX) / (_longJumping ? _dashSpeed : _movementSpeed + BunnyHop(_velocityBoost)));
					}
				}
				else if (Mathf.Abs(_rigidbody.linearVelocityY) > 1e-3f && !_animator.GetBool(AirJump))
				{
					if (_animator.GetBool(Idle))
						_animator.SetBool(Idle, false);
					if (_animator.GetBool(Walk))
						_animator.SetBool(Walk, false);
					if (!_animator.GetBool(Jump) && _rigidbody.linearVelocityY > 0f)
						_animator.SetBool(Jump, true);
					else if (_animator.GetBool(Jump) && _rigidbody.linearVelocityY < 0f)
						_animator.SetBool(Jump, false);
					if (!_animator.GetBool(Fall) && _rigidbody.linearVelocityY < 0f)
						_animator.SetBool(Fall, true);
					else if (_animator.GetBool(Fall) && _rigidbody.linearVelocityY > 0f)
						_animator.SetBool(Fall, false);
					if (_animator.GetBool(AttackJump) && _rigidbody.linearVelocityY < 0f)
						_animator.SetBool(AttackJump, false);
					if (_animator.GetBool(Fall))
					{
						if (_rigidbody.gravityScale < _fallGravityMultiply * _gravityScale)
							_rigidbody.gravityScale = _fallGravityMultiply * _gravityScale;
						if (_fallStarted)
						{
							_fallDamage = Mathf.Abs(_fallStart - transform.position.y);
							if (_fallDamage >= _fallDamageDistance * _fallDamageShowMultiply)
								(_gwambaCanvas.FallDamageText.style.opacity, _gwambaCanvas.FallDamageText.text) = (1f, $"X " + (_fallDamage / _fallDamageDistance).ToString("F1"));
							else if (!_invencibility)
								(_gwambaCanvas.FallDamageText.style.opacity, _gwambaCanvas.FallDamageText.text) = (0f, $"X 0");
						}
						else
							(_fallStarted, _fallStart, _fallDamage) = (true, transform.position.y, 0f);
					}
					else
					{
						if (!_invencibility)
							(_gwambaCanvas.FallDamageText.style.opacity, _gwambaCanvas.FallDamageText.text) = (0f, $"X 0");
						if (_rigidbody.gravityScale > _gravityScale)
							_rigidbody.gravityScale = _gravityScale;
						if (_fallDamage > 0f)
							_fallDamage = 0f;
					}
					if (_attackUsage)
						_rigidbody.linearVelocityY *= _attackVelocityCut;
				}
				_downStairs = false;
				if (!_isOnGround && _canDownStairs && _movementAction != 0f && _lastJumpTime <= 0f)
				{
					_jokerValue.x = 0f;
					do
					{
						_jokerValue.x += WorldBuild.SNAP_LENGTH;
						_originCast = new Vector2(Local.x - (_collider.bounds.extents.x - _jokerValue.x) * _movementAction, Local.y - _collider.bounds.extents.y);
						_castHit = Physics2D.Raycast(_originCast, -transform.up, 1f + WorldBuild.SNAP_LENGTH, WorldBuild.SceneMask);
						_downStairs = _castHit && Mathf.Round((transform.position.y - _collider.bounds.extents.y) * 10f) / 10f != Mathf.Round(_castHit.point.y * 10f) / 10f;
					}
					while (!_downStairs && _jokerValue.x < WorldBuild.SNAP_LENGTH * 6f);
					if (_downStairs)
						transform.position = new Vector2(transform.position.x + _jokerValue.x * _movementAction, transform.position.y - _castHit.distance);
				}
				if (_airJumpMethod is not null)
					_airJumpMethod.MoveNext();
				else
				{
					_jokerValue.x = _longJumping ? _dashSpeed : _movementSpeed + BunnyHop(_velocityBoost);
					_jokerValue.y = _jokerValue.x * _movementAction - _rigidbody.linearVelocityX;
					_jokerValue.z = (Mathf.Abs(_jokerValue.x * _movementAction) > 0f ? _acceleration : _decceleration) + BunnyHop(_potencyBoost);
					_rigidbody.AddForceX(Mathf.Pow(Mathf.Abs(_jokerValue.y) * _jokerValue.z, _velocityPower) * Mathf.Sign(_jokerValue.y) * _rigidbody.mass);
					if (_movementAction != 0f && !_attackUsage)
					{
						if (Mathf.Abs(_rigidbody.linearVelocityX) > 1e-3f)
							transform.TurnScaleX(_rigidbody.linearVelocityX < 0f);
						else if (Mathf.Abs(_rigidbody.linearVelocityX) <= 1e-3f)
							transform.TurnScaleX(_movementAction);
						if (_isOnGround)
							_animator.SetFloat(WalkSpeed, Mathf.Abs(_rigidbody.linearVelocityX) <= 1e-3f ? 1f : Mathf.Abs(_rigidbody.linearVelocityX) / _jokerValue.x);
					}
				}
				if (_attackUsage && !_animator.GetBool(AttackAirJump))
					_rigidbody.linearVelocityX *= _attackVelocityCut;
			}
			if (!_isJumping && _lastJumpTime > 0f && _lastGroundedTime > 0f)
			{
				_animator.SetBool(AttackJump, _comboAttackBuffer);
				(_isJumping, _longJumping, _rigidbody.gravityScale, _rigidbody.linearVelocityY) = (!(_hopActive = false), _animator.GetBool(DashSlide), _gravityScale, 0f);
				if (_bunnyHopBoost > 0f)
				{
					_isHoping = true;
					for (ushort i = 0; i < _bunnyHopBoost; i++)
						_gwambaCanvas.BunnyHop[i].style.backgroundColor = new StyleColor(_gwambaCanvas.BunnyHopColor);
				}
				_rigidbody.AddForceY((_jumpStrenght + BunnyHop(_jumpBoost)) * _rigidbody.mass, ForceMode2D.Impulse);
				EffectsController.SoundEffect(_jumpSound, transform.position);
				if (_comboAttackBuffer)
					EffectsController.SoundEffect(_attackSound, transform.position);
			}
			(_isOnGround, _canDownStairs) = (false, _isOnGround);
		}
		private void GroundCheck()
		{
			if (!_instance || _instance != this)
				return;
			_originCast = new Vector2(Local.x, Local.y + (_collider.bounds.extents.y + WorldBuild.SNAP_LENGTH / 2f) * -transform.up.y);
			_sizeCast = new Vector2(_collider.size.x - WorldBuild.SNAP_LENGTH, WorldBuild.SNAP_LENGTH);
			_isOnGround = Physics2D.BoxCast(_originCast, _sizeCast, 0f, -transform.up, WorldBuild.SNAP_LENGTH, WorldBuild.SceneMask);
		}
		private void OnCollisionEnter2D(Collision2D collision) => GroundCheck();
		private void OnCollisionStay2D(Collision2D collision) => GroundCheck();
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<ICollectable>(out var collectable))
			{
				collectable.Collect();
				SaveController.Load(out SaveFile saveFile);
				(_gwambaCanvas.LifeText.text, _gwambaCanvas.CoinText.text) = ($"X {saveFile.Lifes}", $"X {saveFile.Coins}");
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
		public void Receive(MessageData message)
		{
			if (message.Format == MessageFormat.Event && message.ToggleValue.HasValue)
				if (message.ToggleValue.Value)
					Reanimate();
				else if (message.AdditionalData is Vector2 position)
					transform.position = position;
			if (message.Format == MessageFormat.State && message.ToggleValue.HasValue && message.ToggleValue.Value)
			{
				(_timerOfInvencibility, _invencibility) = (_invencibilityTime, true);
				OnEnable();
			}
		}
	};
};
