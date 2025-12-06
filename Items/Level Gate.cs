using UnityEngine;
using Unity.Cinemachine;
using System;
using System.Collections;
using GwambaPrimeAdventure.Character;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D)), RequireComponent(typeof(Transitioner), typeof(IInteractable))]
	internal sealed class LevelGate : MonoBehaviour, ILoader, IInteractable
	{
		private LevelGateHud _levelGate;
		private CinemachineCamera _gateCamera;
		private readonly Sender _sender = Sender.Create();
		private bool _isOnInteraction = false;
		private short _defaultPriority;
		[Header("Scene Status")]
		[SerializeField, Tooltip("The object that handles the hud of the level gate.")] private LevelGateHud _levelGateObject;
		[SerializeField, Tooltip("The scene of the level.")] private SceneField _levelScene;
		[SerializeField, Tooltip("The scene of the boss.")] private SceneField _bossScene;
		[SerializeField, Tooltip("The offset that the hud will be.")] private Vector2 _offsetPosition;
		[SerializeField, Tooltip("Where the this camera have to be in the hierarchy.")] private short _overlayPriority;
		private void Awake()
		{
			_gateCamera = GetComponentInChildren<CinemachineCamera>();
			_sender.SetFormat(MessageFormat.Event);
			_sender.SetAdditionalData(gameObject);
			_levelGate = Instantiate(_levelGateObject, transform);
		}
		private void OnDestroy()
		{
			SaveController.Load(out SaveFile saveFile);
			_levelGate.Level.clicked -= EnterLevel;
			if (saveFile.LevelsCompleted[ushort.Parse($"{_levelScene.SceneName[^1]}") - 1])
				_levelGate.Boss.clicked -= EnterBoss;
			if (saveFile.DeafetedBosses[ushort.Parse($"{_levelScene.SceneName[^1]}") - 1])
				_levelGate.Scenes.clicked -= ShowScenes;
		}
		public IEnumerator Load()
		{
			_levelGate.transform.localPosition = _offsetPosition;
			SaveController.Load(out SaveFile saveFile);
			_levelGate.Level.clicked += EnterLevel;
			if (saveFile.LevelsCompleted[ushort.Parse($"{_levelScene.SceneName[^1]}") - 1])
				_levelGate.Boss.clicked += EnterBoss;
			if (saveFile.DeafetedBosses[ushort.Parse($"{_levelScene.SceneName[^1]}") - 1])
				_levelGate.Scenes.clicked += ShowScenes;
			_levelGate.Level.SetEnabled(false);
			_levelGate.Boss.SetEnabled(false);
			_levelGate.Scenes.SetEnabled(false);
			_defaultPriority = (short)_gateCamera.Priority.Value;
			yield return null;
		}
		private Action EnterLevel => () => GetComponent<Transitioner>().Transicion(_levelScene);
		private Action EnterBoss => () => GetComponent<Transitioner>().Transicion(_bossScene);
		private Action ShowScenes => () => _sender.Send(MessagePath.Story);
		private void OnTriggerExit2D(Collider2D other)
		{
			if (!_isOnInteraction || !GwambaStateMarker.EqualObject(other.gameObject))
				return;
			_isOnInteraction = false;
			_gateCamera.Priority.Value = _defaultPriority;
			_levelGate.Level.SetEnabled(false);
			_levelGate.Boss.SetEnabled(false);
			_levelGate.Scenes.SetEnabled(false);
		}
		public void Interaction()
		{
			_isOnInteraction = true;
			_gateCamera.Priority.Value = _overlayPriority;
			_levelGate.Level.SetEnabled(true);
			_levelGate.Boss.SetEnabled(true);
			_levelGate.Scenes.SetEnabled(true);
		}
	};
};
