using UnityEngine;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent]
	internal sealed class TouchActivator : Activator
	{
		[SerializeField] private GameObject[] _objectsActivators;
		[SerializeField] private bool _enterCollision;
		[SerializeField] private bool _exitCollision;
		[SerializeField] private bool _enterTrigger;
		[SerializeField] private bool _exitTrigger;
		[SerializeField] private bool _destroyObject;
		private void Activate(bool activationKey, GameObject objectKey)
		{
			if (activationKey)
				if (this._objectsActivators != null)
				{
					foreach (GameObject activator in this._objectsActivators)
						if (activator != null && activator == objectKey)
						{
							this.Activation();
							if (this._destroyObject)
								Destroy(activator);
							return;
						}
				}
				else
					this.Activation();
		}
		private void OnCollisionEnter2D(Collision2D other) => this.Activate(this._enterCollision, other.gameObject);
		private void OnTriggerEnter2D(Collider2D other) => this.Activate(this._enterTrigger, other.gameObject);
		private void OnCollisionExit2D(Collision2D other) => this.Activate(this._exitCollision, other.gameObject);
		private void OnTriggerExit2D(Collider2D other) => this.Activate(this._exitTrigger, other.gameObject);
	};
};
