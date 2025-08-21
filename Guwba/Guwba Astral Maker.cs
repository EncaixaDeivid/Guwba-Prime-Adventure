using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using System;
using System.Collections;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Animator), typeof(SortingGroup))]
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CircleCollider2D))]
	internal sealed class GuwbaAstralMaker : StateController, IConnector, IDestructible
	{
		private static GuwbaAstralMaker _instance;
		private VisualizableGuwba _visualizableGuwba;
		private DamageableGuwba[] _damageableGuwbas;
		private Animator _animator;
		private Rigidbody2D _rigidbody;
		private BoxCollider2D _collider;
		private InputController _inputController;
		private readonly Sender _sender = Sender.Create();
		private Vector2 _redAxis = new();
		private Vector2 _normalSize = new();
		private short _vitality;
		private ushort _recoverVitality = 0;
		private float _gravityScale = 0f;
		private float _movementAction = 0f;
		private float _yMovement = 0f;
		private float _dashMovement = 0f;
		private float _guardDashMovement = 0f;
		private float _lastGroundedTime = 0f;
		private float _lastJumpTime = 0f;
		private bool _isDamaged = false;
		private bool _isOnGround = false;
		private bool _downStairs = false;
		private bool _isJumping = false;
		private bool _dashActive = false;
		[Header("World Interaction")]
		[SerializeField, Tooltip("The layer mask that Guwba identifies the ground.")] private LayerMask _groundLayerMask;
		[SerializeField, Tooltip("The layer mask that Guwba identifies a interactive object.")] private LayerMask _InteractionLayerMask;
		[SerializeField, Tooltip("Size of the collider in dash slide.")] private Vector2 _dashSlideSize;
		[SerializeField, Tooltip("Size of the collider in death.")] private Vector2 _deadSize;
		[Header("Visual Interaction")]
		[SerializeField, Tooltip("The object of the Guwba hud.")] private VisualizableGuwba _visualizableGuwbaObject;
		[SerializeField, Tooltip("The name of the hubby world scene.")] private string _levelSelectorScene;
		[SerializeField, Tooltip("The amount of time that Guwba gets invencible.")] private float _invencibilityTime;
		[SerializeField, Tooltip("The value applied to visual when a hit is taken.")] private float _invencibilityValue;
		[SerializeField, Tooltip("The amount of time that the has to stay before fade.")] private float _timeStep;
		[SerializeField, Tooltip("The amount of time taht Guwba will be stunned after recover.")] private float _stunnedTime;
		[SerializeField, Tooltip("The amount of stun that Guwba can resists.")] private ushort _stunResistance;
		[Header("Animation")]
		[SerializeField, Tooltip("Animation parameter.")] private string _isOn;
		[SerializeField, Tooltip("Animation parameter.")] private string _idle;
		[SerializeField, Tooltip("Animation parameter.")] private string _walk;
		[SerializeField, Tooltip("Animation parameter.")] private string _walkSpeed;
		[SerializeField, Tooltip("Animation parameter.")] private string _dashSlide;
		[SerializeField, Tooltip("Animation parameter.")] private string _Jump;
		[SerializeField, Tooltip("Animation parameter.")] private string _fall;
		[SerializeField, Tooltip("Animation parameter.")] private string _attack;
		[SerializeField, Tooltip("Animation parameter.")] private string _attackCombo;
		[SerializeField, Tooltip("Animation parameter.")] private string _attackJump;
		[SerializeField, Tooltip("Animation parameter.")] private string _attackSlide;
		[SerializeField, Tooltip("Animation parameter.")] private string _stun;
		[SerializeField, Tooltip("Animation parameter.")] private string _death;
		[Header("Physics Stats")]
		[SerializeField, Tooltip("Size of collider for checking the ground below the feet.")] private float _groundChecker;
		[SerializeField, Tooltip("Size of collider for checking the wall to climb stairs.")] private float _wallChecker;
		[SerializeField, Tooltip("Size of top part of the wall collider to climb stairs.")] private float _topWallChecker;
		[SerializeField, Tooltip("Offset of bottom part of the wall collider to climb stairs.")] private float _bottomCheckerOffset;
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
		[SerializeField, Tooltip("The amount of gravity to increase the fall.")] private float _fallGravityMultiply;
		[Header("Attack")]
		[SerializeField, Tooltip("The amount of time to stop the game when hit is given.")] private float _hitStopTime;
		[SerializeField, Tooltip("The amount of time to slow the game when hit is given.")] private float _hitSlowTime;
		[SerializeField, Tooltip("The amount of time the recover's charge of the attack will recover.")] private float _recoverRate;
		[SerializeField, Tooltip("If Guwba is attacking in the moment.")] private bool _attackUsage;
		[SerializeField, Tooltip("The buffer moment that Guwba have to execute a combo attack.")] private bool _comboAttackBuffer;
		[SerializeField, Tooltip("If Guwba is invencible at the moment.")] private bool _invencibility;
		public PathConnection PathConnection => PathConnection.Guwba;
		public short Health => this._vitality;
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
			this._visualizableGuwba = Instantiate(this._visualizableGuwbaObject, this.transform);
			this._damageableGuwbas = this.GetComponentsInChildren<DamageableGuwba>(true);
			this._redAxis = new(Mathf.Abs(this.transform.right.x), Mathf.Abs(this.transform.right.y));
			this.transform.right = this._turnLeft ? this._redAxis * Vector2.left : this._redAxis * Vector2.right;
			foreach (DamageableGuwba damageableGuwba in this._damageableGuwbas)
				damageableGuwba.SetAttack(this.Attack);
			this._sender.SetStateForm(StateForm.Disable);
			SaveController.Load(out SaveFile saveFile);
			this._visualizableGuwba.LifeText.text = $"X {saveFile.lifes}";
			this._visualizableGuwba.CoinText.text = $"X {saveFile.coins}";
			this._vitality = (short)this._visualizableGuwba.Vitality;
			this._gravityScale = this._rigidbody.gravityScale;
			this._normalSize = this._collider.size;
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
				damageableGuwba.Alpha = 1f;
				damageableGuwba.UnsetAttack(this.Attack);
			}
			Sender.Exclude(this);
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			if (this.gameObject.scene.name == this._levelSelectorScene)
				this._visualizableGuwba.RootElement.style.display = DisplayStyle.None;
			else
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
			if (movementValue.x > 0f)
				this._movementAction = 1f;
			else if (movementValue.x < 0f)
				this._movementAction = -1f;
			if (movementValue.y > 0.5f)
				this._lastJumpTime = this._jumpBufferTime;
			if (this._isJumping && this._rigidbody.linearVelocityY > 0f && movementValue.y < 0.5f)
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
					this.transform.right = this._dashMovement < 0f ? this._redAxis * Vector2.left : this._redAxis * Vector2.right;
					float dashLocation = this.transform.position.x;
					this._collider.size = this._dashSlideSize;
					this._collider.offset = new Vector2(this._collider.offset.x, -((this._normalSize.y - this._collider.size.y) / 2f));
					bool isActive = true;
					bool onWall = false;
					while (isActive || onWall)
					{
						float xAxisPosition = (this._collider.bounds.extents.x + this._wallChecker / 2f) * this._dashMovement;
						Vector2 origin = new(this.transform.position.x + xAxisPosition, this.transform.position.y + this._collider.offset.y);
						Vector2 size = new(this._wallChecker, this._collider.size.y - 0.025f);
						Vector2 direction = this.transform.right * this._dashMovement;
						float angle = this.transform.rotation.z * Mathf.Rad2Deg;
						bool collision = Physics2D.BoxCast(origin, size, angle, direction, this._wallChecker, this._groundLayerMask);
						this._rigidbody.linearVelocityX = this._dashSpeed * this._dashMovement;
						bool valid = MathF.Abs(this.transform.position.x - dashLocation) >= this._dashDistance;
						float yAxisPoint = this._normalSize.y / 2f + this._groundChecker;
						Vector2 wallOrigin = new(this.transform.position.x, this.transform.position.y + yAxisPoint);
						Vector2 wallSize = new(this._normalSize.x, this._normalSize.y - this._groundChecker);
						onWall = Physics2D.BoxCast(wallOrigin, wallSize, angle, this.transform.up, this._groundChecker, this._groundLayerMask);
						if ((valid || collision || !this._isOnGround || !this._dashActive) && !onWall)
						{
							this._animator.SetBool(this._attackSlide, false);
							this._animator.SetBool(this._dashSlide, isActive = this._dashActive = false);
							this._collider.size = this._normalSize;
							this._collider.offset = Vector2.zero;
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
			Vector2 point = this.transform.position;
			if (this._isOnGround && this._movementAction == 0f)
			{
				float angle = this.transform.rotation.z * Mathf.Rad2Deg;
				foreach (Collider2D collider in Physics2D.OverlapBoxAll(point, this._collider.size, angle, this._InteractionLayerMask))
					if (collider.TryGetComponent<IInteractable>(out var interactable))
					{
						interactable.Interaction();
						return;
					}
			}
		};
		private UnityAction<DamageableGuwba, IDestructible> Attack => (damageableGuwba, destructible) =>
		{
			if (destructible.Damage(damageableGuwba.AttackDamage))
			{
				destructible.Stun(damageableGuwba.AttackDamage, damageableGuwba.StunTime);
				EffectsController.HitStop(this._hitStopTime, this._hitSlowTime);
				this.StartCoroutine(RecoverVitality());
				IEnumerator RecoverVitality()
				{
					Color backgroundColor = this._visualizableGuwba.BackgroundColor;
					Color borderColor = this._visualizableGuwba.BorderColor;
					Color missingColor = this._visualizableGuwba.MissingVitalityColor;
					for (ushort amount = 0; amount < damageableGuwba.AttackDamage - Mathf.Abs(destructible.Health); amount++)
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
			this._redAxis = new(Mathf.Abs(this.transform.right.x), Mathf.Abs(this.transform.right.y));
			float movementValue = this._movementAction != 0f ? this._movementAction > 0f ? 1f : -1f : 0f;
			Vector2 direction = this.transform.right * movementValue;
			float angle = this.transform.rotation.z * Mathf.Rad2Deg;
			float rootHeight = this._collider.size.y / this._collider.size.y;
			bool downStairs = false;
			if (!this._isOnGround && this._downStairs && this._movementAction != 0f && this._lastJumpTime <= 0f && !this._dashActive)
			{
				float xOrigin = this.transform.position.x - ((this._collider.bounds.extents.x - .025f) * movementValue);
				Vector2 downRayOrigin = new(xOrigin, this.transform.position.y - this._collider.bounds.extents.y);
				float distance = rootHeight + this._groundChecker;
				RaycastHit2D downRay = Physics2D.Raycast(downRayOrigin, -this.transform.up, distance, this._groundLayerMask);
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
				}
				else if (this._rigidbody.linearVelocityY != 0f && !downStairs)
				{
					this._animator.SetBool(this._idle, false);
					this._animator.SetBool(this._walk, false);
					this._animator.SetBool(this._Jump, this._rigidbody.linearVelocityY > 0f);
					this._animator.SetBool(this._fall, this._rigidbody.linearVelocityY < 0f);
					if (this._animator.GetBool(this._attackJump))
						this._animator.SetBool(this._attackJump, this._rigidbody.linearVelocityY > 0f);
					float fallGravity = this._fallGravityMultiply * this._gravityScale;
					this._rigidbody.gravityScale = this._animator.GetBool(this._fall) ? fallGravity : this._gravityScale;
					this._lastGroundedTime -= Time.fixedDeltaTime;
					this._lastJumpTime -= Time.fixedDeltaTime;
					this._downStairs = false;
				}
			if (!this._dashActive)
			{
				if (this._movementAction != 0f && this._isOnGround)
				{
					float xPosition = this.transform.position.x + (this._collider.bounds.extents.x + this._wallChecker / 2f) * movementValue;
					Vector2 topOrigin = new(xPosition, this.transform.position.y + rootHeight * .5f);
					Vector2 bottomOrigin = new(xPosition, this.transform.position.y - rootHeight * this._bottomCheckerOffset);
					Vector2 topSize = new(this._wallChecker, rootHeight * this._topWallChecker - this._wallChecker);
					Vector2 bottomSize = new(this._wallChecker, rootHeight - this._wallChecker);
					LayerMask layerMask = this._groundLayerMask;
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
						RaycastHit2D lineWallStep = Physics2D.Linecast(lineStart, lineEnd, this._groundLayerMask);
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
					this.transform.right = this._movementAction < 0f ? this._redAxis * Vector2.left : this._redAxis * Vector2.right;
				float xPoint = (this._collider.bounds.extents.x + this._groundChecker / 2f) * this._movementAction;
				Vector2 origin = new(this.transform.position.x + xPoint, this.transform.position.y);
				Vector2 size = new(this._wallChecker, this._collider.size.y - this._wallChecker);
				bool wallBlock = Physics2D.BoxCast(origin, size, angle, direction, this._wallChecker, this._groundLayerMask);
				this._animator.SetFloat(this._walkSpeed, wallBlock ? 1f : Mathf.Abs(this._rigidbody.linearVelocityX) / this._movementSpeed);
				float targetSpeed = (this._attackUsage ? this._movementSpeed / 2f : this._movementSpeed) * this._movementAction;
				float speedDiferrence = targetSpeed - this._rigidbody.linearVelocityX;
				float accelerationRate = Mathf.Abs(targetSpeed) > 0f ? this._acceleration : this._decceleration;
				float movement = Mathf.Pow(Mathf.Abs(speedDiferrence) * accelerationRate, this._velocityPower) * Mathf.Sign(speedDiferrence);
				this._rigidbody.AddForceX(movement * this._rigidbody.mass);
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
				while (this._isDamaged)
				{
					foreach (DamageableGuwba damageableGuwba in this._damageableGuwbas)
						damageableGuwba.Alpha = damageableGuwba.Alpha >= 1f ? this._invencibilityValue : 1f;
					yield return new WaitTime(this, this._timeStep);
				}
			}
			yield return new WaitTime(this, this._invencibilityTime);
			this._isDamaged = false;
			foreach (DamageableGuwba damageableGuwba in this._damageableGuwbas)
				damageableGuwba.Alpha = 1f;
		}
		private void OnCollision()
		{
			float yPoint = this.transform.position.y - this._collider.bounds.extents.y + this._collider.offset.y - this._groundChecker / 2f;
			Vector2 origin = new(this.transform.position.x, yPoint);
			Vector2 size = new(this._collider.size.x - this._groundChecker, this._groundChecker);
			float angle = this.transform.rotation.z * Mathf.Rad2Deg;
			this._isOnGround = Physics2D.BoxCast(origin, size, angle, -this.transform.up, this._groundChecker, this._groundLayerMask);
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
		public bool Damage(ushort damage)
		{
			if (this._invencibility || this._isDamaged || damage < 1f)
				return false;
			this._isDamaged = true;
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
				this._collider.size = this._deadSize;
				this._sender.SetToWhereConnection(PathConnection.Hud);
				this._sender.SetToggle(true);
				this._sender.Send();
				this._sender.SetToggle(false);
				this._sender.SetToWhereConnection(PathConnection.Boss);
				this._sender.Send();
				this._sender.SetToWhereConnection(PathConnection.Enemy);
				this._sender.Send();
				return true;
			}
			this.StartCoroutine(this.Invencibility());
			return true;
		}
		public void Stun(ushort stunStrength, float stunTime)
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
				this.transform.right = this._turnLeft ? this._redAxis * Vector2.left : this._redAxis * Vector2.right;
				this._animator.SetBool(this._death, false);
				this._collider.size = this._normalSize;
			}
			if (data.StateForm == StateForm.Enable && data.ToggleValue.HasValue && data.ToggleValue.Value)
			{
				this._isDamaged = true;
				this.StartCoroutine(this.Invencibility());
				this.OnEnable();
			}
		}
	};
};
