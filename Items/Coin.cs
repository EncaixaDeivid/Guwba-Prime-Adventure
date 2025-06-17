using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(CircleCollider2D))]
	internal sealed class Coin : StateController, ICollectable
	{
		private Animator _animator;
		[Header("Condition")]
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
		}
		private void OnEnable() => this._animator.enabled = true;
		private void OnDisable() => this._animator.enabled = false;
		public void Collect()
		{
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.coins < 100f)
				saveFile.coins += 1;
			if (saveFile.lifes < 99f && saveFile.coins >= 100f)
			{
				saveFile.coins = 0;
				saveFile.lifes += 1;
			}
			if (saveFile.lifes >= 99f && saveFile.coins >= 99f)
				saveFile.coins = 99;
			if (this._saveOnSpecifics && !saveFile.generalObjects.Contains(this.gameObject.name))
				saveFile.generalObjects.Add(this.gameObject.name);
			SaveController.WriteSave(saveFile);
			Destroy(this.gameObject);
		}
	};
};