using UnityEngine;
using UnityEngine.SceneManagement;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	public sealed class HiddenObject : MonoBehaviour, IConnector
	{
		[Header("Interactions")]
		[SerializeField, Tooltip("If this object have scenes to use.")] private SceneField[] _scenes;
		[SerializeField, Tooltip("If this object will activate the children.")] private bool _initialActive;
		[SerializeField, Tooltip("If this object will turn off the collisions.")] private bool _offCollision;
		public PathConnection PathConnection => PathConnection.System;
		private void Awake()
		{
			if (!this._initialActive && this._scenes == null && this._scenes.Length <= 0f)
				for (ushort i = 0; i < this.transform.childCount; i++)
					this.transform.GetChild(i).gameObject.SetActive(false);
			this.GetComponent<BoxCollider2D>().enabled = !this._offCollision;
			Sender.Include(this);
		}
		private void OnDestroy() => Sender.Exclude(this);
		internal void Execution(bool activate)
		{
			if (this._scenes != null && this._scenes.Length > 0f)
			{
				foreach (SceneField scene in this._scenes)
					for (ushort i = 0; i < SceneManager.loadedSceneCount; i++)
						if (activate && SceneManager.GetSceneAt(i).name != scene)
							SceneManager.LoadScene(scene, LoadSceneMode.Additive);
						else if (SceneManager.GetSceneAt(i).name == scene)
							SceneManager.UnloadSceneAsync(scene);
			}
			else
				for (ushort i = 0; i < this.transform.childCount; i++)
					this.transform.GetChild(i).gameObject.SetActive(activate);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (this == additionalData as HiddenObject && data.StateForm == StateForm.State && data.ToggleValue.HasValue)
					for (ushort i = 0; i < this.transform.childCount; i++)
						this.transform.GetChild(i).gameObject.SetActive(data.ToggleValue.Value);
		}
	};
};
