using UnityEngine;
using System.Collections;
using NaughtyAttributes;
using GwambaPrimeAdventure.Data;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	internal sealed class PointSetter : StateController, IConnector
	{
		private static PointSetter _instance;
		private readonly Sender _sender = Sender.Create();
		[Space(WorldBuild.FIELD_SPACE_LENGTH * 2f)]
		[SerializeField, BoxGroup("Hubby World Interaction"), Tooltip("The name of the hubby world scene.")] private SceneField _hubbyWorldScene;
		[SerializeField, BoxGroup("Hubby World Interaction"), Tooltip("Which point setter is setted when scene is the hubby world.")] private ushort _selfIndex;
		public PathConnection PathConnection => PathConnection.Character;
		private new void Awake()
		{
			base.Awake();
			_sender.SetStateForm(StateForm.Event);
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
			if (gameObject.scene.name == _hubbyWorldScene && saveFile.LastLevelEntered != "")
				if (ushort.Parse($"{saveFile.LastLevelEntered[^1]}") == _selfIndex)
					_sender.Send(PathConnection.Character);
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (GwambaStateMarker.EqualObject(other.gameObject) && this != _instance)
				_instance = this;
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Event && data.ToggleValue.HasValue && data.ToggleValue.Value && this == _instance)
				_sender.Send(PathConnection.Character);
		}
	};
};
