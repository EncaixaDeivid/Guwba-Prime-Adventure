using UnityEngine;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class Book : StateController, ICollectable
	{
		[SerializeField] private Sprite _bookCacthed;
		[SerializeField] private bool _saveOnSpecifics;
		private new void Awake()
		{
			base.Awake();
			if (DataFile.Books.ContainsKey(this.gameObject.name))
			{
				if (DataFile.Books[this.gameObject.name])
					this.GetComponent<SpriteRenderer>().sprite = this._bookCacthed;
				return;
			}
			DataFile.Books.Add(this.gameObject.name, false);
		}
		public void Collect()
		{
			if (!DataFile.Books[this.gameObject.name])
				DataFile.Books[this.gameObject.name] = true;
			this.GetComponent<SpriteRenderer>().sprite = this._bookCacthed;
			if (this._saveOnSpecifics)
				DataFile.GeneralObjects.Add(this.gameObject.name);
		}
	};
};
