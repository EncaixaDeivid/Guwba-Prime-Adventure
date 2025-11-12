using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System;
using GwambaPrimeAdventure.Data;
namespace GwambaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Transitioner))]
	internal sealed class MenuController : MonoBehaviour
	{
		private static MenuController _instance;
		private MenuHud _menuHud;
		private InputController _inputController;
		private bool _isPlay = false;
		[Header("Interaction Object")]
		[SerializeField, Tooltip("The object that handles the hud of the menu.")] private MenuHud _menuHudObject;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			_menuHud = Instantiate(_menuHudObject, transform);
			_menuHud.SaveName[0].value = FilesController.Select(1);
			_menuHud.SaveName[1].value = FilesController.Select(2);
			_menuHud.SaveName[2].value = FilesController.Select(3);
			_menuHud.SaveName[3].value = FilesController.Select(4);
			_menuHud.Play.clicked += Play;
			_menuHud.Configurations.clicked += OpenConfigurations;
			_menuHud.Quit.clicked += Quit;
			_menuHud.Back.clicked += Back;
			_menuHud.SaveName[0].RegisterCallback(ChangeName1);
			_menuHud.SaveName[1].RegisterCallback(ChangeName2);
			_menuHud.SaveName[2].RegisterCallback(ChangeName3);
			_menuHud.SaveName[3].RegisterCallback(ChangeName4);
			_menuHud.Load[0].clicked += SelectSaveFile1;
			_menuHud.Load[1].clicked += SelectSaveFile2;
			_menuHud.Load[2].clicked += SelectSaveFile3;
			_menuHud.Load[3].clicked += SelectSaveFile4;
			_menuHud.Delete[0].clicked += DeleteSaveFile1;
			_menuHud.Delete[1].clicked += DeleteSaveFile2;
			_menuHud.Delete[2].clicked += DeleteSaveFile3;
			_menuHud.Delete[3].clicked += DeleteSaveFile4;
			_inputController = new InputController();
			_inputController.Commands.HideHud.canceled += HideHud;
			_inputController.Commands.HideHud.Enable();
		}
		private void OnDestroy()
		{
			if (!_instance || _instance != this)
				return;
			_menuHud.Play.clicked -= Play;
			_menuHud.Configurations.clicked += OpenConfigurations;
			_menuHud.Quit.clicked -= Quit;
			_menuHud.Back.clicked -= Back;
			_menuHud.SaveName[0].UnregisterCallback(ChangeName1);
			_menuHud.SaveName[1].UnregisterCallback(ChangeName2);
			_menuHud.SaveName[2].UnregisterCallback(ChangeName3);
			_menuHud.SaveName[3].UnregisterCallback(ChangeName4);
			_menuHud.Load[0].clicked -= SelectSaveFile1;
			_menuHud.Load[1].clicked -= SelectSaveFile2;
			_menuHud.Load[2].clicked -= SelectSaveFile3;
			_menuHud.Load[3].clicked -= SelectSaveFile4;
			_menuHud.Delete[0].clicked -= DeleteSaveFile1;
			_menuHud.Delete[1].clicked -= DeleteSaveFile2;
			_menuHud.Delete[2].clicked -= DeleteSaveFile3;
			_menuHud.Delete[3].clicked -= DeleteSaveFile4;
			_inputController.Commands.HideHud.canceled -= HideHud;
			_inputController.Commands.HideHud.Disable();
			_inputController.Dispose();
		}
		private Action<InputAction.CallbackContext> HideHud => _ => Back.Invoke();
		private Action Play => () =>
		{
			_menuHud.Buttons.style.display = DisplayStyle.None;
			_menuHud.Saves.style.display = DisplayStyle.Flex;
			_isPlay = true;
			ConfigurationController.Instance.SetActive(false);
		};
		private Action OpenConfigurations => () => ConfigurationController.Instance.OpenCloseConfigurations();
		private Action Quit => () => Application.Quit();
		private Action Back => () =>
		{
			if (!_isPlay)
				return;
			_menuHud.Saves.style.display = DisplayStyle.None;
			_menuHud.Buttons.style.display = DisplayStyle.Flex;
			_isPlay = false;
			ConfigurationController.Instance.SetActive(true);
		};
		private EventCallback<KeyUpEvent> ChangeName1 => eventCallback =>
		{
			if (eventCallback.keyCode != KeyCode.KeypadEnter)
				return;
			SaveController.RenameData(1, _menuHud.SaveName[0].text);
		};
		private EventCallback<KeyUpEvent> ChangeName2 => eventCallback =>
		{
			if (eventCallback.keyCode != KeyCode.KeypadEnter)
				return;
			SaveController.RenameData(2, _menuHud.SaveName[1].text);
		};
		private EventCallback<KeyUpEvent> ChangeName3 => eventCallback =>
		{
			if (eventCallback.keyCode != KeyCode.KeypadEnter)
				return;
			SaveController.RenameData(3, _menuHud.SaveName[2].text);
		};
		private EventCallback<KeyUpEvent> ChangeName4 => eventCallback =>
		{
			if (eventCallback.keyCode != KeyCode.KeypadEnter)
				return;
			SaveController.RenameData(4, _menuHud.SaveName[3].text);
		};
		private void SetSaveFile(ushort newSaveFile)
		{
			SaveController.SetActualSaveFile(newSaveFile);
			GetComponent<Transitioner>().Transicion();
			if (!SaveController.FileExists())
				SaveController.SaveData();
			if (!SettingsController.FileExists())
			{
				SettingsController.Load(out Settings settings);
				SettingsController.WriteSave(settings);
			}
		}
		private Action SelectSaveFile1 => () => SetSaveFile(1);
		private Action SelectSaveFile2 => () => SetSaveFile(2);
		private Action SelectSaveFile3 => () => SetSaveFile(3);
		private Action SelectSaveFile4 => () => SetSaveFile(4);
		private Action DeleteSaveFile1 => () => _menuHud.SaveName[0].value = SaveController.DeleteData(1);
		private Action DeleteSaveFile2 => () => _menuHud.SaveName[1].value = SaveController.DeleteData(2);
		private Action DeleteSaveFile3 => () => _menuHud.SaveName[2].value = SaveController.DeleteData(3);
		private Action DeleteSaveFile4 => () => _menuHud.SaveName[3].value = SaveController.DeleteData(4);
	};
};
