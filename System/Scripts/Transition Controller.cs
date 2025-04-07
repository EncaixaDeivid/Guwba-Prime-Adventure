using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using GuwbaPrimeAdventure.Hud;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	public sealed class TransitionController : MonoBehaviour
	{
		private bool _touchActivate = true;
		[SerializeField] private TransicionHud _transicionHud;
		[SerializeField] private string _sceneTransicion;
		[SerializeField] private bool _touchTransicion;
		private void OnCollision(GameObject collisionObject)
		{
			if (collisionObject.CompareTag(this.gameObject.tag))
				if (this._touchTransicion && this._touchActivate)
				{
					this._touchActivate = false;
					this.Transicion();
				}
		}
		public void Transicion(string sceneName = "")
		{
			this.StartCoroutine(SceneTransicion());
			IEnumerator SceneTransicion()
			{
				StateController.SetState(false);
				TransicionHud transicionHud = Instantiate(this._transicionHud);
				for (float i = 0f; i < 1.1f; i += Time.fixedDeltaTime)
				{
					transicionHud.BaseElement.style.opacity = i;
					yield return new WaitForFixedUpdate();
				}
				transicionHud.BaseElement.style.opacity = 1f;
				string newSceneName = sceneName != "" ? sceneName : this._sceneTransicion;
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Single);
				if (newSceneName != this.gameObject.scene.name)
					for (ushort i = 0; i < SaveController.LevelsCompleted.Length; i++)
						if (newSceneName.Contains($"{i}"))
							SaveController.InternalLastLevelEntered = newSceneName;
				asyncOperation.allowSceneActivation = true;
			}
		}
		private void OnCollisionEnter2D(Collision2D other) => this.OnCollision(other.gameObject);
		private void OnTriggerEnter2D(Collider2D other) => this.OnCollision(other.gameObject);
	};
};
