using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	public sealed class Transitioner : MonoBehaviour
	{
		[Header("Scene Interaction")]
		[SerializeField, Tooltip("The object that handles the hud of the trancision.")] private TransicionHud _transicionHud;
		[SerializeField, Tooltip("The name of the scene that will trancisionate to.")] private SceneAsset _sceneTransicion;
		public void Transicion(SceneAsset scene = null)
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
				SceneAsset newScene = scene != null ? scene : this._sceneTransicion;
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(newScene.name, LoadSceneMode.Single);
				if (newScene.name != this.gameObject.scene.name)
					for (ushort i = 0; i < saveFile.levelsCompleted.Length; i++)
						if (newScene.name.Contains($"{i}"))
							saveFile.lastLevelEntered = newScene.name;
				yield return new WaitUntil(() => asyncOperation.isDone);
				asyncOperation.allowSceneActivation = true;
			}
		}
	};
};
