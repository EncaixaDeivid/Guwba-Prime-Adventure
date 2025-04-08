using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(CircleCollider2D))]
	internal sealed class Coin : StateController, ICollectable
	{
		private Animator _animator;
		private SaveFile _saveFile;
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
			SaveController.Load(out this._saveFile);
		}
		private void OnEnable() => this._animator.enabled = true;
		private void OnDisable() => this._animator.enabled = false;
		[SerializeField] private bool _saveOnSpecifics;
		public void Collect()
		{
			if (this._saveFile.coins < 100f)
				this._saveFile.coins += 1;
			if (this._saveFile.lifes < 99f && this._saveFile.coins >= 100f)
			{
				this._saveFile.coins = 0;
				this._saveFile.lifes += 1;
			}
			if (this._saveFile.lifes >= 99f && this._saveFile.coins >= 99f)
				this._saveFile.coins = 99;
			if (this._saveOnSpecifics && !this._saveFile.generalObjects.Contains(this.gameObject.name))
				this._saveFile.generalObjects.Add(this.gameObject.name);
			SaveController.WriteSave(this._saveFile);
			Destroy(this.gameObject);
		}
	};
};
