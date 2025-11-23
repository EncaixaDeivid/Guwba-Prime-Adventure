using UnityEngine;
using System.Collections;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	public sealed class HiddenObject : MonoBehaviour, IConnector
	{
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
			StateController[] childrenCollection = GetComponentsInChildren<StateController>();
			yield return new WaitUntil(() =>
			{
				foreach (StateController child in childrenCollection)
					if (!child.enabled)
						return false;
				return true;
			});
			for (ushort i = 0; i < transform.childCount; i++)
				transform.GetChild(i).gameObject.SetActive(_initialActive);
		}
		internal void Execution(bool activate)
		{
			for (ushort i = 0; i < transform.childCount; i++)
				transform.GetChild(i).gameObject.SetActive(activate);
		}
		public void Receive(DataConnection data)
		{
			if (this == data.AdditionalData as HiddenObject && data.StateForm == StateForm.State && data.ToggleValue.HasValue)
					for (ushort i = 0; i < transform.childCount; i++)
						transform.GetChild(i).gameObject.SetActive(data.ToggleValue.Value);
		}
	};
};
