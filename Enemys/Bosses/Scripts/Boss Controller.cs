using UnityEngine;
using System;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[RequireComponent(typeof(Transform), typeof(Rigidbody2D), typeof(Collider2D))]
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
		[Header("Boss Controller")]
		[SerializeField, Tooltip("The layer mask to identify the ground.")] protected LayerMask _groundLayer;
		[SerializeField, Tooltip("The layer mask to identify the target of the attacks.")] protected LayerMask _targetLayerMask;
		[Header("Animation")]
		[SerializeField, Tooltip("Animation parameter.")] protected string _idle;
		[SerializeField, Tooltip("Animation parameter.")] protected string _walk;
		[SerializeField, Tooltip("Animation parameter.")] protected string _dash;
		[SerializeField, Tooltip("Animation parameter.")] protected string _jump;
		[SerializeField, Tooltip("Animation parameter.")] protected string _fall;
		[Header("Boss Stats")]
		[SerializeField, Tooltip("The size of the ground identifier.")] private float _groundSize;
		[SerializeField, Tooltip("The amount of speed to move the boss.")] protected ushort _movementSpeed;
		[SerializeField, Tooltip("The maount of damage to hit the target.")] private ushort _damage;
		[SerializeField, Tooltip("If the boss will move firstly to the left.")] protected bool _invertMovementSide;
		[SerializeField, Tooltip("If this boss will not do damage.")] private bool _noDealDamage;
		[Header("Boss Events")]
		[SerializeField, Tooltip("If this boss has a toggle atribute to switch.")] protected bool _hasToggle;
		[SerializeField, Tooltip("If this boss has a index atribute to use.")] protected bool _hasIndex;
		[SerializeField, Tooltip("If this boss will react to any damage taken.")] protected bool _reactToDamage;
		[SerializeField, Tooltip("If this boss will start a trancision.")] private bool _isTransitioner;
		[SerializeField, Tooltip("If this boss have any dialog to start after his death.")] private bool _haveDialog;
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
			Vector2 point = new(this.transform.position.x, this.transform.position.y - this._collider.bounds.extents.y - this._groundSize / 2f);
			Vector2 size = new(this._collider.bounds.size.x - 0.025f, this._groundSize);
			return Physics2D.OverlapBox(point, size, this.transform.eulerAngles.z, this._groundLayer);
		}
		protected void FixedUpdate()
		{
			if (this.SurfacePerception())
			{
				this._animator.SetBool(this._idle, true);
				this._animator.SetBool(this._jump, false);
				this._animator.SetBool(this._fall, false);
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
			if (!this._noDealDamage && collision.TryGetComponent<IDamageable>(out var damageable))
				damageable.Damage(this._damage);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			BossType bossType = (BossType)additionalData;
			if (bossType.HasFlag(BossType.Controller) && data.StateForm == StateForm.Disable && !_isDeafeted)
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
	};
	[Flags]
	internal enum BossType
	{
		All,
		Controller,
		Runner,
		Jumper,
		Summoner,
		Weak,
		Place
	};
};
