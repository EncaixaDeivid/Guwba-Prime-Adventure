using UnityEngine;
using GwambaPrimeAdventure.Connection;
using GwambaPrimeAdventure.Enemy.Utility;
namespace GwambaPrimeAdventure.Enemy
{
	internal abstract class MovingEnemy : EnemyProvider, IConnector
	{
		protected Vector2 _originCast;
		protected Vector2 _sizeCast;
		private bool _onGround = false;
		protected bool _detected = false;
		protected bool _isDashing = false;
		protected float _stoppedTime = 0f;
		protected short _movementSide = 1;
		[Header("Moving Enemy")]
		[SerializeField, Tooltip("The moving statitics of this enemy.")] private MovingStatistics _moving;
		[SerializeField, Tooltip("If this enemy will moves firstly to the left.")] private bool _invertMovementSide;
		protected bool OnGround => _onGround;
		protected new void Awake()
		{
			base.Awake();
			_sender.SetStateForm(StateForm.State);
			transform.TurnScaleX(_movementSide = (short)(_invertMovementSide ? -1 : 1));
			Sender.Include(this);
		}
		protected new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		protected void FixedUpdate() => _onGround = false;
		private void GroundCheck()
		{
			_originCast = new Vector2(transform.position.x + _collider.offset.x, transform.position.y + _collider.offset.y);
			_originCast.y += (_collider.bounds.extents.y + WorldBuild.SNAP_LENGTH / 2f) * -transform.up.y;
			_sizeCast = new Vector2(_collider.bounds.size.x - WorldBuild.SNAP_LENGTH, WorldBuild.SNAP_LENGTH);
			_onGround = Physics2D.BoxCast(_originCast, _sizeCast, 0f, -transform.up, WorldBuild.SNAP_LENGTH, _moving.Physics.GroundLayer);
		}
		private void OnCollisionEnter2D(Collision2D collision) => GroundCheck();
		private void OnCollisionStay2D(Collision2D collision) => GroundCheck();
		public void Receive(DataConnection data)
		{
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				_stopWorking = !data.ToggleValue.Value;
		}
	};
};
