using UnityEngine;
using UnityEngine.UIElements;
using System;
using GuwbaPrimeAdventure.Hud;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(TransitionController))]
	internal sealed class MenuController : MonoBehaviour
	{
		private static MenuController _instance;
		private MenuHud _menuHud;
		[SerializeField] private MenuHud _menuHudObject;
		public ConnectionObject ConnectionObject => ConnectionObject.Controller;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			this._menuHud = Instantiate(this._menuHudObject, this.transform);
			this._menuHud.SaveName[0].value = FilesController.Select(1);
			this._menuHud.SaveName[1].value = FilesController.Select(2);
			this._menuHud.SaveName[2].value = FilesController.Select(3);
			this._menuHud.SaveName[3].value = FilesController.Select(4);
			this._menuHud.Play.clicked += this.Play;
			this._menuHud.Configurations.clicked += this.OpenConfigurations;
			this._menuHud.Quit.clicked += this.Quit;
			this._menuHud.Back.clicked += this.Back;
			this._menuHud.SaveName[0].RegisterCallback(this.ChangeName1);
			this._menuHud.SaveName[1].RegisterCallback(this.ChangeName2);
			this._menuHud.SaveName[2].RegisterCallback(this.ChangeName3);
			this._menuHud.SaveName[3].RegisterCallback(this.ChangeName4);
			this._menuHud.Load[0].clicked += this.SelectSaveFile1;
			this._menuHud.Load[1].clicked += this.SelectSaveFile2;
			this._menuHud.Load[2].clicked += this.SelectSaveFile3;
			this._menuHud.Load[3].clicked += this.SelectSaveFile4;
			this._menuHud.Delete[0].clicked += this.DeleteSaveFile1;
			this._menuHud.Delete[1].clicked += this.DeleteSaveFile2;
			this._menuHud.Delete[2].clicked += this.DeleteSaveFile3;
			this._menuHud.Delete[3].clicked += this.DeleteSaveFile4;
		}
		private void OnDestroy()
		{
			if (!_instance || _instance != this)
				return;
			this._menuHud.Play.clicked -= this.Play;
			this._menuHud.Configurations.clicked += this.OpenConfigurations;
			this._menuHud.Quit.clicked -= this.Quit;
			this._menuHud.Back.clicked -= this.Back;
			this._menuHud.SaveName[0].UnregisterCallback(this.ChangeName1);
			this._menuHud.SaveName[1].UnregisterCallback(this.ChangeName2);
			this._menuHud.SaveName[2].UnregisterCallback(this.ChangeName3);
			this._menuHud.SaveName[3].UnregisterCallback(this.ChangeName4);
			this._menuHud.Load[0].clicked -= this.SelectSaveFile1;
			this._menuHud.Load[1].clicked -= this.SelectSaveFile2;
			this._menuHud.Load[2].clicked -= this.SelectSaveFile3;
			this._menuHud.Load[3].clicked -= this.SelectSaveFile4;
			this._menuHud.Delete[0].clicked -= this.DeleteSaveFile1;
			this._menuHud.Delete[1].clicked -= this.DeleteSaveFile2;
			this._menuHud.Delete[2].clicked -= this.DeleteSaveFile3;
			this._menuHud.Delete[3].clicked -= this.DeleteSaveFile4;
		}
		private Action Play => () =>
		{
			this._menuHud.Buttons.style.display = DisplayStyle.None;
			this._menuHud.Saves.style.display = DisplayStyle.Flex;
		};
		private Action OpenConfigurations => () => Sender.Create()
			.SetConnectionObject(this.ConnectionObject).SetConnectionState(ConnectionState.Enable).SetToggle(true).Send();
		private Action Quit => () => Application.Quit();
		private Action Back => () =>
		{
			this._menuHud.Saves.style.display = DisplayStyle.None;
			this._menuHud.Buttons.style.display = DisplayStyle.Flex;
		};
		private EventCallback<KeyUpEvent> ChangeName1 => (KeyUpEvent eventCallback) =>
		{
			if (eventCallback.keyCode != KeyCode.KeypadEnter)
				return;
			SaveController.RenameData(1, this._menuHud.SaveName[0].text);
		};
		private EventCallback<KeyUpEvent> ChangeName2 => (KeyUpEvent eventCallback) =>
		{
			if (eventCallback.keyCode != KeyCode.KeypadEnter)
				return;
			SaveController.RenameData(2, this._menuHud.SaveName[1].text);
		};
		private EventCallback<KeyUpEvent> ChangeName3 => (KeyUpEvent eventCallback) =>
		{
			if (eventCallback.keyCode != KeyCode.KeypadEnter)
				return;
			SaveController.RenameData(3, this._menuHud.SaveName[2].text);
		};
		private EventCallback<KeyUpEvent> ChangeName4 => (KeyUpEvent eventCallback) =>
		{
			if (eventCallback.keyCode != KeyCode.KeypadEnter)
				return;
			SaveController.RenameData(4, this._menuHud.SaveName[3].text);
		};
		private void SetSaveFile(ushort newSaveFile)
		{
			SaveController.SetActualSaveFile(newSaveFile);
			this.GetComponent<TransitionController>().Transicion();
			if (!SaveController.FileExists())
				SaveController.SaveData();
			if (!SettingsController.FileExists())
				SettingsController.SaveSettings();
		}
		private Action SelectSaveFile1 => () => this.SetSaveFile(1);
		private Action SelectSaveFile2 => () => this.SetSaveFile(2);
		private Action SelectSaveFile3 => () => this.SetSaveFile(3);
		private Action SelectSaveFile4 => () => this.SetSaveFile(4);
		private Action DeleteSaveFile1 => () => this._menuHud.SaveName[0].value = SaveController.DeleteData(1);
		private Action DeleteSaveFile2 => () => this._menuHud.SaveName[1].value = SaveController.DeleteData(2);
		private Action DeleteSaveFile3 => () => this._menuHud.SaveName[2].value = SaveController.DeleteData(3);
		private Action DeleteSaveFile4 => () => this._menuHud.SaveName[3].value = SaveController.DeleteData(4);
	};
};
