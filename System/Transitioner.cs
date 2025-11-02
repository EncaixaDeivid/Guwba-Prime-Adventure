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
		public void Transicion(SceneField scene = null, string sceneName = null)
		{
			StartCoroutine(SceneTransicion());
			IEnumerator SceneTransicion()
			{
				SaveController.Load(out SaveFile saveFile);
				StateController.SetState(false);
				TransicionHud transicionHud = Instantiate(_transicionHud);
				for (float i = 0f; transicionHud.RootElement.style.opacity.value < 1f; i += 0.1f)
				{
					transicionHud.RootElement.style.opacity = i;
					yield return new WaitForEndOfFrame();
				}
				string newScene = scene ?? sceneName ?? _sceneTransicion;
				if (newScene != gameObject.scene.name)
					for (ushort i = 0; i < saveFile.levelsCompleted.Length; i++)
						if (newScene.Contains($"{i}"))
							saveFile.lastLevelEntered = newScene;
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
				if (newScene != _menuScene)
					yield return new WaitUntil(() => asyncOperation.isDone);
				else
				{
					transicionHud.LoadingBar.highValue = 100f;
					while (!asyncOperation.isDone)
					{
						transicionHud.LoadingBar.value = asyncOperation.progress * 100f;
						yield return new WaitForEndOfFrame();
					}
				}
				asyncOperation.allowSceneActivation = true;
			}
		}
	};
};
