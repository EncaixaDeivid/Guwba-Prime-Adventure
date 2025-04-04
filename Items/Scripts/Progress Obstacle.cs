using UnityEngine;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal sealed class ProgressObstacle : StateController
	{
		[SerializeField] private ushort _progressIndex;
		[SerializeField] private bool _isBossProgress, _saveOnSpecifics;
		private new void Awake()
		{
			base.Awake();
			bool levelCompleted = SaveController.LevelsCompleted[this._progressIndex];
			if (this._isBossProgress ? SaveController.DeafetedBosses[this._progressIndex] : levelCompleted)
			{
				if (this._saveOnSpecifics)
					SaveController.GeneralObjects.Add(this.gameObject.name);
				Destroy(this.gameObject, 0.001f);
			}
		}
	};
};
