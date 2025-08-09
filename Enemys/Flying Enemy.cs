using UnityEngine;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class FlyingEnemy : MovingEnemy, IConnector
	{
		private Vector2 _pointOrigin = new();
		private Vector2 _targetPoint = new();
		private bool _normal = true;
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
			Sender.Include(this);
			if (this._endlessPursue)
				Destroy(this.gameObject, this._fadeTime);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
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
						this._detected = !Physics2D.Linecast(this.transform.position, this._targetPoint, this._groundLayer);
						if (this._detected)
							this._spriteRenderer.flipX = collider.transform.position.x < this.transform.position.x;
						break;
					}
			if (this._detected)
			{
				if (!this._isDashing && Vector2.Distance(this.transform.position, this._targetPoint) <= this._targetDistance)
					if (this._detectionStop)
					{
						this._stopWorking = true;
						if (this._stopToShoot)
							this._sender.Send();
						return;
					}
					else if (this._shootDetection)
						this._sender.Send();
				float maxDistanceDelta = this._isDashing ? Time.fixedDeltaTime * this._dashSpeed : Time.fixedDeltaTime * this._movementSpeed;
				this.transform.position = Vector2.MoveTowards(this.transform.position, this._targetPoint, maxDistanceDelta);
				if (this._isDashing && Vector2.Distance(this.transform.position, this._targetPoint) <= 0f)
					this._isDashing = false;
			}
			else if ((Vector2)this.transform.position != this._pointOrigin)
			{
				this._spriteRenderer.flipX = this._pointOrigin.x < this.transform.position.x;
				float maxDistanceDelta = this._returnSpeed * Time.fixedDeltaTime;
				this.transform.position = Vector2.MoveTowards(this.transform.position, this._pointOrigin, maxDistanceDelta);
			}
			else if (this._trail.Length > 0f && !this._detected)
			{
				Vector2 target = this._trail[this._pointIndex];
				if (this._repeatWay)
				{
					if ((ushort)Vector2.Distance(this.transform.position, target) <= 0f)
						this._pointIndex = (ushort)(this._pointIndex < this._trail.Length - 1f ? this._pointIndex + 1f : 0f);
				}
				else if (this._normal)
				{
					if ((ushort)Vector2.Distance(this.transform.position, target) <= 0f)
						this._pointIndex += 1;
					this._normal = this._pointIndex != this._trail.Length - 1f;
				}
				else if (!this._normal)
				{
					if ((ushort)Vector2.Distance(this.transform.position, target) <= 0f)
						this._pointIndex -= 1;
					this._normal = this._pointIndex == 0f;
				}
				this._spriteRenderer.flipX = target.x < this.transform.position.x;
				this.transform.position = Vector2.MoveTowards(this.transform.position, target, this._movementSpeed * Time.fixedDeltaTime);
				this._pointOrigin = this.transform.position;
			}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			if (additionalData as GameObject != this.gameObject)
				return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				this._stopWorking = !data.ToggleValue.Value;
		}
	};
};
