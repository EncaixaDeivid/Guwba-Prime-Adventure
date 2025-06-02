using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class RunnerBoss : BossController, IConnector
	{
		private float _gravityScale = 0f;
		private bool _stopMovement = false;
		private bool _dashIsOn = false;
		private bool _stopVelocity = false;
		[Header("Runner Boss")]
		[SerializeField, Tooltip("In the react to damage it already have a target.")] private Vector2 _otherTarget;
		[SerializeField, Tooltip("If it have a ray to detect the target.")] private bool _rayDetection;
		[SerializeField, Tooltip("If the dash is timed to start when the boss is instantiate.")] private bool _timedDash;
		[SerializeField, Tooltip("If the boss can climb walls.")] private bool _climbWall;
		[SerializeField, Tooltip("If the boss will increase the speed while climbing.")] private bool _speedUpOnClimb;
		[SerializeField, Tooltip("If the boss can jump while dashing.")] private bool _jumpDash;
		[SerializeField, Tooltip("If the boss will target other object when react to damage.")] private bool _useOtherTarget;
		[SerializeField, Tooltip("The speed of the boss while dashing.")] private ushort _dashSpeed;
		[SerializeField, Tooltip("The distance of the rays to hit the ground.")] private float _groundDistance;
		[SerializeField, Tooltip("The distance of the dash ray.")] private float _rayDistance;
		[SerializeField, Tooltip("The speed of movement while climbing.")] private float _climbSpeedUp;
		[SerializeField, Tooltip("The maount of time that before the dash start.")] private float _stopDashTime;
		[SerializeField, Tooltip("The distance of dash will run.")] private float _dashDistance;
		[SerializeField, Tooltip("The amount of time to wait the timed dash to go.")] private float _timeToDash;
		private IEnumerator Dash()
		{
			this._dashIsOn = true;
			Sender jumpSender = Sender.Create();
			jumpSender.SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Action);
			jumpSender.SetBossType(BossType.Jumper).SetToggle(this._jumpDash).Send();
			this._animator.SetBool(this._walk, false);
			Vector2 actualPosition = this.transform.position;
			yield return new WaitTime(this, this._stopDashTime);
			float dashValue = this._movementSide < 0f ? -this._movementSide : this._movementSide;
			this._animator.SetBool(this._walk, true);
			this._animator.SetFloat(this._dash, this._dashSpeed * Time.deltaTime + dashValue);
			Vector2 runnedDistance = actualPosition;
			Vector2Int cellPosition = new((int)actualPosition.x, (int)actualPosition.y);
			Vector2Int oldCellPosition = cellPosition;
			yield return new WaitUntil(() =>
			{
				float speedUp = this._climbSpeedUp * this._movementSide;
				if (this.enabled)
					if (this.transform.rotation.z == 0f)
						this._rigidybody.linearVelocityX = this._movementSide * this._dashSpeed;
					else if (this._speedUpOnClimb)
						this._rigidybody.linearVelocity = (this._movementSide + speedUp) * this._dashSpeed * this.transform.right;
					else
						this._rigidybody.linearVelocity = this._movementSide * this._dashSpeed * this.transform.right;
				else
					this._rigidybody.linearVelocity = Vector2.zero;
				cellPosition = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
				if (oldCellPosition != cellPosition)
				{
					oldCellPosition = cellPosition;
					runnedDistance += (Vector2)cellPosition;
				}
				return Vector2.Distance(actualPosition, runnedDistance) >= this._dashDistance && this.enabled;
			});
			this._dashIsOn = false;
			jumpSender.SetToggle(true).Send();
		}
		private new void Awake()
		{
			base.Awake();
			this._gravityScale = this._rigidybody.gravityScale;
			if (this._timedDash)
				this.StartCoroutine(TimedDash());
			IEnumerator TimedDash()
			{
				yield return new WaitTime(this, this._timeToDash);
				this.StartCoroutine(this.Dash());
				this.StartCoroutine(TimedDash());
			}
		}
		private new void FixedUpdate()
		{
			base.FixedUpdate();
			if (this._stopMovement && !this._dashIsOn)
			{
				this._animator.SetBool(this._walk, false);
				this._animator.SetFloat(this._dash, 1f);
				this.StopAllCoroutines();
				if (this._stopVelocity)
				{
					this._stopVelocity = false;
					this._rigidybody.linearVelocityX = 0f;
				}
				if (this.SurfacePerception())
				{
					this._movementSide = (short)Random.Range(-1f, 1f);
					if (this._movementSide >= 0f && this._movementSide < 1f)
						this._movementSide = 1;
					else if (this._movementSide < 0f && this._movementSide > -1f)
						this._movementSide = -1;
				}
				return;
			}
			if (this._rayDetection && !this._dashIsOn && this.transform.rotation.z == 0f)
			{
				Vector2 dashOrigin = this.transform.position;
				Vector2 dashDirection = this.transform.right * this._movementSide;
				RaycastHit2D[] raycastHits = Physics2D.RaycastAll(dashOrigin, dashDirection, this._rayDistance, this._targetLayerMask);
				if (GuwbaAstral<CommandGuwba>.EqualObject(raycastHits))
					this.StartCoroutine(this.Dash());
			}
			if (this._climbWall && this.transform.rotation.z != 0f)
			{
				float xAxis = 0f;
				float yAxis = 0f;
				if (this.transform.rotation.z > 0f)
				{
					xAxis = this.transform.position.x + this._collider.bounds.extents.x;
					yAxis = this.transform.position.y + this._collider.bounds.extents.y * this._movementSide;
				}
				else if (this.transform.rotation.z < 0f)
				{
					xAxis = this.transform.position.x - this._collider.bounds.extents.x;
					yAxis = this.transform.position.y + this._collider.bounds.extents.y * -this._movementSide;
				}
				Vector2 origin = new(xAxis, yAxis);
				bool endClimbSurface = !Physics2D.Raycast(origin, -this.transform.up, this._groundDistance, this._groundLayer);
				if (endClimbSurface)
					this._movementSide *= -1;
			}
			Vector2 size = new(this._collider.bounds.size.x - this._groundDistance, this._collider.bounds.size.y - this._groundDistance);
			Vector2 direction = this.transform.right * this._movementSide;
			bool blockPerception = Physics2D.BoxCast(this.transform.position, size, 0f, direction, this._groundDistance, this._groundLayer);
			if (this.transform.rotation.z == 0f)
				this._rigidybody.gravityScale = this._gravityScale;
			else
				this._rigidybody.gravityScale = 0f;
			if (this._climbWall && blockPerception && this.SurfacePerception())
				this._rigidybody.rotation += this._movementSide * 90f;
			else if (blockPerception)
				this._movementSide *= -1;
			this._spriteRenderer.flipX = this._movementSide < 0f;
			if (!this._dashIsOn)
			{
				this._animator.SetBool(this._walk, true);
				this._animator.SetFloat(this._dash, 1f);
				float speedUp = this._climbSpeedUp * this._movementSide;
				if (this.transform.rotation.z == 0f)
					this._rigidybody.linearVelocityX = this._movementSide * this._movementSpeed;
				else if (this._speedUpOnClimb)
					this._rigidybody.linearVelocity = (this._movementSide + speedUp) * this._movementSpeed * this.transform.right;
				else
					this._rigidybody.linearVelocity = this._movementSide * this._movementSpeed * this.transform.right;
			}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			BossType bossType = (BossType)additionalData;
			if (bossType.HasFlag(BossType.Runner) || bossType.HasFlag(BossType.All))
				if (data.ConnectionState == ConnectionState.Action && data.ToggleValue.HasValue && this._hasToggle)
					this._stopVelocity = this._stopMovement = !data.ToggleValue.Value;
				else if (data.ConnectionState == ConnectionState.Action && this._reactToDamage)
				{
					Vector2 targetPosition;
					if (this._useOtherTarget)
						targetPosition = this._otherTarget;
					else
						targetPosition = GuwbaAstral<CommandGuwba>.Position;
					this._movementSide = (short)(targetPosition.x < this.transform.position.x ? -1f : 1f);
					this.StartCoroutine(this.Dash());
				}
		}
	};
};
