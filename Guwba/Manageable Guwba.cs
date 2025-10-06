using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Unity.Cinemachine;
using System;
using System.Collections;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Animator), typeof(SortingGroup))]
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CircleCollider2D)), RequireComponent(typeof(CinemachineImpulseSource))]
	internal sealed class ManageableGuwba : StateController, IConnector
	{
		private static ManageableGuwba _instance;
		private VisualizableGuwba _visualizableGuwba;
		private DamageableGuwba[] _damageableGuwbas;
		private Animator _animator;
		private Rigidbody2D _rigidbody;
		private BoxCollider2D _collider;
		private CinemachineImpulseSource _screenShaker;
		private InputController _inputController;
		private readonly Sender _sender = Sender.Create();
		private Vector2 _normalOffset = new();
		private Vector2 _normalSize = new();
		private int _isOn;
		private int _idle;
		private int _walk;
		private int _walkSpeed;
		private int _dashSlide;
		private int _Jump;
		private int _fall;
		private int _attack;
		private int _attackCombo;
		private int _attackJump;
		private int _attackSlide;
		private int _stun;
		private int _death;
		private short _vitality;
		private ushort _recoverVitality = 0;
		private float _gravityScale = 0f;
		private float _movementAction = 0f;
		private float _yMovement = 0f;
		private float _dashMovement = 0f;
		private float _guardDashMovement = 0f;
		private float _lastGroundedTime = 0f;
		private float _lastJumpTime = 0f;
		private float _fallStart = 0f;
		private float _fallDamage = 0f;
		private bool _isOnGround = false;
		private bool _downStairs = false;
		private bool _isJumping = false;
		private bool _dashActive = false;
		private bool _fallStarted = false;
		private bool _invencibility = false;
		[Header("World Interaction")]
		[SerializeField, Tooltip("The layer mask that Guwba identifies the ground.")] private LayerMask _groundLayer;
		[SerializeField, Tooltip("The layer mask that Guwba identifies a interactive object.")] private LayerMask _InteractionLayer;
		[Header("Visual Interaction")]
		[SerializeField, Tooltip("The object of the Guwba hud.")] private VisualizableGuwba _visualizableGuwbaObject;
		[SerializeField, Tooltip("The name of the scene that contains the hubby world.")] private string _hubbyWorldScene;
		[SerializeField, Tooltip("The amount of time that Guwba gets invencible.")] private float _invencibilityTime;
		[SerializeField, Tooltip("The value applied to visual when a hit is taken.")] private float _invencibilityValue;
		[SerializeField, Tooltip("The amount of time that the has to stay before fade.")] private float _timeStep;
		[SerializeField, Tooltip("The amount of time taht Guwba will be stunned after recover.")] private float _stunnedTime;
		[SerializeField, Tooltip("The amount of stun that Guwba can resists.")] private ushort _stunResistance;
		[Header("Animation")]
		[SerializeField, Tooltip("Animation parameter.")] private string _isOnAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _idleAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _walkAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _walkSpeedAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _dashSlideAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _JumpAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _fallAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _attackAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _attackComboAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _attackJumpAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _attackSlideAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _stunAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _deathAnimation;
		[Header("Physics Stats")]
		[SerializeField, Tooltip("Size of collider for checking the ground below the feet.")] private float _groundChecker;
		[SerializeField, Tooltip("Size of collider for checking the wall to climb stairs.")] private float _wallChecker;
		[SerializeField, Tooltip("Size of top part of the wall collider to climb stairs.")] private float _topWallChecker;
		[SerializeField, Tooltip("Offset of bottom part of the wall collider to climb stairs.")] private float _bottomCheckerOffset;
		[SerializeField, Tooltip("The amount of gravity to multiply on the fall.")] private float _fallGravityMultiply;
		[SerializeField, Tooltip("The amount of fall's distance to take damage.")] private float _fallDamageDistance;
		[SerializeField, Tooltip("The amount of time to fade the show of fall's damage.")] private float _timeToFadeShow;
		[SerializeField, Range(0f, 1f), Tooltip("The amount of fall's distance to start show the fall damage.")]
		private float _fallDamageShowMultiply;
		[SerializeField, Range(0f, 1f), Tooltip("The amount of velocity to cut during the attack.")] private float _attackVelocityCut;
		[Header("Movement")]
		[SerializeField, Tooltip("The amount of speed that Guwba moves yourself.")] private float _movementSpeed;
		[SerializeField, Tooltip("The amount of acceleration Guwba will apply to the Movement.")] private float _acceleration;
		[SerializeField, Tooltip("The amount of decceleration Guwba will apply to the Movement.")] private float _decceleration;
		[SerializeField, Tooltip("The amount of power the velocity Guwba will apply to the Movement.")] private float _velocityPower;
		[SerializeField, Tooltip("The amount of friction Guwba will apply to the end of Movement.")] private float _frictionAmount;
		[SerializeField, Tooltip("The amount of speed in both dashes.")] private float _dashSpeed;
		[SerializeField, Tooltip("The amount of distance Guwba will go in both dashes.")] private float _dashDistance;
		[SerializeField, Tooltip("If Guwba will look firstly to the left.")] private bool _turnLeft;
		[Header("Jump")]
		[SerializeField, Tooltip("The amount of strenght that Guwba can Jump.")] private float _jumpStrenght;
		[SerializeField, Tooltip("The amount of time that Guwba can Jump before thouching ground.")] private float _jumpBufferTime;
		[SerializeField, Tooltip("The amount of time that Guwba can Jump when get out of the ground.")] private float _jumpCoyoteTime;
		[SerializeField, Range(0f, 1f), Tooltip("The amount of cut that Guwba's jump will suffer at up.")] private float _jumpCut;
		[Header("Attack")]
		[SerializeField, Tooltip("The amount of time to stop the game when hit is given.")] private float _hitStopTime;
		[SerializeField, Tooltip("The amount of time to slow the game when hit is given.")] private float _hitSlowTime;
		[SerializeField, Tooltip("The amount of time the recover's charge of the attack will recover.")] private float _recoverRate;
		[SerializeField, Tooltip("If Guwba is attacking in the moment.")] private bool _attackUsage;
		[SerializeField, Tooltip("The buffer moment that Guwba have to execute a combo attack.")] private bool _comboAttackBuffer;
		public PathConnection PathConnection => PathConnection.Guwba;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			this._animator = this.GetComponent<Animator>();
			this._rigidbody = this.GetComponent<Rigidbody2D>();
			this._collider = this.GetComponent<BoxCollider2D>();
			this._screenShaker = this.GetComponent<CinemachineImpulseSource>();
			this._visualizableGuwba = Instantiate(this._visualizableGuwbaObject, this.transform);
			this._damageableGuwbas = this.GetComponentsInChildren<DamageableGuwba>(true);
			SaveController.Load(out SaveFile saveFile);
			this._visualizableGuwba.LifeText.text = $"X {saveFile.lifes}";
			this._visualizableGuwba.CoinText.text = $"X {saveFile.coins}";
			this._vitality = (short)this._visualizableGuwba.Vitality;
			foreach (DamageableGuwba damageableGuwba in this._damageableGuwbas)
			{
				damageableGuwba.DamageableHurt += this.Hurt;
				damageableGuwba.DamageableStun += this.Stun;
				damageableGuwba.DamageableAttack += this.Attack;
			}
			this._isOn = Animator.StringToHash(this._isOnAnimation);
			this._idle = Animator.StringToHash(this._idleAnimation);
			this._walk = Animator.StringToHash(this._walkAnimation);
			this._walkSpeed = Animator.StringToHash(this._walkSpeedAnimation);
			this._dashSlide = Animator.StringToHash(this._dashSlideAnimation);
			this._Jump = Animator.StringToHash(this._JumpAnimation);
			this._fall = Animator.StringToHash(this._fallAnimation);
			this._attack = Animator.StringToHash(this._attackAnimation);
			this._attackCombo = Animator.StringToHash(this._attackComboAnimation);
			this._attackJump = Animator.StringToHash(this._attackJumpAnimation);
			this._attackSlide = Animator.StringToHash(this._attackSlideAnimation);
			this._stun = Animator.StringToHash(this._stunAnimation);
			this._death = Animator.StringToHash(this._deathAnimation);
			this.transform.localScale = new Vector3()
			{
				x = this._turnLeft ? -Mathf.Abs(this.transform.localScale.x) : Mathf.Abs(this.transform.localScale.x),
				y = this.transform.localScale.y,
				z = this.transform.localScale.z
			};
			this._gravityScale = this._rigidbody.gravityScale;
			this._normalOffset = this._collider.offset;
			this._normalSize = this._collider.size;
			if (this.gameObject.scene.name == this._hubbyWorldScene)
			{
				foreach (VisualElement vitality in this._visualizableGuwba.VitalityVisual)
					vitality.style.display = DisplayStyle.None;
				foreach (VisualElement recoverVitality in this._visualizableGuwba.RecoverVitalityVisual)
					recoverVitality.style.display = DisplayStyle.None;
				this._visualizableGuwba.FallDamageText.style.display = DisplayStyle.None;
			}
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!_instance || _instance != this)
				return;
			this.StopAllCoroutines();
			foreach (DamageableGuwba damageableGuwba in this._damageableGuwbas)
			{
				damageableGuwba.DamageableHurt -= this.Hurt;
				damageableGuwba.DamageableStun -= this.Stun;
				damageableGuwba.DamageableAttack -= this.Attack;
				damageableGuwba.Alpha = 1f;
			}
			Sender.Exclude(this);
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			this._visualizableGuwba.RootElement.style.display = DisplayStyle.Flex;
			this._animator.SetFloat(this._isOn, 1f);
			this._animator.SetFloat(this._walkSpeed, 1f);
			this.EnableCommands();
			if (this._dashActive)
				this._dashMovement = this._guardDashMovement;
			this._rigidbody.gravityScale = this._gravityScale;
			this._rigidbody.linearVelocityY = this._yMovement;
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			this._visualizableGuwba.RootElement.style.display = DisplayStyle.None;
			this._animator.SetFloat(this._isOn, 0f);
			this._animator.SetFloat(this._walkSpeed, 0f);
			this.DisableCommands();
			this._movementAction = 0f;
			if (this._dashActive)
			{
				this._guardDashMovement = this._dashMovement;
				this._dashMovement = 0f;
			}
			this._yMovement = this._rigidbody.linearVelocityY;
			this._rigidbody.gravityScale = 0f;
			this._rigidbody.linearVelocity = Vector2.zero;
		}
		private void EnableCommands()
		{
			this._inputController = new InputController();
			this._inputController.Commands.Movement.started += this.Movement;
			this._inputController.Commands.Movement.performed += this.Movement;
			this._inputController.Commands.Movement.canceled += this.Movement;
			this._inputController.Commands.AttackUse.started += this.AttackUse;
			this._inputController.Commands.AttackUse.canceled += this.AttackUse;
			this._inputController.Commands.Interaction.started += this.Interaction;
			this._inputController.Commands.Movement.Enable();
			this._inputController.Commands.AttackUse.Enable();
			this._inputController.Commands.Interaction.Enable();
		}
		private void DisableCommands()
		{
			this._inputController.Commands.Movement.started -= this.Movement;
			this._inputController.Commands.Movement.performed -= this.Movement;
			this._inputController.Commands.Movement.canceled -= this.Movement;
			this._inputController.Commands.AttackUse.started -= this.AttackUse;
			this._inputController.Commands.AttackUse.canceled -= this.AttackUse;
			this._inputController.Commands.Interaction.started -= this.Interaction;
			this._inputController.Commands.Movement.Disable();
			this._inputController.Commands.AttackUse.Disable();
			this._inputController.Commands.Interaction.Disable();
			this._inputController.Dispose();
		}
		private Action<InputAction.CallbackContext> Movement => movementAction =>
		{
			Vector2 movementValue = movementAction.ReadValue<Vector2>();
			this._movementAction = movementValue.x;
			if (Mathf.Abs(movementValue.x) > 0.5f)
				if (movementValue.x > 0f)
					this._movementAction = 1f;
				else if (movementValue.x < 0f)
					this._movementAction = -1f;
			if (movementValue.y > 0.25f)
				this._lastJumpTime = this._jumpBufferTime;
			if (this._isJumping && this._rigidbody.linearVelocityY > 0f && movementValue.y < 0.25f)
			{
				this._isJumping = false;
				this._rigidbody.AddForceY(this._rigidbody.linearVelocityY * this._jumpCut * -this._rigidbody.mass, ForceMode2D.Impulse);
				this._lastJumpTime = 0f;
			}
			bool valid = !this._dashActive && this._isOnGround && (!this._attackUsage || this._comboAttackBuffer);
			if (this._movementAction != 0f && movementValue.y < -0.5f && valid)
			{
				this.StartCoroutine(Dash());
				IEnumerator Dash()
				{
					if (this._comboAttackBuffer)
						this._animator.SetBool(this._attackSlide, true);
					this._animator.SetBool(this._dashSlide, this._dashActive = true);
					this._dashMovement = this._movementAction;
					this.transform.localScale = new Vector3()
					{
						x = this._dashMovement * Mathf.Abs(this.transform.localScale.x),
						y = this.transform.localScale.y,
						z = this.transform.localScale.z
					};
					Vector2 direction = this.transform.right * this._dashMovement;
					float dashLocation = this.transform.position.x;
					bool isActive = true;
					bool onWall = false;
					while (isActive || onWall)
					{
						float xAxisSize = this._collider.bounds.extents.x + this._wallChecker / 2f;
						float xAxisPosition = this._collider.offset.x + xAxisSize * this._dashMovement;
						Vector2 point = new(this.transform.position.x + xAxisPosition, this.transform.position.y + this._collider.offset.y);
						Vector2 size = new(this._wallChecker, this._collider.size.y - this._wallChecker);
						float angle = this.transform.eulerAngles.z;
						RaycastHit2D blockRay = Physics2D.BoxCast(point, size, angle, direction, this._wallChecker, this._groundLayer);
						bool block = blockRay && blockRay.collider.TryGetComponent<Surface>(out var blockSurface) && blockSurface.IsScene;
						this._rigidbody.linearVelocityX = this._dashSpeed * this._dashMovement;
						bool valid = Mathf.Abs(this.transform.position.x - dashLocation) >= this._dashDistance;
						float yPoint = this.transform.position.y + this._normalOffset.y + this._wallChecker;
						Vector2 wallPoint = new(this.transform.position.x + this._normalOffset.x, yPoint);
						Vector2 wallSize = this._normalSize;
						Vector2 upDirection = this.transform.up;
						RaycastHit2D wallRay = Physics2D.BoxCast(wallPoint, wallSize, angle, upDirection, this._wallChecker, this._groundLayer);
						onWall = wallRay && wallRay.collider.TryGetComponent<Surface>(out var wallSurface) && wallSurface.IsScene;
						if (!onWall && (valid || !this._dashActive || block || !this._isOnGround))
						{
							this._animator.SetBool(this._attackSlide, false);
							this._animator.SetBool(this._dashSlide, isActive = this._dashActive = false);
						}
						yield return new WaitForFixedUpdate();
						yield return new WaitUntil(() => this.enabled);
					}
				}
			}
		};
		private Action<InputAction.CallbackContext> AttackUse => attackAction =>
		{
			if (this._dashActive)
				return;
			if (attackAction.started && !this._attackUsage)
				this._animator.SetTrigger(this._attack);
			if (attackAction.canceled && this._comboAttackBuffer)
				this._animator.SetTrigger(this._attackCombo);
		};
		private Action<InputAction.CallbackContext> Interaction => InteractionAction =>
		{
			if (this._isOnGround && this._movementAction == 0f)
			{
				Vector2 point = this.transform.position;
				float angle = this.transform.eulerAngles.z;
				foreach (Collider2D collider in Physics2D.OverlapBoxAll(point, this._normalSize, angle, this._InteractionLayer))
					if (collider.TryGetComponent<IInteractable>(out _))
					{
						foreach (IInteractable interactable in collider.GetComponents<IInteractable>())
							interactable.Interaction();
						return;
					}
			}
		};
		public Predicate<ushort> Hurt => damage =>
		{
			if (this._invencibility || damage < 1f)
				return false;
			this._invencibility = true;
			this._vitality -= (short)damage;
			for (ushort i = (ushort)this._visualizableGuwba.VitalityVisual.Length; i > (this._vitality >= 0f ? this._vitality : 0f); i--)
			{
				Color missingColor = this._visualizableGuwba.MissingVitalityColor;
				this._visualizableGuwba.VitalityVisual[i - 1].style.backgroundColor = new StyleColor(missingColor);
				this._visualizableGuwba.VitalityVisual[i - 1].style.borderBottomColor = new StyleColor(missingColor);
				this._visualizableGuwba.VitalityVisual[i - 1].style.borderLeftColor = new StyleColor(missingColor);
				this._visualizableGuwba.VitalityVisual[i - 1].style.borderRightColor = new StyleColor(missingColor);
				this._visualizableGuwba.VitalityVisual[i - 1].style.borderTopColor = new StyleColor(missingColor);
			}
			if (this._vitality <= 0f)
			{
				SaveController.Load(out SaveFile saveFile);
				saveFile.lifes -= 1;
				this._visualizableGuwba.LifeText.text = $"X {saveFile.lifes}";
				SaveController.WriteSave(saveFile);
				this.StopAllCoroutines();
				foreach (DamageableGuwba damageableGuwba in this._damageableGuwbas)
					damageableGuwba.Alpha = 1f;
				this.OnDisable();
				this._animator.SetBool(this._death, true);
				this._rigidbody.gravityScale = this._gravityScale;
				this._sender.SetToggle(false);
				this._sender.SetStateForm(StateForm.Action);
				this._sender.Send(PathConnection.Hud);
				this._sender.SetStateForm(StateForm.State);
				this._sender.Send(PathConnection.Hud);
				this._sender.SetStateForm(StateForm.None);
				this._sender.Send(PathConnection.Enemy);
				return true;
			}
			this.StartCoroutine(this.Invencibility());
			return true;
		};
		public UnityAction<ushort, float> Stun => (stunStrength, stunTime) =>
		{
			if (this._stunResistance - stunStrength < 0f)
				this.StartCoroutine(StunTimer());
			IEnumerator StunTimer()
			{
				this._animator.SetBool(this._stun, true);
				this.DisableCommands();
				this._dashActive = false;
				yield return new WaitTime(this, stunTime);
				this._animator.SetBool(this._stun, false);
				this.EnableCommands();
			}
		};
		private UnityAction<DamageableGuwba, IDestructible> Attack => (damageableGuwba, destructible) =>
		{
			if (destructible.Hurt(damageableGuwba.AttackDamage))
			{
				destructible.Stun(damageableGuwba.AttackDamage, damageableGuwba.StunTime);
				EffectsController.HitStop(this._hitStopTime, this._hitSlowTime);
				this.StartCoroutine(RecoverVitality());
				IEnumerator RecoverVitality()
				{
					Color backgroundColor = this._visualizableGuwba.BackgroundColor;
					Color borderColor = this._visualizableGuwba.BorderColor;
					Color missingColor = this._visualizableGuwba.MissingVitalityColor;
					short damageDifference = (short)(damageableGuwba.AttackDamage - Mathf.Abs(destructible.Health));
					for (ushort amount = 0; amount < (destructible.Health >= 0f ? damageableGuwba.AttackDamage : damageDifference); amount++)
					{
						bool valid = this._vitality < this._visualizableGuwba.Vitality;
						if (this._recoverVitality >= this._visualizableGuwba.RecoverVitality && valid)
						{
							this._recoverVitality = 0;
							for (ushort i = 0; i < this._visualizableGuwba.RecoverVitality; i++)
								this._visualizableGuwba.RecoverVitalityVisual[i].style.backgroundColor = new StyleColor(missingColor);
							this._vitality += 1;
							for (ushort i = 0; i < this._vitality; i++)
							{
								this._visualizableGuwba.VitalityVisual[i].style.backgroundColor = new StyleColor(backgroundColor);
								this._visualizableGuwba.VitalityVisual[i].style.borderBottomColor = new StyleColor(borderColor);
								this._visualizableGuwba.VitalityVisual[i].style.borderLeftColor = new StyleColor(borderColor);
								this._visualizableGuwba.VitalityVisual[i].style.borderRightColor = new StyleColor(borderColor);
								this._visualizableGuwba.VitalityVisual[i].style.borderTopColor = new StyleColor(borderColor);
							}
						}
						else if (this._recoverVitality < this._visualizableGuwba.RecoverVitality)
						{
							this._recoverVitality += 1;
							for (ushort i = 0; i < this._recoverVitality; i++)
								this._visualizableGuwba.RecoverVitalityVisual[i].style.backgroundColor = new StyleColor(borderColor);
						}
						yield return new WaitTime(this, this._recoverRate);
					}
				}
			}
		};
		private void FixedUpdate()
		{
			float movementValue = this._movementAction != 0f ? this._movementAction > 0f ? 1f : -1f : 0f;
			Vector2 direction = this.transform.right * movementValue;
			float angle = this.transform.eulerAngles.z;
			float rootHeight = this._collider.size.y / this._collider.size.y;
			bool downStairs = false;
			if (!this._isOnGround && this._downStairs && this._movementAction != 0f && this._lastJumpTime <= 0f && !this._dashActive)
			{
				float xOrigin = this.transform.position.x - ((this._collider.bounds.extents.x - .025f) * movementValue);
				Vector2 downRayOrigin = new(xOrigin, this.transform.position.y - this._collider.bounds.extents.y);
				float distance = rootHeight + this._groundChecker;
				RaycastHit2D downRay = Physics2D.Raycast(downRayOrigin, -this.transform.up, distance, this._groundLayer);
				downStairs = downRay;
				if (downStairs)
					this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y - downRay.distance);
			}
			if (!this._dashActive)
				if (this._isOnGround)
				{
					this._animator.SetBool(this._idle, this._movementAction == 0f);
					this._animator.SetBool(this._walk, this._movementAction != 0f);
					this._animator.SetBool(this._Jump, false);
					this._animator.SetBool(this._fall, false);
					this._lastGroundedTime = this._jumpCoyoteTime;
					this._downStairs = true;
					this._isJumping = false;
					if (this._fallDamage > 0f)
					{
						this._screenShaker.GenerateImpulseWithForce(this._fallDamage / this._fallDamageDistance);
						this.Hurt.Invoke((ushort)Mathf.Floor(this._fallDamage / this._fallDamageDistance));
						this._fallStarted = false;
						this._fallDamage = 0f;
						if (this._invencibility)
							this.StartCoroutine(KeepShow());
						else
						{
							this._visualizableGuwba.FallDamageText.style.opacity = 0f;
							this._visualizableGuwba.FallDamageText.text = $"X 0";
						}
						IEnumerator KeepShow()
						{
							yield return new WaitTime(this, this._timeToFadeShow);
							this._visualizableGuwba.FallDamageText.style.opacity = 0f;
							this._visualizableGuwba.FallDamageText.text = $"X 0";
						}
					}
				}
				else if (this._rigidbody.linearVelocityY != 0f && !downStairs)
				{
					this._animator.SetBool(this._idle, false);
					this._animator.SetBool(this._walk, false);
					this._animator.SetBool(this._Jump, this._rigidbody.linearVelocityY > 0f);
					this._animator.SetBool(this._fall, this._rigidbody.linearVelocityY < 0f);
					if (this._animator.GetBool(this._attackJump))
						this._animator.SetBool(this._attackJump, this._rigidbody.linearVelocityY > 0f);
					if (this._animator.GetBool(this._fall))
					{
						this._rigidbody.gravityScale = this._fallGravityMultiply * this._gravityScale;
						if (this._fallStarted)
						{
							this._fallDamage = Mathf.Abs(this.transform.position.y - this._fallStart);
							if (this._fallDamage >= this._fallDamageDistance * this._fallDamageShowMultiply)
							{
								this._visualizableGuwba.FallDamageText.style.opacity = 1f;
								this._visualizableGuwba.FallDamageText.text = $"X {this._fallDamage / this._fallDamageDistance}";
							}
							else if (!this._invencibility)
							{
								this._visualizableGuwba.FallDamageText.style.opacity = 0f;
								this._visualizableGuwba.FallDamageText.text = $"X 0";
							}
						}
						else
						{
							this._fallStarted = true;
							this._fallStart = this.transform.position.y;
							this._fallDamage = 0f;
						}
					}
					else
					{
						if (!this._invencibility)
						{
							this._visualizableGuwba.FallDamageText.style.opacity = 0f;
							this._visualizableGuwba.FallDamageText.text = $"X 0";
						}
						this._rigidbody.gravityScale = this._gravityScale;
						this._fallDamage = 0f;
					}
					if (this._attackUsage)
						this._rigidbody.linearVelocityY *= this._attackVelocityCut;
					this._lastGroundedTime -= Time.fixedDeltaTime;
					this._lastJumpTime -= Time.fixedDeltaTime;
					this._downStairs = false;
				}
			if (!this._dashActive)
			{
				if (this._isOnGround && this._movementAction != 0f)
				{
					float xPosition = this.transform.position.x + (this._collider.bounds.extents.x + this._wallChecker / 2f) * movementValue;
					Vector2 topOrigin = new(xPosition, this.transform.position.y + rootHeight * .5f);
					Vector2 bottomOrigin = new(xPosition, this.transform.position.y - rootHeight * this._bottomCheckerOffset);
					Vector2 topSize = new(this._wallChecker, rootHeight * this._topWallChecker - this._wallChecker);
					Vector2 bottomSize = new(this._wallChecker, rootHeight - this._wallChecker);
					LayerMask layerMask = this._groundLayer;
					RaycastHit2D bottomCast = Physics2D.BoxCast(bottomOrigin, bottomSize, angle, direction, this._wallChecker, layerMask);
					bool topCast = !Physics2D.BoxCast(topOrigin, topSize, angle, direction, this._wallChecker, layerMask);
					float walkSpeed = Mathf.Abs(this._rigidbody.linearVelocityX) / this._movementSpeed;
					this._animator.SetFloat(this._walkSpeed, topCast ? walkSpeed : 1f);
					if (bottomCast && topCast)
					{
						float topCorner = this.transform.position.y + this._collider.bounds.extents.y;
						float bottomCorner = this.transform.position.y - this._collider.bounds.extents.y;
						Vector2 lineStart = new(xPosition + this._wallChecker / 2f * movementValue, topCorner);
						Vector2 lineEnd = new(xPosition + this._wallChecker / 2f * movementValue, bottomCorner);
						RaycastHit2D lineWallStep = Physics2D.Linecast(lineStart, lineEnd, this._groundLayer);
						if (lineWallStep && lineWallStep.collider == bottomCast.collider)
						{
							float xDistance = this.transform.position.x + this._wallChecker * movementValue;
							float yDistance = this.transform.position.y + (lineWallStep.point.y - bottomCorner);
							this.transform.position = new Vector2(xDistance, yDistance);
							this._rigidbody.linearVelocityX = this._movementSpeed * this._movementAction;
						}
					}
				}
				if (this._movementAction != 0f)
					this.transform.localScale = new Vector3()
					{
						x = this._movementAction * Mathf.Abs(this.transform.localScale.x),
						y = this.transform.localScale.y,
						z = this.transform.localScale.z
					};
				float xPoint = (this._collider.bounds.extents.x + this._groundChecker / 2f) * this._movementAction;
				Vector2 origin = new(this.transform.position.x + xPoint, this.transform.position.y);
				Vector2 size = new(this._wallChecker, this._collider.size.y - this._wallChecker);
				bool wallBlock = Physics2D.BoxCast(origin, size, angle, direction, this._wallChecker, this._groundLayer);
				this._animator.SetFloat(this._walkSpeed, wallBlock ? 1f : Mathf.Abs(this._rigidbody.linearVelocityX) / this._movementSpeed);
				float targetSpeed = this._movementSpeed * this._movementAction;
				float speedDiferrence = targetSpeed - this._rigidbody.linearVelocityX;
				float accelerationRate = Mathf.Abs(targetSpeed) > 0f ? this._acceleration : this._decceleration;
				float movement = Mathf.Pow(Mathf.Abs(speedDiferrence) * accelerationRate, this._velocityPower) * Mathf.Sign(speedDiferrence);
				this._rigidbody.AddForceX(movement * this._rigidbody.mass);
				if (this._attackUsage)
					this._rigidbody.linearVelocityX *= this._attackVelocityCut;
			}
			if (this._isOnGround && this._movementAction == 0f && !this._dashActive)
			{
				float frictionAmount = Mathf.Min(Mathf.Abs(this._rigidbody.linearVelocityX), Mathf.Abs(this._frictionAmount));
				frictionAmount *= Mathf.Sign(this._rigidbody.linearVelocityX);
				this._rigidbody.AddForceX(-frictionAmount * this._rigidbody.mass, ForceMode2D.Impulse);
			}
			if (!this._isJumping && this._lastJumpTime > 0f && this._lastGroundedTime > 0f)
			{
				if (this._comboAttackBuffer)
					this._animator.SetBool(this._attackJump, true);
				this._isJumping = true;
				this._rigidbody.gravityScale = this._gravityScale;
				this._rigidbody.linearVelocityY = 0f;
				this._rigidbody.AddForceY(this._jumpStrenght * this._rigidbody.mass, ForceMode2D.Impulse);
			}
			this._isOnGround = false;
		}
		private IEnumerator Invencibility()
		{
			this.StartCoroutine(VisualEffect());
			IEnumerator VisualEffect()
			{
				while (this._invencibility)
				{
					foreach (DamageableGuwba damageableGuwba in this._damageableGuwbas)
						damageableGuwba.Alpha = damageableGuwba.Alpha >= 1f ? this._invencibilityValue : 1f;
					yield return new WaitTime(this, this._timeStep);
				}
			}
			yield return new WaitTime(this, this._invencibilityTime);
			this._invencibility = false;
			foreach (DamageableGuwba damageableGuwba in this._damageableGuwbas)
				damageableGuwba.Alpha = 1f;
		}
		private void OnCollision()
		{
			float yDirection = this._collider.bounds.extents.y - this._groundChecker / 2f;
			float yPoint = this.transform.position.y + this._collider.offset.y + yDirection * -this.transform.up.y;
			Vector2 point = new(this.transform.position.x + this._collider.offset.x, yPoint);
			Vector2 size = new(this._collider.size.x - this._groundChecker, this._groundChecker);
			float angle = this.transform.eulerAngles.z;
			this._isOnGround = Physics2D.BoxCast(point, size, angle, -this.transform.up, this._groundChecker, this._groundLayer);
		}
		private void OnCollisionEnter2D(Collision2D other) => this.OnCollision();
		private void OnCollisionStay2D(Collision2D other) => this.OnCollision();
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<ICollectable>(out var collectable))
			{
				collectable.Collect();
				SaveController.Load(out SaveFile saveFile);
				this._visualizableGuwba.LifeText.text = $"X {saveFile.lifes}";
				this._visualizableGuwba.CoinText.text = $"X {saveFile.coins}";
			}
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue && data.ToggleValue.Value)
			{
				for (ushort i = 0; i < this._visualizableGuwba.VitalityVisual.Length; i++)
				{
					this._visualizableGuwba.VitalityVisual[i].style.backgroundColor = new StyleColor(this._visualizableGuwba.BackgroundColor);
					this._visualizableGuwba.VitalityVisual[i].style.borderBottomColor = new StyleColor(this._visualizableGuwba.BorderColor);
					this._visualizableGuwba.VitalityVisual[i].style.borderLeftColor = new StyleColor(this._visualizableGuwba.BorderColor);
					this._visualizableGuwba.VitalityVisual[i].style.borderRightColor = new StyleColor(this._visualizableGuwba.BorderColor);
					this._visualizableGuwba.VitalityVisual[i].style.borderTopColor = new StyleColor(this._visualizableGuwba.BorderColor);
				}
				for (ushort i = 0; i < this._visualizableGuwba.RecoverVitalityVisual.Length; i++)
				{
					Color missingColor = this._visualizableGuwba.MissingVitalityColor;
					this._visualizableGuwba.RecoverVitalityVisual[i].style.backgroundColor = new StyleColor(missingColor);
				}
				this._vitality = (short)this._visualizableGuwba.Vitality;
				this.transform.localScale = new Vector3()
				{
					x = this._turnLeft ? -Mathf.Abs(this.transform.localScale.x) : Mathf.Abs(this.transform.localScale.x),
					y = this.transform.localScale.y,
					z = this.transform.localScale.z
				};
				this._animator.SetBool(this._death, false);
				this._collider.size = this._normalSize;
			}
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue && data.ToggleValue.Value)
			{
				this._invencibility = true;
				this.StartCoroutine(this.Invencibility());
				this.OnEnable();
			}
		}
	};
};
