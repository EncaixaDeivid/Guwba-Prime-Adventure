using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class GuwbaHud : MonoBehaviour
	{
		private static GuwbaHud _instance;
		private GroupBox _baseElement;
		private VisualElement[] _vitality;
		private Label _lifeText, _coinsText;
		[SerializeField] private string _baseElementObject, _vitalityVisual, _lifeTextObject, _coinsTextObject;
		internal GroupBox BaseElement => this._baseElement;
		internal VisualElement[] Vitality => this._vitality;
		internal Label LifeText => this._lifeText;
		internal Label CoinsText => this._coinsText;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this._baseElement = root.Q<GroupBox>(this._baseElementObject);
			this._vitality = new VisualElement[4];
			for (ushort i = 0; i < this._vitality.Length; i++)
				this._vitality[i] = root.Q<VisualElement>($"{this._vitalityVisual}{i + 1f}");
			this._lifeText = root.Q<Label>(this._lifeTextObject);
			this._coinsText = root.Q<Label>(this._coinsTextObject);
		}
		internal void SetVitality(ushort vitality)
		{
			if (vitality > this._vitality.Length)
				return;
			for (ushort i = (ushort)this._vitality.Length; i > vitality; i--)
				this._vitality[i - 1].style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0f));
		}
	};
};
