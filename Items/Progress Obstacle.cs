using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class ProgressObstacle : StateController
	{
		[Header("Progress Interactions")]
		[SerializeField, Tooltip("The index that this object will check if theres anything completed.")] private ushort _progressIndex;
		[SerializeField, Tooltip("If the index is about the boss.")] private bool _isBossProgress;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			SaveController.Load(out SaveFile saveFile);
			if (_isBossProgress ? saveFile.DeafetedBosses[_progressIndex - 1] : saveFile.LevelsCompleted[_progressIndex - 1])
			{
				if (_saveOnSpecifics && !saveFile.GeneralObjects.Contains(gameObject.name))
				{
					saveFile.GeneralObjects.Add(gameObject.name);
					SaveController.WriteSave(saveFile);
				}
				Destroy(gameObject);
			}
		}
	};
};
