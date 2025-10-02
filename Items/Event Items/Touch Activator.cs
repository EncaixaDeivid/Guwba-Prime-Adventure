using UnityEngine;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Collider2D))]
	internal sealed class TouchActivator : Activator
	{
		[Header("Touch Activator")]
		[SerializeField, Tooltip("The variety of objects that this activator can be activated.")] private GameObject[] _objectsActivators;
		[SerializeField, Tooltip("If this activator will be activated on a enter of a collision.")] private bool _enterCollision;
		[SerializeField, Tooltip("If this activator will be activated on a exit of a collision.")] private bool _exitCollision;
		[SerializeField, Tooltip("If this activator will be activated on a enter of a trigger collision.")] private bool _enterTrigger;
		[SerializeField, Tooltip("If this activator will be activated on a exit of a trigger collision.")] private bool _exitTrigger;
		[SerializeField, Tooltip("If this activator will destroy the object that activated the activator.")] private bool _destroyObject;
		private void Activate(bool activationKey, GameObject objectKey)
		{
			if (activationKey && this.Usable)
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
