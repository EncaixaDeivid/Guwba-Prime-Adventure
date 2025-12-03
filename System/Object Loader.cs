using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
namespace GwambaPrimeAdventure
{
	internal sealed class ObjectLoader : MonoBehaviour
	{
		private static ushort _progressIndex = 0;
		public IEnumerator Load(ProgressBar progressBar)
		{
			ILoader[] loaders = GetComponentsInChildren<ILoader>();
			float progress = 0F;
			for (ushort i = 0; i < loaders.Length; i++)
			{
				yield return StartCoroutine(loaders[i].Load());
				progressBar.value -= progress;
				progress = (i + 1F) / loaders.Length;
				progressBar.value += progress;
			}
			if (loaders.Length <= 0)
				progressBar.value += (_progressIndex += 1) / _progressIndex;
			else
				progressBar.value += (_progressIndex += 1) - progressBar.value;
			transform.DetachChildren();
			Destroy(gameObject);
		}
	};
};
