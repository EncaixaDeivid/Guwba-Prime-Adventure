using UnityEngine;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent]
	internal sealed class TouchActivator : Activator
	{
		[SerializeField] private GameObject _objectActivator;
		[SerializeField] private bool _enterCollision;
		[SerializeField] private bool _exitCollision;
		[SerializeField] private bool _enterTrigger;
		[SerializeField] private bool _exitTrigger;
		[SerializeField] private bool _destroyObject;
		private void Activate(bool activationKey, bool objectKey)
		{
			if (activationKey)
				if (this._objectActivator)
				{
					if (objectKey)
					{
						this.Activation();
						if (this._destroyObject)
							Destroy(this._objectActivator);
					}
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
