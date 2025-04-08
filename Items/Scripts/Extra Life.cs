using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class ExtraLife : StateController, ICollectable
	{
		[SerializeField] private bool _saveOnSpecifics;
		private new void Awake()
		{
			base.Awake();
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.lifesAcquired.Contains(this.gameObject.name))
				Destroy(this.gameObject, 0.001f);
		}
		public void Collect()
		{
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.lifes < 99f)
				saveFile.lifes += 1;
			saveFile.lifesAcquired.Add(this.gameObject.name);
			if (this._saveOnSpecifics && !saveFile.generalObjects.Contains(this.gameObject.name))
				saveFile.generalObjects.Add(this.gameObject.name);
			SaveController.WriteSave(saveFile);
			Destroy(this.gameObject);
		}
	};
};
