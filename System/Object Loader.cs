using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
namespace GuwbaPrimeAdventure
{
	internal sealed class ObjectLoader : MonoBehaviour
	{
		[SerializeField, Tooltip("The objects to be lodaed.")] private ObjectLoader[] _objectLoaders;
		public IEnumerator Load(ProgressBar progressBar, ushort totalComplete)
		{
			ushort index = 0;
			float stillProgress;
			ILoader[] loaders = GetComponentsInChildren<ILoader>();
			foreach (ObjectLoader loader in _objectLoaders)
			{
				stillProgress = index++ / (_objectLoaders.Length + loaders.Length + totalComplete);
				progressBar.value += stillProgress;
				yield return StartCoroutine(Instantiate(loader).Load(progressBar, (ushort)(_objectLoaders.Length + loaders.Length + totalComplete)));
				progressBar.value -= stillProgress;
			}
			foreach (ILoader loader in loaders)
			{
				stillProgress = index++ / (_objectLoaders.Length + loaders.Length + totalComplete);
				progressBar.value += stillProgress;
				yield return StartCoroutine(loader.Load());
				progressBar.value -= stillProgress;
			}
			transform.DetachChildren();
			Destroy(gameObject);
		}
	};
};