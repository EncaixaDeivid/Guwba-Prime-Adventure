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
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			Sender.Include(this);
		}
		private void OnDestroy()
		{
			if (!_instance || _instance != this)
				return;
			if (_deathScreenHud)
			{
				_deathScreenHud.Continue.clicked -= Continue;
				_deathScreenHud.OutLevel.clicked -= OutLevel;
				_deathScreenHud.GameOver.clicked -= GameOver;
			}
			Sender.Exclude(this);
		}
		private Action Continue => () =>
		{
			if (gameObject.scene.name.Contains("Boss"))
				GetComponent<Transitioner>().Transicion(_bossScene);
			else
			{
				StartCoroutine(Curtain());
				IEnumerator Curtain()
				{
					_deathScreenHud.Text.style.display = DisplayStyle.None;
					_deathScreenHud.Continue.style.display = DisplayStyle.None;
					_deathScreenHud.OutLevel.style.display = DisplayStyle.None;
					_deathScreenHud.GameOver.style.display = DisplayStyle.None;
					_deathScreenHud.Curtain.style.display = DisplayStyle.Flex;
					for (float i = 0f; _deathScreenHud.Curtain.style.opacity.value < 1f; i += 0.05f)
					{
						_deathScreenHud.Curtain.style.opacity = i;
						yield return new WaitForEndOfFrame();
					}
					_sender.SetToggle(true);
					_sender.SetStateForm(StateForm.Action);
					_sender.Send(PathConnection.System);
					_sender.Send(PathConnection.Character);
					_sender.SetStateForm(StateForm.State);
					_sender.Send(PathConnection.Item);
					for (float i = 1f; _deathScreenHud.Curtain.style.opacity.value > 0f; i -= 0.05f)
					{
						_deathScreenHud.Curtain.style.opacity = i;
						yield return new WaitForEndOfFrame();
					}
					_sender.Send(PathConnection.Character);
					_sender.SetStateForm(StateForm.None);
					_sender.Send(PathConnection.Enemy);
					ConfigurationController.Instance.SetActive(true);
					if (_deathScreenHud)
					{
						_deathScreenHud.Continue.clicked -= Continue;
						_deathScreenHud.OutLevel.clicked -= OutLevel;
						_deathScreenHud.GameOver.clicked -= GameOver;
						Destroy(_deathScreenHud.gameObject);
					}
				}
			}
		};
		private Action OutLevel => () => GetComponent<Transitioner>().Transicion(_levelSelectorScene);
		private Action GameOver => () =>
		{
			SaveController.RefreshData();
			GetComponent<Transitioner>().Transicion();
		};
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue && !data.ToggleValue.Value)
			{
				SaveController.Load(out SaveFile saveFile);
				_deathScreenHud = Instantiate(_deathScreenHudObject, transform);
				_deathScreenHud.Continue.clicked += Continue;
				_deathScreenHud.OutLevel.clicked += OutLevel;
				_deathScreenHud.GameOver.clicked += GameOver;
				if (saveFile.lifes < 0f)
				{
					_deathScreenHud.Text.text = "Fim de Jogo";
					_deathScreenHud.Continue.style.display = DisplayStyle.None;
					_deathScreenHud.OutLevel.style.display = DisplayStyle.None;
					_deathScreenHud.GameOver.style.display = DisplayStyle.Flex;
				}
			}
		}
	};
};
