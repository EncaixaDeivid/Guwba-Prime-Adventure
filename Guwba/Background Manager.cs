using UnityEngine;
using UnityEngine.Rendering;
using Unity.Cinemachine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Camera), typeof(CinemachineBrain))]
	[RequireComponent(typeof(SortingGroup))]
	internal sealed class BackgroundManager : StateController, IConnector
	{
		private static BackgroundManager _instance;
		private Transform[] _childrenTransforms;
		private SpriteRenderer[] _childrenRenderers;
		private Vector2 _startPosition = Vector2.zero;
		private Vector2 _positionDamping = new();
		[Header("Camera Objects")]
		[SerializeField, Tooltip("The object that handles the follow of the camera.")] private CinemachineFollow _cinemachineFollow;
		[SerializeField, Tooltip("The amount of time to wait to start restoring.")] private float _waitTime;
		[Header("Background Objects")]
		[SerializeField, Tooltip("The object that handles the backgrounds.")] private Transform _backgroundTransform;
		[SerializeField, Tooltip("The images that are placed in each background.")] private Sprite[] _backgroundImages;
		[Header("Background Stats")]
		[SerializeField, Tooltip("The amount of speed that the background will move horizontally.")] private float _horizontalBackgroundSpeed;
		[SerializeField, Tooltip("The amount of speed that the background will move vertically.")] private float _verticalBackgroundSpeed;
		[SerializeField, Tooltip("The amount to slow horizontally for each layer that is after the first.")] private float _slowHorizontal;
		[SerializeField, Tooltip("The amount to slow vertically for each layer that is after the first.")] private float _slowVertical;
		public PathConnection PathConnection => PathConnection.Guwba;
		private new void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject);
				return;
			}
			_instance = this;
			base.Awake();
			this._childrenTransforms = new Transform[this._backgroundImages.Length];
			this._childrenRenderers = new SpriteRenderer[this._backgroundImages.Length];
			this._positionDamping = this._cinemachineFollow.TrackerSettings.PositionDamping;
			for (ushort ia = 0; ia < this._backgroundImages.Length; ia++)
			{
				this._childrenTransforms[ia] = Instantiate(this._backgroundTransform, this.transform);
				this._childrenRenderers[ia] = this._childrenTransforms[ia].GetComponent<SpriteRenderer>();
				this._childrenRenderers[ia].sprite = this._backgroundImages[ia];
				this._childrenRenderers[ia].sortingOrder = this._backgroundImages.Length - 1 - ia;
				this._childrenTransforms[ia].GetComponent<SortingGroup>().sortingOrder = this._childrenRenderers[ia].sortingOrder;
				float centerX = this._childrenTransforms[ia].position.x;
				float centerY = this._childrenTransforms[ia].position.y;
				Vector2 imageSize = this._childrenRenderers[ia].sprite.bounds.size;
				float right = centerX + imageSize.x;
				float left = centerX - imageSize.x;
				float top = centerY + imageSize.y;
				float bottom = centerY - imageSize.y;
				this._childrenTransforms[ia].GetChild(0).position = new Vector2(left, top);
				this._childrenTransforms[ia].GetChild(1).position = new Vector2(centerX, top);
				this._childrenTransforms[ia].GetChild(2).position = new Vector2(right, top);
				this._childrenTransforms[ia].GetChild(3).position = new Vector2(left, centerY);
				this._childrenTransforms[ia].GetChild(4).position = new Vector2(right, centerY);
				this._childrenTransforms[ia].GetChild(5).position = new Vector2(left, bottom);
				this._childrenTransforms[ia].GetChild(6).position = new Vector2(centerX, bottom);
				this._childrenTransforms[ia].GetChild(7).position = new Vector2(right, bottom);
				for (ushort ib = 0; ib < this._childrenTransforms[ia].childCount; ib++)
					this._childrenTransforms[ia].GetChild(ib).GetComponent<SpriteRenderer>().sprite = this._backgroundImages[ia];
			}
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			if (!_instance || _instance != this)
				return;
			Sender.Exclude(this);
		}
		private void Update()
		{
			for (ushort i = 0; i < this._backgroundImages.Length; i++)
			{
				float axisX = 1f - (this._horizontalBackgroundSpeed - (i * this._slowHorizontal));
				float axisY = 1f - (this._verticalBackgroundSpeed - (i * this._slowVertical));
				float movementAxisX = this.transform.position.x * axisX;
				float movementAxisY = this.transform.position.y * axisY;
				this._childrenTransforms[i].position = new Vector2(this._startPosition.x + movementAxisX, this._startPosition.y + movementAxisY);
				Vector2 imageSize = this._childrenRenderers[i].sprite.bounds.size;
				float distanceAxisX = this.transform.position.x * (1f - axisX);
				float distanceAxisY = this.transform.position.y * (1f - axisY);
				if (distanceAxisX > this._startPosition.x + imageSize.x)
					this._startPosition = new Vector2(this._startPosition.x + imageSize.x, this._startPosition.y);
				else if (distanceAxisX < this._startPosition.x - imageSize.x)
					this._startPosition = new Vector2(this._startPosition.x - imageSize.x, this._startPosition.y);
				if (distanceAxisY > this._startPosition.y + imageSize.y)
					this._startPosition = new Vector2(this._startPosition.x, this._startPosition.y + imageSize.y);
				else if (distanceAxisY < this._startPosition.y - imageSize.y)
					this._startPosition = new Vector2(this._startPosition.x, this._startPosition.y - imageSize.y);
			}
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Action)
			{
				this._cinemachineFollow.TrackerSettings.PositionDamping = Vector2.zero;
				this.StartCoroutine(RestoreDamping());
				IEnumerator RestoreDamping()
				{
					yield return new WaitTime(this, this._waitTime);
					float time = 0f;
					while ((Vector2)this._cinemachineFollow.TrackerSettings.PositionDamping != this._positionDamping)
					{
						this._cinemachineFollow.TrackerSettings.PositionDamping = Vector2.Lerp(Vector2.zero, this._positionDamping, time);
						time += Time.deltaTime;
						yield return new WaitUntil(() => this.enabled);
						yield return new WaitForEndOfFrame();
					}
				}
			}
		}
	};
};
