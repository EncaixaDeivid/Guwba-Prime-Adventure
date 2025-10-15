using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	internal sealed class PointSetter : StateController, IConnector
	{
		private static Vector2 _checkpointIndex = new();
		private bool _isChecked = false;
		[Header("Hubby World Interaction")]
		[SerializeField, Tooltip("The name of the hubby world scene.")] private string _levelSelectorScene;
		[SerializeField, Tooltip("Which point is checked when scene is the level selector.")] private ushort _selfIndex;
		public PathConnection PathConnection => PathConnection.Character;
		private new void Awake()
		{
			base.Awake();
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			SaveController.Load(out SaveFile saveFile);
			if (gameObject.scene.name == _levelSelectorScene && saveFile.lastLevelEntered != "")
				if (ushort.Parse($"{saveFile.lastLevelEntered[^1]}") == _selfIndex)
					GuwbaCentralizer.Position = transform.position;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!_isChecked && GuwbaCentralizer.EqualObject(other.gameObject))
			{
				_isChecked = true;
				_checkpointIndex = transform.position;
			}
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (_isChecked && data.StateForm == StateForm.Action && data.ToggleValue.HasValue && data.ToggleValue.Value)
				GuwbaCentralizer.Position = _checkpointIndex;
		}
	};
};
