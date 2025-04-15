using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class GroundEnemy : OppositeEnemy, IConnector
	{
		private bool _rotate = true;
		[Header("Ground Enemy"), SerializeField] private Vector2 _sensorOriginPoint;
		[SerializeField] private Vector2 _sensorDestinyPoint;
		[SerializeField] private bool _useGroundPursue, _useCrawlMovement, _useFaceLookVerifier, _targetEveryone;
		[SerializeField] private ushort _increasedSpeed, _faceLookDistance;
		[SerializeField] private float _crawlRayDistance;
		public PathConnection PathConnection => PathConnection.Enemy;
		private new void Awake()
		{
			base.Awake();
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private void FixedUpdate()
		{
			if (this._stopMovement || this.Paralyzed)
				return;
			bool groundWalk = false;
			if (this._useGroundPursue)
				foreach (RaycastHit2D ray in Physics2D.LinecastAll(this._sensorOriginPoint, this._sensorDestinyPoint, this._targetLayerMask))
					if (ray.collider.TryGetComponent<IDamageable>(out _))
					{
						groundWalk = true;
						break;
					}
			bool faceLook = false;
			if (this._useFaceLookVerifier)
			{
				Vector2 rayDirection = this._useCrawlMovement ? this.transform.right * this._movementSide : Vector2.right * this._movementSide;
				float rayDistance = this._faceLookDistance;
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(this.transform.position, rayDirection, rayDistance, this._targetLayerMask))
					if (ray.collider.TryGetComponent<IDamageable>(out _))
					{
						faceLook = true;
						break;
					}
			}
			float speedIncreased = this._movementSpeed + this._increasedSpeed;
			this._spriteRenderer.flipX = this._movementSide < 0f;
			if (this._useCrawlMovement)
			{
				float crawlRayDistance = this._collider.bounds.extents.y + this._crawlRayDistance;
				bool rayValue = Physics2D.Raycast(this.transform.position, -this.transform.up, crawlRayDistance, this._groundLayer);
				if (this._rotate && !rayValue)
				{
					this._rotate = false;
					this.transform.eulerAngles += new Vector3(0f, 0f, this._movementSide * -90f);
				}
				if (rayValue)
					this._rotate = true;
				Vector2 normalSpeed = this._movementSpeed * this.transform.right;
				Vector2 upedSpeed = speedIncreased * this.transform.right;
				this._rigidybody.linearVelocity = faceLook || groundWalk ? upedSpeed : normalSpeed;
				return;
			}
			Vector2 size = new(this._collider.bounds.size.x + .05f, this._collider.bounds.extents.y - .05f);
			bool blockPerception = Physics2D.OverlapBox(this.transform.position, size, 0f, this._groundLayer);
			float xAxis = this.transform.position.x + this._collider.bounds.extents.x * this._movementSide;
			float yAxis = this.transform.position.y - this._collider.bounds.extents.y * this.transform.up.y;
			bool endWalkableSurface = !Physics2D.Raycast(new Vector2(xAxis, yAxis), -this.transform.up, .05f, this._groundLayer);
			if (blockPerception || endWalkableSurface)
				this._movementSide *= -1;
			bool goStraight = faceLook || groundWalk;
			this._rigidybody.linearVelocityX = goStraight ? this._movementSide * speedIncreased : this._movementSpeed * this._movementSide;
		}
		public void Receive(DataConnection data)
		{
			if (data.ConnectionState == ConnectionState.Enable && data.ToggleValue.HasValue && data.ToggleValue.Value)
				this._stopMovement = false;
			else if (data.ConnectionState == ConnectionState.Disable && data.ToggleValue.HasValue && data.ToggleValue.Value)
				this._stopMovement = true;
		}
	};
};
