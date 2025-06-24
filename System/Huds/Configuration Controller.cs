using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Transitioner))]
	internal sealed class ConfigurationController : MonoBehaviour, IConnector
	{
		private static ConfigurationController _instance;
		private ConfigurationHud _configurationHud;
		private InputController _inputController;
		[Header("Interaction Objects")]
		[SerializeField, Tooltip("The object that handles the hud of the configurations.")] private ConfigurationHud _configurationHudObject;
		[SerializeField, Tooltip("The name of the hubby world scene.")] private string _levelSelectorScene;
		[SerializeField, Tooltip("The name of the menu scene.")] private string _menuScene;
		public PathConnection PathConnection => PathConnection.Controller;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			Sender.Include(this);
		}
		private void OnDestroy()
		{
			if (!_instance || _instance != this)
				return;
			Sender.Exclude(this);
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			this._inputController = new InputController();
			this._inputController.commands.hideHud.canceled += this.HideHudAction;
			this._inputController.commands.hideHud.Enable();
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			this._inputController.commands.hideHud.canceled -= this.HideHudAction;
			this._inputController.commands.hideHud.Disable();
			this._inputController.Dispose();
		}
		private Action<InputAction.CallbackContext> HideHudAction => hideHudAction => this.OpenCloseConfigurations();
		private void OpenCloseConfigurations()
		{
			if (this._configurationHud)
				this.CloseConfigurations.Invoke();
			else
			{
				SaveController.Load(out SaveFile saveFile);
				StateController.SetState(false);
				this._configurationHud = Instantiate(this._configurationHudObject, this.transform);
				if (this.gameObject.scene.name == this._menuScene)
				{
					this._configurationHud.OutLevel.style.display = DisplayStyle.None;
					this._configurationHud.SaveGame.style.display = DisplayStyle.None;
				}
				for (ushort i = 1; i <= 12f; i++)
					if (this.gameObject.scene.name.Contains($"{i}"))
					{
						this._configurationHud.SaveGame.style.display = DisplayStyle.None;
						break;
					}
				this._configurationHud.Close.clicked += this.CloseConfigurations;
				this._configurationHud.OutLevel.clicked += this.OutLevel;
				this._configurationHud.SaveGame.clicked += this.SaveGame;
				this._configurationHud.GeneralVolume.RegisterValueChangedCallback<float>(this.GeneralVolume);
				this._configurationHud.EffectsVolume.RegisterValueChangedCallback<float>(this.EffectsVolume);
				this._configurationHud.MusicVolume.RegisterValueChangedCallback<float>(this.MusicVolume);
				this._configurationHud.DialogSpeed.RegisterValueChangedCallback<float>(this.DialogSpeed);
				this._configurationHud.FullScreen.RegisterValueChangedCallback<bool>(this.FullScreen);
				this._configurationHud.GeneralVolumeToggle.RegisterValueChangedCallback<bool>(this.GeneralVolumeToggle);
				this._configurationHud.EffectsVolumeToggle.RegisterValueChangedCallback<bool>(this.EffectsVolumeToggle);
				this._configurationHud.MusicVolumeToggle.RegisterValueChangedCallback<bool>(this.MusicVolumeToggle);
				this._configurationHud.DialogToggle.RegisterValueChangedCallback<bool>(this.DialogToggle);
				this._configurationHud.Yes.clicked += this.YesBackLevel;
				this._configurationHud.No.clicked += this.NoBackLevel;
			}
		}
		private Action CloseConfigurations => () =>
		{
			this._configurationHud.Close.clicked -= this.CloseConfigurations;
			this._configurationHud.OutLevel.clicked -= this.OutLevel;
			this._configurationHud.SaveGame.clicked -= this.SaveGame;
			this._configurationHud.GeneralVolume.UnregisterValueChangedCallback<float>(this.GeneralVolume);
			this._configurationHud.EffectsVolume.UnregisterValueChangedCallback<float>(this.EffectsVolume);
			this._configurationHud.MusicVolume.UnregisterValueChangedCallback<float>(this.MusicVolume);
			this._configurationHud.DialogSpeed.UnregisterValueChangedCallback<float>(this.DialogSpeed);
			this._configurationHud.FullScreen.UnregisterValueChangedCallback<bool>(this.FullScreen);
			this._configurationHud.GeneralVolumeToggle.UnregisterValueChangedCallback<bool>(this.GeneralVolumeToggle);
			this._configurationHud.EffectsVolumeToggle.UnregisterValueChangedCallback<bool>(this.EffectsVolumeToggle);
			this._configurationHud.MusicVolumeToggle.UnregisterValueChangedCallback<bool>(this.MusicVolumeToggle);
			this._configurationHud.DialogToggle.UnregisterValueChangedCallback<bool>(this.DialogToggle);
			this._configurationHud.Yes.clicked -= this.YesBackLevel;
			this._configurationHud.No.clicked -= this.NoBackLevel;
			Destroy(this._configurationHud.gameObject);
			StateController.SetState(true);
			SettingsController.SaveSettings();
		};
		private Action OutLevel => () =>
		{
			this._configurationHud.Settings.style.display = DisplayStyle.None;
			this._configurationHud.Confirmation.style.display = DisplayStyle.Flex;
		};
		private Action SaveGame => () => SaveController.SaveData();
		private EventCallback<ChangeEvent<float>> GeneralVolume => value =>
		{
			SettingsController.Load(out Settings settings);
			settings.generalVolume = (ushort)value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<float>> EffectsVolume => value =>
		{
			SettingsController.Load(out Settings settings);
			settings.effectsVolume = (ushort)value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<float>> MusicVolume => value =>
		{
			SettingsController.Load(out Settings settings);
			settings.musicVolume = (ushort)value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<float>> DialogSpeed => value =>
		{
			SettingsController.Load(out Settings settings);
			settings.dialogSpeed = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> FullScreen => value =>
		{
			SettingsController.Load(out Settings settings);
			settings.fullScreen = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> GeneralVolumeToggle => value =>
		{
			SettingsController.Load(out Settings settings);
			settings.generalVolumeToggle = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> EffectsVolumeToggle => value =>
		{
			SettingsController.Load(out Settings settings);
			settings.effectsVolumeToggle = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> MusicVolumeToggle => value =>
		{
			SettingsController.Load(out Settings settings);
			settings.musicVolumeToggle = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> DialogToggle => value =>
		{
			SettingsController.Load(out Settings settings);
			settings.dialogToggle = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private Action YesBackLevel => () =>
		{
			SettingsController.SaveSettings();
			if (this.gameObject.scene.name != this._levelSelectorScene)
				this.GetComponent<Transitioner>().Transicion();
			else
				this.GetComponent<Transitioner>().Transicion(this._menuScene);
		};
		private Action NoBackLevel => () =>
		{
			this._configurationHud.Settings.style.display = DisplayStyle.Flex;
			this._configurationHud.Confirmation.style.display = DisplayStyle.None;
		};
		public void Receive(DataConnection data, object additionalData)
		{
			bool hasToggle = data.ToggleValue.HasValue && data.ToggleValue.Value;
			if (this.gameObject.scene.name == this._menuScene && data.StateForm == StateForm.Enable && hasToggle)
				this.OpenCloseConfigurations();
			else if (data.StateForm == StateForm.Disable && hasToggle)
				this._inputController.commands.hideHud.Disable();
			if ((data.StateForm == StateForm.Action || data.StateForm == StateForm.Enable) && hasToggle)
				this._inputController.commands.hideHud.Enable();
			else if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue && !data.ToggleValue.Value)
				this._inputController.commands.hideHud.Disable();
		}
	};
};
