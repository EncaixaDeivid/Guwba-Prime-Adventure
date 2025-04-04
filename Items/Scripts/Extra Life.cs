using UnityEngine;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class ExtraLife : StateController, ICollectable
	{
		[SerializeField] private bool _saveOnSpecifics;
		private new void Awake()
		{
			base.Awake();
			if (SaveController.LifesAcquired.Contains(this.gameObject.name))
				Destroy(this.gameObject, 0.001f);
		}
		public void Collect()
		{
			if (SaveController.Lifes < 99f)
				SaveController.Lifes += 1;
			SaveController.LifesAcquired.Add(this.gameObject.name);
			if (this._saveOnSpecifics)
				SaveController.GeneralObjects.Add(this.gameObject.name);
			Destroy(this.gameObject);
		}
	};
};
