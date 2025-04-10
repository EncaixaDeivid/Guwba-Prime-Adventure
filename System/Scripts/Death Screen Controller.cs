using UnityEngine;
using UnityEngine.UIElements;
using System;
using GuwbaPrimeAdventure.Hud;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(TransitionController))]
	public sealed class DeathScreenController : ControllerConnector
	{
		private static DeathScreenController _instance;
		private DeathScreenHud _deathScreenHud;
		[SerializeField] private DeathScreenHud _deathScreenHudObject;
		private void Awake()
		{
			base.Awake<DeathScreenController>();
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
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
		}
		protected override void Event()
		{
			SaveController.Load(out SaveFile saveFile);
			this.Connect<ConfigurationController>();
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
		private Action Continue => () => this.GetComponent<TransitionController>().Transicion(this.gameObject.scene.name);
		private Action OutLevel => () => this.GetComponent<TransitionController>().Transicion();
		private Action GameOver => () =>
		{
			SaveController.RefreshData();
			this.GetComponent<TransitionController>().Transicion();
		};
		public static void Death() => _instance.Event();
	};
};
