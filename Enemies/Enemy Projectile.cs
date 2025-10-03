using UnityEngine;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
	internal sealed class EnemyProjectile : StateController
	{
		private Animator _animator;
		private Rigidbody2D _rigidbody;
		private readonly List<EnemyProjectile> _projectiles = new();
		private Vector2 _guardVelocity = new();
		private Vector2Int _oldCellPosition = new();
		private Vector2Int _cellPosition = new();
		private ushort _angleMulti = 0;
		private ushort _pointToJump = 0;
		private ushort _pointToBreak = 0;
		private ushort _internalBreakPoint = 0;
		private ushort _pointToReturn = 0;
		private ushort _internalReturnPoint = 0;
		private bool _breakInUse = false;
		[Header("Projectile")]
		[SerializeField, Tooltip("The statitics of this projectile.")] private ProjectileStatistics _statistics;
		private void CommonInstance()
		{
			for (ushort i = 0; i < this._statistics.QuantityToSummon; i++)
			{
				Quaternion rotation;
				if (this._statistics.UseSelfRotation)
				{
					float selfRotation = this.transform.eulerAngles.z + this._statistics.BaseAngle + this._statistics.SpreadAngle * i;
					rotation = Quaternion.AngleAxis(selfRotation, Vector3.forward);
				}
				else
					rotation = Quaternion.AngleAxis(this._statistics.BaseAngle + this._statistics.SpreadAngle * i, Vector3.forward);
				Instantiate(this._statistics.SecondProjectile, this.transform.position, rotation);
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
							this._breakInUse = this._statistics.AlwaysBreak;
						}
					if (!this._breakInUse || this._pointToBreak < this._internalBreakPoint)
					{
						if (this._breakInUse)
						{
							this._pointToBreak++;
							this._pointToReturn = 0;
						}
						this._pointToJump = this._statistics.JumpPoints;
						float angle = this._statistics.BaseAngle + this._statistics.SpreadAngle * this._angleMulti;
						Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
						Vector2 position = new(this._cellPosition.x + .5f, this._cellPosition.y + .5f);
						if (this._statistics.UseQuantity)
							this._projectiles.Add(Instantiate(this._statistics.SecondProjectile, position, rotation));
						else
							Instantiate(this._statistics.SecondProjectile, position, rotation);
						this._angleMulti++;
					}
				}
				else if (this._pointToJump > 0f)
					this._pointToJump--;
			}
		}
		private void CellInstanceRange()
		{
			LayerMask groundLayer = this._statistics.GroundLayer;
			float distanceRay = this._statistics.DistanceRay;
			float distance = Physics2D.Raycast(this.transform.position, this.transform.up, distanceRay, groundLayer).distance;
			if (this._statistics.UseQuantity)
				distance = this._statistics.QuantityToSummon;
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
			if (this._statistics.UseQuantity && this._statistics.QuantityToSummon == this._projectiles.Count || this._statistics.StayInPlace)
				return;
			this._cellPosition = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
			this.CellInstance();
		}
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
			this._rigidbody = this.GetComponent<Rigidbody2D>();
			this._pointToJump = this._statistics.JumpPoints;
			this._breakInUse = this._statistics.UseBreak;
			this._internalBreakPoint = this._statistics.BreakPoint;
			this._internalReturnPoint = this._statistics.ReturnPoint;
			if (this._statistics.RandomBreak)
			{
				float rangeMax = this._statistics.ReturnPoint - this._statistics.MinimumRandomValue;
				bool valid = this._statistics.ExtrictRandom;
				this._internalBreakPoint = (ushort)Random.Range(this._statistics.BreakPoint, rangeMax);
				if (this._internalReturnPoint - this._internalBreakPoint < this._statistics.MinimumRandomValue)
					for (ushort i = 0; i < this._statistics.MinimumRandomValue - (this._internalReturnPoint - this._internalBreakPoint); i++)
						if (this._internalBreakPoint <= this._statistics.MinimumRandomValue)
							this._internalReturnPoint++;
						else
							this._internalBreakPoint--;
				else if (valid && this._internalReturnPoint - this._internalBreakPoint > this._statistics.MinimumRandomValue)
					for (ushort i = 0; i < this._statistics.MinimumRandomValue - (this._internalReturnPoint - this._internalBreakPoint); i++)
						if (this._internalBreakPoint <= this._statistics.MinimumRandomValue)
							this._internalBreakPoint++;
						else
							this._internalReturnPoint--;
			}
			this._cellPosition = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
			this._oldCellPosition = this._cellPosition;
			if (this._statistics.SideMovement)
				this.transform.rotation = Quaternion.AngleAxis(this._statistics.InvertSide ? 90f : -90f, Vector3.forward);
			if (this._statistics.SecondProjectile && this._statistics.InCell && !this._statistics.ContinuosSummon)
				this.CellInstanceRange();
			else if (this._statistics.SecondProjectile && !this._statistics.InCell)
				this.CommonInstance();
			Destroy(this.gameObject, this._statistics.TimeToFade);
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
			float movementSpeed = this._statistics.MovementSpeed;
			if (!this._statistics.StayInPlace)
				if (this._statistics.UseForce)
				{
					Vector2 force = (this._statistics.InvertSide ? -this.transform.up : this.transform.up) * movementSpeed;
					this._rigidbody.AddForce(force, this._statistics.ForceMode);
				}
				else
					this._rigidbody.linearVelocity = (this._statistics.InvertSide ? -this.transform.up : this.transform.up) * movementSpeed;
		}
		private void FixedUpdate()
		{
			float movementSpeed = this._statistics.MovementSpeed;
			if (this._statistics.SecondProjectile && this._statistics.InCell && this._statistics.ContinuosSummon)
				this.CellInstanceOnce();
			this._rigidbody.rotation += this._statistics.RotationSpeed * movementSpeed * Time.fixedDeltaTime;
			if (!this._statistics.StayInPlace && this._statistics.RotationMatter)
				this._rigidbody.linearVelocity = (this._statistics.InvertSide ? -this.transform.up : this.transform.up) * movementSpeed;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._statistics.IsInoffensive)
				return;
			if (other.TryGetComponent<IDestructible>(out var destructible))
			{
				if (destructible.Hurt(this._statistics.Damage))
					Destroy(this.gameObject);
			}
			else
			{
				if (this._statistics.InDeath)
					if (this._statistics.EnemyOnDeath)
						Instantiate(this._statistics.EnemyOnDeath, this.transform.position, this._statistics.EnemyOnDeath.transform.rotation);
					else if (this._statistics.SecondProjectile)
						if (this._statistics.InCell)
							this.CellInstanceRange();
						else
							this.CommonInstance();
				Destroy(this.gameObject);
			}
		}
	};
};
