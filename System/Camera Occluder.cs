using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Camera), typeof(CinemachineBrain))]
	[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
	internal sealed class CameraOccluder : StateController, IConnector
	{
		private static CameraOccluder _instance;
		private Camera _mainCamera;
		private BoxCollider2D _cameraCollider;
		private Vector2 _positionDamping = new();
		[Header("Camera Objects")]
		[SerializeField, Tooltip("The object that handles the follow of the camera.")] private CinemachineFollow _cinemachineFollow;
		[SerializeField, Tooltip("The amount of time to wait to start restoring.")] private float _waitTime;
		public PathConnection PathConnection => PathConnection.System;
		private new void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject);
				return;
			}
			_instance = this;
			base.Awake();
			this._mainCamera = this.GetComponent<Camera>();
			this._cameraCollider = this.GetComponent<BoxCollider2D>();
			this._positionDamping = this._cinemachineFollow.TrackerSettings.PositionDamping;
			float sizeY = this._mainCamera.orthographicSize * 2f;
			float sizeX = sizeY * this._mainCamera.aspect;
			this._cameraCollider.size = new Vector2(sizeX, sizeY);
			foreach (Collider2D objects in Physics2D.OverlapBoxAll(this.transform.position, this._cameraCollider.size, 0f))
				this.SetOtherChildren(objects.gameObject, true);
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			if (!_instance || _instance != this)
				return;
			Sender.Exclude(this);
		}
		private void SetOtherChildren(GameObject gameObject, bool activeValue)
		{
			if (gameObject.TryGetComponent<HiddenObject>(out var hiddenObject))
				for (ushort i = 0; i < hiddenObject.transform.childCount; i++)
					hiddenObject.transform.GetChild(i).gameObject.SetActive(activeValue);
		}
		private void OnTriggerEnter2D(Collider2D other) => this.SetOtherChildren(other.gameObject, true);
		private void OnTriggerExit2D(Collider2D other) => this.SetOtherChildren(other.gameObject, false);
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
