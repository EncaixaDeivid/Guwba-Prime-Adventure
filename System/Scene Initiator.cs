using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using System.Collections;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Camera), typeof(CinemachineCamera))]
	public sealed class SceneInitiator : MonoBehaviour
	{
		public static SceneInitiator KeepTrancision { get; private set; }
		[SerializeField, Tooltip("The object that handles the hud of the trancision.")] private TransicionHud _transicionHud;
		[SerializeField, Tooltip("The sub scenes to be lodaed.")] private SceneField[] _subScenes;
		private void Awake()
		{
			if (KeepTrancision)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			KeepTrancision = this;
		}
		private IEnumerator Start()
		{
			if (!KeepTrancision || KeepTrancision != this)
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
