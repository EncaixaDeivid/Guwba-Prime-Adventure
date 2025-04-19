using UnityEngine;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	internal sealed class UseObject : StateController
    {
		private Animator _animator;
		[SerializeField] private string _isOn;
		[SerializeField] private string _use;
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
		}
		private void OnEnable() => this._animator.SetFloat(this._isOn, 1f);
		private void OnDisable() => this._animator.SetFloat(this._isOn, 0f);
		private void OnTriggerEnter2D(Collider2D other) => this._animator.SetTrigger(this._use);
	};
};
