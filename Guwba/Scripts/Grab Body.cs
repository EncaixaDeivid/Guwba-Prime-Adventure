using UnityEngine;
using GuwbaPrimeAdventure.Effects;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
	public sealed class GrabBody : StateController
	{
		private Rigidbody2D _rigidbody;
		private Vector2 _guardVelocity = new();
		private (int _layer, Transform _parent, LayerMask[] _include, LayerMask[] _exclude, LayerMask[] _contact, LayerMask[] _callback) _backDrop;
		private float _gravityScale = 0f;
		private bool _isThrew = false;
		[Header("Grab Body"), SerializeField] private LayerMask _hitLayers;
		[SerializeField] private LayerMask _noHitLayers;
		[SerializeField] private ushort _throwSpeed, _throwDamage;
		[SerializeField] private float _throwHitStopTime, _throwHitSlowTime, _fadeTime, _timeToBack;
		[SerializeField] private bool _isNotGrabtable, _gravityOnThrow, _isDamageable;
		internal bool IsGrabtable => !this._isNotGrabtable;
		internal bool IsDamageable => this._isDamageable;
		private new void Awake()
		{
			base.Awake();
			this._rigidbody = this.GetComponent<Rigidbody2D>();
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
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._isThrew)
				if (other.TryGetComponent<IDamageable>(out var damageable) && damageable.Damage(this._throwDamage))
				{
					EffectsController.SetHitStop(this._throwHitStopTime, this._throwHitSlowTime);
					Destroy(this.gameObject);
				}
		}
		internal void Stop(ushort objectLayer)
		{
			this._backDrop._layer = this.gameObject.layer;
			this._backDrop._parent = this.transform.parent;
			this.GetComponent<IGrabtable>()?.Paralyze();
			if (this._gravityOnThrow || this._rigidbody.gravityScale != 0f)
				this._gravityScale = this._rigidbody.gravityScale;
			this.gameObject.layer = objectLayer;
			this.transform.parent = null;
			this._rigidbody.gravityScale = 0f;
			this._rigidbody.linearVelocity = Vector2.zero;
			Collider2D[] colliders = this.GetComponents<Collider2D>();
			this._backDrop._include = new LayerMask[colliders.Length];
			this._backDrop._exclude = new LayerMask[colliders.Length];
			this._backDrop._contact = new LayerMask[colliders.Length];
			this._backDrop._callback = new LayerMask[colliders.Length];
			for (ushort i = 0; i < colliders.Length; i++)
			{
				this._backDrop._include[i] = colliders[i].includeLayers;
				this._backDrop._exclude[i] = colliders[i].excludeLayers;
				this._backDrop._contact[i] = colliders[i].contactCaptureLayers;
				this._backDrop._callback[i] = colliders[i].callbackLayers;
				colliders[i].isTrigger = true;
				colliders[i].includeLayers = this._hitLayers;
				colliders[i].excludeLayers = this._noHitLayers;
				colliders[i].contactCaptureLayers = this._hitLayers;
				colliders[i].callbackLayers = this._hitLayers;
			}
		}
		internal void Throw(Vector2 direction)
		{
			if (this._gravityOnThrow)
				this._rigidbody.gravityScale = this._gravityScale;
			this._isThrew = true;
			this._rigidbody.linearVelocity = direction * this._throwSpeed;
			Destroy(this.gameObject, this._fadeTime);
		}
		internal void Drop()
		{
			this.GetComponent<IGrabtable>()?.Unparalyze();
			if (this._gravityScale != 0f)
				this._rigidbody.gravityScale = _gravityScale;
			this.gameObject.layer = this._backDrop._layer;
			this.transform.parent = this._backDrop._parent;
			Collider2D[] colliders = this.GetComponents<Collider2D>();
			for (ushort i = 0; i < colliders.Length; i ++)
			{
				colliders[i].isTrigger = false;
				colliders[i].includeLayers = this._backDrop._include[i];
				colliders[i].excludeLayers = this._backDrop._exclude[i];
				colliders[i].contactCaptureLayers = this._backDrop._contact[i];
				colliders[i].callbackLayers = this._backDrop._callback[i];
			}
		}
	};
};
