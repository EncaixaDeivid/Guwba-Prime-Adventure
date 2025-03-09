using UnityEngine;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class FlyingEnemy : OpositeEnemy
	{
		private Vector2 _pointOrigin = new();
		private bool _normal = true;
		private ushort _pointIndex = 0;
		[Header("Flying Enemy"), SerializeField] private GameObject _target;
		[SerializeField] private Vector2[] _trail;
		[SerializeField] private ushort _speedTrail;
		[SerializeField] private float _radiusDetection, _speedReturn, _targetDistance;
		[SerializeField] private bool _stopOnTarget;
		private new void Awake()
		{
			base.Awake();
			this._pointOrigin = this.transform.position;
			this._toggleEvent = (bool toggleValue) => this._stopMovement = !toggleValue;
		}
		private void FixedUpdate()
		{
			if (this._stopMovement || this.Paralyzed)
				return;
			if (this._target)
			{
				this._rigidybody.linearVelocity = this._target.transform.position - this.transform.position * this._movementSpeed;
				return;
			}
			Vector2 targetPoint = new();
			bool followTarget = false;
			if (this._targetLayerMask > -1f)
				foreach (Collider2D collider in Physics2D.OverlapCircleAll(this._pointOrigin, this._radiusDetection, this._targetLayerMask))
					if (GuwbaTransformer<VisualHudGuwba>.EqualObject(collider.gameObject))
					{
						targetPoint = collider.transform.position;
						followTarget = !Physics2D.Linecast(this.transform.position, targetPoint, this._groundLayer);
						if (followTarget)
							this._spriteRenderer.flipX = collider.transform.position.x < this.transform.position.x;
						break;
					}
			if (followTarget)
			{
				Vector2 direction = (targetPoint - (Vector2)this.transform.position).normalized;
				float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
				direction = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up;
				this._rigidybody.linearVelocity = direction * this._movementSpeed;
				if (Vector2.Distance(this.transform.position, targetPoint) <= this._targetDistance || this._stopOnTarget)
					this._rigidybody.linearVelocity = Vector2.zero;
			}
			else if ((Vector2)this.transform.position != this._pointOrigin)
			{
				this._spriteRenderer.flipX = this._pointOrigin.x < this.transform.position.x;
				this._rigidybody.linearVelocity = Vector2.zero;
				this.transform.position = Vector2.Lerp(this.transform.position, this._pointOrigin, this._speedReturn * Time.deltaTime);
			}
			else if (this._trail.Length > 0f && !followTarget)
			{
				Vector2 target = this._trail[this._pointIndex];
				if (this._normal)
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
				this.transform.position = Vector2.MoveTowards(this.transform.position, target, this._speedTrail * Time.deltaTime);
				this._pointOrigin = this.transform.position;
			}
		}
	};
};
