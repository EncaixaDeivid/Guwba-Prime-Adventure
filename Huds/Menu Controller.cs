using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using GwambaPrimeAdventure.Connection;
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
				Destroy(gameObject, WorldBuild.MINIMUM_TIME_SPACE_LIMIT);
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
			_menuHud.SaveName[0].RegisterValueChangedCallback(ChangeName1);
			_menuHud.SaveName[1].RegisterValueChangedCallback(ChangeName2);
			_menuHud.SaveName[2].RegisterValueChangedCallback(ChangeName3);
			_menuHud.SaveName[3].RegisterValueChangedCallback(ChangeName4);
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
			_menuHud.SaveName[0].UnregisterValueChangedCallback(ChangeName1);
			_menuHud.SaveName[1].UnregisterValueChangedCallback(ChangeName2);
			_menuHud.SaveName[2].UnregisterValueChangedCallback(ChangeName3);
			_menuHud.SaveName[3].UnregisterValueChangedCallback(ChangeName4);
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
		private void HideHud(InputAction.CallbackContext hideHud) => Back();
		private void Play()
		{
			_menuHud.Buttons.style.display = DisplayStyle.None;
			_menuHud.Saves.style.display = DisplayStyle.Flex;
			_isPlay = true;
			ConfigurationController.Instance.SetActive(false);
		}
		private void OpenConfigurations() => ConfigurationController.Instance.OpenCloseConfigurations();
		private void Quit() => Application.Quit();
		private void Back()
		{
			if (!_isPlay)
				return;
			_menuHud.Saves.style.display = DisplayStyle.None;
			_menuHud.Buttons.style.display = DisplayStyle.Flex;
			_isPlay = false;
			ConfigurationController.Instance.SetActive(true);
		}
		private void ChangeName1(ChangeEvent<string> write) => SaveController.RenameData(1, write.newValue);
		private void ChangeName2(ChangeEvent<string> write)=> SaveController.RenameData(2, write.newValue);
		private void ChangeName3(ChangeEvent<string> write) => SaveController.RenameData(3, write.newValue);
		private void ChangeName4(ChangeEvent<string> write) => SaveController.RenameData(4, write.newValue);
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
		private void SelectSaveFile1() => SetSaveFile(1);
		private void SelectSaveFile2() => SetSaveFile(2);
		private void SelectSaveFile3() => SetSaveFile(3);
		private void SelectSaveFile4() => SetSaveFile(4);
		private void DeleteSaveFile1() => _menuHud.SaveName[0].value = SaveController.DeleteData(1);
		private void DeleteSaveFile2() => _menuHud.SaveName[1].value = SaveController.DeleteData(2);
		private void DeleteSaveFile3() => _menuHud.SaveName[2].value = SaveController.DeleteData(3);
		private void DeleteSaveFile4() => _menuHud.SaveName[3].value = SaveController.DeleteData(4);
	};
};
