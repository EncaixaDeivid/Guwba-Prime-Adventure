using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class GuwbaHud : MonoBehaviour
	{
		private static GuwbaHud _instance;
		[SerializeField, Tooltip("User interface element.")] private string _rootElementObject;
		[SerializeField, Tooltip("User interface element.")] private string _vitalityVisual;
		[SerializeField, Tooltip("User interface element.")] private string _vitalityPieceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _lifeTextObject;
		[SerializeField, Tooltip("User interface element.")] private string _coinTextObject;
		[SerializeField, Tooltip("The total widht of Guwba's vitality bar.")] private ushort _totalWidth;
		[SerializeField, Tooltip("The total of vitality that Guwba have.")] private ushort _vitality;
		internal VisualElement RootElement { get; private set; }
		internal VisualElement[] VitalityVisual { get; private set; }
		internal Label LifeText { get; private set; }
		internal Label CoinText { get; private set; }
		internal ushort Vitality => (ushort)this._vitality;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.RootElement = root.Q<VisualElement>(this._rootElementObject);
			this.LifeText = root.Q<Label>(this._lifeTextObject);
			this.CoinText = root.Q<Label>(this._coinTextObject);
			VisualElement vitality = root.Q<VisualElement>($"{this._vitalityVisual}");
			vitality.style.width = new StyleLength(new Length(this._totalWidth, LengthUnit.Pixel));
			VisualElement vitalityPiece = root.Q<VisualElement>($"{this._vitalityPieceVisual}");
			this.VitalityVisual = new VisualElement[this._vitality];
			for (ushort i = 0; i < this._vitality; i++)
			{
				VisualElement vitalityPieceClone = new() { name = vitalityPiece.name };
				vitalityPieceClone.style.width = new StyleLength(new Length(this._totalWidth / this._vitality, LengthUnit.Pixel));
				vitality.Add(vitalityPieceClone);
				this.VitalityVisual[i] = vitality[i + 1];
			}
			vitality.Remove(vitalityPiece);
		}
	};
};
