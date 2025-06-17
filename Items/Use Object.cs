using UnityEngine;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	internal sealed class UseObject : StateController, IInteractable
    {
		private Animator _animator;
		[Header("Animation")]
		[SerializeField, Tooltip("Animation parameter.")] private string _isOn;
		[SerializeField, Tooltip("Animation parameter.")] private string _use;
		[Header("Interaction")]
		[SerializeField, Tooltip("If it have a interaction.")] private bool _isInteractive;
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
		}
		private void OnEnable() => this._animator.SetFloat(this._isOn, 1f);
		private void OnDisable() => this._animator.SetFloat(this._isOn, 0f);
		private void OnTriggerEnter2D(Collider2D other) => this._animator.SetTrigger(this._use);
		public void Interaction() => this._animator.SetTrigger(this._use);
	};
};