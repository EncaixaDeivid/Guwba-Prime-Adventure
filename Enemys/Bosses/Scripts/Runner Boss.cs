using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class RunnerBoss : BossController, IConnector
	{
		private readonly Sender _sender = Sender.Create();
		private bool _stopMovement = false;
		private bool _dashIsOn = false;
		private bool _stopVelocity = false;
		private bool _blockPerception = false;
		private float _runnedDistance = 0f;
		[Header("Runner Boss")]
		[SerializeField, Tooltip("In the react to damage it already have a target.")] private Vector2 _otherTarget;
		[SerializeField, Tooltip("If it have a ray to detect the target.")] private bool _rayDetection;
		[SerializeField, Tooltip("If the dash is timed to start when the boss is instantiate.")] private bool _timedDash;
		[SerializeField, Tooltip("If the boss can jump while dashing.")] private bool _jumpDash;
		[SerializeField, Tooltip("If the boss will target other object when react to damage.")] private bool _useOtherTarget;
		[SerializeField, Tooltip("The speed of the boss while dashing.")] private ushort _dashSpeed;
		[SerializeField, Tooltip("The distance of the rays to hit the ground.")] private float _groundDistance;
		[SerializeField, Tooltip("The distance of the dash ray.")] private float _rayDistance;
		[SerializeField, Tooltip("The maount of time that before the dash start.")] private float _stopDashTime;
		[SerializeField, Tooltip("The distance of dash will run.")] private float _dashDistance;
		[SerializeField, Tooltip("The amount of time to wait the timed dash to go.")] private float _timeToDash;
		private IEnumerator Dash()
		{
			this._dashIsOn = true;
			this._sender.SetToggle(this._jumpDash).Send();
			this._animator.SetBool(this._walk, false);
			float actualPosition = this.transform.position.x;
			yield return new WaitTime(this, this._stopDashTime);
			float dashValue = this._movementSide < 0f ? -this._movementSide : this._movementSide;
			this._animator.SetBool(this._walk, true);
			this._animator.SetFloat(this._dash, this._dashSpeed * Time.fixedDeltaTime + dashValue);
			yield return new WaitUntil(() =>
			{
				Vector2 linearVelocity = new(this._dashSpeed * this._movementSide, this._rigidybody.linearVelocity.y);
				this._rigidybody.linearVelocity = this.enabled ? linearVelocity : Vector2.zero;
				if (this._blockPerception)
					this._runnedDistance += Mathf.Abs(this.transform.position.x - actualPosition);
				return Mathf.Abs(this.transform.position.x - actualPosition) + this._runnedDistance >= this._dashDistance && this.enabled;
			});
			this._runnedDistance = 0f;
			this._dashIsOn = false;
			this._sender.SetToggle(true).Send();
		}
		private new void Awake()
		{
			base.Awake();
			this._sender.SetToWhereConnection(PathConnection.Boss).SetStateForm(StateForm.Action);
			this._sender.SetAdditionalData(BossType.Jumper);
			if (this._timedDash)
				this.StartCoroutine(TimedDash());
			IEnumerator TimedDash()
			{
				yield return new WaitTime(this, this._timeToDash);
				yield return this.Dash();
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
			if (this._rayDetection && !this._dashIsOn)
			{
				Vector2 dashOrigin = this.transform.position;
				Vector2 dashDirection = this.transform.right * this._movementSide;
				RaycastHit2D[] raycastHits = Physics2D.RaycastAll(dashOrigin, dashDirection, this._rayDistance, this._targetLayerMask);
				if (GuwbaAstral<CommandGuwba>.EqualObject(raycastHits))
					this.StartCoroutine(this.Dash());
			}
			float xPoint = (this._collider.bounds.extents.x + this._groundDistance / 2f) * this._movementSide;
			Vector2 point = new(this.transform.position.x + xPoint, this.transform.position.y);
			Vector2 size = new(this._groundDistance, this._collider.bounds.size.y - this._groundDistance);
			this._blockPerception = Physics2D.OverlapBox(point, size, this.transform.eulerAngles.z, this._groundLayer);
			if (this._blockPerception)
				this._movementSide *= -1;
			this._spriteRenderer.flipX = this._movementSide < 0f;
			if (!this._dashIsOn)
			{
				this._animator.SetBool(this._walk, true);
				this._animator.SetFloat(this._dash, 1f);
				this._rigidybody.linearVelocityX = this._movementSide * this._movementSpeed;
			}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			BossType bossType = (BossType)additionalData;
			if (bossType.HasFlag(BossType.Runner) || bossType.HasFlag(BossType.All))
				if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue && this._hasToggle)
					this._stopVelocity = this._stopMovement = !data.ToggleValue.Value;
				else if (data.StateForm == StateForm.Action && this._reactToDamage)
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
