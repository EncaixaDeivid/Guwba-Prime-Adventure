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
				Destroy(gameObject, 1e-3f);
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
			_inputController = new InputController();
			_inputController.Commands.HideHud.canceled += HideHudAction;
			_inputController.Commands.HideHud.Enable();
		}
		private void OnDisable()
		{
			if (!Instance || Instance != this)
				return;
			_inputController.Commands.HideHud.canceled -= HideHudAction;
			_inputController.Commands.HideHud.Disable();
			_inputController.Dispose();
		}
		private IEnumerator Start() => new WaitWhile(() => !(_isActive = !SceneInitiator.IsInTrancision()));
		private Action<InputAction.CallbackContext> HideHudAction => _ => OpenCloseConfigurations();
		private Action CloseConfigurations => () =>
		{
			_configurationHud.Close.clicked -= CloseConfigurations;
			_configurationHud.OutLevel.clicked -= OutLevel;
			_configurationHud.SaveGame.clicked -= SaveGame;
			_configurationHud.ScreenResolution.UnregisterValueChangedCallback<string>(ScreenResolution);
			_configurationHud.FullScreenModes.UnregisterValueChangedCallback<string>(FullScreenModes);
			_configurationHud.DialogToggle.UnregisterValueChangedCallback<bool>(DialogToggle);
			_configurationHud.GeneralVolumeToggle.UnregisterValueChangedCallback<bool>(GeneralVolumeToggle);
			_configurationHud.EffectsVolumeToggle.UnregisterValueChangedCallback<bool>(EffectsVolumeToggle);
			_configurationHud.MusicVolumeToggle.UnregisterValueChangedCallback<bool>(MusicVolumeToggle);
			_configurationHud.DialogSpeed.UnregisterValueChangedCallback<float>(DialogSpeed);
			_configurationHud.ScreenBrightness.UnregisterValueChangedCallback<float>(ScreenBrightness);
			_configurationHud.FrameRate.UnregisterValueChangedCallback<int>(FrameRate);
			_configurationHud.GeneralVolume.UnregisterValueChangedCallback<int>(GeneralVolume);
			_configurationHud.EffectsVolume.UnregisterValueChangedCallback<int>(EffectsVolume);
			_configurationHud.MusicVolume.UnregisterValueChangedCallback<int>(MusicVolume);
			_configurationHud.Yes.clicked -= YesBackLevel;
			_configurationHud.No.clicked -= NoBackLevel;
			Destroy(_configurationHud.gameObject);
			StateController.SetState(true);
		};
		private Action OutLevel => () =>
		{
			_configurationHud.Settings.style.display = DisplayStyle.None;
			_configurationHud.Confirmation.style.display = DisplayStyle.Flex;
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
			_configurationHud.FrameRateText.text = frameRate.newValue.ToString();
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
			CloseConfigurations.Invoke();
			_isActive = false;
			if (gameObject.scene.name != _levelSelectorScene)
				GetComponent<Transitioner>().Transicion(_levelSelectorScene);
			else
				GetComponent<Transitioner>().Transicion(_menuScene);
		};
		private Action NoBackLevel => () =>
		{
			_configurationHud.Settings.style.display = DisplayStyle.Flex;
			_configurationHud.Confirmation.style.display = DisplayStyle.None;
		};
		internal void OpenCloseConfigurations()
		{
			if (!_isActive)
				return;
			if (_configurationHud)
				CloseConfigurations.Invoke();
			else
			{
				SaveController.Load(out SaveFile saveFile);
				StateController.SetState(false);
				_configurationHud = Instantiate(_configurationHudObject, transform);
				if (gameObject.scene.name == _menuScene)
				{
					_configurationHud.OutLevel.style.display = DisplayStyle.None;
					_configurationHud.SaveGame.style.display = DisplayStyle.None;
				}
				for (ushort i = 1; i <= 12f; i++)
					if (gameObject.scene.name.Contains($"{i}"))
					{
						_configurationHud.SaveGame.style.display = DisplayStyle.None;
						break;
					}
				_configurationHud.Close.clicked += CloseConfigurations;
				_configurationHud.OutLevel.clicked += OutLevel;
				_configurationHud.SaveGame.clicked += SaveGame;
				_configurationHud.ScreenResolution.RegisterValueChangedCallback<string>(ScreenResolution);
				_configurationHud.FullScreenModes.RegisterValueChangedCallback<string>(FullScreenModes);
				_configurationHud.DialogToggle.RegisterValueChangedCallback<bool>(DialogToggle);
				_configurationHud.GeneralVolumeToggle.RegisterValueChangedCallback<bool>(GeneralVolumeToggle);
				_configurationHud.EffectsVolumeToggle.RegisterValueChangedCallback<bool>(EffectsVolumeToggle);
				_configurationHud.MusicVolumeToggle.RegisterValueChangedCallback<bool>(MusicVolumeToggle);
				_configurationHud.ScreenBrightness.RegisterValueChangedCallback<float>(ScreenBrightness);
				_configurationHud.DialogSpeed.RegisterValueChangedCallback<float>(DialogSpeed);
				_configurationHud.FrameRate.RegisterValueChangedCallback<int>(FrameRate);
				_configurationHud.GeneralVolume.RegisterValueChangedCallback<int>(GeneralVolume);
				_configurationHud.EffectsVolume.RegisterValueChangedCallback<int>(EffectsVolume);
				_configurationHud.MusicVolume.RegisterValueChangedCallback<int>(MusicVolume);
				_configurationHud.Yes.clicked += YesBackLevel;
				_configurationHud.No.clicked += NoBackLevel;
			}
		}
		internal void SetActive(bool isActive) => _isActive = isActive;
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				_isActive = data.ToggleValue.Value;
		}
	};
};
