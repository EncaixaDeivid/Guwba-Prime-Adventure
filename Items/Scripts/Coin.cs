using UnityEngine;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(CircleCollider2D))]
	internal sealed class Coin : StateController, ICollectable
	{
		private Animator _animator;
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
		}
		private void OnEnable() => this._animator.enabled = true;
		private void OnDisable() => this._animator.enabled = false;
		[SerializeField] private bool _saveOnSpecifics;
		public void Collect()
		{
			if (DataFile.Coins < 100f)
				DataFile.Coins += 1;
			if (DataFile.Lifes < 99f && DataFile.Coins >= 100f)
			{
				DataFile.Coins = 0;
				DataFile.Lifes += 1;
			}
			if (DataFile.Lifes >= 99f && DataFile.Coins >= 99f)
				DataFile.Coins = 99;
			if (this._saveOnSpecifics)
				DataFile.GeneralObjects.Add(this.gameObject.name);
			Destroy(this.gameObject);
		}
	};
};
