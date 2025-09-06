using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	internal sealed class HiddenObject : MonoBehaviour, IConnector
	{
		public PathConnection PathConnection => PathConnection.System;
		private void Awake()
		{
			for (ushort i = 0; i < this.transform.childCount; i++)
				this.transform.GetChild(i).gameObject.SetActive(false);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (additionalData as GameObject == this.gameObject && data.StateForm == StateForm.Action)
				for (ushort i = 0; i < this.transform.childCount; i++)
					this.transform.GetChild(i).gameObject.SetActive(true);
		}
	};
};
