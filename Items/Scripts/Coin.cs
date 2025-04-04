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
			if (SaveController.Coins < 100f)
				SaveController.Coins += 1;
			if (SaveController.Lifes < 99f && SaveController.Coins >= 100f)
			{
				SaveController.Coins = 0;
				SaveController.Lifes += 1;
			}
			if (SaveController.Lifes >= 99f && SaveController.Coins >= 99f)
				SaveController.Coins = 99;
			if (this._saveOnSpecifics)
				SaveController.GeneralObjects.Add(this.gameObject.name);
			Destroy(this.gameObject);
		}
	};
};
