using UnityEngine;
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
		private void Start()
		{
			SaveController.Load(out SaveFile saveFile);
			if (this.gameObject.scene.name == this._levelSelectorScene && saveFile.lastLevelEntered != "")
				if (ushort.Parse($"{saveFile.lastLevelEntered[^1]}") == this._selfIndex)
					CentralizableGuwba.Position = this.transform.position;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!this._isChecked && CentralizableGuwba.EqualObject(other.gameObject))
			{
				this._isChecked = true;
				_checkpointIndex = this.transform.position;
			}
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (this._isChecked && data.StateForm == StateForm.Action && data.ToggleValue.HasValue && data.ToggleValue.Value)
				CentralizableGuwba.Position = _checkpointIndex;
		}
	};
};
