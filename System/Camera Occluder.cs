using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using System.Collections;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(CinemachineCamera), typeof(Rigidbody2D))]
	[RequireComponent(typeof(BoxCollider2D))]
	internal sealed class CameraOccluder : StateController, IConnector
	{
		private static CameraOccluder _instance;
		private Vector2 _positionDamping = new();
		[Header("Camera Objects")]
		[SerializeField, Tooltip("The object that handles the follow of the camera.")] private CinemachineFollow _cinemachineFollow;
		[SerializeField, Tooltip("The scene of the menu.")] private SceneField _menuScene;
		[SerializeField, Tooltip("The amount of time to wait to start restoring.")] private float _waitTime;
		public MessagePath Path => MessagePath.System;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			SceneManager.sceneLoaded += SceneLoaded;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!_instance || _instance != this)
				return;
			StopAllCoroutines();
			SceneManager.sceneLoaded -= SceneLoaded;
			Sender.Exclude(this);
		}
		private IEnumerator Start()
		{
			if (!_instance || _instance != this)
				yield break;
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			CinemachineCamera camera = GetComponent<CinemachineCamera>();
			GetComponent<BoxCollider2D>().size = new Vector2(camera.Lens.OrthographicSize * 2f * camera.Lens.Aspect, camera.Lens.OrthographicSize * 2f);
			_positionDamping = _cinemachineFollow.TrackerSettings.PositionDamping;
			DontDestroyOnLoad(gameObject);
		}
		private UnityAction<Scene, LoadSceneMode> SceneLoaded => (scene, loadMode) =>
		{
			if (scene.name == _menuScene)
				Destroy(gameObject);
		};
		private void SetOtherChildren(GameObject gameObject, bool activate)
		{
			if (!_instance || _instance != this)
				return;
			if (gameObject.TryGetComponent<OcclusionObject>(out var occlusion))
				occlusion.Execution(activate);
		}
		private void OnTriggerEnter2D(Collider2D other) => SetOtherChildren(other.gameObject, true);
		private void OnTriggerExit2D(Collider2D other) => SetOtherChildren(other.gameObject, false);
		public void Receive(MessageData message)
		{
			if (message.Format == MessageFormat.Event)
			{
				_cinemachineFollow.TrackerSettings.PositionDamping = Vector2.zero;
				StartCoroutine(RestoreDamping());
				IEnumerator RestoreDamping()
				{
					yield return new WaitTime(this, _waitTime, true);
					float time = 0f;
					while (time < 1f)
					{
						_cinemachineFollow.TrackerSettings.PositionDamping = Vector2.Lerp(Vector2.zero, _positionDamping, time);
						time += Time.deltaTime;
						yield return new WaitUntil(() => isActiveAndEnabled);
					}
				}
			}
		}
	};
};
