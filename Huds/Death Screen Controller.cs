using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using GwambaPrimeAdventure.Data;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Hud
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
		[SerializeField, Tooltip("The scene of the menu.")] private SceneField _menuScene;
		public PathConnection PathConnection => PathConnection.Hud;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			_deathScreenHud = Instantiate(_deathScreenHudObject, transform);
			SceneManager.sceneLoaded += SceneLoaded;
			Sender.Include(this);
		}
		private void OnDestroy()
		{
			if (!_instance || _instance != this)
				return;
			_deathScreenHud.Continue.clicked -= Continue;
			_deathScreenHud.OutLevel.clicked -= OutLevel;
			_deathScreenHud.GameOver.clicked -= GameOver;
			SceneManager.sceneLoaded -= SceneLoaded;
			Sender.Exclude(this);
		}
		private IEnumerator Start()
		{
			if (!_instance || _instance != this)
				yield break;
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			_deathScreenHud.Continue.clicked += Continue;
			_deathScreenHud.OutLevel.clicked += OutLevel;
			_deathScreenHud.GameOver.clicked += GameOver;
			DontDestroyOnLoad(gameObject);
		}
		private UnityAction<Scene, LoadSceneMode> SceneLoaded => (scene, loadMode) =>
		{
			if (scene.name == _levelSelectorScene || scene.name == _menuScene)
				Destroy(gameObject);
			else
			{
				_deathScreenHud.Text.text = "You have died";
				_deathScreenHud.Text.style.display = DisplayStyle.Flex;
				_deathScreenHud.Continue.style.display = DisplayStyle.Flex;
				_deathScreenHud.OutLevel.style.display = DisplayStyle.Flex;
				_deathScreenHud.GameOver.style.display = DisplayStyle.None;
				_deathScreenHud.Curtain.style.display = DisplayStyle.None;
				_deathScreenHud.RootElement.style.display = DisplayStyle.None;
			}
		};
		private Action Continue => () =>
		{
			if (SceneManager.GetActiveScene().name.ContainsInvariantCultureIgnoreCase("Boss"))
				GetComponent<Transitioner>().Transicion(sceneName: SceneManager.GetActiveScene().name);
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
					for (float i = 0f; _deathScreenHud.Curtain.style.opacity.value < 1f; i += 5e-2f)
						yield return _deathScreenHud.Curtain.style.opacity = i;
					_sender.SetToggle(true);
					_sender.SetStateForm(StateForm.Event);
					_sender.Send(PathConnection.System);
					_sender.Send(PathConnection.Character);
					_sender.SetStateForm(StateForm.State);
					_sender.Send(PathConnection.Item);
					for (float i = 1f; _deathScreenHud.Curtain.style.opacity.value > 0f; i -= 5e-2f)
						yield return _deathScreenHud.Curtain.style.opacity = i;
					_sender.Send(PathConnection.Character);
					_sender.SetStateForm(StateForm.None);
					_sender.Send(PathConnection.Enemy);
					ConfigurationController.Instance.SetActive(true);
					_deathScreenHud.RootElement.style.display = DisplayStyle.None;
					_deathScreenHud.Text.text = "You have died";
					_deathScreenHud.Text.style.display = DisplayStyle.Flex;
					_deathScreenHud.Continue.style.display = DisplayStyle.Flex;
					_deathScreenHud.OutLevel.style.display = DisplayStyle.Flex;
					_deathScreenHud.GameOver.style.display = DisplayStyle.None;
					_deathScreenHud.Curtain.style.display = DisplayStyle.None;
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
			if (data.StateForm == StateForm.Event && data.ToggleValue.HasValue && !data.ToggleValue.Value)
			{
				SaveController.Load(out SaveFile saveFile);
				if (saveFile.Lifes < 0f)
				{
					_deathScreenHud.Text.text = "Game Over";
					_deathScreenHud.Continue.style.display = DisplayStyle.None;
					_deathScreenHud.OutLevel.style.display = DisplayStyle.None;
					_deathScreenHud.GameOver.style.display = DisplayStyle.Flex;
				}
				_deathScreenHud.RootElement.style.display = DisplayStyle.Flex;
			}
		}
	};
};
