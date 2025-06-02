using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class JumperBoss : BossController, IConnector
	{
		private bool _stopJump = false;
		[Header("Jumper Boss")]
		[SerializeField, Tooltip("The collection of the objet that carry the jump")] private JumpPointStructure[] _jumpPointStructures;
		[SerializeField, Tooltip("The other target to move to on jump.")] private Vector2 _otherTarget;
		[SerializeField, Tooltip("The speed to moves on a high jump.")] private ushort _followSpeed;
		[SerializeField, Tooltip("The strenght of the jump on a react of damage.")] private ushort _strenghtReact;
		[SerializeField, Tooltip("Will stops the execution of a event jump.")] private bool _waitEvent;
		[SerializeField, Tooltip("If the react to damage jump is a high jump.")] private bool _highReact;
		[SerializeField, Tooltip("If it will stop moving on react to damage.")] private bool _stopMoveReact;
		[SerializeField, Tooltip("If the react to damage will use other target.")] private bool _useTarget;
		[SerializeField, Tooltip("If the target to follow will be random.")] private bool _randomFollow;
		private void HighJump(Vector2 otherTarget, bool useTarget)
		{
			this.StartCoroutine(FollowTarget());
			IEnumerator FollowTarget()
			{
				yield return new WaitUntil(() => !this.SurfacePerception() && this.enabled);
				Sender sender = Sender.Create();
				sender.SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Action);
				sender.SetBossType(BossType.Runner).SetToggle(false).Send();
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
					if (this.enabled)
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
						yield return new WaitUntil(() => this.SurfacePerception() && this.enabled && (!this._waitEvent || this._stopJump));
						if (!this._stopJump)
							if (this._jumpPointStructures[index].RemovalJumpCount-- <= 0f)
							{
								if (this._jumpPointStructures[index].StopMove)
								{
									Sender sender = Sender.Create();
									sender.SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Action);
									sender.SetBossType(BossType.Runner).SetToggle(false).Send();
									this._rigidybody.linearVelocityX = 0f;
								}
								this._rigidybody.AddForceY(this._jumpPointStructures[index].Strength);
								if (this._jumpPointStructures[index].High)
									this.HighJump(this._jumpPointStructures[index].OtherTarget, this._jumpPointStructures[index].UseTarget);
								this._jumpPointStructures[index].RemovalJumpCount = (short)this._jumpPointStructures[index].JumpCount;
							}
					}
				});
			}
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			BossType bossType = (BossType)additionalData;
			if (bossType.HasFlag(BossType.Jumper) || bossType.HasFlag(BossType.All))
				if (data.ConnectionState == ConnectionState.Action && data.ToggleValue.HasValue && this._hasToggle)
					this._stopJump = !data.ToggleValue.Value;
				else if (data.ConnectionState == ConnectionState.Action && this._reactToDamage)
				{
					if (this._stopMoveReact)
					{
						Sender sender = Sender.Create();
						sender.SetToWhereConnection(PathConnection.Boss).SetConnectionState(ConnectionState.Action);
						sender.SetBossType(BossType.Runner).SetToggle(false).Send();
						this._rigidybody.linearVelocityX = 0f;
					}
					this._collider.isTrigger = true;
					this._rigidybody.AddForceY(this._strenghtReact);
					if (this._highReact)
						this.HighJump(this._otherTarget, this._useTarget);
				}
		}
		[System.Serializable]
		private struct JumpPointStructure
		{
			[SerializeField, Tooltip("The object to activate the jump.")] private JumpPoint _jumpPointObject;
			[SerializeField, Tooltip("Where the jump point will be.")] private Vector2 _point;
			[SerializeField, Tooltip("To where this have to go if theres no target.")] private Vector2 _otherTarget;
			[SerializeField, Tooltip("The amount of times the boss have to pass by to activate the jump.")] private Vector2Int _jumpCountMaxMin;
			[SerializeField, Tooltip("The strenght of the jump.")] private ushort _strength;
			[SerializeField, Tooltip("If in the high jumo it will stop moving.")] private bool _stopMove;
			[SerializeField, Tooltip("If this is a high jump.")] private bool _high;
			[SerializeField, Tooltip("If for this jump it will use the other target.")] private bool _useTarget;
			internal readonly JumpPoint JumpPointObject => this._jumpPointObject;
			internal readonly Vector2 Point => this._point;
			internal readonly Vector2 OtherTarget => this._otherTarget;
			internal readonly ushort Strength => this._strength;
			internal readonly ushort JumpCount => (ushort)Random.Range(this._jumpCountMaxMin.x, this._jumpCountMaxMin.y);
			internal readonly bool StopMove => this._stopMove;
			internal readonly bool High => this._high;
			internal readonly bool UseTarget => this._useTarget;
			internal short RemovalJumpCount { get; set; }
		};
	};
};
