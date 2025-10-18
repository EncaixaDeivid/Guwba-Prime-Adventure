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
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_animator = GetComponent<Animator>();
			_collider = GetComponent<CircleCollider2D>();
		}
		private void OnEnable()
		{
			if (_spriteRenderer.enabled)
				_animator.enabled = true;
		}
		private void OnDisable()
		{
			if (_spriteRenderer.enabled)
				_animator.enabled = false;
		}
		public void Collect()
		{
			SaveController.Load(out SaveFile saveFile);
			if (saveFile.coins < 100f)
				saveFile.coins += 1;
			if (saveFile.lifes < 100f && saveFile.coins >= 100f)
			{
				saveFile.coins = 0;
				saveFile.lifes += 1;
			}
			if (saveFile.lifes >= 100f && saveFile.coins >= 99f)
				saveFile.coins = 100;
			if (_saveOnSpecifics && !saveFile.generalObjects.Contains(gameObject.name))
				saveFile.generalObjects.Add(gameObject.name);
			SaveController.WriteSave(saveFile);
			_collider.enabled = _animator.enabled = _spriteRenderer.enabled = false;
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue && data.ToggleValue.Value)
				_collider.enabled = _animator.enabled = _spriteRenderer.enabled = true;
		}
	};
};
