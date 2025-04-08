using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class ExtraLife : StateController, ICollectable
	{
		private SaveFile _saveFile;
		[SerializeField] private bool _saveOnSpecifics;
		private new void Awake()
		{
			base.Awake();
			SaveController.Load(out this._saveFile);
			if (this._saveFile.lifesAcquired.Contains(this.gameObject.name))
				Destroy(this.gameObject, 0.001f);
		}
		public void Collect()
		{
			if (this._saveFile.lifes < 99f)
				this._saveFile.lifes += 1;
			this._saveFile.lifesAcquired.Add(this.gameObject.name);
			if (this._saveOnSpecifics && !this._saveFile.generalObjects.Contains(this.gameObject.name))
				this._saveFile.generalObjects.Add(this.gameObject.name);
			SaveController.WriteSave(this._saveFile);
			Destroy(this.gameObject);
		}
	};
};
