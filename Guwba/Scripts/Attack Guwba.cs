using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using GuwbaPrimeAdventure.Effects;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
	public sealed class AttackGuwba : GuwbaTransformer<AttackGuwba>
	{
		private static AttackGuwba _instance;
		private SpriteRenderer _spriteRenderer;
		private Rigidbody2D _rigidbody;
		private Vector2 _guardVelocity = new();
		private bool _isAttacking = false;
		[SerializeField] private ushort _movementSpeed, _movementDistance, _damage;
		[SerializeField] private float _hitStopTime, _hitSlowTime;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
				Destroy(_instance.gameObject);
			_instance = this;
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
			this._rigidbody = this.GetComponent<Rigidbody2D>();
			this._spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
			_actualState += this.Movement;
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			_actualState -= this.Movement;
			this.StopAllCoroutines();
		}
		private void OnEnable() => this._rigidbody.linearVelocity = this._guardVelocity;
		private void OnDisable()
		{
			this._guardVelocity = this._rigidbody.linearVelocity;
			this._rigidbody.linearVelocity = Vector2.zero;
		}
		private UnityAction<bool> Movement => (bool isAttacking) =>
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
					Vector2 targetPosition = GuwbaTransformer<CommandGuwba>.Position;
					if (Vector2.Distance(this.transform.position, targetPosition) >= this._movementDistance)
					{
						GuwbaTransformer<CommandGuwba>._returnAttack = true;
						_returnAttack = true;
					}
					if (_returnAttack)
					{
						Vector2 targetDirection = ((Vector2)this.transform.position - targetPosition).normalized;
						float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
						this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
						this._rigidbody.linearVelocity = this._movementSpeed * -this.transform.up;
						if (_grabObject)
							_grabObject.transform.position = this.transform.position;
					}
					else
						this._rigidbody.linearVelocity = this._movementSpeed * this.transform.up;
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => _instance.enabled);
				}
			}
		};
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_returnAttack || this._isAttacking)
				return;
			if (other.TryGetComponent<GrabBody>(out var grabBody) && grabBody.IsGrabtable)
			{
				GuwbaTransformer<CommandGuwba>._returnAttack = true;
				_returnAttack = true;
				GuwbaTransformer<CommandGuwba>._grabObject = grabBody;
				GuwbaTransformer<VisualGuwba>._grabObject = grabBody;
				_grabObject = grabBody;
				_grabObject.Stop((ushort)this.gameObject.layer);
			}
			else if (other.TryGetComponent<IDamageable>(out var damageable))
			{
				GuwbaTransformer<CommandGuwba>._returnAttack = true;
				_returnAttack = true;
				if (damageable.Damage(this._damage))
					EffectsController.SetHitStop(this._hitStopTime, this._hitSlowTime);
			}
		}
	};
};
