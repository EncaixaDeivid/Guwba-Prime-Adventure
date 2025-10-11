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
		[SerializeField, Tooltip("The scene of the menu.")] private SceneField _menuScene;
		public void Transicion(SceneField scene = null)
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
				SceneField newScene = scene ?? this._sceneTransicion;
				if (newScene != this.gameObject.scene.name)
					for (ushort i = 0; i < saveFile.levelsCompleted.Length; i++)
						if (newScene.SceneName.Contains($"{i}"))
							saveFile.lastLevelEntered = newScene;
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
				if (newScene != this._menuScene)
				{
					DontDestroyOnLoad(transicionHud);
					yield return new WaitUntil(() => asyncOperation.isDone);
				}
				else
				{
					this._transicionHud.LoadingBar.highValue = 100f;
					while (!asyncOperation.isDone)
					{
						this._transicionHud.LoadingBar.value = asyncOperation.progress * 100f;
						yield return new WaitForEndOfFrame();
					}
				}
				asyncOperation.allowSceneActivation = true;
			}
		}
	};
};
