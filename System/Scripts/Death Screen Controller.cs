using UnityEngine;
using UnityEngine.UIElements;
using System;
using GuwbaPrimeAdventure.Hud;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(TransitionController))]
	public sealed class DeathScreenController : MonoBehaviour
	{
		private static DeathScreenController _instance;
		private DeathScreenHud _deathScreenHud;
		[SerializeField] private DeathScreenHud _deathScreenHudObject;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			this._deathScreenHud = Instantiate(this._deathScreenHudObject, this.transform);
			if (DataFile.Lifes < 0f)
			{
				this._deathScreenHud.Text.text = "Fim de Jogo";
				this._deathScreenHud.Continue.style.display = DisplayStyle.None;
				this._deathScreenHud.OutLevel.style.display = DisplayStyle.None;
				this._deathScreenHud.GameOver.style.display = DisplayStyle.Flex;
			}
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			this._deathScreenHud.Continue.clicked += this.Continue;
			this._deathScreenHud.OutLevel.clicked += this.OutLevel;
			this._deathScreenHud.GameOver.clicked += this.GameOver;
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			this._deathScreenHud.Continue.clicked -= this.Continue;
			this._deathScreenHud.OutLevel.clicked -= this.OutLevel;
			this._deathScreenHud.GameOver.clicked -= this.GameOver;
		}
		private Action Continue => () => this.GetComponent<TransitionController>().Transicion(this.gameObject.scene.name);
		private Action OutLevel => () => this.GetComponent<TransitionController>().Transicion();
		private Action GameOver => () =>
		{
			DataFile.RefreshData();
			this.GetComponent<TransitionController>().Transicion();
		};
	};
};
