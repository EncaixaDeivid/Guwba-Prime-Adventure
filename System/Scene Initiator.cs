using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
namespace GuwbaPrimeAdventure
{
	internal sealed class SceneInitiator : MonoBehaviour
	{
		private TransicionHud _transicionHud;
		[SerializeField, Tooltip("The sub scenes to be lodaed.")] private SceneField[] _subScenes;
		private IEnumerator Start()
		{
			this._transicionHud = FindFirstObjectByType<TransicionHud>();
			this._transicionHud.LoadingBar.highValue = this._subScenes.Length * 2f;
			AsyncOperation asyncOperation;
			float stillProgress;
			foreach (SceneField scene in this._subScenes)
			{
				asyncOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
				while (!asyncOperation.isDone)
				{
					this._transicionHud.LoadingBar.value += asyncOperation.progress;
					stillProgress = asyncOperation.progress;
					yield return new WaitForEndOfFrame();
					this._transicionHud.LoadingBar.value -= stillProgress;
				}
				this._transicionHud.LoadingBar.value += asyncOperation.progress;
				asyncOperation.allowSceneActivation = true;
			}
			Destroy(this.gameObject);
			Destroy(this._transicionHud.gameObject);
		}
	};
};
