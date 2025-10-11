using UnityEngine;
using UnityEditor;
using Unity.Cinemachine;
using System;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	[RequireComponent(typeof(Transitioner), typeof(IInteractable))]
	internal sealed class LevelGate : MonoBehaviour, IInteractable
	{
		private LevelGateHud _levelGate;
		private CinemachineCamera _gateCamera;
		private readonly Sender _sender = Sender.Create();
		private bool _isOnInteraction = false;
		private short _defaultPriority;
		[Header("Scene Status")]
		[SerializeField, Tooltip("The object that handles the hud of the level gate.")] private LevelGateHud _levelGateObject;
		[SerializeField, Tooltip("The name of the level scene.")] private SceneAsset _levelScene;
		[SerializeField, Tooltip("The name of the boss scene.")] private SceneAsset _bossScene;
		[SerializeField, Tooltip("The offset that the hud will be.")] private Vector2 _offsetPosition;
		[SerializeField, Tooltip("Where the this camera have to be in the hierarchy.")] private short _overlayPriority;
		private void Awake()
		{
			this._gateCamera = this.GetComponentInChildren<CinemachineCamera>();
			this._sender.SetStateForm(StateForm.Action);
			this._sender.SetAdditionalData(this.gameObject);
			this._levelGate = Instantiate(this._levelGateObject, this.transform);
			this._levelGate.transform.localPosition = this._offsetPosition;
			SaveController.Load(out SaveFile saveFile);
			this._levelGate.Level.clicked += this.EnterLevel;
			if (saveFile.levelsCompleted[ushort.Parse($"{this._levelScene.name[^1]}") - 1])
				this._levelGate.Boss.clicked += this.EnterBoss;
			if (saveFile.deafetedBosses[ushort.Parse($"{this._levelScene.name[^1]}") - 1])
				this._levelGate.Scenes.clicked += this.ShowScenes;
			this._levelGate.Level.SetEnabled(false);
			this._levelGate.Boss.SetEnabled(false);
			this._levelGate.Scenes.SetEnabled(false);
			this._defaultPriority = (short)this._gateCamera.Priority.Value;
		}
		private void OnDestroy()
		{
			SaveController.Load(out SaveFile saveFile);
			this._levelGate.Level.clicked -= this.EnterLevel;
			if (saveFile.levelsCompleted[ushort.Parse($"{this._levelScene.name[^1]}") - 1])
				this._levelGate.Boss.clicked -= this.EnterBoss;
			if (saveFile.deafetedBosses[ushort.Parse($"{this._levelScene.name[^1]}") - 1])
				this._levelGate.Scenes.clicked -= this.ShowScenes;
		}
		private Action EnterLevel => () => this.GetComponent<Transitioner>().Transicion(this._levelScene);
		private Action EnterBoss => () => this.GetComponent<Transitioner>().Transicion(this._bossScene);
		private Action ShowScenes => () => this._sender.Send(PathConnection.Story);
		private void OnTriggerExit2D(Collider2D other)
		{
			if (!this._isOnInteraction || !GuwbaCentralizer.EqualObject(other.gameObject))
				return;
			this._isOnInteraction = false;
			this._gateCamera.Priority.Value = this._defaultPriority;
			this._levelGate.Level.SetEnabled(false);
			this._levelGate.Boss.SetEnabled(false);
			this._levelGate.Scenes.SetEnabled(false);
		}
		public void Interaction()
		{
			this._isOnInteraction = true;
			this._gateCamera.Priority.Value = this._overlayPriority;
			this._levelGate.Level.SetEnabled(true);
			this._levelGate.Boss.SetEnabled(true);
			this._levelGate.Scenes.SetEnabled(true);
		}
	};
};
