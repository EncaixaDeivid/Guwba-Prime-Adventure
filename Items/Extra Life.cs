using UnityEngine;
using System.Collections;
using GwambaPrimeAdventure.Data;
namespace GwambaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class ExtraLife : StateController, ILoader, ICollectable
	{
		[Header("Condition")]
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		public IEnumerator Load()
		{
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.LifesAcquired.Contains(gameObject.name))
				Destroy(gameObject);
			yield return new WaitForEndOfFrame();
		}
		public void Collect()
		{
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.Lifes < 100f)
				saveFile.Lifes += 1;
			saveFile.LifesAcquired.Add(gameObject.name);
			if (_saveOnSpecifics && !saveFile.GeneralObjects.Contains(gameObject.name))
				saveFile.GeneralObjects.Add(gameObject.name);
			SaveController.WriteSave(saveFile);
			Destroy(gameObject);
		}
	};
};
