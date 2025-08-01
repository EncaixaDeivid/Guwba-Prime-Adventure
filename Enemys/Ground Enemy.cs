using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class GroundEnemy : EnemyController, IConnector
	{
		private readonly Sender _sender = Sender.Create();
		private bool _rotate = true;
		private bool _runTowards = false;
		private ushort _runnedTimes = 0;
		private float _timeRun = 0f;
		[Header("Ground Enemy")]
		[SerializeField, Tooltip("The distance to check for the block perception.")] private float _blockDistance;
		[SerializeField, Tooltip("If this enemy will do some action when look to a target.")] private bool _useFaceLook;
		[SerializeField, Tooltip("If the target is anything.")] private bool _targetEveryone;
		[SerializeField, Tooltip("If this enemy will run away from the target.")] private bool _runFromTarget;
		[SerializeField, Tooltip("If this enemy will run away from the target.")] private bool _runTowardsAfter;
		[SerializeField, Tooltip("Will become invencible while pursuing a target.\nRequires: Defender Enemy")] private bool _invenciblePursue;
		[SerializeField, Tooltip("The distance of the face look.")] private ushort _faceLookDistance;
		[SerializeField, Tooltip("The amount of speed to increase.")] private ushort _increasedSpeed;
		[SerializeField, Tooltip("The amount of times this enemy have to run away from the target.")] private ushort _timesToRun;
		[SerializeField, Tooltip("The amount of time this enemy will run away from or pursue the target.")] private float _runOfTime;
		[Header("Crawl Movement")]
		[SerializeField, Tooltip("If this enemy will crawl on the walls.")] private bool _useCrawlMovement;
		[SerializeField, Tooltip("The gravity applied when crawling.")] private float _crawlGravity;
		[SerializeField, Tooltip("The distance of the ray when crawling.")] private float _crawlRayDistance;
		private new void Awake()
		{
			base.Awake();
			this._timeRun = this._timesToRun;
			this._sender.SetToWhereConnection(PathConnection.Enemy);
			this._sender.SetStateForm(StateForm.Action);
			this._sender.SetAdditionalData(this.gameObject);
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private void FixedUpdate()
		{
			if (this._stopMovement)
				return;
			bool faceLook = false;
			if (this._runFromTarget)
			{
				if (this._timeRun > 0f)
				{
					this._timeRun -= Time.fixedDeltaTime;
					faceLook = true;
				}
				if (this._timeRun <= 0f && faceLook)
				{
					if (this._runTowardsAfter && this._runnedTimes >= this._timesToRun)
					{
						this._runnedTimes = 0;
						this._runTowards = true;
					}
					else if (this._runTowardsAfter)
						this._runnedTimes++;
					faceLook = false;
				}
			}
			if (this._useFaceLook)
			{
				Vector2 rayDirection = this._useCrawlMovement ? this.transform.right * this._movementSide : Vector2.right * this._movementSide;
				float rayDistance = this._faceLookDistance;
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(this.transform.position, rayDirection, rayDistance, this._targetLayerMask))
					if (ray.collider.TryGetComponent<IDamageable>(out _))
					{
						faceLook = true;
						break;
					}
				if (this._invenciblePursue)
				{
					this._sender.SetToggle(faceLook);
					this._sender.Send();
				}
			}
			float speedIncreased = this._movementSpeed + this._increasedSpeed;
			this._spriteRenderer.flipX = this._movementSide < 0f;
			float xAxisOrigin = (this._collider.bounds.extents.x + this._blockDistance / 2f) * this._movementSide;
			float yAxisOrigin = (this._collider.bounds.extents.y + this._blockDistance / 2f) * this._movementSide;
			float xOrigin = this.transform.position.x + xAxisOrigin * Mathf.Abs(this.transform.right.x);
			float yOrigin = this.transform.position.y + yAxisOrigin * Mathf.Abs(this.transform.right.y);
			Vector2 origin = new(xOrigin, yOrigin);
			float xSize = this._blockDistance + (this._collider.bounds.size.x - this._blockDistance * 2f) * Mathf.Abs(this.transform.right.y);
			float ySize = this._blockDistance + (this._collider.bounds.size.y - this._blockDistance * 2f) * Mathf.Abs(this.transform.right.x);
			Vector2 size = new(xSize, ySize);
			Vector2 direction = this.transform.right;
			float angle = this.transform.rotation.z * Mathf.Rad2Deg;
			RaycastHit2D blockCast = Physics2D.BoxCast(origin, size, angle, direction, this._blockDistance, this._groundLayer);
			bool blockPerception = blockCast && blockCast.collider.TryGetComponent<Surface>(out var surface) && surface.IsScene;
			if (this._runFromTarget && this._timeRun <= 0f && faceLook)
			{
				this._timeRun = this._runOfTime;
				if (this._runTowards)
					this._runTowards = false;
				else
					this._movementSide *= -1;
			}
			if (blockPerception && this._rotate)
				this._movementSide *= -1;
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
				Vector2 gravity = Physics2D.gravity.y * this._crawlGravity * Time.deltaTime * this.transform.up;
				Vector2 normalSpeed = this._movementSpeed * this._movementSide * (Vector2)this.transform.right + gravity;
				Vector2 upedSpeed = speedIncreased * this._movementSide * (Vector2)this.transform.right + gravity;
				this._rigidybody.linearVelocity = faceLook ? upedSpeed : normalSpeed;
				return;
			}
			float xAxis = this.transform.position.x + this._collider.bounds.extents.x * this._movementSide;
			float yAxis = this.transform.position.y - this._collider.bounds.extents.y * this.transform.up.y;
			if (!Physics2D.Raycast(new Vector2(xAxis, yAxis), -this.transform.up, .05f, this._groundLayer))
				this._movementSide *= -1;
			this._rigidybody.linearVelocityX = faceLook ? this._movementSide * speedIncreased : this._movementSpeed * this._movementSide;
		}
		public new void Receive(DataConnection data, object additionalData)
		{
			base.Receive(data, additionalData);
			if (additionalData as GameObject != this.gameObject)
				return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				this._stopMovement = !data.ToggleValue.Value;
		}
	};
};
