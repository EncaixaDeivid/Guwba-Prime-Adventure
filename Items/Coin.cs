using UnityEngine;
using GwambaPrimeAdventure.Data;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Item
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
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
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
			if (saveFile.Coins < 100f)
				saveFile.Coins += 1;
			if (saveFile.Lifes < 100f && saveFile.Coins >= 100f)
			{
				saveFile.Coins = 0;
				saveFile.Lifes += 1;
			}
			if (saveFile.Lifes >= 100f && saveFile.Coins >= 99f)
				saveFile.Coins = 100;
			if (_saveOnSpecifics && !saveFile.GeneralObjects.Contains(name))
				saveFile.GeneralObjects.Add(name);
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
