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
		private float _gravityScale = 0f;
		private bool _isThrew = false;
		[Header("Grab Body"), SerializeField] private LayerMask _hitLayers;
		[SerializeField] private LayerMask _noHitLayers;
		[SerializeField] private ushort _throwSpeed, _throwDamage;
		[SerializeField] private float _throwHitStopTime, _throwHitSlowTime, _fadeTime, _timeToBack;
		[SerializeField] private bool _isNotGrabtable, _gravityOnThrow;
		internal bool IsGrabtable => !this._isNotGrabtable;
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
			this.GetComponent<IGrabtable>()?.Paralyze();
			if (this._gravityOnThrow)
				this._gravityScale = this._rigidbody.gravityScale;
			this.gameObject.layer = objectLayer;
			this.transform.parent = null;
			this._rigidbody.gravityScale = 0f;
			this._rigidbody.linearVelocity = Vector2.zero;
			foreach (Collider2D collider in this.GetComponents<Collider2D>())
			{
				collider.isTrigger = true;
				collider.includeLayers = this._hitLayers;
				collider.excludeLayers = this._noHitLayers;
				collider.contactCaptureLayers = this._hitLayers;
				collider.callbackLayers = this._hitLayers;
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
	};
};
