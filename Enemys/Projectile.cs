using UnityEngine;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
	public sealed class Projectile : StateController, IGrabtable
	{
		private Animator _animator;
		private Rigidbody2D _rigidbody;
		private readonly List<Projectile> _projectiles = new();
		private Vector2 _guardVelocity = new();
		private Vector2Int _oldCellPosition = new();
		private Vector2Int _cellPosition = new();
		private ushort _angleMulti = 0;
		private ushort _pointToJump = 0;
		private ushort _pointToBreak = 0;
		private ushort _internalBreakPoint = 0;
		private ushort _pointToReturn = 0;
		private ushort _internalReturnPoint = 0;
		private bool _isParalyzed = false, _breakInUse = false;
		[Header("Projectile")]
		[SerializeField, Tooltip("The second projectile this will instantiate.")] private Projectile _secondProjectile;
		[SerializeField, Tooltip("The second projectile this will instantiate.")] private EnemyController _enemyOnDeath;
		[SerializeField, Tooltip("The layer mask to identify the ground.")] private LayerMask _groundLayerMask;
		[SerializeField, Tooltip("The fore mode to applied in the projectile.")] private ForceMode2D _forceMode;
		[SerializeField, Tooltip("If this projectile will use force mode to move.")] private bool _useForce;
		[SerializeField, Tooltip("If this projectile won't move.")] private bool _stayInPlace;
		[SerializeField, Tooltip("If this peojectile will move in side ways.")] private bool _sideMovement;
		[SerializeField, Tooltip("If this projectile will move in the opposite way.")] private bool _invertSide;
		[SerializeField, Tooltip("If the rotation of this projectile will be used.")] private bool _useSelfRotation;
		[SerializeField, Tooltip("If the rotation of this projectile impacts its movement.")] private bool _rotationMatter;
		[SerializeField, Tooltip("If this projectile will instantiate another ones in an amount of quantity.")] private bool _useQuantity;
		[SerializeField, Tooltip("If this projectile will instantiate another after its death.")] private bool _inDeath;
		[SerializeField, Tooltip("If this projectile won't cause any type of damage.")] private bool _isInoffensive;
		[SerializeField, Tooltip("The amount of speed this projectile will move.")] private ushort _movementSpeed;
		[SerializeField, Tooltip("The amount of damage this projectile will cause to a target.")] private ushort _damage;
		[SerializeField, Tooltip("The amount of second projectiles to instantiate.")] private ushort _quantityToSummon;
		[SerializeField, Tooltip("The amount of speed the rotation spins.")] private float _rotationSpeed;
		[SerializeField, Tooltip("The angle the second projectile will be instantiated.")] private float _baseAngle;
		[SerializeField, Tooltip("The angle the second projectile have to be spreaded")] private float _spreadAngle;
		[SerializeField, Tooltip("The amount of time this projectile will exists after fade away.")] private float _timeToFade;
		[Header("Cell Projectile")]
		[SerializeField, Tooltip("If the second projectile will be instantiated in a cell.")] private bool _inCell;
		[SerializeField, Tooltip("If the second projectile will instantiate in a continuos sequence.")] private bool _continuosSummon;
		[SerializeField, Tooltip("If the instantiation of the second projectile will break after a moment.")] private bool _useBreak;
		[SerializeField, Tooltip("If the instantiation of the second projectile will always break after the first.")] private bool _alwaysBreak;
		[SerializeField, Tooltip("If the points of break are randomized between the maximum and minimum.")] private bool _randomBreak;
		[SerializeField, Tooltip("If the break point is restricted at a specific break point.")] private bool _extrictRandom;
		[SerializeField, Tooltip("The amount of cell points to jump the instantiation.")] private ushort _jumpPoints;
		[SerializeField, Tooltip("The exact point where the break of the instantiantion start.")] private ushort _breakPoint;
		[SerializeField, Tooltip("The exact point where the instantiation returns.")] private ushort _returnPoint;
		[SerializeField, Tooltip("The minimum value the break point can break.")] private ushort _minimumRandomValue;
		[SerializeField, Tooltip("The distance of the range ray to the instantiation.")] private float _distanceRay;
		private void CommonInstance()
		{
			for (ushort i = 0; i < this._quantityToSummon; i++)
			{
				Quaternion rotation;
				if (this._useSelfRotation)
				{
					float selfRotation = this.transform.eulerAngles.z;
					rotation = Quaternion.AngleAxis(selfRotation + this._baseAngle + this._spreadAngle * i, Vector3.forward);
				}
				else
					rotation = Quaternion.AngleAxis(this._baseAngle + this._spreadAngle * i, Vector3.forward);
				Instantiate(this._secondProjectile, this.transform.position, rotation);
			}
		}
		private void CellInstance()
		{
			if (this._oldCellPosition != this._cellPosition)
			{
				this._oldCellPosition = this._cellPosition;
				if (this._pointToJump == 0f)
				{
					if (this._pointToBreak >= this._internalBreakPoint)
						if (this._pointToReturn++ >= this._internalReturnPoint)
						{
							this._pointToBreak = 0;
							this._breakInUse = this._alwaysBreak;
						}
					if (!this._breakInUse || this._pointToBreak < this._internalBreakPoint)
					{
						if (this._breakInUse)
						{
							this._pointToBreak++;
							this._pointToReturn = 0;
						}
						this._pointToJump = this._jumpPoints;
						float angle = this._baseAngle + this._spreadAngle * this._angleMulti;
						Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
						Vector2 position = new(this._cellPosition.x + .5f, this._cellPosition.y + .5f);
						if (this._useQuantity)
							this._projectiles.Add(Instantiate(this._secondProjectile, position, rotation));
						else
							Instantiate(this._secondProjectile, position, rotation);
						this._angleMulti++;
					}
				}
				else if (this._pointToJump > 0f)
					this._pointToJump--;
			}
		}
		private void CellInstanceRange()
		{
			float distance = Physics2D.Raycast(this.transform.position, this.transform.up, this._distanceRay, this._groundLayerMask).distance;
			if (this._useQuantity)
				distance = this._quantityToSummon;
			short xAxis = (short)this._cellPosition.x;
			short yAxis = (short)this._cellPosition.y;
			for (ushort i = 0; i < distance; i++)
			{
				xAxis += (short)this.transform.up.x;
				yAxis += (short)this.transform.up.y;
				this._cellPosition = new Vector2Int(xAxis, yAxis);
				this.CellInstance();
			}
		}
		private void CellInstanceOnce()
		{
			if (this._useQuantity && this._quantityToSummon == this._projectiles.Count || this._stayInPlace || this._isParalyzed)
				return;
			this._cellPosition = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
			this.CellInstance();
		}
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
			this._rigidbody = this.GetComponent<Rigidbody2D>();
			this._pointToJump = this._jumpPoints;
			this._breakInUse = this._useBreak;
			this._internalBreakPoint = this._breakPoint;
			this._internalReturnPoint = this._returnPoint;
			if (this._randomBreak)
			{
				this._internalBreakPoint = (ushort)Random.Range(this._breakPoint, this._returnPoint - this._minimumRandomValue);
				if (this._internalReturnPoint - this._internalBreakPoint < this._minimumRandomValue)
					for (ushort i = 0; i < this._minimumRandomValue - (this._internalReturnPoint - this._internalBreakPoint); i++)
						if (this._internalBreakPoint <= this._minimumRandomValue)
							this._internalReturnPoint++;
						else
							this._internalBreakPoint--;
				else if (this._extrictRandom && this._internalReturnPoint - this._internalBreakPoint > this._minimumRandomValue)
					for (ushort i = 0; i < this._minimumRandomValue - (this._internalReturnPoint - this._internalBreakPoint); i++)
						if (this._internalBreakPoint <= this._minimumRandomValue)
							this._internalBreakPoint++;
						else
							this._internalReturnPoint--;
			}
			this._cellPosition = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
			this._oldCellPosition = this._cellPosition;
			if (this._sideMovement)
				this.transform.rotation = Quaternion.AngleAxis(this._invertSide ? 90f : -90f, Vector3.forward);
			if (this._secondProjectile && this._inCell && !this._continuosSummon)
				this.CellInstanceRange();
			else if (this._secondProjectile && !this._inCell)
				this.CommonInstance();
			Destroy(this.gameObject, this._timeToFade);
		}
		private void OnEnable()
		{
			this._animator.enabled = true;
			this._rigidbody.linearVelocity = this._guardVelocity;
		}
		private void OnDisable()
		{
			this._animator.enabled = false;
			this._guardVelocity = this._rigidbody.linearVelocity;
			this._rigidbody.linearVelocity = Vector2.zero;
		}
		private void Start()
		{
			if (!this._stayInPlace)
				if (this._useForce)
					this._rigidbody.AddForce((this._invertSide ? -this.transform.up : this.transform.up) * this._movementSpeed, this._forceMode);
				else
					this._rigidbody.linearVelocity = (this._invertSide ? -this.transform.up : this.transform.up) * this._movementSpeed;
		}
		private void FixedUpdate()
		{
			if (this._isParalyzed)
				return;
			if (this._secondProjectile && this._inCell && this._continuosSummon)
				this.CellInstanceOnce();
			this._rigidbody.rotation += this._rotationSpeed * this._movementSpeed * Time.fixedDeltaTime;
			if (!this._stayInPlace && this._rotationMatter)
				this._rigidbody.linearVelocity = (this._invertSide ? -this.transform.up : this.transform.up) * this._movementSpeed;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._isParalyzed && this._isInoffensive)
				return;
			if (other.TryGetComponent<IDamageable>(out var damageable))
			{
				if (damageable.Damage(this._damage))
					Destroy(this.gameObject);
			}
			else
			{
				if (this._inDeath)
					if (this._enemyOnDeath)
						Instantiate(this._enemyOnDeath);
					else if (this._secondProjectile)
						if (this._inCell)
							this.CellInstanceRange();
						else
							this.CommonInstance();
				Destroy(this.gameObject);
			}
		}
		public void Paralyze(bool value) => this._isParalyzed = value;
	};
};
