using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class RunnerBoss : BossController
	{
		private float _gravityScale = 0f;
		private bool _stopMovement = false, _dashIsOn = false, _stopVelocity = false;
		[Header("Runner Boss"), SerializeField] private Vector2 _otherTarget;
		[SerializeField] private bool
			_rayDetection,
			_frontUpDetection,
			_backUpDetection,
			_turnOnBack,
			_turnOnDash,
			_timedDash,
			_climbWall,
			_speedUpOnClimb,
			_dashOnClimb,
			_jumpDash,
			_eventOnBlock,
			_eventOnDash,
			_indexSummon,
			_useOtherTarget;
		[SerializeField] private ushort _dashSpeed, _summonIndex;
		[SerializeField] private float _groundDistance, _rayDistance, _climbSpeedUp, _stopDashTime, _dashDistance, _timeToDash;
		private IEnumerator Dash()
		{
			this._dashIsOn = true;
			this.Toggle<JumperBoss>(this._jumpDash);
			this._animator.SetBool(this._walk, false);
			Vector2 actualPosition = this.transform.position;
			yield return new WaitTime(this, this._stopDashTime);
			float dashValue = this._movementSide < 0f ? -this._movementSide : this._movementSide;
			this._animator.SetBool(this._walk, true);
			this._animator.SetFloat(this._dash, this._dashSpeed * Time.deltaTime + dashValue);
			if (this._eventOnDash)
				if (this._indexSummon)
					this.Index<SummonerBoss>(this._summonIndex);
				else
					this.Toggle<JumperBoss>(true);
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
			this.Toggle<JumperBoss>(true);
		}
		private new void Awake()
		{
			base.Awake();
			this._gravityScale = this._rigidybody.gravityScale;
			this._toggleEvent = (bool toggleValue) => this._stopVelocity = this._stopMovement = !toggleValue;
			this._reactToDamageEvent = () =>
			{
				Vector2 targetPosition = new();
				if (this._useOtherTarget)
					targetPosition = this._otherTarget;
				else
					targetPosition = GuwbaTransformer<CommandGuwba>.Position;
				this._movementSide = (short)(targetPosition.x < this.transform.position.x ? -1f : 1f);
				this.StartCoroutine(this.Dash());
			};
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
			if ((this._eventOnDash || this._eventOnBlock) && !this._indexSummon)
				this.Toggle<JumperBoss>(false);
			Vector2 dashDirection = this.transform.right * this._movementSide;
			bool frontDashValue = false;
			bool backDashValue = false;
			if (this._rayDetection)
			{
				Vector2 dashOrigin = this.transform.position;
				if (this._frontUpDetection)
					dashOrigin = new Vector2(dashOrigin.x, dashOrigin.y + this._collider.bounds.extents.y);
				RaycastHit2D[] raycastHits = Physics2D.RaycastAll(dashOrigin, dashDirection, this._rayDistance, this._targetLayerMask);
				frontDashValue = GuwbaTransformer<CommandGuwba>.EqualObject(raycastHits);
			}
			if (this._rayDetection || this._turnOnBack)
			{
				Vector2 dashOrigin = this.transform.position;
				if (this._backUpDetection)
					dashOrigin = new Vector2(dashOrigin.x, dashOrigin.y + this._collider.bounds.extents.y);
				RaycastHit2D[] raycastHits = Physics2D.RaycastAll(dashOrigin, -dashDirection, this._rayDistance, this._targetLayerMask);
				backDashValue = GuwbaTransformer<CommandGuwba>.EqualObject(raycastHits);
			}
			if (!this._dashIsOn && this.transform.rotation.z == 0f)
			{
				if (backDashValue && (this._turnOnBack || this._turnOnDash))
					this._movementSide *= -1;
				if (frontDashValue || backDashValue && this._turnOnDash)
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
				bool endClimbSurface = !Physics2D.Raycast(new Vector2(xAxis, yAxis), -this.transform.up, this._groundDistance, this._groundLayer);
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
			if (this._eventOnBlock && blockPerception && !this._climbWall)
				if (this._indexSummon)
					this.Index<SummonerBoss>(this._summonIndex);
				else
					this.Toggle<JumperBoss>(true);
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
	};
};
