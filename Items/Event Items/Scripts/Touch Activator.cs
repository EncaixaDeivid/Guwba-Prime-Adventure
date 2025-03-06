using UnityEngine;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent]
	internal sealed class TouchActivator : Activator
	{
		[SerializeField] private bool _enterCollisionActivation, _exitCollisionActivation, _enterTriggerActivation, _exitTriggerActivation;
		private void OnCollisionEnter2D(Collision2D other)
		{
			if (this._enterCollisionActivation)
				this.Activation();
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this._enterTriggerActivation)
				this.Activation();
		}
		private void OnCollisionExit2D(Collision2D other)
		{
			if (this._exitCollisionActivation)
				this.Activation();
		}
		private void OnTriggerExit2D(Collider2D other)
		{
			if (this._exitTriggerActivation)
				this.Activation();
		}
	};
};