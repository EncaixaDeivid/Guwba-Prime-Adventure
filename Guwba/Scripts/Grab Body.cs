using UnityEngine;
using GuwbaPrimeAdventure.Effects;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
	public sealed class GrabBody : StateController
	{
		private Rigidbody2D _rigidbody;
		private Collider2D[] _colliders;
		private Vector2 _guardVelocity = new();
		private (int _layer, bool[] _isTrigger, LayerMask[,] _layerMask) _backDrop;
		private float _gravityScale = 0f;
		private bool _isThrew = false;
		[Header("Grab Body"), SerializeField] private LayerMask _hitLayers;
		[SerializeField] private LayerMask _noHitLayers;
		[SerializeField] private ushort _throwSpeed, _throwDamage, _hitsToDestruct = 1;
		[SerializeField] private float _throwGravity, _throwHitStopTime, _throwHitSlowTime, _fadeTime;
		[SerializeField] private bool _isNotGrabtable, _isDamageable, _isIndestructible, _fadeAway;
		internal bool IsGrabtable => !this._isNotGrabtable;
		internal bool IsDamageable => this._isDamageable;
		private new void Awake()
		{
			base.Awake();
			this._rigidbody = this.GetComponent<Rigidbody2D>();
			this._colliders = this.GetComponents<Collider2D>();
		}
		private void OnEnable()
		{
			if (this._rigidbody.bodyType == RigidbodyType2D.Dynamic)
				this._rigidbody.linearVelocity = this._guardVelocity;
		}
		private void OnDisable()
		{
			if (this._rigidbody.bodyType == RigidbodyType2D.Dynamic)
			{
				this._guardVelocity = this._rigidbody.linearVelocity;
				this._rigidbody.linearVelocity = Vector2.zero;
			}
		}
		private void OnCollision(GameObject collisionObject)
		{
			if (this._isThrew)
			{
				bool isDamageable = collisionObject.TryGetComponent(out IDamageable damageable);
				if (isDamageable && damageable.Damage(this._throwDamage))
					EffectsController.SetHitStop(this._throwHitStopTime, this._throwHitSlowTime);
				if (isDamageable || collisionObject.TryGetComponent<Surface>(out _))
				{
					this._isThrew = false;
					this.gameObject.layer = this._backDrop._layer;
					for (ushort i = 0; i < this._colliders.Length; i++)
					{
						this._colliders[i].includeLayers = this._backDrop._layerMask[0, i];
						this._colliders[i].excludeLayers = this._backDrop._layerMask[1, i];
						this._colliders[i].contactCaptureLayers = this._backDrop._layerMask[2, i];
						this._colliders[i].callbackLayers = this._backDrop._layerMask[3, i];
					}
					if (!this._isIndestructible && this._hitsToDestruct-- <= 0f)
						Destroy(this.gameObject);
				}
			}
		}
		private void OnCollisionEnter2D(Collision2D other) => this.OnCollision(other.gameObject);
		private void OnTriggerEnter2D(Collider2D other) => this.OnCollision(other.gameObject);
		internal void Stop(ushort objectLayer)
		{
			this._backDrop._layer = this.gameObject.layer;
			this.GetComponent<IGrabtable>()?.Paralyze(true);
			this._gravityScale = this._rigidbody.gravityScale;
			this.gameObject.layer = objectLayer;
			this.transform.parent = null;
			this._rigidbody.gravityScale = 0f;
			this._rigidbody.linearVelocity = Vector2.zero;
			this._backDrop._isTrigger = new bool[this._colliders.Length];
			this._backDrop._layerMask = new LayerMask[4, this._colliders.Length];
			for (ushort i = 0; i < this._colliders.Length; i++)
			{
				this._backDrop._isTrigger[i] = this._colliders[i].isTrigger;
				this._backDrop._layerMask[0, i] = this._colliders[i].includeLayers;
				this._backDrop._layerMask[1, i] = this._colliders[i].excludeLayers;
				this._backDrop._layerMask[2, i] = this._colliders[i].contactCaptureLayers;
				this._backDrop._layerMask[3, i] = this._colliders[i].callbackLayers;
				this._colliders[i].isTrigger = true;
				this._colliders[i].includeLayers = this._hitLayers;
				this._colliders[i].excludeLayers = this._noHitLayers;
				this._colliders[i].contactCaptureLayers = this._hitLayers;
				this._colliders[i].callbackLayers = this._hitLayers;
			}
		}
		internal void Throw(Vector2 direction)
		{
			this._rigidbody.gravityScale = this._throwGravity;
			this._isThrew = true;
			for (ushort i = 0; i < this._colliders.Length; i++)
				this._colliders[i].isTrigger = this._backDrop._isTrigger[i];
			this._rigidbody.AddForce(direction * this._throwSpeed, ForceMode2D.Force);
			if (this._fadeAway)
				Destroy(this.gameObject, this._fadeTime);
		}
		internal void Drop()
		{
			this.GetComponent<IGrabtable>()?.Paralyze(false);
			if (this._gravityScale != 0f)
				this._rigidbody.gravityScale = this._gravityScale;
			this.gameObject.layer = this._backDrop._layer;
			for (ushort i = 0; i < this._colliders.Length; i++)
			{
				this._colliders[i].isTrigger = this._backDrop._isTrigger[i];
				this._colliders[i].includeLayers = this._backDrop._layerMask[0, i];
				this._colliders[i].excludeLayers = this._backDrop._layerMask[1, i];
				this._colliders[i].contactCaptureLayers = this._backDrop._layerMask[2, i];
				this._colliders[i].callbackLayers = this._backDrop._layerMask[3, i];
			}
		}
	};
};
