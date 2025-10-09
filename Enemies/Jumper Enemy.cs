using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class JumperEnemy : MovingEnemy, IConnector
	{
		private Animator _animator;
		private bool _isJumping = false;
		private bool _stopJump = false;
		[Header("Jumper Enemy")]
		[SerializeField, Tooltip("The jumper statitics of this enemy.")] private JumperStatistics _statistics;
		private void BasicJump(Vector2 target)
		{
			this.StartCoroutine(Jump());
			IEnumerator Jump()
			{
				this._detected = true;
				if (this._statistics.DetectionStop)
				{
					this._sender.SetToggle(false);
					this._sender.Send(PathConnection.Enemy);
					yield return new WaitTime(this, this._statistics.StopTime);
					yield return new WaitUntil(() => this.isActiveAndEnabled && !this.IsStunned);
				}
				this._isJumping = true;
				this._rigidybody.AddForceY(this._rigidybody.mass * this._statistics.JumpStrenght, ForceMode2D.Impulse);
				this.FollowJump(target, true);
			}
		}
		private void FollowJump(Vector2 otherTarget, bool useTarget)
		{
			this.StartCoroutine(FollowTarget());
			IEnumerator FollowTarget()
			{
				yield return new WaitUntil(() => !this.SurfacePerception() && this.isActiveAndEnabled && !this.IsStunned && !this._stopJump);
				this._rigidybody.linearVelocityX = 0f;
				float targetPosition = GuwbaCentralizer.Position.x;
				if (this._statistics.RandomFollow)
					targetPosition = Random.Range(-1, 1) >= 0f ? GuwbaCentralizer.Position.x : otherTarget.x;
				else if (useTarget)
					targetPosition = otherTarget.x;
				this._movementSide = (short)(targetPosition > this.transform.position.x ? 1f : -1f);
				this.transform.localScale = new Vector3()
				{
					x = this._movementSide * Mathf.Abs(this.transform.localScale.x),
					y = this.transform.localScale.y,
					z = this.transform.localScale.z
				};
				while (!this.SurfacePerception())
				{
					bool valid = this.isActiveAndEnabled && !this.IsStunned;
					if (valid && Mathf.Abs(targetPosition - this.transform.position.x) > this._statistics.DistanceToTarget)
						this._rigidybody.linearVelocityX = this._movementSide * this._statistics.MovementSpeed;
					else
						this._rigidybody.linearVelocityX = 0f;
					yield return new WaitForFixedUpdate();
				}
				this._rigidybody.linearVelocityX = 0f;
			}
		}
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
			this._sender.SetStateForm(StateForm.State);
			for (ushort i = 0; i < this._statistics.JumpPointStructures.Length; i++)
			{
				JumpPoint jumpPoint = this._statistics.JumpPointStructures[i].JumpPointObject;
				JumpPoint jumpPointInstance = Instantiate(jumpPoint, this._statistics.JumpPointStructures[i].Point, Quaternion.identity);
				this._statistics.JumpPointStructures[i].RemovalJumpCount = (short)this._statistics.JumpPointStructures[i].JumpCount;
				jumpPointInstance.GetTouch(i, index =>
				{
					this.StartCoroutine(WaitToHitSurface());
					IEnumerator WaitToHitSurface()
					{
						bool valid = this.SurfacePerception() && !this._detected && this.isActiveAndEnabled && !this.IsStunned;
						yield return new WaitUntil(() => valid);
						if (this._stopJump)
							yield break;
						if (this._statistics.JumpPointStructures[index].RemovalJumpCount-- <= 0f)
						{
							if (this._statistics.JumpPointStructures[index].JumpStats.StopMove)
							{
								this._sender.SetToggle(false);
								this._sender.Send(PathConnection.Enemy);
							}
							this._isJumping = true;
							float jumpStrenght = this._statistics.JumpPointStructures[index].JumpStats.Strength * this._rigidybody.mass;
							this._rigidybody.AddForceY(jumpStrenght, ForceMode2D.Impulse);
							if (this._statistics.JumpPointStructures[index].JumpStats.Follow)
							{
								bool useTarget = this._statistics.JumpPointStructures[index].JumpStats.UseTarget;
								this.FollowJump(this._statistics.JumpPointStructures[index].JumpStats.OtherTarget, useTarget);
							}
							short jumpCount = (short)this._statistics.JumpPointStructures[index].JumpCount;
							this._statistics.JumpPointStructures[index].RemovalJumpCount = jumpCount;
						}
					}
				});
			}
			if (this._statistics.SequentialTimmedJumps)
			{
				this.StartCoroutine(SequentialJumps());
				IEnumerator SequentialJumps()
				{
					for (ushort index = 0; index < this._statistics.TimedJumps.Length; index++)
						yield return TimedJump(this._statistics.TimedJumps[index]);
					if (this._statistics.RepeatTimmedJumps)
						this.StartCoroutine(SequentialJumps());
				}
			}
			else
				foreach (JumpStats jumpStats in this._statistics.TimedJumps)
					this.StartCoroutine(TimedJump(jumpStats));
			IEnumerator TimedJump(JumpStats stats)
			{
				bool valid = this.SurfacePerception() && !this._detected && this.isActiveAndEnabled && !this.IsStunned && !this._stopJump;
				yield return new WaitUntil(() => valid);
				yield return new WaitTime(this, stats.TimeToExecute);
				if (stats.StopMove)
				{
					this._sender.SetToggle(false);
					this._sender.Send(PathConnection.Enemy);
					this._rigidybody.linearVelocityX = 0f;
				}
				this._isJumping = true;
				this._rigidybody.AddForceY(stats.Strength * this._rigidybody.mass, ForceMode2D.Impulse);
				if (stats.Follow)
					this.FollowJump(stats.OtherTarget, stats.UseTarget);
				if (!this._statistics.SequentialTimmedJumps)
					this.StartCoroutine(TimedJump(stats));
			}
		}
		private new void OnEnable()
		{
			base.OnEnable();
			this._animator.enabled = true;
		}
		private new void OnDisable()
		{
			base.OnDisable();
			this._animator.enabled = false;
		}
		private void FixedUpdate()
		{
			if (this.IsStunned)
				return;
			if (this._isJumping && this.SurfacePerception())
			{
				this._isJumping = false;
				this._detected = false;
				this._sender.SetToggle(true);
				this._sender.Send(PathConnection.Enemy);
			}
			if (this._stopWorking)
				return;
			Vector2 right = this.transform.right * (this.transform.localScale.x > 0f ? 1f : -1f);
			LayerMask targetLayer = this._statistics.Physics.TargetLayer;
			float lookDistance = this._statistics.LookDistance;
			if (!this._detected && this._statistics.LookPerception && this.SurfacePerception())
				if (this._statistics.CircularDetection)
				{
					foreach (Collider2D collider in Physics2D.OverlapCircleAll(this.transform.position, lookDistance, targetLayer))
						if (collider.TryGetComponent<IDestructible>(out _))
						{
							this.BasicJump(collider.transform.position);
							return;
						}
				}
				else
					foreach (RaycastHit2D ray in Physics2D.RaycastAll(this.transform.position, right, lookDistance, targetLayer))
						if (ray.collider.TryGetComponent<IDestructible>(out _))
						{
							this.BasicJump(ray.collider.transform.position);
							return;
						}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			foreach (EnemyController enemy in (EnemyController[])additionalData)
				if (enemy != this)
					return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				this._stopJump = !data.ToggleValue.Value;
			else if (data.StateForm == StateForm.Action && this._statistics.ReactToDamage)
			{
				if (this._statistics.StopMoveReact)
				{
					this._sender.SetToggle(false);
					this._sender.Send(PathConnection.Enemy);
				}
				this._isJumping = true;
				this._rigidybody.AddForceY(this._statistics.StrenghtReact * this._rigidybody.mass, ForceMode2D.Impulse);
				if (this._statistics.FollowReact)
					this.FollowJump(this._statistics.OtherTarget, this._statistics.UseTarget);
			}
		}
	};
};
