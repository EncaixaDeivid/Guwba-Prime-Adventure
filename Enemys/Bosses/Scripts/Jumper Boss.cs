using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class JumperBoss : BossController, IConnector
	{
		private readonly Sender _sender = Sender.Create();
		private bool _stopJump = false;
		[Header("Jumper Boss")]
		[SerializeField, Tooltip("The collection of the objet that carry the jump")] private JumpPointStructure[] _jumpPointStructures;
		[SerializeField, Tooltip("The collection of the jumps timed for this boss.")] private JumpStats[] _timedJumps;
		[SerializeField, Tooltip("The other target to move to on jump.")] private Vector2 _otherTarget;
		[SerializeField, Tooltip("The speed to moves on a high jump.")] private ushort _followSpeed;
		[SerializeField, Tooltip("The strenght of the jump on a react of damage.")] private ushort _strenghtReact;
		[SerializeField, Tooltip("If the react to damage jump is a high jump.")] private bool _highReact;
		[SerializeField, Tooltip("If it will stop moving on react to damage.")] private bool _stopMoveReact;
		[SerializeField, Tooltip("If the react to damage will use other target.")] private bool _useTarget;
		[SerializeField, Tooltip("If the target to follow will be random.")] private bool _randomFollow;
		[SerializeField, Tooltip("The distance the boss will be to the follow target.")] private float _distanceToTarget;
		private void HighJump(Vector2 otherTarget, bool useTarget)
		{
			this.StartCoroutine(FollowTarget());
			IEnumerator FollowTarget()
			{
				yield return new WaitUntil(() => !this.SurfacePerception() && this.enabled);
				this._sender.Send();
				this._rigidybody.linearVelocityX = 0f;
				float randomDirection = 0f;
				if (this._randomFollow)
					randomDirection = Random.Range(-1f, 1f);
				while (!this.SurfacePerception())
				{
					float targetPosition = GuwbaAstral<CommandGuwba>.Position.x;
					if (useTarget)
						targetPosition = otherTarget.x;
					if (this._randomFollow)
					{
						if (randomDirection >= 0f)
							targetPosition = GuwbaAstral<CommandGuwba>.Position.x;
						else if (randomDirection < 0f)
							targetPosition = otherTarget.x;
					}
					float targetDirection = targetPosition - this.transform.position.x;
					this._movementSide = (short)(targetDirection > 0f ? 1f : -1f);
					if (this.enabled && Mathf.Abs(targetPosition - this.transform.position.x) > this._distanceToTarget)
						this._rigidybody.linearVelocityX = this._movementSide * this._followSpeed;
					else
						this._rigidybody.linearVelocityX = 0f;
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => this.enabled);
				}
			}
		}
		private new void Awake()
		{
			base.Awake();
			this._sender.SetToWhereConnection(PathConnection.Boss).SetStateForm(StateForm.Action);
			this._sender.SetAdditionalData(BossType.Runner).SetToggle(false);
			for (ushort i = 0; i < this._jumpPointStructures.Length; i++)
			{
				JumpPoint jumpPoint = this._jumpPointStructures[i].JumpPointObject;
				JumpPoint jumpPointInstance = Instantiate(jumpPoint, this._jumpPointStructures[i].Point, Quaternion.identity);
				this._jumpPointStructures[i].RemovalJumpCount = (short)this._jumpPointStructures[i].JumpCount;
				jumpPointInstance.GetTouch(i, index =>
				{
					this.StartCoroutine(WaitToHitSurface());
					IEnumerator WaitToHitSurface()
					{
						yield return new WaitUntil(() => this.SurfacePerception() && this.enabled);
						if (this._jumpPointStructures[index].RemovalJumpCount-- <= 0f)
						{
							if (this._jumpPointStructures[index].JumpStats.StopMove)
							{
								this._sender.Send();
								this._rigidybody.linearVelocityX = 0f;
							}
							this._rigidybody.AddForceY(this._jumpPointStructures[index].JumpStats.Strength * this._rigidybody.mass);
							if (this._jumpPointStructures[index].JumpStats.High)
							{
								bool useTarget = this._jumpPointStructures[index].JumpStats.UseTarget;
								this.HighJump(this._jumpPointStructures[index].JumpStats.OtherTarget, useTarget);
							}
							this._jumpPointStructures[index].RemovalJumpCount = (short)this._jumpPointStructures[index].JumpCount;
						}
					}
				});
			}
			foreach (JumpStats jumpStats in this._timedJumps)
			{
				this.StartCoroutine(TimedJump(jumpStats));
				IEnumerator TimedJump(JumpStats stats)
				{
					yield return new WaitTime(this, stats.TimeToExecute);
					yield return new WaitUntil(() => this.SurfacePerception() && this.enabled && !this._stopJump);
					if (stats.StopMove)
					{
						this._sender.Send();
						this._rigidybody.linearVelocityX = 0f;
					}
					this._rigidybody.AddForceY(stats.Strength * this._rigidybody.mass);
					if (stats.High)
						this.HighJump(stats.OtherTarget, stats.UseTarget);
					this.StartCoroutine(TimedJump(jumpStats));
				}
			}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			BossType bossType = (BossType)additionalData;
			if (bossType.HasFlag(BossType.Jumper) || bossType.HasFlag(BossType.All))
				if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue && this._hasToggle)
					this._stopJump = !data.ToggleValue.Value;
				else if (data.StateForm == StateForm.Action && this._reactToDamage)
				{
					if (this._stopMoveReact)
					{
						this._sender.Send();
						this._rigidybody.linearVelocityX = 0f;
					}
					this._collider.isTrigger = true;
					this._rigidybody.AddForceY(this._strenghtReact * this._rigidybody.mass);
					if (this._highReact)
						this.HighJump(this._otherTarget, this._useTarget);
				}
		}
		[System.Serializable]
		private struct JumpStats
		{
			[SerializeField, Tooltip("To where this have to go if theres no target.")] private Vector2 _otherTarget;
			[SerializeField, Tooltip("The strenght of the jump.")] private ushort _strength;
			[SerializeField, Tooltip("If in the high jumo it will stop moving.")] private bool _stopMove;
			[SerializeField, Tooltip("If this is a high jump.")] private bool _high;
			[SerializeField, Tooltip("If for this jump it will use the other target.")] private bool _useTarget;
			[SerializeField, Tooltip("The amount of time for this jump to execute.")] private float _timeToExecute;
			internal readonly Vector2 OtherTarget => this._otherTarget;
			internal readonly ushort Strength => this._strength;
			internal readonly bool StopMove => this._stopMove;
			internal readonly bool High => this._high;
			internal readonly bool UseTarget => this._useTarget;
			internal readonly float TimeToExecute => this._timeToExecute;
		};
		[System.Serializable]
		private struct JumpPointStructure
		{
			[SerializeField, Tooltip("The object to activate the jump.")] private JumpPoint _jumpPointObject;
			[SerializeField, Tooltip("The jump stats to use in this structure.")] private JumpStats _jumpStats;
			[SerializeField, Tooltip("Where the jump point will be.")] private Vector2 _point;
			[SerializeField, Tooltip("The amount of times the boss have to pass by to activate the jump.")] private Vector2Int _jumpCountMaxMin;
			internal readonly JumpPoint JumpPointObject => this._jumpPointObject;
			internal readonly JumpStats JumpStats => this._jumpStats;
			internal readonly Vector2 Point => this._point;
			internal readonly ushort JumpCount => (ushort)Random.Range(this._jumpCountMaxMin.x, this._jumpCountMaxMin.y);
			internal short RemovalJumpCount { get; set; }
		};
	};
};
