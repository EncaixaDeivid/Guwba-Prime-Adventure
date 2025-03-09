using UnityEngine;
using UnityEngine.InputSystem;
using System;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(Rigidbody2D))]
	[RequireComponent(typeof(BoxCollider2D), typeof(CircleCollider2D))]
	public sealed class CommandGuwba : GuwbaTransformer<CommandGuwba>
	{
		private static CommandGuwba _instance;
		private SpriteRenderer _spriteRenderer;
		private Animator _animator;
		private Rigidbody2D _rigidbody;
		private BoxCollider2D _collider;
		private ActionsGuwba _actions;
		private Vector2 _attackValue = new();
		private float _gravityScale = 0f, _movementAction = 0f, _yMovement = 0f;
		private bool _isOnGround = false, _canJump = false, _grabState = false;
		[SerializeField] private LayerMask _groundLayerMask, _interactionLayerMask;
		[SerializeField] private string _isOn, _idle, _walk, _slowWalk, _jump, _fall, _attack, _hold;
		[SerializeField] private ushort _movementSpeed, _jumpStrenght;
		[SerializeField] private float _groundChecker, _wallChecker, _topWallChecker, _lowHoldOffset;
		[SerializeField] private bool _turnLeft;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
				Destroy(_instance.gameObject);
			_instance = this;
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
			this._animator = this.GetComponent<Animator>();
			this._rigidbody = this.GetComponent<Rigidbody2D>();
			this._collider = this.GetComponent<BoxCollider2D>();
			this._spriteRenderer.flipX = this._turnLeft;
			this._gravityScale = this._rigidbody.gravityScale;
		}
		private void OnEnable()
		{
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
		private Action<InputAction.CallbackContext> Movement => (InputAction.CallbackContext movementAction) =>
		{
			Vector2 movementValue = movementAction.ReadValue<Vector2>();
			this._movementAction = movementValue.x;
			if (movementValue.x > 0f)
				this._movementAction = 1f + movementValue.y;
			else if (movementValue.x < 0f)
				this._movementAction = -1f - movementValue.y;
			if (movementAction.performed && this._movementAction != 0f)
				this._spriteRenderer.flipX = this._movementAction < 0f;
			this._rigidbody.linearVelocityX = this._movementAction * this._movementSpeed;
			this._animator.SetBool(this._walk, this._movementAction != 0f);
			if (this._animator.GetBool(this._walk))
				this._animator.SetFloat(this._slowWalk, this._movementAction < 0f ? this._movementAction * -1f : this._movementAction);
		};
		private Action<InputAction.CallbackContext> Jump => (InputAction.CallbackContext jumpAction) =>
		{
			if (this._isOnGround || this._canJump)
			{
				if (this._canJump && !this._isOnGround)
					this._canJump = false;
				this._rigidbody.gravityScale = this._gravityScale;
				this._rigidbody.linearVelocityY = 0f;
				this._rigidbody.AddForce(this.transform.up * this._jumpStrenght);
			}
		};
		private Action<InputAction.CallbackContext> AttackRotationConsole =>
			(InputAction.CallbackContext attackAction) => this._attackValue = attackAction.ReadValue<Vector2>();
		private Action<InputAction.CallbackContext> AttackRotationKeyboard => (InputAction.CallbackContext attackAction) =>
		{
			Vector2 attackTarget = attackAction.ReadValue<Vector2>();
			Vector2 attackValue = Camera.main.ScreenToWorldPoint(new Vector3(attackTarget.x, attackTarget.y, 0f));
			this._attackValue = (attackValue - (Vector2)this.transform.position).normalized;
		};
		private Action<InputAction.CallbackContext> AttackUse => (InputAction.CallbackContext attackAction) =>
		{
			if (this._grabState)
			{
				this._animator.SetBool(this._hold, false);
				_activeState = false;
				GuwbaTransformer<AttackGuwba>._activeState = false;
				this._grabState = false;
				_grabObject.transform.position = (Vector2)this.transform.position + this._attackValue;
				float angle = Mathf.Atan2(this._attackValue.y, this._attackValue.x) * Mathf.Rad2Deg - 90f;
				_grabObject.Throw(Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up);
				_grabObject = null;
				GuwbaTransformer<VisualHudGuwba>._grabObject = null;
				GuwbaTransformer<AttackGuwba>._grabObject = null;
			}
			else if (this._attackValue != Vector2.zero)
			{
				this._animator.SetBool(this._attack, true);
				float angle = Mathf.Atan2(this._attackValue.y, this._attackValue.x) * Mathf.Rad2Deg - 90f;
				GuwbaTransformer<AttackGuwba>.SetRotation(angle);
				_activeState = true;
				GuwbaTransformer<AttackGuwba>._activeState = true;
			}
		};
		private Action<InputAction.CallbackContext> Interaction => (InputAction.CallbackContext interactionAction) =>
		{
			Vector2 point = this.transform.position;
			if (this._isOnGround && this._movementAction == 0f && !this._animator.GetBool(this._attack))
				foreach (Collider2D collider in Physics2D.OverlapBoxAll(point, this._collider.size, 0f, this._interactionLayerMask))
					if (collider.TryGetComponent<IInteractable>(out var interactionObject))
						interactionObject.Interaction();
		};
		private void FixedUpdate()
		{
			float yPoint = this.transform.position.y - this._collider.bounds.extents.y - this._groundChecker / 2f;
			Vector2 pointGround = new(this.transform.position.x, yPoint);
			Vector2 sizeGround = new(this._collider.size.x - .025f, this._groundChecker);
			this._isOnGround = Physics2D.OverlapBox(pointGround, sizeGround, 0f, this._groundLayerMask);
			if (this._isOnGround)
			{
				this._animator.SetBool(this._idle, this._movementAction == 0f);
				this._animator.SetBool(this._walk, this._movementAction != 0f);
				if (this._animator.GetBool(this._walk))
					this._animator.SetFloat(this._slowWalk, this._movementAction < 0f ? this._movementAction * -1f : this._movementAction);
				this._animator.SetBool(this._jump, false);
				this._animator.SetBool(this._fall, false);
				this._canJump = true;
				this._rigidbody.gravityScale = this._gravityScale;
				this._rigidbody.linearVelocityY = 0f;
			}
			else if (this._rigidbody.linearVelocityY > 0f)
			{
				this._animator.SetBool(this._idle, false);
				this._animator.SetBool(this._walk, false);
				this._animator.SetBool(this._jump, true);
				this._animator.SetBool(this._fall, false);
				this._rigidbody.gravityScale = this._gravityScale;
			}
			else if (this._rigidbody.linearVelocityY < 0f)
			{
				this._animator.SetBool(this._idle, false);
				this._animator.SetBool(this._walk, false);
				this._animator.SetBool(this._jump, false);
				this._animator.SetBool(this._fall, true);
				if (this._rigidbody.gravityScale < this._gravityScale * 2f)
					this._rigidbody.gravityScale += this._gravityScale * 2f * Time.deltaTime;
				else
					this._rigidbody.gravityScale = this._gravityScale * 2f;
			}
			if (this._movementAction != 0f)
			{
				this._spriteRenderer.flipX = this._movementAction < 0f;
				float movementValue = this._movementAction > 0f ? 1f : -1f;
				float rootHeight = this._collider.size.y / this._collider.size.y;
				float xPosition = this.transform.position.x + (this._collider.bounds.extents.x + this._wallChecker / 2f) * movementValue;
				Vector2 topPosition = new(xPosition, this.transform.position.y + rootHeight * .5f);
				Vector2 bottomPosition = new(xPosition, this.transform.position.y - rootHeight * 1.5f);
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
			if (this._grabState)
			{
				Vector2 newPosition = new(this.transform.position.x, this.transform.position.y + (this._collider.size.y - this._lowHoldOffset));
				_grabObject.transform.position = newPosition;
			}
		}
		private void OnTrigger(GameObject collisionObject)
		{
			if (_returnState && GuwbaTransformer<AttackGuwba>.EqualObject(collisionObject))
			{
				if (_grabObject)
					this._animator.SetBool(this._hold, this._grabState = true);
				else
					this._animator.SetBool(this._hold, false);
				this._animator.SetBool(this._attack, false);
				_activeState = false;
				GuwbaTransformer<AttackGuwba>._activeState = false;
				_returnState = false;
				GuwbaTransformer<AttackGuwba>._returnState = false;
				GuwbaTransformer<AttackGuwba>.Position = this.transform.position;
			}
		}
		private void OnTriggerEnter2D(Collider2D other) => this.OnTrigger(other.gameObject);
		private void OnTriggerStay2D(Collider2D other) => this.OnTrigger(other.gameObject);
	};
};
