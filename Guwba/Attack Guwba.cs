using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
	public sealed class AttackGuwba : GuwbaAstral<AttackGuwba>
	{
		private static AttackGuwba _instance;
		private SpriteRenderer _spriteRenderer;
		private Rigidbody2D _rigidbody;
		private Vector2 _guardVelocity = new();
		private bool _isAttacking = false;
		[Header("Movement")]
		[SerializeField, Tooltip("The speed of the attack of Guwba.")] private float _movementSpeed;
		[SerializeField, Tooltip("The maximum distance to move forward.")] private float _movementDistance;
		[Header("Damage Interaction")]
		[SerializeField, Tooltip("The amount of damage that the attack of Guwba hits.")] private ushort _damage;
		[SerializeField, Tooltip("The amount of time to stop the game when hit is given.")] private float _hitStopTime;
		[SerializeField, Tooltip("The amount of time to slow the game when hit is given.")] private float _hitSlowTime;
		[SerializeField, Tooltip("The value of vitality to grab.")] private ushort _valueToGrab;
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
			this._rigidbody = this.GetComponent<Rigidbody2D>();
			this._sender.SetToWhereConnection(PathConnection.Character);
			this._sender.SetStateForm(StateForm.Action);
			this._sender.SetToggle(true);
			this._spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
			_actualState += this.Movement;
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!_instance || _instance != this)
				return;
			_actualState -= this.Movement;
			this.StopAllCoroutines();
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			this._rigidbody.linearVelocity = this._guardVelocity;
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			this._guardVelocity = this._rigidbody.linearVelocity;
			this._rigidbody.linearVelocity = Vector2.zero;
		}
		private UnityAction<bool> Movement => isAttacking =>
		{
			this._isAttacking = isAttacking;
			if (this._isAttacking)
				this.StartCoroutine(Attack());
			else
			{
				this._spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
				this._rigidbody.bodyType = RigidbodyType2D.Kinematic;
			}
			IEnumerator Attack()
			{
				this._spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
				this._rigidbody.bodyType = RigidbodyType2D.Dynamic;
				while (this._isAttacking)
				{
					Vector2 targetPosition = GuwbaAstral<CommandGuwba>.Position;
					if (Vector2.Distance(this.transform.position, targetPosition) >= this._movementDistance)
					{
						GuwbaAstral<CommandGuwba>._returnAttack = true;
						_returnAttack = true;
						this._rigidbody.linearVelocity = Vector2.zero;
					}
					if (_returnAttack)
					{
						Vector2 targetDirection = ((Vector2)this.transform.position - targetPosition).normalized;
						float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
						this.transform.eulerAngles = new Vector3(0f, 0f, angle);
						this._rigidbody.linearVelocity = this._movementSpeed * -this.transform.up;
						if (_grabObject)
							_grabObject.transform.position = this.transform.position;
					}
					else
						this._rigidbody.linearVelocity = this._movementSpeed * this.transform.up;
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => this.enabled);
				}
			}
		};
		private void OnTrigger(GameObject collisionObject)
		{
			if (_returnAttack || !this._isAttacking)
				return;
			bool isGrabtable = collisionObject.TryGetComponent<GrabBody>(out var grabBody);
			bool isDamageable = collisionObject.TryGetComponent<IDamageable>(out var damageable);
			bool valid = isGrabtable && !isDamageable || (isGrabtable && isDamageable && damageable.Health == this._valueToGrab);
			if (valid && grabBody.IsGrabtable)
			{
				GuwbaAstral<CommandGuwba>._returnAttack = true;
				_returnAttack = true;
				GuwbaAstral<CommandGuwba>._grabObject = grabBody;
				GuwbaAstral<VisualGuwba>._grabObject = grabBody;
				_grabObject = grabBody;
				_grabObject.Stop((ushort)this.gameObject.layer);
				if (isDamageable)
					this._sender.Send();
			}
			else if (isDamageable)
			{
				GuwbaAstral<CommandGuwba>._returnAttack = true;
				_returnAttack = true;
				if (damageable.Damage(this._damage))
				{
					EffectsController.SetHitStop(this._hitStopTime, this._hitSlowTime);
					this._sender.Send();
				}
			}
			else if (collisionObject.TryGetComponent<Surface>(out _))
			{
				GuwbaAstral<CommandGuwba>._returnAttack = true;
				_returnAttack = true;
			}
		}
		private void OnTriggerEnter2D(Collider2D other) => this.OnTrigger(other.gameObject);
		private void OnTriggerStay2D(Collider2D other) => this.OnTrigger(other.gameObject);
	};
};
