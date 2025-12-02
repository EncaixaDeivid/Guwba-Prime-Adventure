using UnityEngine;
using System.Collections;
namespace GwambaPrimeAdventure.Connection
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	public sealed class SceneInitiator : MonoBehaviour
	{
		private static SceneInitiator _instance;
		[SerializeField, Tooltip("The object that handles the hud of the trancision.")] private TransicionHud _transicionHud;
		[SerializeField, Tooltip("The objects to be lodaed.")] private ObjectLoader[] _objectLoaders;
		public static bool IsInTrancision() => _instance;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
		}
		private IEnumerator Start()
		{
			if (!_instance || _instance != this)
				yield break;
			TransicionHud transicionHud = Instantiate(_transicionHud, transform);
			(transicionHud.RootElement.style.opacity, transicionHud.LoadingBar.highValue) = (1f, _objectLoaders.Length);
			for (ushort i = 0; i < _objectLoaders.Length; i++)
				yield return StartCoroutine(Instantiate(_objectLoaders[i]).Load(transicionHud.LoadingBar));
			Destroy(gameObject);
			StateController.SetState(true);
		}
	};
};