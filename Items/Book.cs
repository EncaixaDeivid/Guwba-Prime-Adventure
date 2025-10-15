using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class Book : StateController, ICollectable
	{
		[Header("Conditions")]
		[SerializeField, Tooltip("The sprite to show when the book gor cacthed.")] private Sprite _bookCacthed;
		private new void Awake()
		{
			base.Awake();
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.books.ContainsKey(gameObject.name))
			{
				if (saveFile.books[gameObject.name])
					GetComponent<SpriteRenderer>().sprite = _bookCacthed;
				return;
			}
			saveFile.books.Add(gameObject.name, false);
		}
		public void Collect()
		{
			SaveController.Load(out SaveFile saveFile);
			if (!saveFile.books[gameObject.name])
				saveFile.books[gameObject.name] = true;
			GetComponent<SpriteRenderer>().sprite = _bookCacthed;
			if (!saveFile.generalObjects.Contains(gameObject.name))
				saveFile.generalObjects.Add(gameObject.name);
			SaveController.WriteSave(saveFile);
		}
	};
};
