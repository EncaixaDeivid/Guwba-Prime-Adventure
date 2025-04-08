using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Collections;
using GuwbaPrimeAdventure.Effects;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument), typeof(BoxCollider2D))]
	public sealed class VisualGuwba : GuwbaTransformer<VisualGuwba>, IDamageable
	{
		private static VisualGuwba _instance;
		private SpriteRenderer _spriteRenderer;
		private GroupBox _baseElement;
		private Label _lifeText, _coinsText;
		private SaveFile _saveFile;
		private bool _invencibility = false;
		[SerializeField] private string _baseElementObject, _lifeTextObject, _coinsTextObject, _levelSelectorScene;
		[SerializeField] private short _vitality;
		[SerializeField] private ushort _invencibilityTime;
		[SerializeField] private float _invencibilityValue, _timeStep, _hitStopTime, _hitStopSlow;
		public ushort Health => (ushort)this._vitality;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			SaveController.Load(out this._saveFile);
			this._spriteRenderer = this.GetComponentInParent<SpriteRenderer>();
			UIDocument hudDocument = this.GetComponent<UIDocument>();
			this._baseElement = hudDocument.rootVisualElement.Q<GroupBox>(this._baseElementObject);
			this._lifeText = hudDocument.rootVisualElement.Q<Label>(this._lifeTextObject);
			this._coinsText = hudDocument.rootVisualElement.Q<Label>(this._coinsTextObject);
			this._lifeText.text = $"X {this._saveFile.lifes}";
			this._coinsText.text = $"X {this._saveFile.coins}";
			_actualState += this.ManualInvencibility;
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (_instance != this)
				return;
			_actualState -= this.ManualInvencibility;
			this.StopAllCoroutines();
			this._spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			if (this.gameObject.scene.name == this._levelSelectorScene)
				this._baseElement.style.display = DisplayStyle.None;
			else
				this._baseElement.style.display = DisplayStyle.Flex;
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			this._baseElement.style.display = DisplayStyle.None;
		}
		private IEnumerator Invencibility()
		{
			this.StartCoroutine(VisualEffect());
			IEnumerator VisualEffect()
			{
				while (this._invencibility)
				{
					this._spriteRenderer.color = new Color(1f, 1f, 1f, this._spriteRenderer.color.a >= 1f ? this._invencibilityValue : 1f);
					yield return new WaitTime(this, this._timeStep);
				}
			}
			yield return new WaitTime(this, this._invencibilityTime);
			this._invencibility = false;
			this._spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
		}
		private UnityAction<bool> ManualInvencibility => (bool isInvencible) =>
		{
			if (isInvencible)
				this.StartCoroutine(this.Invencibility());
			else
				this.StopCoroutine(this.Invencibility());
		};
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<ICollectable>(out var collectable))
			{
				collectable.Collect();
				this._lifeText.text = $"X {this._saveFile.lifes}";
				this._coinsText.text = $"X {this._saveFile.coins}";
			}
		}
		public bool Damage(ushort damage)
		{
			if (this._invencibility || damage < 1f)
				return false;
			this._invencibility = true;
			if ((this._vitality -= (short)damage) <= 0f)
			{
				this._vitality = 0;
				this._saveFile.lifes -= 1;
				this._lifeText.text = $"X {(this._saveFile.lifes >= 0f ? this._saveFile.lifes : 0f)}";
				SaveController.WriteSave(this._saveFile);
				if (_grabObject)
					Destroy(_grabObject.gameObject);
				GuwbaTransformer<CommandGuwba>._actualState.Invoke(true);
				this.ManualInvencibility.Invoke(false);
				GuwbaTransformer<AttackGuwba>._actualState.Invoke(false);
				GuwbaTransformer<AttackGuwba>.Position = this.transform.position;
				ConfigurationController.DeathScreen();
				return true;
			}
			EffectsController.SetHitStop(this._hitStopTime, this._hitStopSlow);
			this.StartCoroutine(this.Invencibility());
			return true;
		}
	};
};
