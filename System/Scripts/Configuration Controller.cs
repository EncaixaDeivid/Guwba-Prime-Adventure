using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System;
using GuwbaPrimeAdventure.Hud;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(TransitionController))]
	public sealed class ConfigurationController : MonoBehaviour
	{
		private static ConfigurationController _instance;
		private ConfigurationHud _configurationHudInstance;
		private ActionsGuwba _actions;
		[SerializeField] private ConfigurationHud _configurationHud;
		[SerializeField] private DeathScreenController _deathScreenController;
		[SerializeField] private string _levelSelectorScene, _menuScene;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			if (this.gameObject.scene.name == this._menuScene)
				this.OpenCloseConfigurations();
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			this._actions = new ActionsGuwba();
			this._actions.commands.hideHud.canceled += this.HideHudAction;
			this._actions.commands.hideHud.Enable();
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			this._actions.commands.hideHud.canceled -= this.HideHudAction;
			this._actions.commands.hideHud.Disable();
			this._actions.Dispose();
		}
		private Action<InputAction.CallbackContext> HideHudAction => (InputAction.CallbackContext hideHudAction) => this.OpenCloseConfigurations();
		private void OpenCloseConfigurations()
		{
			if (this._configurationHudInstance)
				this.CloseConfigurations();
			else
			{
				StateController.SetState(false);
				this._configurationHudInstance = Instantiate(this._configurationHud, this.transform);
				if (this.gameObject.scene.name == this._menuScene)
				{
					this._configurationHudInstance.OutLevel.style.display = DisplayStyle.None;
					this._configurationHudInstance.SaveGame.style.display = DisplayStyle.None;
				}
				if (DataFile.FileExists())
					for (ushort i = 0; i < DataFile.LevelsCompleted.Length; i++)
						if (this.gameObject.scene.name.Contains($"{i}"))
						{
							this._configurationHudInstance.SaveGame.style.display = DisplayStyle.None;
							break;
						}
				this._configurationHudInstance.OutLevel.clicked += this.OutLevel;
				this._configurationHudInstance.Yes.clicked += this.YesBackLevel;
				this._configurationHudInstance.No.clicked += this.NoBackLevel;
				this._configurationHudInstance.SaveGame.clicked += this.SaveGame;
				this._configurationHudInstance.Close.clicked += this.CloseConfigurations;
			}
		}
		private Action OutLevel => () =>
		{
			this._configurationHudInstance.Buttons.style.display = DisplayStyle.None;
			this._configurationHudInstance.Confirmation.style.display = DisplayStyle.Flex;
		};
		private Action YesBackLevel => () =>
		{
			if (this.gameObject.scene.name != this._levelSelectorScene)
				this.GetComponent<TransitionController>().Transicion(this._levelSelectorScene);
			else
				this.GetComponent<TransitionController>().Transicion(this._menuScene);
		};
		private Action NoBackLevel => () =>
		{
			this._configurationHudInstance.Buttons.style.display = DisplayStyle.Flex;
			this._configurationHudInstance.Confirmation.style.display = DisplayStyle.None;
		};
		private Action SaveGame => () => DataFile.SaveData();
		private Action CloseConfigurations => () =>
		{
			this._configurationHudInstance.OutLevel.clicked -= this.OutLevel;
			this._configurationHudInstance.Yes.clicked -= this.YesBackLevel;
			this._configurationHudInstance.No.clicked -= this.NoBackLevel;
			this._configurationHudInstance.SaveGame.clicked -= this.SaveGame;
			this._configurationHudInstance.Close.clicked -= this.CloseConfigurations;
			Destroy(this._configurationHudInstance.gameObject);
			StateController.SetState(true);
			if (this.gameObject.scene.name == this._menuScene)
				Destroy(this.gameObject);
		};
		public static void DeathScreen()
		{
			Instantiate(_instance._deathScreenController);
			Destroy(_instance.gameObject);
		}
	};
};
