using UnityEngine;
using UnityEngine.UIElements;
using System;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Transitioner))]
	internal sealed class DeathScreenController : MonoBehaviour, IConnector
	{
		private static DeathScreenController _instance;
		private DeathScreenHud _deathScreenHud;
		[SerializeField] private DeathScreenHud _deathScreenHudObject;
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
			if (this._deathScreenHud)
			{
				this._deathScreenHud.Continue.clicked -= this.Continue;
				this._deathScreenHud.OutLevel.clicked -= this.OutLevel;
				this._deathScreenHud.GameOver.clicked -= this.GameOver;
			}
			Sender.Exclude(this);
		}
		private Action Continue => () => this.GetComponent<Transitioner>().Transicion(this.gameObject.scene.name);
		private Action OutLevel => () => this.GetComponent<Transitioner>().Transicion();
		private Action GameOver => () =>
		{
			SaveController.RefreshData();
			this.GetComponent<Transitioner>().Transicion();
		};
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.ConnectionState == ConnectionState.Disable)
			{
				SaveController.Load(out SaveFile saveFile);
				this._deathScreenHud = Instantiate(this._deathScreenHudObject, this.transform);
				this._deathScreenHud.Continue.clicked += this.Continue;
				this._deathScreenHud.OutLevel.clicked += this.OutLevel;
				this._deathScreenHud.GameOver.clicked += this.GameOver;
				if (saveFile.lifes < 0f)
				{
					this._deathScreenHud.Text.text = "Fim de Jogo";
					this._deathScreenHud.Continue.style.display = DisplayStyle.None;
					this._deathScreenHud.OutLevel.style.display = DisplayStyle.None;
					this._deathScreenHud.GameOver.style.display = DisplayStyle.Flex;
				}
			}
			else if (data.ConnectionState == ConnectionState.Enable)
				if (this._deathScreenHud)
				{
					this._deathScreenHud.Continue.clicked -= this.Continue;
					this._deathScreenHud.OutLevel.clicked -= this.OutLevel;
					this._deathScreenHud.GameOver.clicked -= this.GameOver;
					Destroy(this._deathScreenHud.gameObject);
				}
		}
	};
};
