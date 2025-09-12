using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	internal sealed class GroundEnemy : MovingEnemy
	{
		private bool _runTowards = false;
		private ushort _runnedTimes = 0;
		private float _timeRun = 0f;
		private float _dashedTime = 0f;
		[Header("Ground Enemy")]
		[SerializeField, Tooltip("The distance to check for the block perception.")] private float _blockDistance;
		[SerializeField, Tooltip("If this enemy will do some action when look to a target.")] private bool _useFaceLook;
		[SerializeField, Tooltip("If the target is anything.")] private bool _targetEveryone;
		[SerializeField, Tooltip("If this enemy will run away from the target.")] private bool _runFromTarget;
		[SerializeField, Tooltip("If this enemy will run away from the target.")] private bool _runTowardsAfter;
		[SerializeField, Tooltip("The distance of the detection of target.")] private ushort _faceLookDistance;
		[SerializeField, Tooltip("The amount of times this enemy have to run away from the target.")] private ushort _timesToRun;
		[SerializeField, Tooltip("The amount of time this enemy will run away from or pursue the target.")] private float _runOfTime;
		[SerializeField, Tooltip("The amount of time this enemy will be dashing upon the target.")] private float _timeDashing;
		private new void Awake()
		{
			base.Awake();
			this._timeRun = this._timesToRun;
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
				return;
			}
			if (this._stopWorking)
				return;
			if (this._runFromTarget)
			{
				if (this._timeRun > 0f)
				{
					this._timeRun -= Time.deltaTime;
					this._isDashing = true;
				}
				if (this._timeRun <= 0f && this._isDashing)
				{
					if (this._runTowardsAfter && this._runnedTimes >= this._timesToRun)
					{
						this._runnedTimes = 0;
						this._runTowards = true;
					}
					else if (this._runTowardsAfter)
						this._runnedTimes++;
					this._isDashing = false;
				}
			}
		}
		private void FixedUpdate()
		{
			if (this._stopWorking || this.IsStunned)
			{
				this._rigidybody.linearVelocity = Vector2.zero;
				return;
			}
			Vector2 right = this.transform.right;
			if (this._isDashing)
			{
				this._dashedTime += Time.deltaTime;
				if (this._dashedTime >= this._timeDashing)
				{
					this._dashedTime = 0f;
					this._isDashing = false;
				}
			}
			else
				this._detected = false;
			if (this._useFaceLook)
			{
				float rayDistance = this._faceLookDistance;
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(this.transform.position, right, rayDistance, this._targetLayerMask))
					if (ray.collider.TryGetComponent<IDestructible>(out _))
					{
						this._detected = true;
						break;
					}
			}
			float xOrigin = (this._collider.bounds.extents.x + this._blockDistance / 2f) * this.transform.right.x;
			Vector2 origin = new(this.transform.position.x + xOrigin, this.transform.position.y);
			Vector2 size = new(this._blockDistance, this._collider.bounds.size.y - this._blockDistance);
			RaycastHit2D blockCast = Physics2D.BoxCast(origin, size, 0f, right, this._blockDistance, this._groundLayer);
			bool blockPerception = blockCast && blockCast.collider.TryGetComponent<Surface>(out var surface) && surface.IsScene;
			if (this._runFromTarget && this._timeRun <= 0f && this._detected)
			{
				this._timeRun = this._runOfTime;
				if (this._runTowards)
					this._runTowards = false;
				else
					this.transform.right *= -1f;
			}
			float xAxis = this.transform.position.x + this._collider.bounds.extents.x * right.x;
			float yAxis = this.transform.position.y - this._collider.bounds.extents.y * this.transform.up.y;
			if (!Physics2D.Raycast(new Vector2(xAxis, yAxis), -this.transform.up, this._blockDistance, this._groundLayer) || blockPerception)
				this.transform.right *= -1f;
			if (this._detected && !this._isDashing)
				if (this._detectionStop)
				{
					this._stopWorking = true;
					if (this._stopToShoot)
						this._sender.Send();
					return;
				}
				else if (this._shootDetection)
					this._sender.Send();
			this._rigidybody.linearVelocity = this._detected ? right * this._dashSpeed : right * this._movementSpeed;
		}
	};
};
