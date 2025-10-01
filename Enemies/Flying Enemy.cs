using UnityEngine;
using System;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy
{
	internal sealed class FlyingEnemy : MovingEnemy
	{
		private Vector2 _pointOrigin = new();
		private Vector2 _targetPoint = new();
		private bool _normal = true;
		private bool _returnDash = false;
		private ushort _pointIndex = 0;
		[Header("Flying Enemy")]
		[SerializeField, Tooltip("The target this enemy have to pursue.")] private GameObject _target;
		[SerializeField, Tooltip("How far this enemy detect any target.")] private float _radiusDetection;
		[SerializeField, Tooltip("The distance to stay away from the target.")] private float _targetDistance;
		[SerializeField, Tooltip("The time this enemy stay alive during the endless pursue.")] private float _fadeTime;
		[SerializeField, Tooltip("If this enemy will pursue the target until fade.")] private bool _endlessPursue;
		[Header("Trail Stats")]
		[SerializeField, Tooltip("The points that this enemy have to make.")] private Vector2[] _trail;
		[SerializeField, Tooltip("The amount of speed that this enemy moves to go back to the original point.")] private float _returnSpeed;
		[SerializeField, Tooltip("If this enemy will repeat the same way it makes before.")] private bool _repeatWay;
		private new void Awake()
		{
			base.Awake();
			this._pointOrigin = this.transform.position;
			if (this._endlessPursue)
				Destroy(this.gameObject, this._fadeTime);
		}
		private new void Update()
		{
			base.Update();
			if (this.IsStunned)
				return;
			if (this._detectionStop && this._stopWorking)
			{
				this._stoppedTime += Time.deltaTime;
				if (this._stoppedTime >= this._stopTime)
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
			if (!this._isDashing && Vector2.Distance(this.transform.position, this._targetPoint) <= this._targetDistance)
			{
				if (this._returnDash)
				{
					maxDistanceDelta = Time.fixedDeltaTime * this._dashSpeed;
					this.transform.position = Vector2.MoveTowards(this.transform.position, this._pointOrigin, maxDistanceDelta);
					this._returnDash = Vector2.Distance(this.transform.position, this._targetPoint) <= this._targetDistance;
					return;
				}
				else if (this._detectionStop)
				{
					this._stopWorking = true;
					if (this._stopToShoot)
						this._sender.Send(PathConnection.Enemy);
					return;
				}
				else if (this._shootDetection)
					this._sender.Send(PathConnection.Enemy);
			}
			else if (!this._isDashing && this._returnDash)
				this._returnDash = false;
			maxDistanceDelta = this._isDashing ? Time.fixedDeltaTime * this._dashSpeed : Time.fixedDeltaTime * this._movementSpeed;
			this.transform.position = Vector2.MoveTowards(this.transform.position, this._targetPoint, maxDistanceDelta);
			if (this._isDashing && Vector2.Distance(this.transform.position, this._targetPoint) <= 0f)
				this._isDashing = !(this._returnDash = true);
		}
		private void Trail()
		{
			if (this._trail.Length <= 0f)
				return;
			if ((Vector2)this.transform.position != this._pointOrigin)
			{
				bool valid = this._pointOrigin.x < this.transform.position.x;
				this.transform.localScale = new Vector3()
				{
					x = valid ? -MathF.Abs(this.transform.localScale.x) : MathF.Abs(this.transform.localScale.x),
					y = this.transform.localScale.y,
					z = this.transform.localScale.z
				};
				float maxDistanceDelta = this._returnSpeed * Time.fixedDeltaTime;
				this.transform.position = Vector2.MoveTowards(this.transform.position, this._pointOrigin, maxDistanceDelta);
			}
			else
			{
				Vector2 target = this._trail[this._pointIndex];
				if (this._repeatWay)
				{
					if ((ushort)Vector2.Distance(this.transform.localPosition, target) <= 0f)
						this._pointIndex = (ushort)(this._pointIndex < this._trail.Length - 1f ? this._pointIndex + 1f : 0f);
				}
				else if (this._normal)
				{
					if ((ushort)Vector2.Distance(this.transform.localPosition, target) <= 0f)
						this._pointIndex += 1;
					this._normal = this._pointIndex != this._trail.Length - 1f;
				}
				else if (!this._normal)
				{
					if ((ushort)Vector2.Distance(this.transform.localPosition, target) <= 0f)
						this._pointIndex -= 1;
					this._normal = this._pointIndex == 0f;
				}
				float maxDistanceDelta = Time.fixedDeltaTime * this._movementSpeed;
				bool valid = target.x < this.transform.localPosition.x;
				this.transform.localScale = new Vector3()
				{
					x = valid ? -MathF.Abs(this.transform.localScale.x) : MathF.Abs(this.transform.localScale.x),
					y = this.transform.localScale.y,
					z = this.transform.localScale.z
				};
				this.transform.localPosition = Vector2.MoveTowards(this.transform.localPosition, target, maxDistanceDelta);
				this._pointOrigin = this.transform.position;
			}
		}
		private void FixedUpdate()
		{
			if (this._stopWorking || this.IsStunned)
				return;
			if (this._target)
			{
				this._targetPoint = this._target.transform.position;
				float maxDistanceDelta = Time.fixedDeltaTime * this._movementSpeed;
				this.transform.position = Vector2.MoveTowards(this.transform.position, this._targetPoint, maxDistanceDelta);
				return;
			}
			if (this._endlessPursue)
			{
				float maxDistanceDelta = Time.fixedDeltaTime * this._movementSpeed;
				this.transform.position = Vector2.MoveTowards(this.transform.position, CentralizableGuwba.Position, maxDistanceDelta);
				return;
			}
			if (!this._isDashing)
				this._detected = false;
			if (this._targetLayerMask > -1f && !this._isDashing)
				foreach (Collider2D collider in Physics2D.OverlapCircleAll(this._pointOrigin, this._radiusDetection, this._targetLayerMask))
					if (CentralizableGuwba.EqualObject(collider.gameObject))
					{
						this._targetPoint = collider.transform.position;
						Vector2 origin = this.transform.position;
						Vector2 size = this._collider.bounds.size;
						float targetDistance = Vector2.Distance(this.transform.position, this._targetPoint);
						this._detected = !Physics2D.BoxCast(origin, size, 0f, this._targetPoint, targetDistance, this._groundLayer);
						if (this._detected)
						{
							bool valid = collider.transform.position.x < this.transform.position.x;
							this.transform.localScale = new Vector3()
							{
								x = valid ? -MathF.Abs(this.transform.localScale.x) : MathF.Abs(this.transform.localScale.x),
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
