using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	[RequireComponent(typeof(Transitioner), typeof(IInteractable))]
	internal sealed class LevelGate : MonoBehaviour, IInteractable
	{
		private LevelGateHud _levelGate;
		private CinemachineCamera _showCamera;
		private InputController _inputController;
		private readonly Sender _sender = Sender.Create();
		private bool _isOnInteraction = false;
		private short _defaultPriority;
		[Header("Scene Status")]
		[SerializeField, Tooltip("The object that handles the hud of the level gate.")] private LevelGateHud _levelGateObject;
		[SerializeField, Tooltip("The offset that the hud will be.")] private Vector2 _offsetPosition;
		[SerializeField, Tooltip("The name of the level scene.")] private string _levelScene;
		[SerializeField, Tooltip("The name of the boss scene.")] private string _bossScene;
		[SerializeField, Tooltip("Where the this camera have to be in the hierarchy.")] private short _overlayPriority;
		private void Awake()
		{
			this._showCamera = this.GetComponentInChildren<CinemachineCamera>();
			this._levelGate = Instantiate(this._levelGateObject, this.transform);
			this._levelGate.transform.localPosition = this._offsetPosition;
			this._inputController = new InputController();
			this._inputController.Commands.HideHud.canceled += this.HideHud;
			this._inputController.Commands.HideHud.Enable();
			SaveController.Load(out SaveFile saveFile);
			this._levelGate.Level.clicked += this.EnterLevel;
			if (saveFile.levelsCompleted[ushort.Parse($"{this._levelScene[^1]}") - 1])
				this._levelGate.Boss.clicked += this.EnterBoss;
			if (saveFile.deafetedBosses[ushort.Parse($"{this._levelScene[^1]}") - 1])
				this._levelGate.Scenes.clicked += this.ShowScenes;
			this._defaultPriority = (short)this._showCamera.Priority.Value;
		}
		private void OnDestroy()
		{
			this._inputController.Commands.HideHud.canceled -= this.HideHud;
			this._inputController.Commands.HideHud.Disable();
			this._inputController.Dispose();
			SaveController.Load(out SaveFile saveFile);
			this._levelGate.Level.clicked -= this.EnterLevel;
			if (saveFile.levelsCompleted[ushort.Parse($"{this._levelScene[^1]}") - 1])
				this._levelGate.Boss.clicked -= this.EnterBoss;
			if (saveFile.deafetedBosses[ushort.Parse($"{this._levelScene[^1]}") - 1])
				this._levelGate.Scenes.clicked -= this.ShowScenes;
		}
		private Action EnterLevel => () => this.GetComponent<Transitioner>().Transicion(this._levelScene);
		private Action EnterBoss => () => this.GetComponent<Transitioner>().Transicion(this._bossScene);
		private Action ShowScenes => () =>
		{
			this._sender.SetStateForm(StateForm.Action);
			this._sender.SetAdditionalData(this.gameObject);
			this._sender.Send(PathConnection.Story);
		};
		private Action<InputAction.CallbackContext> HideHud => _ =>
		{
			if (this._isOnInteraction)
			{
				this._isOnInteraction = false;
				this._showCamera.Priority.Value = this._defaultPriority;
				this._sender.SetStateForm(StateForm.State);
				this._sender.SetToggle(true);
				this._sender.Send(PathConnection.Hud);
				StateController.SetState(true);
			}
		};
		public void Interaction()
		{
			this._isOnInteraction = true;
			this._showCamera.Priority.Value = this._overlayPriority;
			this._sender.SetStateForm(StateForm.State);
			this._sender.SetToggle(false);
			this._sender.Send(PathConnection.Hud);
			StateController.SetState(false);
		}
	};
};
