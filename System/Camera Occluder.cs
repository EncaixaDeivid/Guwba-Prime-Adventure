using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using System.Collections;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(CinemachineCamera), typeof(CinemachineFollow)), RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
	internal sealed class CameraOccluder : StateController
	{
		private static CameraOccluder _instance;
		private CinemachineFollow _cinemachineFollow;
		[Header("Interactions")]
		[SerializeField, Tooltip("The scene of the menu.")] private SceneField _menuScene;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(gameObject, WorldBuild.MINIMUM_TIME_SPACE_LIMIT);
				return;
			}
			_cinemachineFollow = GetComponent<CinemachineFollow>();
			_instance = this;
			SceneManager.sceneLoaded += SceneLoaded;
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!_instance || _instance != this)
				return;
			StopAllCoroutines();
			SceneManager.sceneLoaded -= SceneLoaded;
		}
		private void OnEnable() => _cinemachineFollow.enabled = true;
		private void OnDisable() => _cinemachineFollow.enabled = false;
		private IEnumerator Start()
		{
			if (!_instance || _instance != this)
				yield break;
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			CinemachineCamera camera = GetComponent<CinemachineCamera>();
			GetComponent<BoxCollider2D>().size = new Vector2(camera.Lens.OrthographicSize * 2F * WorldBuild.HEIGHT_WIDTH_PROPORTION, camera.Lens.OrthographicSize * 2F);
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
	};
};
