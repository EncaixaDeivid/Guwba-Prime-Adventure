using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class Book : StateController, ICollectable
	{
		[SerializeField, Tooltip("The sprite to show when the book gor cacthed.")] private Sprite _bookCacthed;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		private new void Awake()
		{
			base.Awake();
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.books.ContainsKey(this.gameObject.name))
			{
				if (saveFile.books[this.gameObject.name])
					this.GetComponent<SpriteRenderer>().sprite = this._bookCacthed;
				return;
			}
			saveFile.books.Add(this.gameObject.name, false);
		}
		public void Collect()
		{
			SaveController.Load(out SaveFile saveFile);
			if (!saveFile.books[this.gameObject.name])
				saveFile.books[this.gameObject.name] = true;
			this.GetComponent<SpriteRenderer>().sprite = this._bookCacthed;
			if (this._saveOnSpecifics && !saveFile.generalObjects.Contains(this.gameObject.name))
				saveFile.generalObjects.Add(this.gameObject.name);
			SaveController.WriteSave(saveFile);
		}
	};
};
