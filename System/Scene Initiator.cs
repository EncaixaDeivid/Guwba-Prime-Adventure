using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Camera))]
	public sealed class SceneInitiator : MonoBehaviour
	{
		private static SceneInitiator _instance;
		[SerializeField, Tooltip("The object that handles the hud of the trancision.")] private TransicionHud _transicionHud;
		[SerializeField, Tooltip("The sub scenes to be lodaed.")] private SceneField[] _subScenes;
		public static bool IsInTrancision() => _instance;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 1e-3f);
				return;
			}
			_instance = this;
		}
		private IEnumerator Start()
		{
			if (!_instance || _instance != this)
				yield break;
			TransicionHud transicionHud = Instantiate(this._transicionHud, this.transform);
			transicionHud.RootVisualElement.style.opacity = 1f;
			transicionHud.LoadingBar.highValue = this._subScenes.Length;
			AsyncOperation asyncOperation;
			float stillProgress;
			foreach (SceneField scene in this._subScenes)
			{
				asyncOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
				while (!asyncOperation.isDone)
				{
					transicionHud.LoadingBar.value += asyncOperation.progress;
					stillProgress = asyncOperation.progress;
					yield return new WaitForEndOfFrame();
					transicionHud.LoadingBar.value -= stillProgress;
				}
				transicionHud.LoadingBar.value += asyncOperation.progress;
				asyncOperation.allowSceneActivation = true;
			}
			Destroy(this.gameObject);
		}
	};
};
