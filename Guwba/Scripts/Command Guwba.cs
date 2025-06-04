using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
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
		private ActionsGuwba _actions;
		private readonly Sender _sender = Sender.Create();
		private Vector2 _attackValue = new();
		private float _gravityScale = 0f;
		private float _movementAction = 0f;
		private float _yMovement = 0f;
		private bool _isOnGround = false;
		private bool _downStairs = false;
		private bool _isJumping = false;
		[SerializeField, Tooltip("The camera that is attached to Guwba.")] private Camera _mainCamera;
		[SerializeField, Tooltip("The layer mask that Guwba identifies the ground.")] private LayerMask _groundLayerMask;
		[SerializeField, Tooltip("The layer mask that Guwba identifies a interactive object.")] private LayerMask _interactionLayerMask;
		[SerializeField, Tooltip("Size of the collider in live.")] private Vector2 _normalSize;
		[SerializeField, Tooltip("Size of the collider in death.")] private Vector2 _deadSize;
		[SerializeField, Tooltip("Animation parameter.")] private string _isOn;
		[SerializeField, Tooltip("Animation parameter.")] private string _idle;
		[SerializeField, Tooltip("Animation parameter.")] private string _walk;
		[SerializeField, Tooltip("Animation parameter.")] private string _slowWalk;
		[SerializeField, Tooltip("Animation parameter.")] private string _jump;
		[SerializeField, Tooltip("Animation parameter.")] private string _fall;
		[SerializeField, Tooltip("Animation parameter.")] private string _attack;
		[SerializeField, Tooltip("Animation parameter.")] private string _hold;
		[SerializeField, Tooltip("Animation parameter.")] private string _death;
		[SerializeField, Tooltip("The amount of speed that Guwba moves yourself.")] private ushort _movementSpeed;
		[SerializeField, Tooltip("The amount of strenght that Guwba can jump.")] private ushort _jumpStrenght;
		[SerializeField, Tooltip("Size of collider for checking the ground below the feet.")] private float _groundChecker;
		[SerializeField, Tooltip("Size of collider for checking the wall to climb stairs.")] private float _wallChecker;
		[SerializeField, Tooltip("Size of top part of the wall collider to climb stairs.")] private float _topWallChecker;
		[SerializeField, Tooltip("Offset of bottom part of the wall collider to climb stairs.")] private float _bottomCheckerOffset;
		[SerializeField, Tooltip("The amount of gravity to increase the fall.")] private float _amountToFall;
		[SerializeField, Tooltip("Lowing the offset of the grab.")] private float _lowHoldOffset;
		[SerializeField, Tooltip("If Guwba will look firstly to the left.")] private bool _turnLeft;
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
			this._spriteRenderer.flipX = this._turnLeft;
			this._gravityScale = this._rigidbody.gravityScale;
			this._sender.SetToWhereConnection(PathConnection.Controller);
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
			this._actions = new ActionsGuwba();
			this._actions.commands.movement.performed += this.Movement;
			this._actions.commands.movement.canceled += this.Movement;
			this._actions.commands.jump.started += this.Jump;
			this._actions.commands.attackRotationConsole.performed += this.AttackRotationConsole;
			this._actions.commands.attackRotationKeyboard.performed += this.AttackRotationKeyboard;
			this._actions.commands.attackUse.started += this.AttackUse;
			this._actions.commands.interaction.started += this.Interaction;
			this._actions.commands.movement.Enable();
			this._actions.commands.jump.Enable();
			this._actions.commands.attackRotationConsole.Enable();
			this._actions.commands.attackRotationKeyboard.Enable();
			this._actions.commands.attackUse.Enable();
			this._actions.commands.interaction.Enable();
			this._animator.SetFloat(this._isOn, 1f);
			this._animator.SetFloat(this._slowWalk, this._movementAction);
			this._rigidbody.gravityScale = this._gravityScale;
			this._rigidbody.linearVelocityY = this._yMovement;
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			this._actions.commands.movement.performed -= this.Movement;
			this._actions.commands.movement.canceled -= this.Movement;
			this._actions.commands.jump.started -= this.Jump;
			this._actions.commands.attackRotationConsole.performed -= this.AttackRotationConsole;
			this._actions.commands.attackRotationKeyboard.performed -= this.AttackRotationKeyboard;
			this._actions.commands.attackUse.started -= this.AttackUse;
			this._actions.commands.interaction.started -= this.Interaction;
			this._actions.commands.movement.Disable();
			this._actions.commands.jump.Disable();
			this._actions.commands.attackRotationConsole.Disable();
			this._actions.commands.attackRotationKeyboard.Disable();
			this._actions.commands.attackUse.Disable();
			this._actions.commands.interaction.Disable();
			this._actions.Dispose();
			this._animator.SetFloat(this._isOn, 0f);
			this._animator.SetFloat(this._slowWalk, 0f);
			this._movementAction = 0f;
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
				this._sender.SetConnectionState(ConnectionState.Enable).SetToggle(true).Send();
			}
			else
			{
				this.OnDisable();
				this._animator.SetBool(this._death, !isAlive);
				this._rigidbody.gravityScale = this._gravityScale;
				this._collider.size = this._deadSize;
				this._sender.SetConnectionState(ConnectionState.Disable).SetToggle(true).Send();
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
			if (movementAction.performed && this._movementAction != 0f)
				this._spriteRenderer.flipX = this._movementAction < 0f;
			this._rigidbody.linearVelocityX = this._movementAction * this._movementSpeed;
			this._animator.SetBool(this._walk, this._movementAction != 0f);
			if (this._animator.GetBool(this._walk))
				this._animator.SetFloat(this._slowWalk, this._movementAction < 0f ? this._movementAction * -1f : this._movementAction);
		};
		private Action<InputAction.CallbackContext> Jump => jumpAction =>
		{
			if (this._isOnGround || _grabObject)
			{
				if (_grabObject && !this._isOnGround)
				{
					this._animator.SetBool(this._hold, false);
					GuwbaAstral<AttackGuwba>._actualState.Invoke(false);
					_grabObject.transform.position = (Vector2)this.transform.position + Vector2.down;
					_grabObject.Throw(Vector2.down);
					_grabObject = null;
					GuwbaAstral<VisualGuwba>._grabObject = null;
					GuwbaAstral<AttackGuwba>._grabObject = null;
				}
				this._rigidbody.gravityScale = this._gravityScale;
				this._rigidbody.linearVelocityY = 0f;
				this._isJumping = true;
				this._rigidbody.AddForceY(this._jumpStrenght);
			}
		};
		private Action<InputAction.CallbackContext> AttackRotationConsole =>
			attackAction => this._attackValue = attackAction.ReadValue<Vector2>();
		private Action<InputAction.CallbackContext> AttackRotationKeyboard => attackAction =>
		{
			Vector2 attackTarget = attackAction.ReadValue<Vector2>();
			Vector2 attackValue = this._mainCamera.ScreenToWorldPoint(new Vector3(attackTarget.x, attackTarget.y, 0f));
			this._attackValue = (attackValue - (Vector2)this.transform.position).normalized;
		};
		private Action<InputAction.CallbackContext> AttackUse => attackAction =>
		{
			if (_grabObject)
			{
				this._animator.SetBool(this._hold, false);
				GuwbaAstral<AttackGuwba>._actualState.Invoke(false);
				_grabObject.transform.position = (Vector2)this.transform.position + this._attackValue;
				float angle = Mathf.Atan2(this._attackValue.y, this._attackValue.x) * Mathf.Rad2Deg - 90f;
				_grabObject.Throw(Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up);
				_grabObject = null;
				GuwbaAstral<VisualGuwba>._grabObject = null;
				GuwbaAstral<AttackGuwba>._grabObject = null;
			}
			else if (this._attackValue != Vector2.zero)
			{
				this._animator.SetBool(this._attack, true);
				if (this._attackValue.x < 0f)
					this._spriteRenderer.flipX = true;
				else if (this._attackValue.x > 0f)
					this._spriteRenderer.flipX = false;
				float angle = Mathf.Atan2(this._attackValue.y, this._attackValue.x) * Mathf.Rad2Deg - 90f;
				GuwbaAstral<AttackGuwba>.SetRotation(angle);
				GuwbaAstral<AttackGuwba>._actualState.Invoke(true);
			}
		};
		private Action<InputAction.CallbackContext> Interaction => interactionAction =>
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
				foreach (Collider2D collider in Physics2D.OverlapBoxAll(point, this._collider.size, 0f, this._interactionLayerMask))
					if (collider.TryGetComponent<IInteractable>(out var interactable))
					{
						interactable.Interaction();
						return;
					}
		};
		private void FixedUpdate()
		{
			float movementValue = this._movementAction != 0f ? this._movementAction > 0f ? 1f : -1f : 0f;
			float rootHeight = this._collider.size.y / this._collider.size.y;
			bool downStairs = false;
			if (!this._isOnGround && this._downStairs && this._movementAction != 0f && !this._isJumping)
			{
				float xOrigin = this.transform.position.x - ((this._collider.bounds.extents.x - .025f) * movementValue);
				Vector2 downRayOrigin = new(xOrigin, this.transform.position.y - this._collider.bounds.extents.y);
				RaycastHit2D downRay = Physics2D.Raycast(downRayOrigin, Vector2.down, rootHeight + this._groundChecker, this._groundLayerMask);
				downStairs = downRay;
				if (downStairs)
					this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y - downRay.distance);
			}
			this._rigidbody.gravityScale = this._gravityScale;
			if (this._isOnGround)
			{
				this._animator.SetBool(this._idle, this._movementAction == 0f);
				this._animator.SetBool(this._walk, this._movementAction != 0f);
				if (this._animator.GetBool(this._walk))
					this._animator.SetFloat(this._slowWalk, this._movementAction < 0f ? this._movementAction * -1f : this._movementAction);
				this._animator.SetBool(this._jump, false);
				this._animator.SetBool(this._fall, false);
				this._rigidbody.gravityScale = this._gravityScale;
				this._rigidbody.linearVelocityY = 0f;
				this._downStairs = true;
			}
			else if (this._rigidbody.linearVelocityY > 0f && !downStairs)
			{
				this._animator.SetBool(this._idle, false);
				this._animator.SetBool(this._walk, false);
				this._animator.SetBool(this._jump, true);
				this._animator.SetBool(this._fall, false);
				this._rigidbody.gravityScale = this._gravityScale;
				this._isJumping = false;
				this._downStairs = false;
			}
			else if (this._rigidbody.linearVelocityY < 0f && !downStairs)
			{
				this._animator.SetBool(this._idle, false);
				this._animator.SetBool(this._walk, false);
				this._animator.SetBool(this._jump, false);
				this._animator.SetBool(this._fall, true);
				if (this._rigidbody.gravityScale < this._gravityScale * this._amountToFall)
					this._rigidbody.gravityScale += this._gravityScale * this._amountToFall * Time.fixedDeltaTime;
				else
					this._rigidbody.gravityScale = this._gravityScale * this._amountToFall;
				this._isJumping = false;
				this._downStairs = false;
			}
			if (this._movementAction != 0f)
			{
				this._spriteRenderer.flipX = this._movementAction < 0f;
				float xPosition = this.transform.position.x + (this._collider.bounds.extents.x + this._wallChecker / 2f) * movementValue;
				Vector2 topPosition = new(xPosition, this.transform.position.y + rootHeight * .5f);
				Vector2 bottomPosition = new(xPosition, this.transform.position.y - rootHeight * this._bottomCheckerOffset);
				Vector2 topSize = new(this._wallChecker, rootHeight * this._topWallChecker - .1f);
				Vector2 bottomSize = new(this._wallChecker, rootHeight - .1f);
				Collider2D bottomCollider = Physics2D.OverlapBox(bottomPosition, bottomSize, 0f, this._groundLayerMask);
				if (bottomCollider && !Physics2D.OverlapBox(topPosition, topSize, 0f, this._groundLayerMask))
				{
					float topCorner = this.transform.position.y + this._collider.bounds.extents.y;
					float bottomCorner = this.transform.position.y - this._collider.bounds.extents.y;
					Vector2 lineStart = new(xPosition + this._wallChecker / 2f * movementValue, topCorner);
					Vector2 lineEnd = new(xPosition + this._wallChecker / 2f * movementValue, bottomCorner);
					RaycastHit2D lineWallStep = Physics2D.Linecast(lineStart, lineEnd, this._groundLayerMask);
					if (lineWallStep && lineWallStep.collider == bottomCollider)
					{
						float xDistance = this.transform.position.x + this._wallChecker * movementValue;
						float yDistance = this.transform.position.y + (lineWallStep.point.y - bottomCorner);
						this.transform.position = new Vector2(xDistance, yDistance);
					}
				}
			}
			this._rigidbody.linearVelocityX = this._movementAction * this._movementSpeed;
			this._isOnGround = false;
		}
		private void OnCollision()
		{
			float yPoint = this.transform.position.y - this._collider.bounds.extents.y - this._groundChecker / 2f;
			Vector2 pointGround = new(this.transform.position.x, yPoint);
			Vector2 sizeGround = new(this._collider.size.x - .025f, this._groundChecker);
			this._isOnGround = Physics2D.OverlapBox(pointGround, sizeGround, this.transform.rotation.eulerAngles.z, this._groundLayerMask);
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
