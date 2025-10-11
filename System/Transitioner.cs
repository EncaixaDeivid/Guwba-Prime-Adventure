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
		[SerializeField, Tooltip("The scene that will be trancisionate to.")] private SceneField _sceneTransicion;
		public void Transicion(SceneField scene = null)
		{
			this.StartCoroutine(SceneTransicion());
			IEnumerator SceneTransicion()
			{
				SaveController.Load(out SaveFile saveFile);
				StateController.SetState(false);
				TransicionHud transicionHud = Instantiate(this._transicionHud);
				DontDestroyOnLoad(transicionHud);
				for (float i = 0f; transicionHud.RootVisualElement.style.opacity.value < 1f; i += 0.1f)
				{
					transicionHud.RootVisualElement.style.opacity = i;
					yield return new WaitForEndOfFrame();
				}
				SceneField newScene = scene ?? this._sceneTransicion;
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
				if (newScene != this.gameObject.scene.name)
					for (ushort i = 0; i < saveFile.levelsCompleted.Length; i++)
						if (newScene.SceneName.Contains($"{i}"))
							saveFile.lastLevelEntered = newScene;
				yield return new WaitUntil(() => asyncOperation.isDone);
				asyncOperation.allowSceneActivation = true;
			}
		}
	};
};
