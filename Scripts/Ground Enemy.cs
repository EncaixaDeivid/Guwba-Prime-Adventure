using UnityEngine;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class GroundEnemy : EnemyController
	{
		private bool _rotate = true;
		[Header("Ground Enemy"), SerializeField] private Vector2 _sensorOriginPoint;
		[SerializeField] private Vector2 _sensorDestinyPoint;
		[SerializeField] private bool _useGroundPursue, _useCrawlMovement, _useFaceLookVerifier, _targetEveryone;
		[SerializeField] private ushort _increasedSpeed, _faceLookDistance;
		[SerializeField] private float _crawlRayDistance;
		private new void Awake()
		{
			base.Awake();
			if (this._useCrawlMovement)
				this._rigidybody.gravityScale = 0f;
			this._toggleEvent = (bool toggleValue) => this._stopMovement = !toggleValue;
		}
		private bool EndWalkableSurface()
		{
			float xAxis = this.transform.position.x + this._collider.bounds.extents.x * this._movementSide;
			float yAxis = this.transform.position.y - this._collider.bounds.extents.y;
			return !Physics2D.Raycast(new Vector2(xAxis, yAxis), Vector2.down, 0.05f, this._groundLayer);
		}
		private bool BlockPerception()
		{
			float pointDirection = (this._collider.bounds.extents.x + 0.025f) * this._movementSide;
			Vector2 point = new(this.transform.position.x + pointDirection, this.transform.position.y);
			Vector2 size = new(0.05f, this._collider.bounds.extents.y - 0.05f);
			return Physics2D.OverlapBox(point, size, this.transform.rotation.z, this._groundLayer);
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
				bool rayValue = Physics2D.Raycast(this.transform.position, -this.transform.up, this._crawlRayDistance, this._groundLayer);
				if (this._rotate && !rayValue)
				{
					this._rotate = false;
					this.transform.eulerAngles += new Vector3(0f, 0f, this._movementSide * -90f);
				}
				if (rayValue)
					this._rotate = true;
				Vector2 normalSpeed = this._movementSpeed * this._movementSide * this.transform.right;
				Vector2 upedSpeed = speedIncreased * this._movementSide * this.transform.right;
				this._rigidybody.linearVelocity = faceLook || groundWalk ? upedSpeed : normalSpeed;
				return;
			}
			if (this.BlockPerception() || this.EndWalkableSurface())
				this._movementSide *= -1;
			this._rigidybody.linearVelocityX = faceLook || groundWalk ? this._movementSide * speedIncreased : this._movementSpeed * this._movementSide;
		}
	};
};