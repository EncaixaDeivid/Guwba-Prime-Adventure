using UnityEngine;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent]
	internal sealed class TouchActivator : Activator
	{
		[SerializeField] private GameObject _objectActivator;
		[SerializeField] private bool _enterCollision, _exitCollision, _enterTrigger, _exitTrigger;
		private void Activate(bool activationKey, bool objectKey)
		{
			if (activationKey)
				if (this._objectActivator)
				{
					if (objectKey)
						this.Activation();
				}
				else
					this.Activation();
		}
		private void OnCollisionEnter2D(Collision2D other) => this.Activate(this._enterCollision, this._objectActivator == other.gameObject);
		private void OnTriggerEnter2D(Collider2D other) => this.Activate(this._enterTrigger, this._objectActivator == other.gameObject);
		private void OnCollisionExit2D(Collision2D other) => this.Activate(this._exitCollision, this._objectActivator == other.gameObject);
		private void OnTriggerExit2D(Collider2D other) => this.Activate(this._exitTrigger, this._objectActivator == other.gameObject);
	};
};
