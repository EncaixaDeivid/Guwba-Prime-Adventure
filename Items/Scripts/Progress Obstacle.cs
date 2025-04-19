using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class ProgressObstacle : StateController
	{
		[SerializeField] private ushort _progressIndex;
		[SerializeField] private bool _isBossProgress;
		[SerializeField] private bool _saveOnSpecifics;
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
