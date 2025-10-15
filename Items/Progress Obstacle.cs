using UnityEngine;
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
		private new void Awake()
		{
			base.Awake();
			SaveController.Load(out SaveFile saveFile);
			bool levelCompleted = saveFile.levelsCompleted[_progressIndex - 1];
			if (_isBossProgress ? saveFile.deafetedBosses[_progressIndex - 1] : levelCompleted)
			{
				if (_saveOnSpecifics && !saveFile.generalObjects.Contains(gameObject.name))
				{
					saveFile.generalObjects.Add(gameObject.name);
					SaveController.WriteSave(saveFile);
				}
				Destroy(gameObject, 1e-3f);
			}
		}
	};
};
