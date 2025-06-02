using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class ProgressObstacle : StateController
	{
		[SerializeField, Tooltip("The index that this object will check if theres anything completed.")] private ushort _progressIndex;
		[SerializeField, Tooltip("If the index is about the boss.")] private bool _isBossProgress;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		private new void Awake()
		{
			base.Awake();
			SaveController.Load(out SaveFile saveFile);
			bool levelCompleted = saveFile.levelsCompleted[this._progressIndex - 1];
			if (this._isBossProgress ? saveFile.deafetedBosses[this._progressIndex - 1] : levelCompleted)
			{
				if (this._saveOnSpecifics && !saveFile.generalObjects.Contains(this.gameObject.name))
				{
					saveFile.generalObjects.Add(this.gameObject.name);
					SaveController.WriteSave(saveFile);
				}
				Destroy(this.gameObject, 0.001f);
			}
		}
	};
};
