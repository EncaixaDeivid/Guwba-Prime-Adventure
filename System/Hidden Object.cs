using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	public sealed class HiddenObject : MonoBehaviour, IConnector
	{
		[SerializeField, Tooltip("If this object will activate the children.")] private bool _initialActive;
		[SerializeField, Tooltip("If this object will turn off the collisions.")] private bool _offCollision;
		public PathConnection PathConnection => PathConnection.System;
		private void Awake()
		{
			for (ushort i = 0; i < this.transform.childCount; i++)
				this.transform.GetChild(i).gameObject.SetActive(this._initialActive);
			this.GetComponent<BoxCollider2D>().enabled = !this._offCollision;
			Sender.Include(this);
		}
		private void OnDestroy() => Sender.Exclude(this);
		public void Receive(DataConnection data, object additionalData)
		{
			if (this == additionalData as HiddenObject && data.StateForm == StateForm.State && data.ToggleValue.HasValue)
					for (ushort i = 0; i < this.transform.childCount; i++)
						this.transform.GetChild(i).gameObject.SetActive(data.ToggleValue.Value);
		}
	};
};
