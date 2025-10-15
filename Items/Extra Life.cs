using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class ExtraLife : StateController, ICollectable
	{
		[Header("Condition")]
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		private new void Awake()
		{
			base.Awake();
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.lifesAcquired.Contains(gameObject.name))
				Destroy(gameObject, 1e-3f);
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
