using UnityEngine;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
	internal sealed class Projectile : StateController, IGrabtable
	{
		private Animator _animator;
		private Rigidbody2D _rigidbody;
		private readonly List<Projectile> _projectiles = new();
		private Vector2 _guardVelocity = new();
		private Vector2Int _oldCellPosition = new(), _cellPosition = new();
		private ushort _angleMulti = 0, _pointToJump = 0, _pointToBreak = 0, _internalBreakPoint = 0, _pointToReturn = 0, _internalReturnPoint = 0;
		private bool _isParalyzed = false, _breakInUse = false;
		[Header("Projectile"), SerializeField] private Projectile _secondProjectile;
		[SerializeField] private LayerMask _groundLayerMask;
		[SerializeField] private bool
			_isInoffensive,
			_stayInPlace,
			_sideMovement,
			_invertSide,
			_rotationMatter,
			_useQuantity,
			_inCell,
			_continuosSummon,
			_inDeath,
			_useBreak,
			_alwaysBreak,
			_randomBreak,
			_extrictRandom;
		[SerializeField] private ushort _movementSpeed, _damage, _quantityToSummon, _jumpPoints, _breakPoint, _returnPoint, _minimumRandomValue;
		[SerializeField] private float _rotationSpeed, _baseAngle, _spreadAngle, _timeToFade, _distanceRay;
		private void CommonInstance()
		{
			for (ushort i = 0; i < this._quantityToSummon; i++)
			{
				Quaternion rotation = Quaternion.AngleAxis(this._baseAngle + this._spreadAngle * i, Vector3.forward);
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
				this._cellPosition = new(xAxis, yAxis);
				this.CellInstance();
			}
		}
		private void CellInstanceOnce()
		{
			if (this._useQuantity && this._quantityToSummon == this._projectiles.Count || this._stayInPlace || this._isParalyzed)
				return;
			this._cellPosition = new((int)this.transform.position.x, (int)this.transform.position.y);
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
			this._cellPosition = new((int)this.transform.position.x, (int)this.transform.position.y);
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
				this._rigidbody.linearVelocity = (this._invertSide ? -this.transform.up : this.transform.up) * this._movementSpeed;
		}
		private void FixedUpdate()
		{
			if (this._isParalyzed)
				return;
			if (this._secondProjectile && this._inCell && this._continuosSummon)
				this.CellInstanceOnce();
			this._rigidbody.rotation += this._rotationSpeed * this._movementSpeed * Time.deltaTime;
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
				if (this._inDeath && this._secondProjectile)
					if (this._inCell)
						this.CellInstanceRange();
					else
						this.CommonInstance();
				Destroy(this.gameObject);
			}
		}
		public void Paralyze() => this._isParalyzed = true;
	};
};
