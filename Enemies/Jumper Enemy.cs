using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer))]
	internal sealed class JumperEnemy : MovingEnemy, IConnector
	{
		private Animator _animator;
		private bool _stopJump = false;
		[Header("Jumper Enemy")]
		[SerializeField, Tooltip("The jumper statitics of this enemy.")] private JumperStatistics _statistics;
		private void HighJump(Vector2 otherTarget, bool useTarget)
		{
			this.StartCoroutine(FollowTarget());
			IEnumerator FollowTarget()
			{
				yield return new WaitUntil(() => !this.SurfacePerception() && this.isActiveAndEnabled);
				this._sender.Send(PathConnection.Enemy);
				this._rigidybody.linearVelocityX = 0f;
				float randomDirection = 0f;
				if (this._statistics.RandomFollow)
					randomDirection = Random.Range(-1f, 1f);
				while (!this.SurfacePerception())
				{
					float targetPosition = CentralizableGuwba.Position.x;
					if (useTarget)
						targetPosition = otherTarget.x;
					if (this._statistics.RandomFollow)
					{
						if (randomDirection >= 0f)
							targetPosition = CentralizableGuwba.Position.x;
						else if (randomDirection < 0f)
							targetPosition = otherTarget.x;
					}
					float targetDirection = targetPosition - this.transform.position.x;
					this._movementSide = (short)(targetDirection > 0f ? 1f : -1f);
					this.transform.localScale = new Vector3()
					{
						x = this._movementSide < 0f ? -Mathf.Abs(this.transform.localScale.x) : Mathf.Abs(this.transform.localScale.x),
						y = this.transform.localScale.y,
						z = this.transform.localScale.z
					};
					if (this.enabled && Mathf.Abs(targetPosition - this.transform.position.x) > this._statistics.DistanceToTarget)
						this._rigidybody.linearVelocityX = this._movementSide * this._statistics.FollowSpeed;
					else
						this._rigidybody.linearVelocityX = 0f;
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => this.isActiveAndEnabled);
				}
				this._rigidybody.linearVelocityX = 0f;
			}
		}
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
			this._sender.SetStateForm(StateForm.State);
			this._sender.SetToggle(false);
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
						yield return new WaitUntil(() => this.SurfacePerception() && this.isActiveAndEnabled);
						if (this._stopJump)
							yield break;
						if (this._statistics.JumpPointStructures[index].RemovalJumpCount-- <= 0f)
						{
							if (this._statistics.JumpPointStructures[index].JumpStats.StopMove)
							{
								this._sender.Send(PathConnection.Enemy);
								this._rigidybody.linearVelocityX = 0f;
							}
							this._rigidybody.AddForceY(this._statistics.JumpPointStructures[index].JumpStats.Strength * this._rigidybody.mass);
							if (this._statistics.JumpPointStructures[index].JumpStats.High)
							{
								bool useTarget = this._statistics.JumpPointStructures[index].JumpStats.UseTarget;
								this.HighJump(this._statistics.JumpPointStructures[index].JumpStats.OtherTarget, useTarget);
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
				yield return new WaitUntil(() => this.SurfacePerception() && this.isActiveAndEnabled && !this._stopJump);
				yield return new WaitTime(this, stats.TimeToExecute);
				if (stats.StopMove)
				{
					this._sender.Send(PathConnection.Enemy);
					this._rigidybody.linearVelocityX = 0f;
				}
				this._rigidybody.AddForceY(stats.Strength * this._rigidybody.mass);
				if (stats.High)
					this.HighJump(stats.OtherTarget, stats.UseTarget);
				if (!this._statistics.SequentialTimmedJumps)
					this.StartCoroutine(TimedJump(stats));
			}
		}
		private new void OnEnable()
		{
			base.OnEnable();
			this._animator.enabled = true;
		}
		private void OnDisable()
		{
			this._animator.enabled = false;
			this._rigidybody.gravityScale = 0f;
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			EnemyController[] enemies = (EnemyController[])additionalData;
			if (enemies != null)
				foreach (EnemyController enemy in enemies)
					if (enemy == this)
					{
						if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
							this._stopJump = !data.ToggleValue.Value;
						else if (data.StateForm == StateForm.Action && this._statistics.ReactToDamage)
						{
							if (this._statistics.StopMoveReact)
							{
								this._sender.Send(PathConnection.Enemy);
								this._rigidybody.linearVelocityX = 0f;
							}
							this._rigidybody.AddForceY(this._statistics.StrenghtReact* this._rigidybody.mass);
							if (this._statistics.HighReact)
								this.HighJump(this._statistics.OtherTarget, this._statistics.UseTarget);
						}
						break;
					}
		}
	};
};
