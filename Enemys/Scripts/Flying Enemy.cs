using UnityEngine;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class FlyingEnemy : OppositeEnemy, IConnector
	{
		private Vector2 _pointOrigin = new();
		private bool _normal = true;
		private ushort _pointIndex = 0;
		[Header("Flying Enemy"), SerializeField] private GameObject _target;
		[SerializeField] private Vector2[] _trail;
		[SerializeField] private float _radiusDetection;
		[SerializeField] private float _speedReturn;
		[SerializeField] private float _targetDistance;
		[SerializeField] private float _fadeTime;
		[SerializeField] private bool _repeatWay;
		[SerializeField] private bool _stopOnTarget;
		[SerializeField] private bool _endlessPursue;
		[SerializeField] private bool _justHorizontal;
		[SerializeField] private bool _justVertical;
		public PathConnection PathConnection => PathConnection.Enemy;
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
		private void FixedUpdate()
		{
			if (this._stopMovement || this.Paralyzed)
				return;
			if (this._target)
			{
				this._rigidybody.linearVelocity = (this._target.transform.position - this.transform.position).normalized * this._movementSpeed;
				return;
			}
			if (this._endlessPursue)
			{
				Vector2 direction = (GuwbaAstral<VisualGuwba>.Position - (Vector2)this.transform.position).normalized;
				this._rigidybody.linearVelocity = direction * this._movementSpeed;
				return;
			}
			Vector2 targetPoint = new();
			bool followTarget = false;
			if (this._targetLayerMask > -1f)
				foreach (Collider2D collider in Physics2D.OverlapCircleAll(this._pointOrigin, this._radiusDetection, this._targetLayerMask))
					if (GuwbaAstral<VisualGuwba>.EqualObject(collider.gameObject))
					{
						targetPoint = collider.transform.position;
						Vector2 topRight = new(this._collider.bounds.extents.x, this._collider.bounds.extents.y);
						Vector2 topLeft = new(-this._collider.bounds.extents.x, this._collider.bounds.extents.y);
						Vector2 bottomRight = new(this._collider.bounds.extents.x, -this._collider.bounds.extents.y);
						Vector2 bottomLeft = new(-this._collider.bounds.extents.x, -this._collider.bounds.extents.y);
						Vector2 topRighPosition = (Vector2)this.transform.position + topRight;
						Vector2 topLeftPosition = (Vector2)this.transform.position + topLeft;
						Vector2 bottomRightPosition = (Vector2)this.transform.position + bottomRight;
						Vector2 bottomLeftPosition = (Vector2)this.transform.position + bottomLeft;
						bool topRightCorner = Physics2D.Linecast(topRighPosition, targetPoint + topRight, this._groundLayer);
						bool topLeftCorner = Physics2D.Linecast(topLeftPosition, targetPoint + topLeft, this._groundLayer);
						bool bottomRightCorner = Physics2D.Linecast(bottomRightPosition, targetPoint + bottomRight, this._groundLayer);
						bool bottomLeftCorner = Physics2D.Linecast(bottomLeftPosition, targetPoint + bottomLeft, this._groundLayer);
						bool center = Physics2D.Linecast(this.transform.position, targetPoint, this._groundLayer);
						followTarget = !center && !topRightCorner && !topLeftCorner && !bottomRightCorner && !bottomLeftCorner;
						if (followTarget)
							this._spriteRenderer.flipX = collider.transform.position.x < this.transform.position.x;
						break;
					}
			if (followTarget)
			{
				Vector2 direction = (targetPoint - (Vector2)this.transform.position).normalized;
				float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
				direction = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up;
				if (this._justHorizontal)
					this._rigidybody.linearVelocityX = direction.x * this._movementSpeed;
				else if (this._justVertical)
					this._rigidybody.linearVelocityY = direction.y * this._movementSpeed;
				else
					this._rigidybody.linearVelocity = direction * this._movementSpeed;
				if (Vector2.Distance(this.transform.position, targetPoint) <= this._targetDistance || this._stopOnTarget)
					this._rigidybody.linearVelocity = Vector2.zero;
			}
			else if ((Vector2)this.transform.position != this._pointOrigin)
			{
				this._spriteRenderer.flipX = this._pointOrigin.x < this.transform.position.x;
				this._rigidybody.linearVelocity = Vector2.zero;
				this.transform.position = Vector2.MoveTowards(this.transform.position, this._pointOrigin, this._speedReturn * Time.fixedDeltaTime);
			}
			else if (this._trail.Length > 0f && !followTarget)
			{
				Vector2 target = this._trail[this._pointIndex];
				if (this._repeatWay)
				{
					if (Vector2.Distance(this.transform.position, target) <= 0f)
						this._pointIndex = (ushort)(this._pointIndex < this._trail.Length - 1f ? this._pointIndex + 1f : 0f);
				}
				else if (this._normal)
				{
					if (Vector2.Distance(this.transform.position, target) <= 0f)
						this._pointIndex += 1;
					this._normal = this._pointIndex != this._trail.Length - 1f;
				}
				else if (!this._normal)
				{
					if (Vector2.Distance(this.transform.position, target) <= 0f)
						this._pointIndex -= 1;
					this._normal = this._pointIndex == 0f;
				}
				this._spriteRenderer.flipX = target.x < this.transform.position.x;
				this.transform.position = Vector2.MoveTowards(this.transform.position, target, this._movementSpeed * Time.fixedDeltaTime);
				this._pointOrigin = this.transform.position;
			}
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.ConnectionState == ConnectionState.Enable && data.ToggleValue.HasValue && data.ToggleValue.Value)
				this._stopMovement = false;
			else if (data.ConnectionState == ConnectionState.Disable && data.ToggleValue.HasValue && data.ToggleValue.Value)
				this._stopMovement = true;
		}
	};
};
