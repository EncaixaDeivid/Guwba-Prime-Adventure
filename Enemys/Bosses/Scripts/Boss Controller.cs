using UnityEngine;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Transitioner)), RequireComponent(typeof(IInteractable))]
	internal abstract class BossController : StateController, IConnector
	{
		protected SpriteRenderer _spriteRenderer;
		protected Animator _animator;
		protected Rigidbody2D _rigidybody;
		protected Collider2D _collider;
		private Vector2 _guardVelocity = new();
		private float _guardGravityScale = 0f;
		protected short _movementSide = 1;
		private static bool _isDeafeted = false;
		[Header("Boss Controller"), SerializeField] protected LayerMask _groundLayer;
		[SerializeField] protected LayerMask _targetLayerMask;
		[SerializeField] private float _groundSize;
		[SerializeField] protected string _idle, _walk, _dash, _jump, _fall;
		[SerializeField] protected ushort _movementSpeed;
		[SerializeField] private ushort _damage;
		[SerializeField] protected bool _invertMovementSide, _hasToggle, _hasIndex, _reactToDamage;
		[SerializeField] private bool _haveDialog, _isTransitioner;
		public PathConnection PathConnection => PathConnection.Boss;
		protected new void Awake()
		{
			base.Awake();
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
			this._animator = this.GetComponent<Animator>();
			this._rigidybody = this.GetComponent<Rigidbody2D>();
			this._collider = this.GetComponent<Collider2D>();
			this._guardGravityScale = this._rigidybody.gravityScale;
			this._movementSide = (short)(this._invertMovementSide ? -1f : 1f);
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
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
				Sender.Create().SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Action)
					.SetBossType(BossType.Runner).SetToggle(true).Send();
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
		public void Receive(DataConnection data)
		{
			bool isValid = data.ToggleValue.HasValue && data.ToggleValue.Value;
			if (data.ConnectionState == ConnectionState.Disable && isValid && !_isDeafeted)
			{
				_isDeafeted = true;
				SaveController.Load(out SaveFile saveFile);
				SettingsController.Load(out Settings settings);
				ushort sceneIndex = (ushort)(ushort.Parse($"{this.gameObject.scene.name[^1]}") - 1f);
				if (!saveFile.deafetedBosses[sceneIndex])
				{
					saveFile.deafetedBosses[sceneIndex] = true;
					SaveController.WriteSave(saveFile);
				}
				if (settings.dialogToggle && this._haveDialog)
					this.GetComponent<IInteractable>().Interaction();
				else if (this._isTransitioner)
					this.GetComponent<Transitioner>().Transicion();
			}
		}
		[RequireComponent(typeof(Transform), typeof(Collider2D))]
		internal abstract class BossProp : StateController
		{
			protected Collider2D _collider;
			private SaveFile _saveFile;
			protected bool _useDestructuion = false;
			[Header("Boss Prop"), SerializeField] protected LayerMask _groundLayer;
			[SerializeField] protected LayerMask _targetLayerMask;
			[SerializeField] private bool _destructBoss, _saveOnSpecifics;
			[SerializeField] protected bool _indexReact;
			[SerializeField] protected ushort _indexEvent;
			private new void Awake()
			{
				base.Awake();
				this._collider = this.GetComponent<Collider2D>();
				SaveController.Load(out this._saveFile);
			}
			private new void OnDestroy()
			{
				base.OnDestroy();
				if (!this._useDestructuion)
					return;
				if (this._saveOnSpecifics && !this._saveFile.generalObjects.Contains(this.gameObject.name))
					this._saveFile.generalObjects.Add(this.gameObject.name);
				if (this._destructBoss)
					Sender.Create().SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Disable).SetToggle(true).Send();
			}
		};
	};
};
