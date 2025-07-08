using UnityEngine;
using UnityEngine.UIElements;
using Unity.Cinemachine;
using System;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	[RequireComponent(typeof(Transitioner), typeof(IInteractable))]
	internal sealed class LevelGate : StateController
	{
		private LevelGateHud _levelGateInstance;
		private CinemachineCamera _showCamera;
		private short _defaultPriority;
		[Header("Scene Objects")]
		[SerializeField, Tooltip("The object that handles the hud of the level gate.")] private LevelGateHud _levelGate;
		[SerializeField, Tooltip("The name of the level scene.")] private string _levelScene;
		[SerializeField, Tooltip("The name of the boss scene.")] private string _bossScene;
		[SerializeField, Tooltip("Where the this camera have to be in the hierarchy.")] private short _overlayPriority;
		private new void Awake()
		{
			base.Awake();
			this._showCamera = this.GetComponentInChildren<CinemachineCamera>();
			this._defaultPriority = (short)this._showCamera.Priority.Value;
		}
		private void OnEnable()
		{
			if (this._levelGateInstance)
				this._levelGateInstance.RootElement.style.display = DisplayStyle.Flex;
		}
		private void OnDisable()
		{
			if (this._levelGateInstance)
				this._levelGateInstance.RootElement.style.display = DisplayStyle.None;
		}
		private Action EnterLevel => () => this.GetComponent<Transitioner>().Transicion(this._levelScene);
		private Action EnterBoss => () => this.GetComponent<Transitioner>().Transicion(this._bossScene);
		private Action ShowScenes => () => this.GetComponent<IInteractable>().Interaction();
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!GuwbaAstral<CommandGuwba>.EqualObject(other.gameObject))
				return;
			SaveController.Load(out SaveFile saveFile);
			this._levelGateInstance = Instantiate(this._levelGate, this.transform);
			this._levelGateInstance.Level.clicked += this.EnterLevel;
			if (saveFile.levelsCompleted[ushort.Parse($"{this._levelScene[^1]}") - 1])
				this._levelGateInstance.Boss.clicked += this.EnterBoss;
			if (saveFile.deafetedBosses[ushort.Parse($"{this._levelScene[^1]}") - 1])
				this._levelGateInstance.Scenes.clicked += this.ShowScenes;
			this._levelGateInstance.Life.text = $"X {saveFile.lifes}";
			this._levelGateInstance.Coin.text = $"X {saveFile.coins}";
			this._showCamera.Priority.Value = this._overlayPriority;
		}
		private void OnTriggerExit2D(Collider2D other)
		{
			if (!GuwbaAstral<CommandGuwba>.EqualObject(other.gameObject))
				return;
			SaveController.Load(out SaveFile saveFile);
			this._levelGateInstance.Level.clicked -= this.EnterLevel;
			if (saveFile.levelsCompleted[ushort.Parse($"{this._levelScene[^1]}") - 1])
				this._levelGateInstance.Boss.clicked -= this.EnterBoss;
			if (saveFile.deafetedBosses[ushort.Parse($"{this._levelScene[^1]}") - 1])
				this._levelGateInstance.Scenes.clicked -= this.ShowScenes;
			this._showCamera.Priority.Value = this._defaultPriority;
			Destroy(this._levelGateInstance.gameObject);
		}
	};
};
