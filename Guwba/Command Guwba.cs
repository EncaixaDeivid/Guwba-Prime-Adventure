using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(Rigidbody2D))]
	[RequireComponent(typeof(BoxCollider2D), typeof(CircleCollider2D))]
	public sealed class CommandGuwba : GuwbaAstral<CommandGuwba>
	{
		private static CommandGuwba _instance;
		private SpriteRenderer _spriteRenderer;
		private Animator _animator;
		private Rigidbody2D _rigidbody;
		private BoxCollider2D _collider;
		private InputController _inputController;
		private Vector2 _normalSize = new();
		private Vector2 _attackAngle = new();
		private float _gravityScale = 0f;
		private float _movementAction = 0f;
		private float _yMovement = 0f;
		private float _dashMovement = 0f;
		private float _guardDashMovement = 0f;
		private float _lastGroundedTime = 0f;
		private float _lastJumpTime = 0f;
		private bool _isOnGround = false;
		private bool _downStairs = false;
		private bool _isJumping = false;
		private bool _dashActive = false;
		[Header("World Interaction")]
		[SerializeField, Tooltip("The camera that is attached to Guwba.")] private Camera _mainCamera;
		[SerializeField, Tooltip("The layer mask that Guwba identifies the ground.")] private LayerMask _groundLayerMask;
		[SerializeField, Tooltip("The layer mask that Guwba identifies a interactive object.")] private LayerMask _InteractionLayerMask;
		[SerializeField, Tooltip("Size of the collider in dash slide.")] private Vector2 _dashSlideSize;
		[SerializeField, Tooltip("Size of the collider in death.")] private Vector2 _deadSize;
		[Header("Animation")]
		[SerializeField, Tooltip("Animation parameter.")] private string _isOn;
		[SerializeField, Tooltip("Animation parameter.")] private string _idle;
		[SerializeField, Tooltip("Animation parameter.")] private string _walk;
		[SerializeField, Tooltip("Animation parameter.")] private string _walkSpeed;
		[SerializeField, Tooltip("Animation parameter.")] private string _dashSlide;
		[SerializeField, Tooltip("Animation parameter.")] private string _Jump;
		[SerializeField, Tooltip("Animation parameter.")] private string _fall;
		[SerializeField, Tooltip("Animation parameter.")] private string _attack;
		[SerializeField, Tooltip("Animation parameter.")] private string _hold;
		[SerializeField, Tooltip("Animation parameter.")] private string _death;
		[Header("Colliders Checkers")]
		[SerializeField, Tooltip("Size of collider for checking the ground below the feet.")] private float _groundChecker;
		[SerializeField, Tooltip("Size of collider for checking the wall to climb stairs.")] private float _wallChecker;
		[SerializeField, Tooltip("Size of top part of the wall collider to climb stairs.")] private float _topWallChecker;
		[SerializeField, Tooltip("Offset of bottom part of the wall collider to climb stairs.")] private float _bottomCheckerOffset;
		[SerializeField, Tooltip("Lowing the offset of the grab.")] private float _lowHoldOffset;
		[Header("Movement")]
		[SerializeField, Tooltip("The amount of speed that Guwba moves yourself.")] private float _movementSpeed;
		[SerializeField, Tooltip("The amount of acceleration Guwba will apply to the Movement.")] private float _acceleration;
		[SerializeField, Tooltip("The amount of decceleration Guwba will apply to the Movement.")] private float _decceleration;
		[SerializeField, Tooltip("The amount of power the velocity Guwba will apply to the Movement.")] private float _velocityPower;
		[SerializeField, Tooltip("The amount of friction Guwba will apply to the end of Movement.")] private float _frictionAmount;
		[SerializeField, Tooltip("The amount of speed in both dashes.")] private float _dashSpeed;
		[SerializeField, Tooltip("The amount of distance Guwba will go in both dashes.")] private float _dashDistance;
		[SerializeField, Tooltip("The speed of the back dash's animation.")] private float _backDashSpeed;
		[SerializeField, Tooltip("If Guwba will look firstly to the left.")] private bool _turnLeft;
		[Header("Jump")]
		[SerializeField, Tooltip("The amount of strenght that Guwba can Jump.")] private float _jumpStrenght;
		[SerializeField, Tooltip("The amount of time that Guwba can Jump before thouching ground.")] private float _jumpBufferTime;
		[SerializeField, Tooltip("The amount of time that Guwba can Jump when get out of the ground.")] private float _jumpCoyoteTime;
		[SerializeField, Tooltip("The amount of gravity to increase the fall.")] private float _fallGravityMultiply;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
			this._animator = this.GetComponent<Animator>();
			this._rigidbody = this.GetComponent<Rigidbody2D>();
			this._collider = this.GetComponent<BoxCollider2D>();
			this._sender.SetToWhereConnection(PathConnection.Hud);
			this._spriteRenderer.flipX = this._turnLeft;
			this._gravityScale = this._rigidbody.gravityScale;
			this._normalSize = this._collider.size;
			_actualState += this.DeathState;
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!_instance || _instance != this)
				return;
			_actualState -= this.DeathState;
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			this._inputController = new InputController();
			this._inputController.Commands.Movement.performed += this.Movement;
			this._inputController.Commands.Movement.canceled += this.Movement;
			this._inputController.Commands.Jump.started += this.Jump;
			this._inputController.Commands.AttackRotationConsole.performed += this.AttackAngleConsole;
			this._inputController.Commands.AttackRotationKeyboard.performed += this.AttackAngleKeyboard;
			this._inputController.Commands.AttackUse.started += this.AttackUse;
			this._inputController.Commands.Interaction.started += this.Interaction;
			this._inputController.Commands.Movement.Enable();
			this._inputController.Commands.Jump.Enable();
			this._inputController.Commands.AttackRotationConsole.Enable();
			this._inputController.Commands.AttackRotationKeyboard.Enable();
			this._inputController.Commands.AttackUse.Enable();
			this._inputController.Commands.Interaction.Enable();
			this._animator.SetFloat(this._isOn, 1f);
			this._animator.SetFloat(this._walkSpeed, this._dashActive ? -1f : 1f);
			if (this._dashActive)
				this._dashMovement = this._guardDashMovement;
			this._rigidbody.gravityScale = this._gravityScale;
			this._rigidbody.linearVelocityY = this._yMovement;
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			this._inputController.Commands.Movement.performed -= this.Movement;
			this._inputController.Commands.Movement.canceled -= this.Movement;
			this._inputController.Commands.Jump.started -= this.Jump;
			this._inputController.Commands.AttackRotationConsole.performed -= this.AttackAngleConsole;
			this._inputController.Commands.AttackRotationKeyboard.performed -= this.AttackAngleKeyboard;
			this._inputController.Commands.AttackUse.started -= this.AttackUse;
			this._inputController.Commands.Interaction.started -= this.Interaction;
			this._inputController.Commands.Movement.Disable();
			this._inputController.Commands.Jump.Disable();
			this._inputController.Commands.AttackRotationConsole.Disable();
			this._inputController.Commands.AttackRotationKeyboard.Disable();
			this._inputController.Commands.AttackUse.Disable();
			this._inputController.Commands.Interaction.Disable();
			this._inputController.Dispose();
			this._animator.SetFloat(this._isOn, 0f);
			this._animator.SetFloat(this._walkSpeed, 0f);
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
		private UnityAction<bool> DeathState => isAlive =>
		{
			if (isAlive)
			{
				this.OnEnable();
				this._animator.SetBool(this._death, !isAlive);
				this._collider.size = this._normalSize;
				this._sender.SetStateForm(StateForm.Enable);
				this._sender.SetToggle(true);
				this._sender.Send();
			}
			else
			{
				this.OnDisable();
				this._animator.SetBool(this._death, !isAlive);
				this._rigidbody.gravityScale = this._gravityScale;
				this._collider.size = this._deadSize;
				this._sender.SetStateForm(StateForm.Disable);
				this._sender.SetToggle(true);
				this._sender.Send();
			}
		};
		private Action<InputAction.CallbackContext> Movement => movementAction =>
		{
			Vector2 movementValue = movementAction.ReadValue<Vector2>();
			this._movementAction = movementValue.x;
			if (movementValue.x > 0f)
				this._movementAction = 1f;
			else if (movementValue.x < 0f)
				this._movementAction = -1f;
			if (!this._dashActive && this._movementAction != 0f && Mathf.Abs(movementValue.y) > 0.5f && this._isOnGround && !_grabObject)
			{
				this.StartCoroutine(Dash());
				IEnumerator Dash()
				{
					this._dashMovement = this._movementAction;
					this._spriteRenderer.flipX = this._dashMovement < 0f;
					float dashDirection = 1f;
					if (movementValue.y < 0f)
						dashDirection = -1f;
					float dashLocation = this.transform.position.x;
					GuwbaAstral<VisualGuwba>._actualState.Invoke(this._dashActive = true);
					if (dashDirection > 0f)
					{
						this._animator.SetBool(this._dashSlide, this._dashActive);
						this._collider.size = this._dashSlideSize;
						this._collider.offset = new Vector2(this._collider.offset.x, (this._normalSize.y - this._collider.size.y) / 2f);
						this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y - this._normalSize.y / 2f);
					}
					else
					{
						this._animator.SetBool(this._walk, true);
						this._animator.SetFloat(this._walkSpeed, -this._backDashSpeed);
					}
					bool onWall = false;
					while (this._dashActive || onWall)
					{
						float xDirection = this._dashMovement * dashDirection;
						float xAxisPosition = (this._collider.bounds.extents.x + this._wallChecker / 2f) * xDirection;
						Vector2 origin = new(this.transform.position.x + xAxisPosition, this.transform.position.y + this._collider.offset.y);
						Vector2 size = new(this._wallChecker, this._collider.size.y - 0.025f);
						Vector2 direction = this.transform.right * xDirection;
						float angle = this.transform.rotation.z * Mathf.Rad2Deg;
						bool collision = Physics2D.BoxCast(origin, size, angle, direction, this._wallChecker, this._groundLayerMask);
						this._rigidbody.linearVelocityX = this._dashSpeed * xDirection;
						bool valid = MathF.Abs(this.transform.position.x - dashLocation) >= this._dashDistance;
						float yAxisPoint = this._normalSize.y / 2f + this._groundChecker;
						Vector2 wallOrigin = new(this.transform.position.x, this.transform.position.y + yAxisPoint);
						Vector2 wallSize = new(this._normalSize.x, this._normalSize.y - this._groundChecker);
						onWall = Physics2D.BoxCast(wallOrigin, wallSize, angle, this.transform.up, this._groundChecker, this._groundLayerMask);
						if ((valid || _grabObject || collision || !this._isOnGround) && !onWall)
						{
							GuwbaAstral<VisualGuwba>._actualState.Invoke(this._dashActive = false);
							if (dashDirection > 0f)
							{
								this._animator.SetBool(this._dashSlide, this._dashActive);
								this._collider.size = this._normalSize;
								this._collider.offset = Vector2.zero;
								if (this._isOnGround)
								{
									float amountToMove = this.transform.position.y + this._normalSize.y / 2f;
									this.transform.position = new Vector2(this.transform.position.x, amountToMove);
								}
							}
							else
								this._animator.SetFloat(this._walkSpeed, 1f);
						}
						yield return new WaitForFixedUpdate();
						yield return new WaitUntil(() => this.enabled);
					}
				}
			}
		};
		private Action<InputAction.CallbackContext> Jump => jumpAction =>
		{
			if (_grabObject && !this._isOnGround)
			{
				this._animator.SetBool(this._hold, false);
				GuwbaAstral<AttackGuwba>._actualState.Invoke(false);
				_grabObject.transform.position = (Vector2)this.transform.position + -(Vector2)this.transform.up;
				_grabObject.Throw(-this.transform.up);
				_grabObject = null;
				GuwbaAstral<VisualGuwba>._grabObject = null;
				GuwbaAstral<AttackGuwba>._grabObject = null;
				this._rigidbody.gravityScale = this._gravityScale;
				this._rigidbody.linearVelocityY = 0f;
				this._rigidbody.AddForceY(this._jumpStrenght * this._rigidbody.mass, ForceMode2D.Impulse);
			}
			else
				this._lastJumpTime = this._jumpBufferTime;
		};
		private Action<InputAction.CallbackContext> AttackAngleConsole => attackAction => this._attackAngle = attackAction.ReadValue<Vector2>();
		private Action<InputAction.CallbackContext> AttackAngleKeyboard => attackAction =>
		{
			Vector2 attackTarget = attackAction.ReadValue<Vector2>();
			Vector2 attackAngle = this._mainCamera.ScreenToWorldPoint(new Vector2(attackTarget.x, attackTarget.y));
			this._attackAngle = (attackAngle - (Vector2)this.transform.position).normalized;
		};
		private Action<InputAction.CallbackContext> AttackUse => attackAction =>
		{
			if (_grabObject)
			{
				this._animator.SetBool(this._hold, false);
				GuwbaAstral<AttackGuwba>._actualState.Invoke(false);
				_grabObject.transform.position = (Vector2)this.transform.position + this._attackAngle;
				float angle = Mathf.Atan2(this._attackAngle.y, this._attackAngle.x) * Mathf.Rad2Deg - 90f;
				_grabObject.Throw(Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up);
				_grabObject = null;
				GuwbaAstral<VisualGuwba>._grabObject = null;
				GuwbaAstral<AttackGuwba>._grabObject = null;
			}
			else if (this._attackAngle != Vector2.zero && !this._animator.GetBool(this._attack))
			{
				this._animator.SetBool(this._attack, true);
				if (this._attackAngle.x < 0f)
					this._spriteRenderer.flipX = true;
				else if (this._attackAngle.x > 0f)
					this._spriteRenderer.flipX = false;
				GuwbaAstral<AttackGuwba>._actualState.Invoke(true);
			}
		};
		private Action<InputAction.CallbackContext> Interaction => InteractionAction =>
		{
			Vector2 point = this.transform.position;
			if (_grabObject && !_grabObject.IsDamageable)
			{
				this._animator.SetBool(this._hold, false);
				_grabObject.Drop();
				_grabObject = null;
				GuwbaAstral<VisualGuwba>._grabObject = null;
				GuwbaAstral<AttackGuwba>._grabObject = null;
			}
			else if (this._isOnGround && this._movementAction == 0f && !this._animator.GetBool(this._attack))
			{
				float angle = this.transform.rotation.eulerAngles.z;
				foreach (Collider2D collider in Physics2D.OverlapBoxAll(point, this._collider.size, angle, this._InteractionLayerMask))
					if (collider.TryGetComponent<IInteractable>(out var interactable))
					{
						interactable.Interaction();
						return;
					}
			}
		};
		private void FixedUpdate()
		{
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
				this._spriteRenderer.flipX = this._movementAction != 0f ? this._movementAction < 0f : this._spriteRenderer.flipX;
				float targetSpeed = this._movementSpeed * this._movementAction;
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
				this._isJumping = true;
				this._rigidbody.gravityScale = this._gravityScale;
				this._rigidbody.linearVelocityY = 0f;
				this._rigidbody.AddForceY(this._jumpStrenght * this._rigidbody.mass, ForceMode2D.Impulse);
			}
			this._isOnGround = false;
		}
		private void OnCollision()
		{
			float yPoint = this.transform.position.y - this._collider.bounds.extents.y + this._collider.offset.y - this._groundChecker / 2f;
			Vector2 origin = new(this.transform.position.x, yPoint);
			Vector2 size = new(this._collider.size.x - this._groundChecker, this._groundChecker);
			float angle = this.transform.rotation.z * Mathf.Rad2Deg;
			this._isOnGround = Physics2D.BoxCast(origin, size, angle, -this.transform.up, this._groundChecker, this._groundLayerMask);
		}
		private void OnTrigger(GameObject collisionObject)
		{
			if (_returnAttack && GuwbaAstral<AttackGuwba>.EqualObject(collisionObject))
			{
				this._animator.SetBool(this._hold, _grabObject);
				this._animator.SetBool(this._attack, false);
				GuwbaAstral<AttackGuwba>._actualState.Invoke(false);
				_returnAttack = false;
				GuwbaAstral<AttackGuwba>._returnAttack = false;
				GuwbaAstral<AttackGuwba>.Position = this.transform.position;
				if (_grabObject)
				{
					_grabObject.transform.parent = this.transform;
					float yPoint = this.transform.position.y + this._collider.size.y - this._lowHoldOffset;
					Vector2 newPosition = new(this.transform.position.x, yPoint);
					_grabObject.transform.position = newPosition;
				}
			}
		}
		private void OnCollisionEnter2D(Collision2D other) => this.OnCollision();
		private void OnCollisionStay2D(Collision2D other) => this.OnCollision();
		private void OnTriggerEnter2D(Collider2D other) => this.OnTrigger(other.gameObject);
		private void OnTriggerStay2D(Collider2D other) => this.OnTrigger(other.gameObject);
	};
};
