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
			if (DataFile.LifesAcquired.Contains(this.gameObject.name))
				Destroy(this.gameObject, 0.001f);
		}
		public void Collect()
		{
			if (DataFile.Lifes < 99f)
				DataFile.Lifes += 1;
			DataFile.LifesAcquired.Add(this.gameObject.name);
			if (this._saveOnSpecifics)
				DataFile.GeneralObjects.Add(this.gameObject.name);
			Destroy(this.gameObject);
		}
	};
};
