using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Rigidbody2D))]
	[RequireComponent(typeof(Collider2D))]
	internal sealed class EnemyProjectile : StateController, IDestructible
	{
		private Rigidbody2D _rigidbody;
		private readonly List<EnemyProjectile> _projectiles = new();
		private Vector2Int _oldCellPosition = new();
		private Vector2Int _cellPosition = new();
		private short _vitality;
		private ushort _angleMulti = 0;
		private ushort _pointToJump = 0;
		private ushort _pointToBreak = 0;
		private ushort _internalBreakPoint = 0;
		private ushort _pointToReturn = 0;
		private ushort _internalReturnPoint = 0;
		private float _stunTimer = 0f;
		private bool _breakInUse = false;
		[Header("Projectile")]
		[SerializeField, Tooltip("The statitics of this projectile.")] private ProjectileStatistics _statistics;
		public short Health => this._vitality;
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
		private void ParabolicProjectile()
		{
			this.StartCoroutine(Parabola());
			IEnumerator Parabola()
			{
				yield return new WaitUntil(() => this.isActiveAndEnabled);
				float time = 0f, x, y;
				while (time > this._statistics.TimeToFade)
				{
					time += Time.fixedDeltaTime;
					x = this._statistics.MovementSpeed * time * Mathf.Cos(this._statistics.BaseAngle * Mathf.Deg2Rad);
					y = this._statistics.MovementSpeed * time * Mathf.Sin(this._statistics.BaseAngle * Mathf.Deg2Rad);
					this.transform.position = new Vector2(x, y - 0.5f * -Physics2D.gravity.y * Mathf.Pow(time, 2));
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => this.isActiveAndEnabled && this._rigidbody.IsAwake());
				}
			}
		}
		private new void Awake()
		{
			base.Awake();
			this._rigidbody = this.GetComponent<Rigidbody2D>();
			this._vitality = (short)this._statistics.Vitality;
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
		private void Update()
		{
			if (this._rigidbody.IsSleeping())
			{
				this._stunTimer -= Time.deltaTime;
				if (this._stunTimer <= 0f)
					this._rigidbody.WakeUp();
			}
		}
		private void FixedUpdate()
		{
			if (this._rigidbody.IsSleeping())
				return;
			float movementSpeed = this._statistics.MovementSpeed;
			if (this._statistics.SecondProjectile && this._statistics.InCell && this._statistics.ContinuosSummon)
			{
				if (this._statistics.UseQuantity && this._statistics.QuantityToSummon == this._projectiles.Count || this._statistics.StayInPlace)
					return;
				this._cellPosition = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
				this.CellInstance();
			}
			this._rigidbody.rotation += this._statistics.RotationSpeed * movementSpeed * Time.fixedDeltaTime;
			if (this._statistics.ParabolicMovement)
				this.ParabolicProjectile();
			else if (!this._statistics.StayInPlace && this._statistics.RotationMatter)
				this._rigidbody.linearVelocity = (this._statistics.InvertSide ? -this.transform.up : this.transform.up) * movementSpeed;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._statistics.IsInoffensive)
				return;
			if (other.TryGetComponent<IDestructible>(out var destructible) && destructible.Hurt(this._statistics.Damage))
			{
				destructible.Stun(this._statistics.Damage, this._statistics.StunTime);
				EffectsController.HitStop(this._statistics.Physics.HitStopTime, this._statistics.Physics.HitSlowTime);
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
		public bool Hurt(ushort damage)
		{
			if (this._statistics.IsInoffensive || damage <= 0 || this._statistics.Vitality <= 0f)
				return false;
			if ((this._vitality -= (short)damage) <= 0f)
				Destroy(this.gameObject);
			return true;
		}
		public void Stun(ushort stunStength, float stunTime)
		{
			if (this._rigidbody.IsSleeping())
				return;
			this._rigidbody.Sleep();
			this._stunTimer = stunTime;
		}
	};
};
