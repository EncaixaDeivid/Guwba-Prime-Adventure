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
			if (SaveFileData.Coins < 100f)
				SaveFileData.Coins += 1;
			if (SaveFileData.Lifes < 99f && SaveFileData.Coins >= 100f)
			{
				SaveFileData.Coins = 0;
				SaveFileData.Lifes += 1;
			}
			if (SaveFileData.Lifes >= 99f && SaveFileData.Coins >= 99f)
				SaveFileData.Coins = 99;
			if (this._saveOnSpecifics)
				SaveFileData.GeneralObjects.Add(this.gameObject.name);
			Destroy(this.gameObject);
		}
	};
};