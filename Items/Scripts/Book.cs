using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class Book : StateController, ICollectable
	{
		private SaveFile _saveFile;
		[SerializeField] private Sprite _bookCacthed;
		[SerializeField] private bool _saveOnSpecifics;
		private new void Awake()
		{
			base.Awake();
			SaveController.Load(out this._saveFile);
			if (this._saveFile.books.ContainsKey(this.gameObject.name))
			{
				if (this._saveFile.books[this.gameObject.name])
					this.GetComponent<SpriteRenderer>().sprite = this._bookCacthed;
				return;
			}
			this._saveFile.books.Add(this.gameObject.name, false);
		}
		public void Collect()
		{
			if (!this._saveFile.books[this.gameObject.name])
				this._saveFile.books[this.gameObject.name] = true;
			this.GetComponent<SpriteRenderer>().sprite = this._bookCacthed;
			if (this._saveOnSpecifics && !this._saveFile.generalObjects.Contains(this.gameObject.name))
				this._saveFile.generalObjects.Add(this.gameObject.name);
			SaveController.WriteSave(this._saveFile);
		}
	};
};
