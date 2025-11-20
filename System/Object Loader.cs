using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
namespace GwambaPrimeAdventure
{
	internal sealed class ObjectLoader : MonoBehaviour
	{
		[SerializeField, Tooltip("The objects to be lodaed.")] private ObjectLoader[] _objectLoaders;
		public IEnumerator Load(ProgressBar progressBar, bool coverProgress, ushort totalComplete)
		{
			ILoader[] loaders = GetComponentsInChildren<ILoader>();
			for (ushort i = 0; i < _objectLoaders.Length; i++)
			{
				progressBar.value += (i + 1f) / (_objectLoaders.Length + loaders.Length + totalComplete);
				yield return StartCoroutine(Instantiate(_objectLoaders[i]).Load(progressBar, false, (ushort)(_objectLoaders.Length + loaders.Length)));
				if (i < _objectLoaders.Length - 1)
					progressBar.value -= (i + 1f) / (_objectLoaders.Length + loaders.Length + totalComplete);
			}
			for (ushort i = 0; i < loaders.Length; i++)
			{
				progressBar.value += (i + 1f) / (_objectLoaders.Length + loaders.Length + totalComplete);
				yield return StartCoroutine(loaders[i].Load());
				if (i < loaders.Length - 1)
					progressBar.value -= (i + 1f) / (_objectLoaders.Length + loaders.Length + totalComplete);
			}
			if (coverProgress && _objectLoaders.Length + loaders.Length <= 0)
				progressBar.value += 1f;
			transform.DetachChildren();
			Destroy(gameObject);
		}
	};
};
