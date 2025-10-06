using UnityEngine;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class RunnerEnemy : MovingEnemy
	{
		private bool _runTowards = false;
		private ushort _runnedTimes = 0;
		private float _timeRun = 0f;
		private float _dashedTime = 0f;
		private float _dashTime = 0f;
		[Header("Runner Enemy")]
		[SerializeField, Tooltip("The runner statitics of this enemy.")] private RunnerStatistics _statistics;
		private new void Awake()
		{
			base.Awake();
			this._timeRun = this._statistics.TimesToRun;
		}
		private new void Update()
		{
			base.Update();
			if (this.IsStunned)
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
				return;
			}
			if (this._stopWorking)
				return;
			if (this._statistics.TimedDash && !this._isDashing)
			{
				this._dashTime += Time.deltaTime;
				if (this._dashTime >= this._statistics.TimeToDash)
				{
					this._dashTime = 0f;
					this._isDashing = true;
				}
			}
			if (this._statistics.RunFromTarget)
			{
				if (this._timeRun > 0f)
				{
					this._timeRun -= Time.deltaTime;
					this._isDashing = true;
				}
				if (this._timeRun <= 0f && this._isDashing)
				{
					if (this._statistics.RunTowardsAfter && this._runnedTimes >= this._statistics.TimesToRun)
					{
						this._runnedTimes = 0;
						this._runTowards = true;
					}
					else if (this._statistics.RunTowardsAfter)
						this._runnedTimes++;
					this._isDashing = false;
				}
			}
		}
		private void FixedUpdate()
		{
			if (this._stopWorking || this.IsStunned)
				return;
			Vector2 right = this.transform.right * this._movementSide;
			LayerMask groundLayer = this._statistics.Physics.GroundLayer;
			LayerMask targetLayer = this._statistics.Physics.TargetLayer;
			float groundChecker = this._statistics.Physics.GroundChecker;
			if (this._isDashing)
			{
				this._dashedTime += Time.deltaTime;
				if (this._dashedTime >= this._statistics.TimeDashing)
				{
					this._dashedTime = 0f;
					this._isDashing = false;
					this._sender.SetToggle(true);
					this._sender.Send(PathConnection.Enemy);
				}
			}
			else
				this._detected = false;
			if (this._statistics.LookPerception)
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(this.transform.position, right, this._statistics.LookDistance, targetLayer))
					if (ray.collider.TryGetComponent<IDestructible>(out _))
					{
						this._detected = true;
						break;
					}
			float xOrigin = (this._collider.bounds.extents.x + groundChecker / 2f) * right.x;
			Vector2 origin = new(this.transform.position.x + xOrigin, this.transform.position.y);
			Vector2 size = new(groundChecker, this._collider.bounds.size.y - groundChecker);
			RaycastHit2D blockCast = Physics2D.BoxCast(origin, size, 0f, right, groundChecker, groundLayer);
			bool blockPerception = blockCast && blockCast.collider.TryGetComponent<Surface>(out var surface) && surface.IsScene;
			if (this._statistics.RunFromTarget && this._timeRun <= 0f && this._detected)
			{
				this._timeRun = this._statistics.RunOfTime;
				if (this._runTowards)
					this._runTowards = false;
				else
					this._movementSide *= -1;
			}
			float xAxis = this.transform.position.x + this._collider.bounds.extents.x * right.x;
			float yAxis = this.transform.position.y + this._collider.bounds.extents.y * -this.transform.up.y;
			if (!Physics2D.Raycast(new Vector2(xAxis, yAxis), -this.transform.up, groundChecker, groundLayer) || blockPerception)
				this._movementSide *= -1;
			if (this._detected && !this._isDashing && this._statistics.DetectionStop)
			{
				this._stopWorking = true;
				this._sender.SetToggle(this._statistics.JumpDash);
				this._sender.Send(PathConnection.Enemy);
				return;
			}
			this.transform.localScale = new Vector3()
			{
				x = this._movementSide < 0f ? -Mathf.Abs(this.transform.localScale.x) : Mathf.Abs(this.transform.localScale.x),
				y = this.transform.localScale.y,
				z = this.transform.localScale.z
			};
			this._rigidybody.linearVelocityX = right.x * (this._detected ? this._statistics.DashSpeed : this._statistics.MovementSpeed);
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			EnemyController[] enemies = (EnemyController[])additionalData;
			if (enemies != null)
				foreach (EnemyController enemy in enemies)
					if (enemy != this)
						return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
			{
				this._rigidybody.linearVelocityX = 0f;
				this._stopWorking = !data.ToggleValue.Value;
			}
			else if (data.StateForm == StateForm.Action && this._statistics.ReactToDamage)
			{
				Vector2 targetPosition;
				if (this._statistics.UseOtherTarget)
					targetPosition = this._statistics.OtherTarget;
				else
					targetPosition = CentralizableGuwba.Position;
				this._movementSide = (short)(targetPosition.x < this.transform.position.x ? -1f : 1f);
				if (this._statistics.DetectionStop)
				{
					this._stopWorking = true;
					this._sender.SetToggle(this._statistics.JumpDash);
					this._sender.Send(PathConnection.Enemy);
				}
			}
		}
	};
};
