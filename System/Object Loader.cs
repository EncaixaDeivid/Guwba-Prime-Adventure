using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
namespace GwambaPrimeAdventure
{
	internal sealed class ObjectLoader : MonoBehaviour
	{
		private static readonly List<ILoader> _loader = new();
		private static ushort _progressIndex = 0;
		public IEnumerator Load(ProgressBar progressBar)
		{
			_loader.Clear();
			GetComponentsInChildren<ILoader>(_loader);
			float progress = 0F;
			for (ushort i = 0; i < _loader.Count; i++)
			{
				yield return StartCoroutine(_loader[i].Load());
				progressBar.value -= progress;
				progress = (i + 1F) / _loader.Count;
				progressBar.value += progress;
			}
			if (_loader.Count <= 0)
				progressBar.value += (_progressIndex += 1) / _progressIndex;
			else
				progressBar.value += (_progressIndex += 1) - progressBar.value;
			_loader.Clear();
			transform.DetachChildren();
			Destroy(gameObject);
		}
	};
};
