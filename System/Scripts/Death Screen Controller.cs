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
		protected override void Event() { }
		private Action Continue => () => this.GetComponent<TransitionController>().Transicion(this.gameObject.scene.name);
		private Action OutLevel => () => this.GetComponent<TransitionController>().Transicion();
		private Action GameOver => () =>
		{
			SaveController.RefreshData();
			this.GetComponent<TransitionController>().Transicion();
		};
		public static void Death()
		{
			SaveController.Load(out SaveFile saveFile);
			_instance.Connect<ConfigurationController>();
			_instance._deathScreenHud = Instantiate(_instance._deathScreenHudObject, _instance.transform);
			_instance._deathScreenHud.Continue.clicked += _instance.Continue;
			_instance._deathScreenHud.OutLevel.clicked += _instance.OutLevel;
			_instance._deathScreenHud.GameOver.clicked += _instance.GameOver;
			if (saveFile.lifes < 0f)
			{
				_instance._deathScreenHud.Text.text = "Fim de Jogo";
				_instance._deathScreenHud.Continue.style.display = DisplayStyle.None;
				_instance._deathScreenHud.OutLevel.style.display = DisplayStyle.None;
				_instance._deathScreenHud.GameOver.style.display = DisplayStyle.Flex;
			}
		}
	};
};
