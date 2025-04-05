using UnityEngine;
using UnityEngine.UIElements;
using Unity.Cinemachine;
using System;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	[RequireComponent(typeof(TransitionController))]
	internal sealed class LevelGate : StateController
	{
		private LevelGateHud _levelGateInstance;
		private CinemachineCamera _showCamera;
		private short _defaultPriority;
		[SerializeField] private LevelGateHud _levelGate;
		[SerializeField] private string _levelScene, _bossScene;
		[SerializeField] private short _overlayPriority;
		[SerializeField] private bool _dontUseBoss;
		private new void Awake()
		{
			base.Awake();
			this._showCamera = this.GetComponentInChildren<CinemachineCamera>();
			this._defaultPriority = (short)this._showCamera.Priority.Value;
		}
		private void OnEnable()
		{
			if (this._levelGateInstance)
				this._levelGateInstance.BaseElement.style.display = DisplayStyle.Flex;
		}
		private void OnDisable()
		{
			if (this._levelGateInstance)
				this._levelGateInstance.BaseElement.style.display = DisplayStyle.None;
		}
		private Action EnterLevel => () => this.GetComponent<TransitionController>().Transicion(this._levelScene);
		private Action EnterBoss => () => this.GetComponent<TransitionController>().Transicion(this._bossScene);
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!GuwbaTransformer<CommandGuwba>.EqualObject(other.gameObject))
				return;
			this._levelGateInstance = Instantiate(this._levelGate, this.transform);
			this._levelGateInstance.Level.clicked += this.EnterLevel;
			if (!this._dontUseBoss && SaveController.LevelsCompleted[ushort.Parse($"{this._levelScene[^1]}") - 1])
				this._levelGateInstance.Boss.clicked += this.EnterBoss;
			this._levelGateInstance.Life.text = $"X {SaveController.Lifes}";
			this._levelGateInstance.Coin.text = $"X {SaveController.Coins}";
			this._showCamera.Priority.Value = this._overlayPriority;
		}
		private void OnTriggerExit2D(Collider2D other)
		{
			if (!GuwbaTransformer<CommandGuwba>.EqualObject(other.gameObject))
				return;
			this._levelGateInstance.Level.clicked -= this.EnterLevel;
			if (!this._dontUseBoss && SaveController.LevelsCompleted[ushort.Parse($"{this._levelScene[^1]}") - 1])
				this._levelGateInstance.Boss.clicked -= this.EnterBoss;
			this._showCamera.Priority.Value = this._defaultPriority;
			Destroy(this._levelGateInstance.gameObject);
		}
	};
};
