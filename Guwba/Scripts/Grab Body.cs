using UnityEngine;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer))]
	[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
	public sealed class GrabBody : StateController
	{
		private Rigidbody2D _rigidbody;
		private Transform _parent;
		private Collider2D[] _colliders;
		private LayerMask[,] _layerMasks;
		private Vector2 _guardVelocity = new();
		private bool[] _isTrigger;
		private int _layer;
		private float _gravityScale = 0f;
		private bool _isThrew = false;
		[Header("Throw Stats")]
		[SerializeField, Tooltip("The layers that the object will can collide.")] private LayerMask _hitLayers;
		[SerializeField, Tooltip("The layers that the object will cannot collide.")] private LayerMask _noHitLayers;
		[SerializeField, Tooltip("The amount of speed that the object will be throw.")] private ushort _throwSpeed;
		[SerializeField, Tooltip("The amount of damage that the object will give.")] private ushort _throwDamage;
		[SerializeField, Tooltip("The quantity of hits that this object need to break.")] private ushort _hitsToDestruct;
		[SerializeField, Tooltip("The gravity of the object at the throw.")] private float _throwGravity;
		[SerializeField, Tooltip("The amount of time to stop the game when hit is given at throw.")] private float _throwHitStopTime;
		[SerializeField, Tooltip("The amount of time to slow the game when hit is given at throw.")] private float _throwHitSlowTime;
		[SerializeField, Tooltip("The amount of time that is given to the object to fade away.")] private float _fadeTime;
		[Header("Object Stats")]
		[SerializeField, Tooltip("Indicates if the object is grabtable.")] private bool _isNotGrabtable;
		[SerializeField, Tooltip("Indicates if the object is damageable.")] private bool _isDamageable;
		[SerializeField, Tooltip("Indicates if the object is indestructible.")] private bool _isIndestructible;
		[SerializeField, Tooltip("Indicates if the object will fade away after the throw.")] private bool _fadeAway;
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
					this.gameObject.layer = this._layer;
					for (ushort i = 0; i < this._colliders.Length; i++)
					{
						this._colliders[i].includeLayers = this._layerMasks[0, i];
						this._colliders[i].excludeLayers = this._layerMasks[1, i];
						this._colliders[i].contactCaptureLayers = this._layerMasks[2, i];
						this._colliders[i].callbackLayers = this._layerMasks[3, i];
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
			this._parent = this.transform.parent;
			this._layer = this.gameObject.layer;
			this.GetComponent<IGrabtable>()?.Paralyze(true);
			this._rigidbody.bodyType = RigidbodyType2D.Kinematic;
			this._gravityScale = this._rigidbody.gravityScale;
			this.gameObject.layer = objectLayer;
			this.transform.parent = null;
			this._rigidbody.gravityScale = 0f;
			this._rigidbody.linearVelocity = Vector2.zero;
			this._isTrigger = new bool[this._colliders.Length];
			this._layerMasks = new LayerMask[4, this._colliders.Length];
			for (ushort i = 0; i < this._colliders.Length; i++)
			{
				this._isTrigger[i] = this._colliders[i].isTrigger;
				this._layerMasks[0, i] = this._colliders[i].includeLayers;
				this._layerMasks[1, i] = this._colliders[i].excludeLayers;
				this._layerMasks[2, i] = this._colliders[i].contactCaptureLayers;
				this._layerMasks[3, i] = this._colliders[i].callbackLayers;
				this._colliders[i].isTrigger = true;
				this._colliders[i].includeLayers = this._hitLayers;
				this._colliders[i].excludeLayers = this._noHitLayers;
				this._colliders[i].contactCaptureLayers = this._hitLayers;
				this._colliders[i].callbackLayers = this._hitLayers;
			}
		}
		internal void Throw(Vector2 direction)
		{
			this.transform.parent = null;
			this._rigidbody.bodyType = RigidbodyType2D.Dynamic;
			this._rigidbody.gravityScale = this._throwGravity;
			this._isThrew = true;
			for (ushort i = 0; i < this._colliders.Length; i++)
				this._colliders[i].isTrigger = this._isTrigger[i];
			this._rigidbody.AddForce(direction * this._throwSpeed, ForceMode2D.Force);
			if (this._fadeAway)
				Destroy(this.gameObject, this._fadeTime);
		}
		internal void Drop()
		{
			this.GetComponent<IGrabtable>()?.Paralyze(false);
			this._rigidbody.bodyType = RigidbodyType2D.Dynamic;
			this.transform.parent = this._parent;
			if (this._gravityScale != 0f)
				this._rigidbody.gravityScale = this._gravityScale;
			this.gameObject.layer = this._layer;
			for (ushort i = 0; i < this._colliders.Length; i++)
			{
				this._colliders[i].isTrigger = this._isTrigger[i];
				this._colliders[i].includeLayers = this._layerMasks[0, i];
				this._colliders[i].excludeLayers = this._layerMasks[1, i];
				this._colliders[i].contactCaptureLayers = this._layerMasks[2, i];
				this._colliders[i].callbackLayers = this._layerMasks[3, i];
			}
		}
	};
};
