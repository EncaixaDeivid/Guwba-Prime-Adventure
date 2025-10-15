using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class GuwbaVisualizer : MonoBehaviour
	{
		private static GuwbaVisualizer _instance;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _rootElementObject;
		[SerializeField, Tooltip("User interface element.")] private string _vitalityVisual;
		[SerializeField, Tooltip("User interface element.")] private string _vitalityPieceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _recoverVitalityVisual;
		[SerializeField, Tooltip("User interface element.")] private string _recoverVitalityPieceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _stunResistanceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _stunResistancePieceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _bunnyHopVisual;
		[SerializeField, Tooltip("User interface element.")] private string _bunnyHopPieceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _fallDamageTextObject;
		[SerializeField, Tooltip("User interface element.")] private string _iconsObject;
		[SerializeField, Tooltip("User interface element.")] private string _lifeTextObject;
		[SerializeField, Tooltip("User interface element.")] private string _coinTextObject;
		[Header("Vitality Visual")]
		[SerializeField, Tooltip("The color of Guwba's vitality bar background.")] private Color _backgroundColor;
		[SerializeField, Tooltip("The color of Guwba's vitality bar border.")] private Color _borderColor;
		[SerializeField, Tooltip("The color of Guwba's stun resistance bar.")] private Color _stunResistanceColor;
		[SerializeField, Tooltip("The color of Guwba's bunny hop bar.")] private Color _bunnyHopColor;
		[SerializeField, Tooltip("The color of Guwba's vitality bar missing vitality piece.")] private Color _missingVitalityColor;
		[SerializeField, Tooltip("The total of vitality that Guwba have.")] private ushort _vitality;
		[SerializeField, Tooltip("The total of recover vitality that Guwba have.")] private ushort _recoverVitality;
		[SerializeField, Tooltip("The total of stun resistance that Guwba have.")] private ushort _stunResistance;
		[SerializeField, Tooltip("The total of bunny hop that Guwba have.")] private ushort _bunnyHop;
		[SerializeField, Tooltip("The total width of Guwba's vitality bar.")] private float _totalWidth;
		[SerializeField, Tooltip("The norder width of Guwba's vitality bar.")] private float _borderWidth;
		internal VisualElement RootElement { get; private set; }
		internal VisualElement[] Vitality { get; private set; }
		internal VisualElement[] RecoverVitality { get; private set; }
		internal VisualElement[] StunResistance { get; private set; }
		internal VisualElement[] BunnyHop { get; private set; }
		internal Label FallDamageText { get; private set; }
		internal Label LifeText { get; private set; }
		internal Label CoinText { get; private set; }
		internal Color BackgroundColor => this._backgroundColor;
		internal Color BorderColor => this._borderColor;
		internal Color StunResistanceColor => this._stunResistanceColor;
		internal Color BunnyHopColor => this._bunnyHopColor;
		internal Color MissingColor => this._missingVitalityColor;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 1e-3f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.RootElement = root.Q<VisualElement>(this._rootElementObject);
			this.FallDamageText = root.Q<Label>(this._fallDamageTextObject);
			this.LifeText = root.Q<Label>(this._lifeTextObject);
			this.CoinText = root.Q<Label>(this._coinTextObject);
			root.Q<GroupBox>(this._iconsObject).style.width = new StyleLength(new Length(this._totalWidth, LengthUnit.Pixel));
			VisualElement vitality = root.Q<VisualElement>($"{this._vitalityVisual}");
			vitality.style.width = new StyleLength(new Length(this._totalWidth, LengthUnit.Pixel));
			VisualElement vitalityPiece = root.Q<VisualElement>($"{this._vitalityPieceVisual}");
			this.Vitality = new VisualElement[this._vitality];
			for (ushort i = 0; i < this._vitality; i++)
			{
				VisualElement vitalityPieceClone = new() { name = vitalityPiece.name };
				vitalityPieceClone.style.backgroundColor = new StyleColor(this._backgroundColor);
				vitalityPieceClone.style.borderBottomColor = new StyleColor(this._borderColor);
				vitalityPieceClone.style.borderLeftColor = new StyleColor(this._borderColor);
				vitalityPieceClone.style.borderRightColor = new StyleColor(this._borderColor);
				vitalityPieceClone.style.borderTopColor = new StyleColor(this._borderColor);
				vitalityPieceClone.style.borderBottomWidth = new StyleFloat(this._borderWidth);
				vitalityPieceClone.style.borderLeftWidth = new StyleFloat(this._borderWidth);
				vitalityPieceClone.style.borderRightWidth = new StyleFloat(this._borderWidth);
				vitalityPieceClone.style.borderTopWidth = new StyleFloat(this._borderWidth);
				vitality.Add(vitalityPieceClone);
				this.Vitality[i] = vitality[i + 1];
			}
			vitality.Remove(vitalityPiece);
			VisualElement recoverVitality = root.Q<VisualElement>($"{this._recoverVitalityVisual}");
			recoverVitality.style.width = new StyleLength(new Length(this._totalWidth, LengthUnit.Pixel));
			VisualElement recoverVitalityPiece = root.Q<VisualElement>($"{this._recoverVitalityPieceVisual}");
			this.RecoverVitality = new VisualElement[this._recoverVitality];
			float recoverVitalityPieceWidth = this._totalWidth / this._recoverVitality - this._borderWidth * 2f;
			for (ushort i = 0; i < this._recoverVitality; i++)
			{
				VisualElement recoverVitalityPieceClone = new() { name = recoverVitalityPiece.name };
				recoverVitalityPieceClone.style.backgroundColor = new StyleColor(this._missingVitalityColor);
				recoverVitalityPieceClone.style.width = new StyleLength(new Length(recoverVitalityPieceWidth, LengthUnit.Pixel));
				recoverVitality.Add(recoverVitalityPieceClone);
				this.RecoverVitality[i] = recoverVitality[i + 1];
			}
			recoverVitality.Remove(recoverVitalityPiece);
			VisualElement stunResistance = root.Q<VisualElement>($"{this._stunResistanceVisual}");
			stunResistance.style.width = new StyleLength(new Length(this._totalWidth, LengthUnit.Pixel));
			VisualElement stunResistancePiece = root.Q<VisualElement>($"{this._stunResistancePieceVisual}");
			this.StunResistance = new VisualElement[this._stunResistance];
			float stunResistancePieceWidth = this._totalWidth / this._stunResistance - this._borderWidth * 2f;
			for (ushort i = 0; i < this._stunResistance; i++)
			{
				VisualElement stunResistancePieceClone = new() { name = stunResistancePiece.name };
				stunResistancePieceClone.style.backgroundColor = new StyleColor(this._stunResistanceColor);
				stunResistancePieceClone.style.width = new StyleLength(new Length(stunResistancePieceWidth, LengthUnit.Pixel));
				stunResistance.Add(stunResistancePieceClone);
				this.StunResistance[i] = stunResistance[i + 1];
			}
			stunResistance.Remove(stunResistancePiece);
			VisualElement bunnyHop = root.Q<VisualElement>(this._bunnyHopVisual);
			bunnyHop.style.width = new StyleLength(new Length(this._totalWidth, LengthUnit.Pixel));
			VisualElement bunnyHopPiece = root.Q<VisualElement>($"{this._bunnyHopPieceVisual}");
			this.BunnyHop = new VisualElement[this._bunnyHop];
			float bunnyHopPieceWidth = this._totalWidth / this._bunnyHop - this._borderWidth * 2f;
			for (ushort i = 0; i < this._bunnyHop; i++)
			{
				VisualElement bunnyHopClone = new() { name = bunnyHopPiece.name };
				bunnyHopClone.style.backgroundColor = new StyleColor(this._missingVitalityColor);
				bunnyHopClone.style.width = new StyleLength(new Length(bunnyHopPieceWidth, LengthUnit.Pixel));
				bunnyHop.Add(bunnyHopClone);
				this.BunnyHop[i] = bunnyHop[i + 1];
			}
			bunnyHop.Remove(bunnyHopPiece);
		}
	};
};
