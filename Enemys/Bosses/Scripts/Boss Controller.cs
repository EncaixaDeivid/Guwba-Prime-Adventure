using UnityEngine;
using UnityEngine.Events;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(TransitionController))]
	internal abstract class BossController : StateController
	{
		protected SpriteRenderer _spriteRenderer;
		protected Animator _animator;
		protected Rigidbody2D _rigidybody;
		protected Collider2D _collider;
		protected UnityAction<bool> _toggleEvent;
		protected UnityAction<ushort> _indexEvent;
		protected UnityAction _reactToDamageEvent;
		private Vector2 _guardVelocity = new();
		private float _guardGravityScale = 0f;
		protected short _movementSide = 1;
		[Header("Boss Controller"), SerializeField] protected LayerMask _groundLayer;
		[SerializeField] protected LayerMask _targetLayerMask;
		[SerializeField] private float _groundSize;
		[SerializeField] protected string _idle, _walk, _dash, _jump, _fall;
		[SerializeField] protected ushort _movementSpeed;
		[SerializeField] private ushort _damage;
		[SerializeField] protected bool _invertMovementSide, _hasToggle, _hasIndex, _reactToDamage;
		protected new void Awake()
		{
			base.Awake();
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
			this._animator = this.GetComponent<Animator>();
			this._rigidybody = this.GetComponent<Rigidbody2D>();
			this._collider = this.GetComponent<Collider2D>();
			this._guardGravityScale = this._rigidybody.gravityScale;
			this._movementSide = (short)(this._invertMovementSide ? -1f : 1f);
		}
		private void OnEnable()
		{
			this._animator.enabled = true;
			this._rigidybody.gravityScale = this._guardGravityScale;
			this._rigidybody.linearVelocity = this._guardVelocity;
		}
		private void OnDisable()
		{
			this._animator.enabled = false;
			this._guardVelocity = this._rigidybody.linearVelocity;
			this._rigidybody.gravityScale = 0f;
			this._rigidybody.linearVelocity = Vector2.zero;
		}
		protected bool SurfacePerception()
		{
			Vector2 size = new(this._collider.bounds.size.x - this._groundSize, this._collider.bounds.size.y - this._groundSize);
			return Physics2D.BoxCast(this.transform.position, size, 0f, -this.transform.up, this._groundSize, this._groundLayer);
		}
		protected void FixedUpdate()
		{
			if (this.SurfacePerception())
			{
				this._animator.SetBool(this._idle, true);
				this._animator.SetBool(this._jump, false);
				this._animator.SetBool(this._fall, false);
				this.Toggle<RunnerBoss>(true);
			}
			else if (this._rigidybody.linearVelocityY > 0f)
			{
				this._animator.SetBool(this._idle, false);
				this._animator.SetBool(this._jump, true);
				this._animator.SetBool(this._fall, false);
			}
			else if (this._rigidybody.linearVelocityY < 0f)
			{
				this._animator.SetBool(this._idle, false);
				this._animator.SetBool(this._jump, false);
				this._animator.SetBool(this._fall, true);
				this._collider.isTrigger = false;
			}
		}
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.TryGetComponent<IDamageable>(out var damageable))
				damageable.Damage(this._damage);
		}
		internal void Toggle<BossInstance>(bool toggleValue) where BossInstance : BossController
		{
			BossInstance bossInstance = this.GetComponent<BossInstance>();
			if ((bool)bossInstance?._hasToggle)
				bossInstance?._toggleEvent?.Invoke(toggleValue);
		}
		internal void Index<BossInstance>(ushort indexValue) where BossInstance : BossController
		{
			BossInstance bossInstance = this.GetComponent<BossInstance>();
			if ((bool)bossInstance?._hasIndex)
				bossInstance?._indexEvent?.Invoke(indexValue);
		}
		internal void Index(ushort indexValue)
		{
			if (this._hasIndex)
				this._indexEvent?.Invoke(indexValue);
		}
		internal void ReactToDamage<BossInstance>() where BossInstance : BossController
		{
			BossInstance bossInstance = this.GetComponent<BossInstance>();
			if ((bool)bossInstance?._reactToDamage)
				bossInstance?._reactToDamageEvent?.Invoke();
		}
		internal void ReactToDamage()
		{
			if (this._reactToDamage)
				this._reactToDamageEvent?.Invoke();
		}
		internal void Destroy(BossProp bossProp)
		{
			if (bossProp)
			{
				if (!DataFile.DeafetedBosses[ushort.Parse($"{this.gameObject.scene.name[^1]}") - 1])
					DataFile.DeafetedBosses[ushort.Parse($"{this.gameObject.scene.name[^1]}") - 1] = true;
				this.GetComponent<TransitionController>().Transicion();
			}
		}
		[RequireComponent(typeof(Transform), typeof(Collider2D))]
		internal abstract class BossProp : StateController
		{
			protected Collider2D _collider;
			protected bool _useDestructuion = false;
			[Header("Boss Prop"), SerializeField] protected BossController[] _bossesControllers;
			[SerializeField] protected LayerMask _groundLayer, _targetLayerMask;
			[SerializeField] private bool _destructBoss, _saveOnSpecifics;
			[SerializeField] protected bool _multipleReact, _multipleIndex, _indexReact;
			[SerializeField] protected ushort _bossIndex, _indexEvent;
			protected BossInstance SpecificBoss<BossInstance>() where BossInstance : BossController
			{
				foreach (var bossController in this._bossesControllers)
					if (bossController is BossInstance)
						return bossController as BossInstance;
				return null;
			}
			private new void Awake()
			{
				base.Awake();
				this._collider = this.GetComponent<Collider2D>();
			}
			private new void OnDestroy()
			{
				base.OnDestroy();
				if (!this._useDestructuion)
					return;
				if (this._saveOnSpecifics)
					DataFile.GeneralObjects.Add(this.gameObject.name);
				if (this._destructBoss)
					this._bossesControllers[0].Destroy(this);
			}
		};
	};
};
