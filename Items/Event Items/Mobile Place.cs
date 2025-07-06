using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Linq;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Tilemap), typeof(TilemapRenderer))]
	[RequireComponent(typeof(TilemapCollider2D), typeof(Rigidbody2D), typeof(CompositeCollider2D)), RequireComponent(typeof(Receptor))]
	internal sealed class MobilePlace : StateController, Receptor.IReceptor
	{
		private Coroutine _movementCoroutine;
		private readonly Sender _sender = Sender.Create();
		private Vector2 _startPosition = new();
		private bool _touchActivate = false;
		private ushort _actualPoint = 0;
		[Header("Mobile Place")]
		[SerializeField, Tooltip("The points that this object have to make the trail.")] private Vector2[] _trail;
		[SerializeField, Tooltip("The offset of the plataform object checker.")] private Vector2 _checkerOffset;
		[SerializeField, Tooltip("The size of the plataform object checker.")] private Vector2 _checkerSize;
		[SerializeField, Tooltip("The size of the plataform object checker.")] private LayerMask _checkerLayerMask;
		[SerializeField, Tooltip("The distance of the checker will go.")] private float _distanceChecker;
		[SerializeField, Tooltip("The speed that this object moves.")] private float _movementSpeed;
		[SerializeField, Tooltip("The speed that this object moves to make the same way back.")] private float _speedReturn;
		[SerializeField, Tooltip("The amount of time to wait after the activation.")] private float _waitStartTime;
		[SerializeField, Tooltip("The amount of time to wait after every point of the trail.")] private float _waitWayTime;
		[SerializeField, Tooltip("The amount of time to wait in the end of the trail.")] private float _waitEndTime;
		[SerializeField, Tooltip("If the object will return the way it makes.")] private bool _returnWay;
		[SerializeField, Tooltip("If it will make the trail one time or always.")] private bool _executeAlways;
		[SerializeField, Tooltip("If this object will receive a signal.")] private bool _isReceptor;
		[SerializeField, Tooltip("If it will go to one point on each activation.")] private bool _execution1X1;
		[SerializeField, Tooltip("If it activates at the touch with other object.")] private bool _touchActivation;
		[SerializeField, Tooltip("If it will stop when other object step out of it.")] private bool _stopOutTouch;
		[SerializeField, Tooltip("If it will make the trail one time or always on touch.")] private bool _executeAlwaysTouch;
		[SerializeField, Tooltip("If it will teleport back when hit the destiny.")] private bool _teleportBack;
		private new void Awake()
		{
			base.Awake();
			this._sender.SetToWhereConnection(PathConnection.Guwba);
			this._sender.SetStateForm(StateForm.Enable);
			this._sender.SetAdditionalData(this.transform);
			this._startPosition = this.transform.position;
			this._touchActivate = this._touchActivation;
			if (!this._isReceptor && !this._touchActivation)
				this.StartCoroutine(this.Movement());
		}
		private IEnumerator Movement()
		{
			IEnumerator PassTrails(bool reverseTrail)
			{
				Vector2[] trail = reverseTrail ? this._trail.Reverse().ToArray() : this._trail;
				yield return new WaitTime(this, this._waitStartTime);
				foreach (Vector2 point in trail)
				{
					yield return new WaitUntil(() =>
					{
						this.transform.position = Vector2.MoveTowards(this.transform.position, point, this._movementSpeed * Time.fixedDeltaTime);
						return (Vector2)this.transform.position == point && this.enabled;
					});
					yield return new WaitTime(this, this._waitWayTime);
				}
				yield return new WaitTime(this, this._waitEndTime);
			}
			do
			{
				yield return PassTrails(false);
				if (this._returnWay)
					yield return PassTrails(true);
			}
			while (this._executeAlways);
			this._touchActivate = this._executeAlwaysTouch;
			if (this._teleportBack)
			{
				this.transform.DetachChildren();
				this.transform.position = this._startPosition;
			}
			this._movementCoroutine = null;
		}
		private IEnumerator Movement1X1()
		{
			Vector2 point = this._trail[this._actualPoint];
			this._actualPoint = (ushort)(this._actualPoint < this._trail.Length - 1f ? this._actualPoint + 1f : 0f);
			yield return new WaitTime(this, this._waitStartTime);
			yield return new WaitUntil(() =>
			{
				this.transform.position = Vector2.MoveTowards(this.transform.position, point, this._movementSpeed * Time.fixedDeltaTime);
				return (Vector2)this.transform.position == point && this.enabled;
			});
		}
		public void Execute()
		{
			if (this._execution1X1)
				this.StartCoroutine(this.Movement1X1());
			else
			{
				if (this._movementCoroutine == null)
					this._movementCoroutine = this.StartCoroutine(this.Movement());
				else
					this.StopCoroutine(this._movementCoroutine);
			}
		}
		private void OnCollision(GameObject gameObject)
		{
			Vector2 point = (Vector2)this.transform.position + this._checkerOffset;
			bool isTouching = Physics2D.OverlapBoxAll(point, this._checkerSize, 0f, this._checkerLayerMask)?.Length > 0f;
			if (!isTouching)
				return;
			if (GuwbaAstral<CommandGuwba>.EqualObject(gameObject))
			{
				this._sender.SetToggle(true);
				this._sender.Send();
			}
			if (this._touchActivation && this._touchActivate)
			{
				this._touchActivate = false;
				if (this._execution1X1)
					this.StartCoroutine(this.Movement1X1());
				else
					this.StartCoroutine(this.Movement());
			}
		}
		private void OnCollisionEnter2D(Collision2D collision) => this.OnCollision(collision.gameObject);
		private void OnCollisionStay2D(Collision2D collision) => this.OnCollision(collision.gameObject);
		private void OnCollisionExit2D(Collision2D collision)
		{
			if (GuwbaAstral<CommandGuwba>.EqualObject(collision.gameObject))
			{
				this._sender.SetToggle(false);
				this._sender.Send();
			}
			if (this._stopOutTouch && !this._execution1X1)
				this.StopCoroutine(this.Movement());
		}
	};
};
