using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
	internal sealed class RunnerBoss : BossController, IConnector
	{
		private SpriteRenderer _spriteRenderer;
		private Animator _animator;
		private Vector2 _guardVelocity = new();
		private float _guardGravityScale = 0f;
		private bool _stopMovement = false;
		private bool _dashIsOn = false;
		private bool _stopVelocity = false;
		private bool _blockPerception = false;
		private float _runnedDistance = 0f;
		[Header("Runner Boss")]
		[SerializeField, Tooltip("In the react to damage it already have a target.")] private Vector2 _otherTarget;
		[SerializeField, Tooltip("The distance of the rays to hit the ground.")] private float _groundDistance;
		[Header("Animation")]
		[SerializeField, Tooltip("Animation parameter.")] private string _idle;
		[SerializeField, Tooltip("Animation parameter.")] private string _walk;
		[SerializeField, Tooltip("Animation parameter.")] private string _dash;
		[Header("Dash")]
		[SerializeField, Tooltip("If it have a ray to detect the target.")] private bool _rayDetection;
		[SerializeField, Tooltip("If the dash is timed to start when the boss is instantiate.")] private bool _timedDash;
		[SerializeField, Tooltip("If the boss can jump while dashing.")] private bool _jumpDash;
		[SerializeField, Tooltip("If the boss will target other object when react to damage.")] private bool _useOtherTarget;
		[SerializeField, Tooltip("The speed of the boss while dashing.")] private ushort _dashSpeed;
		[SerializeField, Tooltip("The distance of the dash ray.")] private float _rayDistance;
		[SerializeField, Tooltip("The amount of time that before the dash start.")] private float _stopDashTime;
		[SerializeField, Tooltip("The distance of dash will run.")] private float _dashDistance;
		[SerializeField, Tooltip("The amount of time to wait the timed dash to go.")] private float _timeToDash;
		private IEnumerator Dash()
		{
			this._dashIsOn = true;
			this._sender.SetToggle(this._jumpDash);
			this._sender.Send(PathConnection.Boss);
			this._animator.SetBool(this._idle, true);
			this._animator.SetBool(this._walk, false);
			float actualPosition = this.transform.position.x;
			yield return new WaitTime(this, this._stopDashTime);
			float dashValue = this._movementSide < 0f ? -this._movementSide : this._movementSide;
			this._animator.SetBool(this._idle, false);
			this._animator.SetBool(this._walk, true);
			this._animator.SetFloat(this._dash, Mathf.Abs(this._rigidybody.linearVelocityX) / this._dashSpeed);
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
			this._sender.SetToggle(true);
			this._sender.Send(PathConnection.Boss);
		}
		private new void Awake()
		{
			base.Awake();
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
			this._animator = this.GetComponent<Animator>();
			this._guardGravityScale = this._rigidybody.gravityScale;
			this._sender.SetStateForm(StateForm.State);
			if (this._timedDash)
				this.StartCoroutine(TimedDash());
			IEnumerator TimedDash()
			{
				yield return new WaitTime(this, this._timeToDash);
				yield return this.Dash();
				this.StartCoroutine(TimedDash());
			}
		}
		private void OnEnable()
		{
			this._animator.enabled = true;
			this._rigidybody.gravityScale = this._guardGravityScale;
			this._rigidybody.linearVelocity = this._guardVelocity;
		}
		private void OnDisable()
		{
			this._animator.enabled = false;
			this._guardVelocity = this._rigidybody.linearVelocity;
			this._rigidybody.gravityScale = 0f;
			this._rigidybody.linearVelocity = Vector2.zero;
		}
		private void FixedUpdate()
		{
			if (this._stopMovement && !this._dashIsOn)
			{
				this._animator.SetBool(this._idle, true);
				this._animator.SetBool(this._walk, false);
				this._animator.SetFloat(this._dash, 0f);
				this.StopAllCoroutines();
				if (this._stopVelocity)
				{
					this._stopVelocity = false;
					this._rigidybody.linearVelocityX = 0f;
				}
				if (this.SurfacePerception())
				{
					this._stopVelocity = this._stopMovement = false;
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
				if (CentralizableGuwba.EqualObject(raycastHits))
					this.StartCoroutine(this.Dash());
			}
			float xAxisOrigin = (this._collider.bounds.extents.x + this._groundDistance / 2f) * this._movementSide;
			float yAxisOrigin = (this._collider.bounds.extents.y + this._groundDistance / 2f) * this._movementSide;
			float xOrigin = this.transform.position.x + xAxisOrigin * Mathf.Abs(this.transform.right.x);
			float yOrigin = this.transform.position.y + yAxisOrigin * Mathf.Abs(this.transform.right.y);
			Vector2 origin = new(xOrigin, yOrigin);
			float xSize = this._groundDistance + (this._collider.bounds.size.x - this._groundDistance * 2f) * Mathf.Abs(this.transform.right.y);
			float ySize = this._groundDistance + (this._collider.bounds.size.y - this._groundDistance * 2f) * Mathf.Abs(this.transform.right.x);
			Vector2 size = new(xSize, ySize);
			Vector2 direction = this.transform.right;
			float angle = this.transform.rotation.z * Mathf.Rad2Deg;
			RaycastHit2D blockCast = Physics2D.BoxCast(origin, size, angle, direction, this._groundDistance, this._groundLayer);
			this._blockPerception = blockCast && blockCast.collider.TryGetComponent<Surface>(out var surface) && surface.IsScene;
			if (this._blockPerception)
				this._movementSide *= -1;
			this._spriteRenderer.flipX = this._movementSide < 0f;
			if (!this._dashIsOn)
			{
				this._animator.SetBool(this._idle, false);
				this._animator.SetBool(this._walk, true);
				this._animator.SetFloat(this._dash, 1f);
				this._rigidybody.linearVelocityX = this._movementSide * this._movementSpeed;
			}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			BossController[] bosses = (BossController[])additionalData;
			if (bosses != null)
				foreach (BossController boss in bosses)
					if (boss == this)
					{
						if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
							this._stopVelocity = this._stopMovement = !data.ToggleValue.Value;
						else if (data.StateForm == StateForm.Action && this._reactToDamage)
						{
							Vector2 targetPosition;
							if (this._useOtherTarget)
								targetPosition = this._otherTarget;
							else
								targetPosition = CentralizableGuwba.Position;
							this._movementSide = (short)(targetPosition.x < this.transform.position.x ? -1f : 1f);
							this.StartCoroutine(this.Dash());
						}
						break;
					}
		}
	};
};
