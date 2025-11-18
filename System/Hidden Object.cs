using UnityEngine;
using System.Collections;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	public sealed class HiddenObject : MonoBehaviour, IConnector
	{
		private static readonly WaitForSeconds _waitForSeconds1e = new(1e-3f);
		[Header("Interactions")]
		[SerializeField, Tooltip("If this object will activate the children.")] private bool _initialActive;
		[SerializeField, Tooltip("If this object will turn off the collisions.")] private bool _offCollision;
		public PathConnection PathConnection => PathConnection.System;
		private void Awake()
		{
			GetComponent<BoxCollider2D>().enabled = !_offCollision;
			Sender.Include(this);
		}
		private void OnDestroy() => Sender.Exclude(this);
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			yield return _waitForSeconds1e;
			if (!_initialActive)
				for (ushort i = 0; i < transform.childCount; i++)
					transform.GetChild(i).gameObject.SetActive(false);
		}
		internal void Execution(bool activate)
		{
			for (ushort i = 0; i < transform.childCount; i++)
				transform.GetChild(i).gameObject.SetActive(activate);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (this == additionalData as HiddenObject && data.StateForm == StateForm.State && data.ToggleValue.HasValue)
					for (ushort i = 0; i < transform.childCount; i++)
						transform.GetChild(i).gameObject.SetActive(data.ToggleValue.Value);
		}
	};
};
