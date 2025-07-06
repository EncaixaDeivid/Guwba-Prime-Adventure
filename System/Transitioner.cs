using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	public sealed class Transitioner : MonoBehaviour
	{
		[Header("Scene Interaction")]
		[SerializeField, Tooltip("The object that handles the hud of the trancision.")] private TransicionHud _transicionHud;
		[SerializeField, Tooltip("The name of the scene that will trancisionate to.")] private string _sceneTransicion;
		public void Transicion(string sceneName = "")
		{
			this.StartCoroutine(SceneTransicion());
			IEnumerator SceneTransicion()
			{
				SaveController.Load(out SaveFile saveFile);
				StateController.SetState(false);
				TransicionHud transicionHud = Instantiate(this._transicionHud);
				for (float i = 0f; transicionHud.RootVisualElement.style.opacity.value < 1f; i += 0.1f)
				{
					transicionHud.RootVisualElement.style.opacity = i;
					yield return new WaitForEndOfFrame();
				}
				string newSceneName = sceneName != "" ? sceneName : this._sceneTransicion;
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Single);
				if (newSceneName != this.gameObject.scene.name)
					for (ushort i = 0; i < saveFile.levelsCompleted.Length; i++)
						if (newSceneName.Contains($"{i}"))
							saveFile.lastLevelEntered = newSceneName;
				while (!asyncOperation.isDone)
				{
					transicionHud.LoadingBar.value = asyncOperation.progress * 100f;
					yield return new WaitForEndOfFrame();
				}
				asyncOperation.allowSceneActivation = true;
			}
		}
	};
};
