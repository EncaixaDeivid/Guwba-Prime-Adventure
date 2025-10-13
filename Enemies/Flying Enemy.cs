using UnityEngine;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(PolygonCollider2D))]
	internal sealed class FlyingEnemy : MovingEnemy
	{
		private PolygonCollider2D _trail;
		private Vector2 _pointOrigin = new();
		private Vector2 _targetPoint = new();
		private bool _normal = true;
		private bool _returnDash = false;
		private ushort _pointIndex = 0;
		[Header("Flying Enemy")]
		[SerializeField, Tooltip("The flying statitics of this enemy.")] private FlyingStatistics _statistics;
		[SerializeField, Tooltip("If this enemy will repeat the same way it makes before.")] private bool _repeatWay;
		private new void Awake()
		{
			base.Awake();
			this._trail = this.GetComponent<PolygonCollider2D>();
			this._pointOrigin = this.transform.position;
			if (this._statistics.EndlessPursue)
				Destroy(this.gameObject, this._statistics.FadeTime);
		}
		private void Update()
		{
			if (this._rigidybody.IsSleeping())
				return;
			if (this._statistics.DetectionStop && this._stopWorking)
			{
				this._stoppedTime += Time.deltaTime;
				if (this._stoppedTime >= this._statistics.StopTime)
				{
					this._stoppedTime = 0f;
					this._stopWorking = false;
					this._isDashing = true;
				}
			}
		}
		private void Chase()
		{
			float maxDistanceDelta;
			if (!this._isDashing && Vector2.Distance(this.transform.position, this._targetPoint) <= this._statistics.TargetDistance)
			{
				if (this._returnDash)
				{
					maxDistanceDelta = Time.fixedDeltaTime * this._statistics.DashSpeed;
					this.transform.position = Vector2.MoveTowards(this.transform.position, this._pointOrigin, maxDistanceDelta);
					this._returnDash = Vector2.Distance(this.transform.position, this._targetPoint) <= this._statistics.TargetDistance;
					return;
				}
				else if (this._statistics.DetectionStop)
				{
					this._stopWorking = true;
					return;
				}
			}
			else if (!this._isDashing && this._returnDash)
				this._returnDash = false;
			maxDistanceDelta = Time.fixedDeltaTime * (this._isDashing ? this._statistics.DashSpeed : this._statistics.MovementSpeed);
			this.transform.position = Vector2.MoveTowards(this.transform.position, this._targetPoint, maxDistanceDelta);
			if (this._isDashing && Vector2.Distance(this.transform.position, this._targetPoint) <= 0f)
				this._isDashing = !(this._returnDash = true);
		}
		private void Trail()
		{
			if ((Vector2)this.transform.position != this._pointOrigin)
			{
				bool valid = this._pointOrigin.x < this.transform.position.x;
				this.transform.localScale = new Vector3()
				{
					x = valid ? -Mathf.Abs(this.transform.localScale.x) : Mathf.Abs(this.transform.localScale.x),
					y = this.transform.localScale.y,
					z = this.transform.localScale.z
				};
				float maxDistanceDelta = this._statistics.ReturnSpeed * Time.fixedDeltaTime;
				this.transform.position = Vector2.MoveTowards(this.transform.position, this._pointOrigin, maxDistanceDelta);
			}
			else if (this._trail.points.Length > 0f)
			{
				Vector2 target = this._trail.points[this._pointIndex];
				if (this._repeatWay)
				{
					if ((ushort)Vector2.Distance(this.transform.localPosition, target) <= 0f)
						this._pointIndex = (ushort)(this._pointIndex < this._trail.points.Length - 1f ? this._pointIndex + 1f : 0f);
				}
				else if (this._normal)
				{
					if ((ushort)Vector2.Distance(this.transform.localPosition, target) <= 0f)
						this._pointIndex += 1;
					this._normal = this._pointIndex != this._trail.points.Length - 1f;
				}
				else if (!this._normal)
				{
					if ((ushort)Vector2.Distance(this.transform.localPosition, target) <= 0f)
						this._pointIndex -= 1;
					this._normal = this._pointIndex == 0f;
				}
				float maxDistanceDelta = Time.fixedDeltaTime * this._statistics.MovementSpeed;
				bool valid = target.x < this.transform.localPosition.x;
				this.transform.localScale = new Vector3()
				{
					x = valid ? -Mathf.Abs(this.transform.localScale.x) : Mathf.Abs(this.transform.localScale.x),
					y = this.transform.localScale.y,
					z = this.transform.localScale.z
				};
				this.transform.localPosition = Vector2.MoveTowards(this.transform.localPosition, target, maxDistanceDelta);
				this._pointOrigin = this.transform.position;
			}
		}
		private void FixedUpdate()
		{
			if (this._stopWorking || this._rigidybody.IsSleeping())
				return;
			LayerMask groundLayer = this._statistics.Physics.GroundLayer;
			LayerMask targetLayer = this._statistics.Physics.TargetLayer;
			float maxDistanceDelta = Time.fixedDeltaTime * this._statistics.MovementSpeed;
			if (this._statistics.Target)
			{
				this._targetPoint = this._statistics.Target.transform.position;
				this.transform.position = Vector2.MoveTowards(this.transform.position, this._targetPoint, maxDistanceDelta);
				return;
			}
			if (this._statistics.EndlessPursue)
			{
				this.transform.position = Vector2.MoveTowards(this.transform.position, GuwbaCentralizer.Position, maxDistanceDelta);
				return;
			}
			if (!this._isDashing)
				this._detected = false;
			if (this._statistics.LookPerception && !this._isDashing)
				foreach (Collider2D collider in Physics2D.OverlapCircleAll(this._pointOrigin, this._statistics.LookDistance, targetLayer))
					if (GuwbaCentralizer.EqualObject(collider.gameObject))
					{
						this._targetPoint = collider.transform.position;
						this._detected = !Physics2D.Linecast(this.transform.position, this._targetPoint, groundLayer);
						if (this._detected)
						{
							bool valid = collider.transform.position.x < this.transform.position.x;
							this.transform.localScale = new Vector3()
							{
								x = valid ? -Mathf.Abs(this.transform.localScale.x) : Mathf.Abs(this.transform.localScale.x),
								y = this.transform.localScale.y,
								z = this.transform.localScale.z
							};
						}
						break;
					}
			if (this._detected)
				this.Chase();
			else
				this.Trail();
		}
	};
};
