using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class VisualizableGuwba : MonoBehaviour
	{
		private static VisualizableGuwba _instance;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _rootElementObject;
		[SerializeField, Tooltip("User interface element.")] private string _vitalityVisual;
		[SerializeField, Tooltip("User interface element.")] private string _vitalityPieceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _recoverVitalityVisual;
		[SerializeField, Tooltip("User interface element.")] private string _recoverVitalityPieceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _lifeTextObject;
		[SerializeField, Tooltip("User interface element.")] private string _coinTextObject;
		[Header("Vitality Visual")]
		[SerializeField, Tooltip("The color of Guwba's vitality bar background.")] private Color _backgroundColor;
		[SerializeField, Tooltip("The color of Guwba's vitality bar border.")] private Color _borderColor;
		[SerializeField, Tooltip("The color of Guwba's vitality bar missing vitality piece.")] private Color _missingVitalityColor;
		[SerializeField, Tooltip("The total of vitality that Guwba have.")] private ushort _vitality;
		[SerializeField, Tooltip("The total of recover vitality that Guwba have.")] private ushort _recoverVitality;
		[SerializeField, Tooltip("The total width of Guwba's vitality bar.")] private float _totalWidth;
		[SerializeField, Tooltip("The norder width of Guwba's vitality bar.")] private float _borderWidth;
		internal VisualElement RootElement { get; private set; }
		internal VisualElement[] VitalityVisual { get; private set; }
		internal VisualElement[] RecoverVitalityVisual { get; private set; }
		internal Label LifeText { get; private set; }
		internal Label CoinText { get; private set; }
		internal Color BackgroundColor => this._backgroundColor;
		internal Color BorderColor => this._borderColor;
		internal Color MissingVitalityColor => this._missingVitalityColor;
		internal ushort Vitality => (ushort)this._vitality;
		internal ushort RecoverVitality => (ushort)this._recoverVitality;
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
				vitalityPieceClone.style.backgroundColor = new StyleColor(this._backgroundColor);
				vitalityPieceClone.style.borderBottomColor = new StyleColor(this._borderColor);
				vitalityPieceClone.style.borderLeftColor = new StyleColor(this._borderColor);
				vitalityPieceClone.style.borderRightColor = new StyleColor(this._borderColor);
				vitalityPieceClone.style.borderTopColor = new StyleColor(this._borderColor);
				vitalityPieceClone.style.width = new StyleLength(new Length(this._totalWidth / this._vitality, LengthUnit.Pixel));
				vitalityPieceClone.style.borderBottomWidth = new StyleFloat(this._borderWidth);
				vitalityPieceClone.style.borderLeftWidth = new StyleFloat(this._borderWidth);
				vitalityPieceClone.style.borderRightWidth = new StyleFloat(this._borderWidth);
				vitalityPieceClone.style.borderTopWidth = new StyleFloat(this._borderWidth);
				vitality.Add(vitalityPieceClone);
				this.VitalityVisual[i] = vitality[i + 1];
			}
			vitality.Remove(vitalityPiece);
			VisualElement recoverVitality = root.Q<VisualElement>($"{this._recoverVitalityVisual}");
			recoverVitality.style.width = new StyleLength(new Length(this._totalWidth, LengthUnit.Pixel));
			VisualElement recoverVitalityPiece = root.Q<VisualElement>($"{this._recoverVitalityPieceVisual}");
			this.RecoverVitalityVisual = new VisualElement[this._recoverVitality];
			for (ushort i = 0; i < this._recoverVitality; i++)
			{
				VisualElement vitalityPieceClone = new() { name = recoverVitalityPiece.name };
				vitalityPieceClone.style.backgroundColor = new StyleColor(this._missingVitalityColor);
				float width = this._totalWidth / this._recoverVitality - this._borderWidth * 2f;
				vitalityPieceClone.style.width = new StyleLength(new Length(width, LengthUnit.Pixel));
				vitalityPieceClone.style.marginLeft = new StyleLength(new Length(this._borderWidth, LengthUnit.Pixel));
				vitalityPieceClone.style.marginRight = new StyleLength(new Length(this._borderWidth, LengthUnit.Pixel));
				recoverVitality.Add(vitalityPieceClone);
				this.RecoverVitalityVisual[i] = recoverVitality[i + 1];
			}
			recoverVitality.Remove(recoverVitalityPiece);
		}
	};
};
