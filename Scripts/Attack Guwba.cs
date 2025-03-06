using UnityEngine;
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
		}
		private void OnEnable() => this._rigidbody.linearVelocity = this._guardVelocity;
		private void OnDisable()
		{
			this._guardVelocity = this._rigidbody.linearVelocity;
			this._rigidbody.linearVelocity = Vector2.zero;
		}
		private void FixedUpdate()
		{
			if (!_activeState)
			{
				this._spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
				this._rigidbody.bodyType = RigidbodyType2D.Kinematic;
				this._rigidbody.linearVelocity = Vector2.zero;
				return;
			}
			this._spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
			this._rigidbody.bodyType = RigidbodyType2D.Dynamic;
			Vector2 targetPosition = GuwbaTransformer<CommandGuwba>.Position;
			if (Vector2.Distance(this.transform.position, targetPosition) >= this._movementDistance)
			{
				GuwbaTransformer<CommandGuwba>._returnState = true;
				_returnState = true;
			}
			if (_returnState)
			{
				Vector2 targetDirection = (Vector2)this.transform.position - targetPosition;
				float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
				this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				this._rigidbody.linearVelocity = this._movementSpeed * -this.transform.up;
				if (_grabObject)
					_grabObject.transform.position = this.transform.position;
			}
			else
				this._rigidbody.linearVelocity = this._movementSpeed * this.transform.up;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_returnState || !_activeState)
				return;
			if (other.TryGetComponent<GrabBody>(out var grabBody) && grabBody.IsGrabtable)
			{
				GuwbaTransformer<CommandGuwba>._returnState = true;
				_returnState = true;
				GuwbaTransformer<CommandGuwba>._grabObject = grabBody;
				GuwbaTransformer<VisualHudGuwba>._grabObject = grabBody;
				_grabObject = grabBody;
				_grabObject.Stop((ushort)this.gameObject.layer);
			}
			else if (other.TryGetComponent<IDamageable>(out var damageable))
			{
				GuwbaTransformer<CommandGuwba>._returnState = true;
				_returnState = true;
				if (damageable.Damage(this._damage))
					EffectsController.SetHitStop(this._hitStopTime, this._hitSlowTime);
			}
		}
	};
};