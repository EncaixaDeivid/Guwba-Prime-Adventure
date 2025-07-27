using UnityEngine;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class FlyingEnemy : EnemyController, IConnector
	{
		private Vector2 _pointOrigin = new();
		private bool _normal = true;
		private ushort _pointIndex = 0;
		[Header("Flying Enemy")]
		[SerializeField, Tooltip("The target this enemy have to pursue.")] private GameObject _target;
		[SerializeField, Tooltip("How far this enemy detect any target.")] private float _radiusDetection;
		[SerializeField, Tooltip("The distance to stay away from the target.")] private float _targetDistance;
		[SerializeField, Tooltip("The time this enemy stay alive during the endless pursue.")] private float _fadeTime;
		[SerializeField, Tooltip("If this enemy will stop when hit a distance from the target.")] private bool _stopOnTarget;
		[SerializeField, Tooltip("If this enemy will pursue the target until fade.")] private bool _endlessPursue;
		[Header("Trail Stats")]
		[SerializeField, Tooltip("The points that this enemy have to make.")] private Vector2[] _trail;
		[SerializeField, Tooltip("The speed that this enemy moves to go back to the original point.")] private float _speedReturn;
		[SerializeField, Tooltip("If this enemy pursue only in the horizontal.")] private bool _justHorizontal;
		[SerializeField, Tooltip("If this enemy pursue only in the vertical.")] private bool _justVertical;
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
				if (this._stopOnTarget && Vector2.Distance(this.transform.position, targetPoint) <= this._targetDistance)
					this._rigidybody.linearVelocity = Vector2.zero;
			}
			else if ((Vector2)this.transform.position != this._pointOrigin)
			{
				this._spriteRenderer.flipX = this._pointOrigin.x < this.transform.position.x;
				this._rigidybody.linearVelocity = Vector2.zero;
				float maxDistanceDelta = this._speedReturn * Time.fixedDeltaTime;
				this.transform.position = Vector2.MoveTowards(this.transform.position, this._pointOrigin, maxDistanceDelta);
			}
			else if (this._trail.Length > 0f && !followTarget)
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
				this.transform.position = Vector2.MoveTowards(this.transform.position, target, this._movementSpeed * Time.deltaTime);
				this._pointOrigin = this.transform.position;
			}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			if (additionalData as GameObject != this.gameObject)
				return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				this._stopMovement = !data.ToggleValue.Value;
		}
	};
};
