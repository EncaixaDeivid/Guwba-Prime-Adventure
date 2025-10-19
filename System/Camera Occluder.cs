using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Camera), typeof(Rigidbody2D))]
	[RequireComponent(typeof(BoxCollider2D))]
	internal sealed class CameraOccluder : StateController, IConnector
	{
		private static CameraOccluder _instance;
		private Vector2 _positionDamping = new();
		[Header("Camera Objects")]
		[SerializeField, Tooltip("The object that handles the follow of the camera.")] private CinemachineFollow _cinemachineFollow;
		[SerializeField, Tooltip("The amount of time to wait to start restoring.")] private float _waitTime;
		public PathConnection PathConnection => PathConnection.System;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			Camera camera = GetComponent<Camera>();
			GetComponent<BoxCollider2D>().size = new Vector2(camera.orthographicSize * 2f * camera.aspect, camera.orthographicSize * 2f);
			_positionDamping = _cinemachineFollow.TrackerSettings.PositionDamping;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!_instance || _instance != this)
				return;
			Sender.Exclude(this);
		}
		private void SetOtherChildren(GameObject gameObject, bool activate)
		{
			if (gameObject.TryGetComponent<HiddenObject>(out var hiddenObject))
				hiddenObject.Execution(activate);
		}
		private void OnTriggerEnter2D(Collider2D other) => SetOtherChildren(other.gameObject, true);
		private void OnTriggerExit2D(Collider2D other) => SetOtherChildren(other.gameObject, false);
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Action)
			{
				_cinemachineFollow.TrackerSettings.PositionDamping = Vector2.zero;
				StartCoroutine(RestoreDamping());
				IEnumerator RestoreDamping()
				{
					yield return new WaitTime(this, _waitTime);
					float time = 0f;
					while (time < 1f)
					{
						_cinemachineFollow.TrackerSettings.PositionDamping = Vector2.Lerp(Vector2.zero, _positionDamping, time);
						time += Time.deltaTime;
						yield return new WaitUntil(() => isActiveAndEnabled);
						yield return new WaitForEndOfFrame();
					}
				}
			}
		}
	};
};
