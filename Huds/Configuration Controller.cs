using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System;
using System.Collections;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Transitioner))]
	internal sealed class ConfigurationController : MonoBehaviour, IConnector
	{
		private ConfigurationHud _configurationHud;
		private InputController _inputController;
		private bool _isActive = true;
		[Header("Interaction Objects")]
		[SerializeField, Tooltip("The object that handles the hud of the configurations.")] private ConfigurationHud _configurationHudObject;
		[SerializeField, Tooltip("The scene of the menu.")] private SceneField _menuScene;
		[SerializeField, Tooltip("The scene of the level selector.")] private SceneField _levelSelectorScene;
		internal static ConfigurationController Instance { get; private set; }
		public PathConnection PathConnection => PathConnection.Hud;
		private void Awake()
		{
			if (Instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			Instance = this;
			Sender.Include(this);
		}
		private void OnDestroy()
		{
			if (!Instance || Instance != this)
				return;
			Sender.Exclude(this);
		}
		private void OnEnable()
		{
			if (!Instance || Instance != this)
				return;
			this._inputController = new InputController();
			this._inputController.Commands.HideHud.canceled += this.HideHudAction;
			this._inputController.Commands.HideHud.Enable();
		}
		private void OnDisable()
		{
			if (!Instance || Instance != this)
				return;
			this._inputController.Commands.HideHud.canceled -= this.HideHudAction;
			this._inputController.Commands.HideHud.Disable();
			this._inputController.Dispose();
		}
		private IEnumerator Start() => new WaitWhile(() => !(this._isActive = !SceneInitiator.IsInTrancision()));
		private Action<InputAction.CallbackContext> HideHudAction => _ => this.OpenCloseConfigurations();
		private Action CloseConfigurations => () =>
		{
			this._configurationHud.Close.clicked -= this.CloseConfigurations;
			this._configurationHud.OutLevel.clicked -= this.OutLevel;
			this._configurationHud.SaveGame.clicked -= this.SaveGame;
			this._configurationHud.ScreenResolution.UnregisterValueChangedCallback<string>(this.ScreenResolution);
			this._configurationHud.FullScreenModes.UnregisterValueChangedCallback<string>(this.FullScreenModes);
			this._configurationHud.DialogToggle.UnregisterValueChangedCallback<bool>(this.DialogToggle);
			this._configurationHud.GeneralVolumeToggle.UnregisterValueChangedCallback<bool>(this.GeneralVolumeToggle);
			this._configurationHud.EffectsVolumeToggle.UnregisterValueChangedCallback<bool>(this.EffectsVolumeToggle);
			this._configurationHud.MusicVolumeToggle.UnregisterValueChangedCallback<bool>(this.MusicVolumeToggle);
			this._configurationHud.DialogSpeed.UnregisterValueChangedCallback<float>(this.DialogSpeed);
			this._configurationHud.ScreenBrightness.UnregisterValueChangedCallback<float>(this.ScreenBrightness);
			this._configurationHud.FrameRate.UnregisterValueChangedCallback<int>(this.FrameRate);
			this._configurationHud.GeneralVolume.UnregisterValueChangedCallback<int>(this.GeneralVolume);
			this._configurationHud.EffectsVolume.UnregisterValueChangedCallback<int>(this.EffectsVolume);
			this._configurationHud.MusicVolume.UnregisterValueChangedCallback<int>(this.MusicVolume);
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
		private EventCallback<ChangeEvent<string>> ScreenResolution => resolution =>
		{
			SettingsController.Load(out Settings settings);
			string[] dimensions = resolution.newValue.Split(new char[] { 'x', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			settings.ScreenResolution = new Vector2Int(ushort.Parse(dimensions[0]), ushort.Parse(dimensions[1]));
			Screen.SetResolution(settings.ScreenResolution.x, settings.ScreenResolution.y, settings.FullScreenMode);
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<string>> FullScreenModes => screenMode =>
		{
			SettingsController.Load(out Settings settings);
			Screen.fullScreenMode = settings.FullScreenMode = Enum.Parse<FullScreenMode>(screenMode.newValue);
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> DialogToggle => toggle =>
		{
			SettingsController.Load(out Settings settings);
			settings.DialogToggle = toggle.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> GeneralVolumeToggle => toggle =>
		{
			SettingsController.Load(out Settings settings);
			settings.GeneralVolumeToggle = toggle.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> EffectsVolumeToggle => toggle =>
		{
			SettingsController.Load(out Settings settings);
			settings.EffectsVolumeToggle = toggle.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<bool>> MusicVolumeToggle => toggle =>
		{
			SettingsController.Load(out Settings settings);
			settings.MusicVolumeToggle = toggle.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<float>> DialogSpeed => value =>
		{
			SettingsController.Load(out Settings settings);
			settings.DialogSpeed = value.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<float>> ScreenBrightness => brightness =>
		{
			SettingsController.Load(out Settings settings);
			Screen.brightness = settings.ScreenBrightness = brightness.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<int>> FrameRate => frameRate =>
		{
			SettingsController.Load(out Settings settings);
			Application.targetFrameRate = settings.FrameRate = (ushort)frameRate.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<int>> GeneralVolume => volume =>
		{
			SettingsController.Load(out Settings settings);
			settings.GeneralVolume = (ushort)volume.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<int>> EffectsVolume => volume =>
		{
			SettingsController.Load(out Settings settings);
			settings.EffectsVolume = (ushort)volume.newValue;
			SettingsController.WriteSave(settings);
		};
		private EventCallback<ChangeEvent<int>> MusicVolume => volume =>
		{
			SettingsController.Load(out Settings settings);
			settings.MusicVolume = (ushort)volume.newValue;
			SettingsController.WriteSave(settings);
		};
		private Action YesBackLevel => () =>
		{
			SettingsController.SaveSettings();
			if (this.gameObject.scene.name != this._levelSelectorScene)
				this.GetComponent<Transitioner>().Transicion(this._levelSelectorScene);
			else
				this.GetComponent<Transitioner>().Transicion(this._menuScene);
		};
		private Action NoBackLevel => () =>
		{
			this._configurationHud.Settings.style.display = DisplayStyle.Flex;
			this._configurationHud.Confirmation.style.display = DisplayStyle.None;
		};
		internal void OpenCloseConfigurations()
		{
			if (!this._isActive)
				return;
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
				this._configurationHud.ScreenResolution.RegisterValueChangedCallback<string>(this.ScreenResolution);
				this._configurationHud.FullScreenModes.RegisterValueChangedCallback<string>(this.FullScreenModes);
				this._configurationHud.DialogToggle.RegisterValueChangedCallback<bool>(this.DialogToggle);
				this._configurationHud.GeneralVolumeToggle.RegisterValueChangedCallback<bool>(this.GeneralVolumeToggle);
				this._configurationHud.EffectsVolumeToggle.RegisterValueChangedCallback<bool>(this.EffectsVolumeToggle);
				this._configurationHud.MusicVolumeToggle.RegisterValueChangedCallback<bool>(this.MusicVolumeToggle);
				this._configurationHud.ScreenBrightness.RegisterValueChangedCallback<float>(this.ScreenBrightness);
				this._configurationHud.DialogSpeed.RegisterValueChangedCallback<float>(this.DialogSpeed);
				this._configurationHud.FrameRate.RegisterValueChangedCallback<int>(this.FrameRate);
				this._configurationHud.GeneralVolume.RegisterValueChangedCallback<int>(this.GeneralVolume);
				this._configurationHud.EffectsVolume.RegisterValueChangedCallback<int>(this.EffectsVolume);
				this._configurationHud.MusicVolume.RegisterValueChangedCallback<int>(this.MusicVolume);
				this._configurationHud.Yes.clicked += this.YesBackLevel;
				this._configurationHud.No.clicked += this.NoBackLevel;
			}
		}
		internal void SetActive(bool isActive) => this._isActive = isActive;
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				this._isActive = data.ToggleValue.Value;
		}
	};
};
