using UnityEngine;
using UnityEngine.UIElements;
using System;
using GuwbaPrimeAdventure.Hud;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(TransitionController))]
	internal sealed class MenuController : StateController
	{
		private static MenuController _instance;
		private MenuHud _menuHud;
		private ConfigurationController _configurationController;
		[SerializeField] private MenuHud _menuHudObject;
		[SerializeField] private string _levelSelectorScene, _level0Scene;
		private new void Awake()
		{
			if (_instance)
				Destroy(_instance.gameObject);
			_instance = this;
			base.Awake();
			this._configurationController = this.GetComponentInChildren<ConfigurationController>(true);
			this._menuHud = Instantiate(this._menuHudObject, this.transform);
			this._menuHud.SaveName[0].value = SaveFileData.DataFileName1;
			this._menuHud.SaveName[1].value = SaveFileData.DataFileName2;
			this._menuHud.SaveName[2].value = SaveFileData.DataFileName3;
			this._menuHud.SaveName[3].value = SaveFileData.DataFileName4;
			this._menuHud.Play.clicked += this.Play;
			this._menuHud.Configurations.clicked += this.OpenConfigurations;
			this._menuHud.Quit.clicked += this.Quit;
			this._menuHud.Back.clicked += this.Back;
			this._menuHud.SaveName[0].RegisterValueChangedCallback(this.ChangeName1);
			this._menuHud.SaveName[1].RegisterValueChangedCallback(this.ChangeName2);
			this._menuHud.SaveName[2].RegisterValueChangedCallback(this.ChangeName3);
			this._menuHud.SaveName[3].RegisterValueChangedCallback(this.ChangeName4);
			this._menuHud.Load[0].clicked += this.SelectSaveFile1;
			this._menuHud.Load[1].clicked += this.SelectSaveFile2;
			this._menuHud.Load[2].clicked += this.SelectSaveFile3;
			this._menuHud.Load[3].clicked += this.SelectSaveFile4;
			this._menuHud.Delete[0].clicked += this.DeleteSaveFile1;
			this._menuHud.Delete[1].clicked += this.DeleteSaveFile2;
			this._menuHud.Delete[2].clicked += this.DeleteSaveFile3;
			this._menuHud.Delete[3].clicked += this.DeleteSaveFile4;
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			this._menuHud.Play.clicked -= this.Play;
			this._menuHud.Configurations.clicked += this.OpenConfigurations;
			this._menuHud.Quit.clicked -= this.Quit;
			this._menuHud.Back.clicked -= this.Back;
			this._menuHud.SaveName[0].UnregisterValueChangedCallback(this.ChangeName1);
			this._menuHud.SaveName[1].UnregisterValueChangedCallback(this.ChangeName2);
			this._menuHud.SaveName[2].UnregisterValueChangedCallback(this.ChangeName3);
			this._menuHud.SaveName[3].UnregisterValueChangedCallback(this.ChangeName4);
			this._menuHud.Load[0].clicked -= this.SelectSaveFile1;
			this._menuHud.Load[1].clicked -= this.SelectSaveFile2;
			this._menuHud.Load[2].clicked -= this.SelectSaveFile3;
			this._menuHud.Load[3].clicked -= this.SelectSaveFile4;
			this._menuHud.Delete[0].clicked -= this.DeleteSaveFile1;
			this._menuHud.Delete[1].clicked -= this.DeleteSaveFile2;
			this._menuHud.Delete[2].clicked -= this.DeleteSaveFile3;
			this._menuHud.Delete[3].clicked -= this.DeleteSaveFile4;
		}
		private void OnEnable() => this._menuHud.Buttons.style.display = DisplayStyle.Flex;
		private void OnDisable() => this._menuHud.Buttons.style.display = DisplayStyle.None;
		private Action Play => () =>
		{
			this._menuHud.Buttons.style.display = DisplayStyle.None;
			this._menuHud.Saves.style.display = DisplayStyle.Flex;
		};
		private Action OpenConfigurations => () => Instantiate(this._configurationController).gameObject.SetActive(true);
		private Action Quit => () => Application.Quit();
		private Action Back => () =>
		{
			this._menuHud.Saves.style.display = DisplayStyle.None;
			this._menuHud.Buttons.style.display = DisplayStyle.Flex;
		};
		private EventCallback<ChangeEvent<string>> ChangeName1 =>
			(ChangeEvent<string> eventCallback) => SaveFileData.RenameData(1, eventCallback.newValue);
		private EventCallback<ChangeEvent<string>> ChangeName2 =>
			(ChangeEvent<string> eventCallback) => SaveFileData.RenameData(2, eventCallback.newValue);
		private EventCallback<ChangeEvent<string>> ChangeName3 =>
			(ChangeEvent<string> eventCallback) => SaveFileData.RenameData(3, eventCallback.newValue);
		private EventCallback<ChangeEvent<string>> ChangeName4 =>
			(ChangeEvent<string> eventCallback) => SaveFileData.RenameData(4, eventCallback.newValue);
		private void SetSaveFile(ushort newSaveFile)
		{
			SaveFileData.SetActualSaveFile(newSaveFile);
			if (SaveFileData.FileExists())
			{
				this.GetComponent<TransitionController>().Transicion(this._levelSelectorScene);
				return;
			}
			this.GetComponent<TransitionController>().Transicion(this._level0Scene);
			SaveFileData.SaveData();
		}
		private Action SelectSaveFile1 => () => this.SetSaveFile(1);
		private Action SelectSaveFile2 => () => this.SetSaveFile(2);
		private Action SelectSaveFile3 => () => this.SetSaveFile(3);
		private Action SelectSaveFile4 => () => this.SetSaveFile(4);
		private Action DeleteSaveFile1 => () =>
		{
			SaveFileData.DeleteData(1);
			this._menuHud.SaveName[0].value = SaveFileData.DataFileName1;
		};
		private Action DeleteSaveFile2 => () =>
		{
			SaveFileData.DeleteData(2);
			this._menuHud.SaveName[1].value = SaveFileData.DataFileName2;
		};
		private Action DeleteSaveFile3 => () =>
		{
			SaveFileData.DeleteData(3);
			this._menuHud.SaveName[2].value = SaveFileData.DataFileName3;
		};
		private Action DeleteSaveFile4 => () =>
		{
			SaveFileData.DeleteData(4);
			this._menuHud.SaveName[3].value = SaveFileData.DataFileName4;
		};
	};
};
