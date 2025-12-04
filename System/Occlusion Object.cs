using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	public sealed class OcclusionObject : MonoBehaviour, IConnector
	{
		private readonly List<Transform> _children = new();
		[Header("Interactions")]
		[SerializeField, Tooltip("If this object will activate the children.")] private bool _initialActive;
		[SerializeField, Tooltip("If this object will turn off the collisions.")] private bool _offCollision;
		[SerializeField, Tooltip("If this object will occlude any other object that enter the collision.")] private bool _collisionOcclusion;
		public MessagePath Path => MessagePath.System;
		private void Awake()
		{
			GetComponent<BoxCollider2D>().enabled = !_offCollision;
			Sender.Include(this);
		}
		private void OnDestroy() => Sender.Exclude(this);
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			GetComponentsInChildren<Transform>(_children);
			StateController[] states = GetComponentsInChildren<StateController>();
			yield return new WaitUntil(() =>
			{
				for (ushort i = 0; i < states.Length; i++)
					if (!states[i])
						return false;
				return true;
			});
			for (ushort i = 0; i < transform.childCount; i++)
				_children[i].gameObject.SetActive(_initialActive);
		}
		internal void Execution(bool activate)
		{
			for (ushort i = 0; i < transform.childCount; i++)
				_children[i].gameObject.SetActive(activate);
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_collisionOcclusion && other.TryGetComponent<IOccludee>(out var occludee) && occludee.Occlude && !other.transform.parent)
				other.transform.SetParent(transform);
		}
		public void Receive(MessageData message)
		{
			if (this == message.AdditionalData as OcclusionObject && message.Format == MessageFormat.State && message.ToggleValue.HasValue)
				for (ushort i = 0; i < transform.childCount; i++)
					_children[i].gameObject.SetActive(message.ToggleValue.Value);
		}
	};
};
