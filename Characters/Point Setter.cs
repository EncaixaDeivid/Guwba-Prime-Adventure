using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	internal sealed class PointSetter : StateController, IConnector
	{
		private static PointSetter _instance;
		private readonly Sender _sender = Sender.Create();
		[Header("Hubby World Interaction")]
		[SerializeField, Tooltip("The name of the hubby world scene.")] private string _levelSelectorScene;
		[SerializeField, Tooltip("Which point is checked when scene is the level selector.")] private ushort _selfIndex;
		public PathConnection PathConnection => PathConnection.Character;
		private new void Awake()
		{
			base.Awake();
			_sender.SetStateForm(StateForm.Action);
			_sender.SetAdditionalData((Vector2)transform.position);
			_sender.SetToggle(false);
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
			if (gameObject.scene.name == _levelSelectorScene && saveFile.LastLevelEntered != "")
				if (ushort.Parse($"{saveFile.LastLevelEntered[^1]}") == _selfIndex)
					_sender.Send(PathConnection.Character);
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (GuwbaAstralMarker.EqualObject(other.gameObject) && this != _instance)
				_instance = this;
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue && data.ToggleValue.Value && this == _instance)
				_sender.Send(PathConnection.Character);
		}
	};
};
