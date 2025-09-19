using UnityEngine;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(CircleCollider2D))]
	internal sealed class Coin : StateController, ICollectable, IConnector
	{
		private SpriteRenderer _spriteRenderer;
		private Animator _animator;
		private CircleCollider2D _collider;
		[Header("Condition")]
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		public PathConnection PathConnection => PathConnection.Item;
		private new void Awake()
		{
			base.Awake();
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
			this._animator = this.GetComponent<Animator>();
			this._collider = this.GetComponent<CircleCollider2D>();
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
			this._spriteRenderer.enabled = false;
			this._collider.enabled = false;
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue && data.ToggleValue.Value)
			{
				this._spriteRenderer.enabled = true;
				this._collider.enabled = true;
			}
		}
	};
};
