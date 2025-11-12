using UnityEngine;
using System.Collections;
using GwambaPrimeAdventure.Data;
namespace GwambaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class Book : StateController, ILoader, ICollectable
	{
		[Header("Conditions")]
		[SerializeField, Tooltip("The sprite to show when the book gor cacthed.")] private Sprite _bookCacthed;
		public IEnumerator Load()
		{
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.Books.ContainsKey(gameObject.name))
			{
				if (saveFile.Books[gameObject.name])
					GetComponent<SpriteRenderer>().sprite = _bookCacthed;
				yield break;
			}
			saveFile.Books.Add(gameObject.name, false);
			yield return new WaitForEndOfFrame();
		}
		public void Collect()
		{
			SaveController.Load(out SaveFile saveFile);
			if (!saveFile.Books[gameObject.name])
				saveFile.Books[gameObject.name] = true;
			GetComponent<SpriteRenderer>().sprite = _bookCacthed;
			if (!saveFile.GeneralObjects.Contains(gameObject.name))
				saveFile.GeneralObjects.Add(gameObject.name);
			SaveController.WriteSave(saveFile);
		}
	};
};
