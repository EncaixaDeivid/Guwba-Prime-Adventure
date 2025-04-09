using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System;
using GuwbaPrimeAdventure.Hud;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(TransitionController))]
	public sealed class ConfigurationController : MonoBehaviour
	{
		private static ConfigurationController _instance;
		private ConfigurationHud _configurationHud;
		private ActionsGuwba _actions;
		[SerializeField] private ConfigurationHud _configurationHudObject;
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
			if (this._configurationHudObject)
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
				if (SaveController.FileExists())
					for (ushort i = 0; i < saveFile.levelsCompleted.Length; i++)
						if (this.gameObject.scene.name.Contains($"{i}"))
						{
							this._configurationHud.SaveGame.style.display = DisplayStyle.None;
							break;
						}
				this._configurationHud.Close.clicked += this.CloseConfigurations;
				this._configurationHud.OutLevel.clicked += this.OutLevel;
				this._configurationHud.SaveGame.clicked += this.SaveGame;
				this._configurationHud.Volumes1.GeneralVolume.RegisterValueChangedCallback<float>(this.GeneralVolume);
				this._configurationHud.Volumes1.EffectsVolume.RegisterValueChangedCallback<float>(this.EffectsVolume);
				this._configurationHud.Volumes2.MusicVolume.RegisterValueChangedCallback<float>(this.MusicVolume);
				this._configurationHud.Volumes2.DialogSpeed.RegisterValueChangedCallback<float>(this.DialogSpeed);
				this._configurationHud.Toggles1.FullScreen.RegisterValueChangedCallback<bool>(this.FullScreen);
				this._configurationHud.Toggles1.GeneralVolumeToggle.RegisterValueChangedCallback<bool>(this.GeneralVolumeToggle);
				this._configurationHud.Toggles2.EffectsVolumeToggle.RegisterValueChangedCallback<bool>(this.EffectsVolumeToggle);
				this._configurationHud.Toggles2.MusicVolumeToggle.RegisterValueChangedCallback<bool>(this.MusicVolumeToggle);
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
			this._configurationHud.Volumes1.GeneralVolume.UnregisterValueChangedCallback<float>(this.GeneralVolume);
			this._configurationHud.Volumes1.EffectsVolume.UnregisterValueChangedCallback<float>(this.EffectsVolume);
			this._configurationHud.Volumes2.MusicVolume.UnregisterValueChangedCallback<float>(this.MusicVolume);
			this._configurationHud.Volumes2.DialogSpeed.UnregisterValueChangedCallback<float>(this.DialogSpeed);
			this._configurationHud.Toggles1.FullScreen.UnregisterValueChangedCallback<bool>(this.FullScreen);
			this._configurationHud.Toggles1.GeneralVolumeToggle.UnregisterValueChangedCallback<bool>(this.GeneralVolumeToggle);
			this._configurationHud.Toggles2.EffectsVolumeToggle.UnregisterValueChangedCallback<bool>(this.EffectsVolumeToggle);
			this._configurationHud.Toggles2.MusicVolumeToggle.UnregisterValueChangedCallback<bool>(this.MusicVolumeToggle);
			this._configurationHud.DialogToggle.UnregisterValueChangedCallback<bool>(this.DialogToggle);
			this._configurationHud.Yes.clicked -= this.YesBackLevel;
			this._configurationHud.No.clicked -= this.NoBackLevel;
			Destroy(this._configurationHud.gameObject);
			StateController.SetState(true);
			if (this.gameObject.scene.name == this._menuScene)
				Destroy(this.gameObject);
			SettingsController.SaveSettings();
		};
		private Action OutLevel => () =>
		{
			this._configurationHud.Buttons.style.display = DisplayStyle.None;
			this._configurationHud.Confirmation.style.display = DisplayStyle.Flex;
		};
		private Action SaveGame => () => SaveController.SaveData();
		private EventCallback<ChangeEvent<float>> GeneralVolume => (ChangeEvent<float> value) =>
		{
			SettingsController.Load(out Settings settings);
			settings.generalVolume = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<float>> EffectsVolume => (ChangeEvent<float> value) =>
		{
			SettingsController.Load(out Settings settings);
			settings.effectsVolume = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<float>> MusicVolume => (ChangeEvent<float> value) =>
		{
			SettingsController.Load(out Settings settings);
			settings.musicVolume = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<float>> DialogSpeed => (ChangeEvent<float> value) =>
		{
			SettingsController.Load(out Settings settings);
			settings.dialogSpeed = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> FullScreen => (ChangeEvent<bool> value) =>
		{
			SettingsController.Load(out Settings settings);
			settings.fullScreen = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> GeneralVolumeToggle => (ChangeEvent<bool> value) =>
		{
			SettingsController.Load(out Settings settings);
			settings.generalVolumeToggle = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> EffectsVolumeToggle => (ChangeEvent<bool> value) =>
		{
			SettingsController.Load(out Settings settings);
			settings.effectsVolumeToggle = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> MusicVolumeToggle => (ChangeEvent<bool> value) =>
		{
			SettingsController.Load(out Settings settings);
			settings.musicVolumeToggle = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> DialogToggle => (ChangeEvent<bool> value) =>
		{
			SettingsController.Load(out Settings settings);
			settings.dialogToggle = value.newValue;
			SettingsController.WriteSave(settings);
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
			this._configurationHud.Buttons.style.display = DisplayStyle.Flex;
			this._configurationHud.Confirmation.style.display = DisplayStyle.None;
		};
		public static void DeathScreen()
		{
			Instantiate(_instance._deathScreenController);
			Destroy(_instance.gameObject);
		}
	};
};
