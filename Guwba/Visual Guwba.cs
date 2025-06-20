using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Collections;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	public sealed class VisualGuwba : GuwbaAstral<VisualGuwba>, IDamageable, IConnector
	{
		private static VisualGuwba _instance;
		private GuwbaHud _guwbaHud;
		private SpriteRenderer _spriteRenderer;
		private readonly Sender _sender = Sender.Create();
		private short _vitality;
		private bool _invencibility = false;
		private bool _isDamaged = false;
		[Header("Visual Interaction")]
		[SerializeField, Tooltip("The object of the Guwba hud.")] private GuwbaHud _guwbaHudObject;
		[SerializeField, Tooltip("The name of the hubby world scene.")] private string _levelSelectorScene;
		[SerializeField, Tooltip("The amount of time that Guwba stays invencible.")] private float _invencibilityTime;
		[SerializeField, Tooltip("The value applied to visual when a hit is taken.")] private float _invencibilityValue;
		[SerializeField, Tooltip("The amount of time that the has to stay before fade.")] private float _timeStep;
		[SerializeField, Tooltip("The amount of time to stop the game when hit is taken.")] private float _hitStopTime;
		[SerializeField, Tooltip("The amount of time to slow the game when hit is taken.")] private float _hitStopSlow;
		public ushort Health => (ushort)this._vitality;
		public PathConnection PathConnection => PathConnection.Character;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			this._spriteRenderer = this.GetComponentInParent<SpriteRenderer>();
			this._guwbaHud = Instantiate(this._guwbaHudObject, this.transform);
			this._sender.SetStateForm(StateForm.Disable).SetToggle(false);
			SaveController.Load(out SaveFile saveFile);
			this._guwbaHud.LifeText.text = $"X {saveFile.lifes}";
			this._guwbaHud.CoinText.text = $"X {saveFile.coins}";
			this._vitality = (short)this._guwbaHud.Vitality;
			_actualState += this.ManualInvencibility;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!_instance || _instance != this)
				return;
			_actualState -= this.ManualInvencibility;
			this.StopAllCoroutines();
			this._spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
			Sender.Exclude(this);
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			if (this.gameObject.scene.name == this._levelSelectorScene)
				this._guwbaHud.RootElement.style.display = DisplayStyle.None;
			else
				this._guwbaHud.RootElement.style.display = DisplayStyle.Flex;
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			this._guwbaHud.RootElement.style.display = DisplayStyle.None;
		}
		private UnityAction<bool> ManualInvencibility => isInvencible => this._invencibility = isInvencible;
		private IEnumerator Invencibility()
		{
			this.StartCoroutine(VisualEffect());
			IEnumerator VisualEffect()
			{
				while (this._isDamaged)
				{
					this._spriteRenderer.color = new Color(1f, 1f, 1f, this._spriteRenderer.color.a >= 1f ? this._invencibilityValue : 1f);
					yield return new WaitTime(this, this._timeStep);
				}
			}
			yield return new WaitTime(this, this._invencibilityTime);
			this._isDamaged = false;
			this._spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<ICollectable>(out var collectable))
			{
				collectable.Collect();
				SaveController.Load(out SaveFile saveFile);
				this._guwbaHud.LifeText.text = $"X {saveFile.lifes}";
				this._guwbaHud.CoinText.text = $"X {saveFile.coins}";
			}
		}
		public bool Damage(ushort damage)
		{
			if (this._invencibility || this._isDamaged || damage < 1f)
				return false;
			this._isDamaged = true;
			this._vitality -= (short)damage;
			for (ushort i = (ushort)this._guwbaHud.VitalityVisual.Length; i > (this._vitality >= 0f ? this._vitality : 0f); i--)
			{
				this._guwbaHud.VitalityVisual[i - 1].style.backgroundColor = new StyleColor(new Color(0.75f, 0.75f, 0.75f));
				this._guwbaHud.VitalityVisual[i - 1].style.borderBottomColor = new StyleColor(new Color(0.75f, 0.75f, 0.75f));
				this._guwbaHud.VitalityVisual[i - 1].style.borderLeftColor = new StyleColor(new Color(0.75f, 0.75f, 0.75f));
				this._guwbaHud.VitalityVisual[i - 1].style.borderRightColor = new StyleColor(new Color(0.75f, 0.75f, 0.75f));
				this._guwbaHud.VitalityVisual[i - 1].style.borderTopColor = new StyleColor(new Color(0.75f, 0.75f, 0.75f));
			}
			if (this._vitality <= 0f)
			{
				this._vitality = 0;
				SaveController.Load(out SaveFile saveFile);
				saveFile.lifes -= 1;
				this._guwbaHud.LifeText.text = $"X {saveFile.lifes}";
				SaveController.WriteSave(saveFile);
				if (_grabObject)
					Destroy(_grabObject.gameObject);
				GuwbaAstral<CommandGuwba>._actualState.Invoke(false);
				this.ManualInvencibility.Invoke(false);
				GuwbaAstral<AttackGuwba>._actualState.Invoke(false);
				GuwbaAstral<AttackGuwba>.Position = this.transform.position;
				this.StopAllCoroutines();
				this._spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
				this._sender.SetToWhereConnection(PathConnection.Boss).Send();
				this._sender.SetToWhereConnection(PathConnection.Enemy).Send();
				return true;
			}
			EffectsController.SetHitStop(this._hitStopTime, this._hitStopSlow);
			this.StartCoroutine(this.Invencibility());
			return true;
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Disable && data.ToggleValue.HasValue && data.ToggleValue.Value)
			{
				for (ushort i = (ushort)this._guwbaHud.VitalityVisual.Length; i > 0f; i--)
				{
					this._guwbaHud.VitalityVisual[i - 1].style.backgroundColor = new StyleColor(new Color(96f / 255f, 0f, 0f));
					this._guwbaHud.VitalityVisual[i - 1].style.borderBottomColor = new StyleColor(new Color(32f / 255f, 0f, 0f));
					this._guwbaHud.VitalityVisual[i - 1].style.borderLeftColor = new StyleColor(new Color(32f / 255f, 0f, 0f));
					this._guwbaHud.VitalityVisual[i - 1].style.borderRightColor = new StyleColor(new Color(32f / 255f, 0f, 0f));
					this._guwbaHud.VitalityVisual[i - 1].style.borderTopColor = new StyleColor(new Color(32f / 255f, 0f, 0f));
				}
				this._invencibility = true;
				this._vitality = (short)this._guwbaHud.Vitality;
				GuwbaAstral<CommandGuwba>._actualState.Invoke(true);
				this.StartCoroutine(this.Invencibility());
			}
			else if (data.StateForm == StateForm.Action && data.ToggleValue.HasValue && data.ToggleValue.Value)
				this._vitality = (short)this._guwbaHud.Vitality;
		}
	};
};
