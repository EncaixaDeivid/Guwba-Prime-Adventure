using UnityEngine;
using UnityEngine.Events;
namespace GuwbaPrimeAdventure.Enemy
{
	[RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
	internal abstract class EnemyController : StateController, IGrabtable, IDamageable
    {
		protected SpriteRenderer _spriteRenderer;
		protected Animator _animator;
		protected Rigidbody2D _rigidybody;
		protected Collider2D _collider;
		protected UnityAction<bool> _toggleEvent;
		private Vector2 _guardVelocity = new();
		private float _gravityScale = 0f;
		protected short _movementSide = 1;
		private bool _paralyzed = false;
		[Header("Enemy Controller"), SerializeField] protected LayerMask _groundLayer;
		[SerializeField] protected LayerMask _targetLayerMask;
		[SerializeField] private short _vitality;
		[SerializeField] protected ushort _movementSpeed;
		[SerializeField] private ushort _damage;
		[SerializeField] protected bool _stopMovement;
		[SerializeField] private bool _invertMovementSide, _noDamage, _noContactDamage, _saveObject;
		protected bool Paralyzed => this._paralyzed;
		protected new void Awake()
		{
			base.Awake();
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
			this._animator = this.GetComponent<Animator>();
			this._rigidybody = this.GetComponent<Rigidbody2D>();
			this._collider = this.GetComponent<Collider2D>();
			this._gravityScale = this._rigidybody.gravityScale;
			this._movementSide = (short)(this._invertMovementSide ? -1 : 1);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (this._saveObject)
				SaveFileData.GeneralObjects.Add(this.gameObject.name);
		}
		private void OnEnable()
		{
			this._animator.enabled = true;
			this._rigidybody.gravityScale = this._gravityScale;
			this._rigidybody.linearVelocity = this._guardVelocity;
		}
		private void OnDisable()
		{
			this._animator.enabled = false;
			this._guardVelocity = this._rigidybody.linearVelocity;
			this._rigidybody.linearVelocity = Vector2.zero;
		}
		private void OnTrigger(GameObject collisionObject)
		{
			if (!this._paralyzed && !this._noContactDamage && collisionObject.TryGetComponent<IDamageable>(out var damageable))
				damageable.Damage(this._damage);
		}
		private void OnTriggerEnter2D(Collider2D other) => this.OnTrigger(other.gameObject);
		private void OnTriggerStay2D(Collider2D other) => this.OnTrigger(other.gameObject);
		internal void Toggle<EnemyInstance>(bool toggleValue) where EnemyInstance : EnemyController
		{
			if (this.TryGetComponent<EnemyInstance>(out var enemyInstance))
				enemyInstance?._toggleEvent?.Invoke(toggleValue);
		}
		public void Paralyze() => this._paralyzed = true;
		public bool Damage(ushort damage)
		{
			if (this._noDamage || this._paralyzed)
				return false;
			this._vitality -= (short)damage;
			if (this._vitality <= 0)
				Destroy(this.gameObject);
			return true;
		}
	};
};