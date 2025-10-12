using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Transitioner))]
	internal sealed class DeathScreenController : MonoBehaviour, IConnector
	{
		private static DeathScreenController _instance;
		private DeathScreenHud _deathScreenHud;
		private readonly Sender _sender = Sender.Create();
		[Header("Interaction Object")]
		[SerializeField, Tooltip("The object that handles the hud of the death screen.")] private DeathScreenHud _deathScreenHudObject;
		[SerializeField, Tooltip("The scene of the level selector.")] private SceneField _levelSelectorScene;
		[SerializeField, Tooltip("The scene of this level, if it's a boss scene.")] private SceneField _bossScene;
		public PathConnection PathConnection => PathConnection.Hud;
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
		private Action Continue => () =>
		{
			if (this.gameObject.scene.name.Contains("Boss"))
				this.GetComponent<Transitioner>().Transicion(this._bossScene);
			else
			{
				this.StartCoroutine(Curtain());
				IEnumerator Curtain()
				{
					this._deathScreenHud.Text.style.display = DisplayStyle.None;
					this._deathScreenHud.Continue.style.display = DisplayStyle.None;
					this._deathScreenHud.OutLevel.style.display = DisplayStyle.None;
					this._deathScreenHud.GameOver.style.display = DisplayStyle.None;
					this._deathScreenHud.Curtain.style.display = DisplayStyle.Flex;
					for (float i = 0f; this._deathScreenHud.Curtain.style.opacity.value < 1f; i += 0.05f)
					{
						this._deathScreenHud.Curtain.style.opacity = i;
						yield return new WaitForEndOfFrame();
					}
					this._sender.SetToggle(true);
					this._sender.SetStateForm(StateForm.Action);
					this._sender.Send(PathConnection.System);
					this._sender.Send(PathConnection.Character);
					this._sender.SetStateForm(StateForm.State);
					this._sender.Send(PathConnection.Item);
					for (float i = 1f; this._deathScreenHud.Curtain.style.opacity.value > 0f; i -= 0.05f)
					{
						this._deathScreenHud.Curtain.style.opacity = i;
						yield return new WaitForEndOfFrame();
					}
					this._sender.Send(PathConnection.Character);
					this._sender.SetStateForm(StateForm.None);
					this._sender.Send(PathConnection.Enemy);
					if (this._deathScreenHud)
					{
						this._deathScreenHud.Continue.clicked -= this.Continue;
						this._deathScreenHud.OutLevel.clicked -= this.OutLevel;
						this._deathScreenHud.GameOver.clicked -= this.GameOver;
						Destroy(this._deathScreenHud.gameObject);
					}
				}
			}
		};
		private Action OutLevel => () => this.GetComponent<Transitioner>().Transicion(this._levelSelectorScene);
		private Action GameOver => () =>
		{
			SaveController.RefreshData();
			this.GetComponent<Transitioner>().Transicion();
		};
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue && !data.ToggleValue.Value)
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
		}
	};
};
