using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class ExtraLife : StateController, ICollectable
	{
		[Header("Condition")]
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.lifesAcquired.Contains(gameObject.name))
				Destroy(gameObject);
		}
		public void Collect()
		{
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.lifes < 100f)
				saveFile.lifes += 1;
			saveFile.lifesAcquired.Add(gameObject.name);
			if (_saveOnSpecifics && !saveFile.generalObjects.Contains(gameObject.name))
				saveFile.generalObjects.Add(gameObject.name);
			SaveController.WriteSave(saveFile);
			Destroy(gameObject);
		}
	};
};
