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
			if (SaveController.Books.ContainsKey(this.gameObject.name))
			{
				if (SaveController.Books[this.gameObject.name])
					this.GetComponent<SpriteRenderer>().sprite = this._bookCacthed;
				return;
			}
			SaveController.Books.Add(this.gameObject.name, false);
		}
		public void Collect()
		{
			if (!SaveController.Books[this.gameObject.name])
				SaveController.Books[this.gameObject.name] = true;
			this.GetComponent<SpriteRenderer>().sprite = this._bookCacthed;
			if (this._saveOnSpecifics)
				SaveController.GeneralObjects.Add(this.gameObject.name);
		}
	};
};
