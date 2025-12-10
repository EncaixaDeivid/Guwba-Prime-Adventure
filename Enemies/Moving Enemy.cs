using UnityEngine;
using System.Collections;
using GwambaPrimeAdventure.Character;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
{
	internal abstract class MovingEnemy : EnemyProvider, IConnector
	{
		protected Vector2 _originCast = Vector2.zero;
		protected Vector2 _sizeCast = Vector2.zero;
		private bool _onGround = false;
		protected bool _detected = false;
		protected bool _isDashing = false;
		protected float _stoppedTime = 0F;
		protected short _movementSide = 1;
		[Header("Moving Enemy")]
		[SerializeField, Tooltip("The moving statitics of this enemy.")] private MovingStatistics _moving;
		protected bool OnGround => _onGround;
		protected new void Awake()
		{
			base.Awake();
			_sender.SetFormat(MessageFormat.State);
			Sender.Include(this);
		}
		protected new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		protected IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			transform.TurnScaleX(_movementSide = (short)((GwambaStateMarker.Localization.x < transform.position.x ? -1 : 1) * (_moving.InvertMovementSide ? -1 : 1)));
		}
		protected void FixedUpdate() => _onGround = false;
		protected void OnCollisionStay2D(Collision2D collision)
		{
			_originCast.Set(transform.position.x + _collider.offset.x, transform.position.y + _collider.offset.y);
			_originCast.y += (_collider.bounds.extents.y + WorldBuild.SNAP_LENGTH / 2f) * -transform.up.y;
			_sizeCast.Set(_collider.bounds.size.x - WorldBuild.SNAP_LENGTH, WorldBuild.SNAP_LENGTH);
			_onGround = Physics2D.BoxCast(_originCast, _sizeCast, 0F, -transform.up, WorldBuild.SNAP_LENGTH, WorldBuild.SCENE_LAYER_MASK);
		}
		public void Receive(MessageData message)
		{
			if (MessageFormat.State == message.Format && message.ToggleValue.HasValue)
				_stopWorking = !message.ToggleValue.Value;
		}
	};
};
