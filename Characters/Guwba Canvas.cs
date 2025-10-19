using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
namespace GuwbaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class GuwbaCanvas : MonoBehaviour
	{
		private static GuwbaCanvas _instance;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _vitalityVisual;
		[SerializeField, Tooltip("User interface element.")] private string _vitalityPieceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _recoverVitalityVisual;
		[SerializeField, Tooltip("User interface element.")] private string _recoverVitalityPieceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _stunResistanceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _stunResistancePieceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _bunnyHopVisual;
		[SerializeField, Tooltip("User interface element.")] private string _bunnyHopPieceVisual;
		[SerializeField, Tooltip("User interface element.")] private string _fallDamageTextObject;
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
		internal Color BackgroundColor => _backgroundColor;
		internal Color BorderColor => _borderColor;
		internal Color StunResistanceColor => _stunResistanceColor;
		internal Color BunnyHopColor => _bunnyHopColor;
		internal Color MissingColor => _missingVitalityColor;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			RootElement = GetComponent<UIDocument>().rootVisualElement;
			FallDamageText = RootElement.Q<Label>(_fallDamageTextObject);
			LifeText = RootElement.Q<Label>(_lifeTextObject);
			CoinText = RootElement.Q<Label>(_coinTextObject);
		}
		private IEnumerator Start()
		{
			yield return new WaitWhile(() => SceneInitiator.IsInTrancision());
			VisualElement vitality = RootElement.Q<VisualElement>(_vitalityVisual);
			vitality.style.width = new StyleLength(new Length(_totalWidth, LengthUnit.Pixel));
			VisualElement vitalityPiece = RootElement.Q<VisualElement>(_vitalityPieceVisual);
			Vitality = new VisualElement[_vitality];
			for (ushort i = 0; i < _vitality; i++)
			{
				VisualElement vitalityPieceClone = new() { name = vitalityPiece.name };
				vitalityPieceClone.style.backgroundColor = new StyleColor(_backgroundColor);
				vitalityPieceClone.style.borderBottomColor = new StyleColor(_borderColor);
				vitalityPieceClone.style.borderLeftColor = new StyleColor(_borderColor);
				vitalityPieceClone.style.borderRightColor = new StyleColor(_borderColor);
				vitalityPieceClone.style.borderTopColor = new StyleColor(_borderColor);
				vitalityPieceClone.style.borderBottomWidth = new StyleFloat(_borderWidth);
				vitalityPieceClone.style.borderLeftWidth = new StyleFloat(_borderWidth);
				vitalityPieceClone.style.borderRightWidth = new StyleFloat(_borderWidth);
				vitalityPieceClone.style.borderTopWidth = new StyleFloat(_borderWidth);
				vitality.Add(vitalityPieceClone);
				Vitality[i] = vitality[i + 1];
			}
			vitality.Remove(vitalityPiece);
			VisualElement recoverVitality = RootElement.Q<VisualElement>(_recoverVitalityVisual);
			recoverVitality.style.width = new StyleLength(new Length(_totalWidth, LengthUnit.Pixel));
			VisualElement recoverVitalityPiece = RootElement.Q<VisualElement>(_recoverVitalityPieceVisual);
			RecoverVitality = new VisualElement[_recoverVitality];
			for (ushort i = 0; i < _recoverVitality; i++)
			{
				VisualElement recoverVitalityPieceClone = new() { name = recoverVitalityPiece.name };
				recoverVitalityPieceClone.style.backgroundColor = new StyleColor(_missingVitalityColor);
				recoverVitalityPieceClone.style.width = new StyleLength(new Length(_totalWidth / _recoverVitality - _borderWidth * 2f, LengthUnit.Pixel));
				recoverVitality.Add(recoverVitalityPieceClone);
				RecoverVitality[i] = recoverVitality[i + 1];
			}
			recoverVitality.Remove(recoverVitalityPiece);
			VisualElement stunResistance = RootElement.Q<VisualElement>(_stunResistanceVisual);
			stunResistance.style.width = new StyleLength(new Length(_totalWidth, LengthUnit.Pixel));
			VisualElement stunResistancePiece = RootElement.Q<VisualElement>(_stunResistancePieceVisual);
			StunResistance = new VisualElement[_stunResistance];
			for (ushort i = 0; i < _stunResistance; i++)
			{
				VisualElement stunResistancePieceClone = new() { name = stunResistancePiece.name };
				stunResistancePieceClone.style.backgroundColor = new StyleColor(_stunResistanceColor);
				stunResistancePieceClone.style.width = new StyleLength(new Length(_totalWidth / _stunResistance - _borderWidth * 2f, LengthUnit.Pixel));
				stunResistance.Add(stunResistancePieceClone);
				StunResistance[i] = stunResistance[i + 1];
			}
			stunResistance.Remove(stunResistancePiece);
			VisualElement bunnyHop = RootElement.Q<VisualElement>(_bunnyHopVisual);
			bunnyHop.style.width = new StyleLength(new Length(_totalWidth, LengthUnit.Pixel));
			VisualElement bunnyHopPiece = RootElement.Q<VisualElement>(_bunnyHopPieceVisual);
			BunnyHop = new VisualElement[_bunnyHop];
			for (ushort i = 0; i < _bunnyHop; i++)
			{
				VisualElement bunnyHopPieceClone = new() { name = bunnyHopPiece.name };
				bunnyHopPieceClone.style.backgroundColor = new StyleColor(_missingVitalityColor);
				bunnyHopPieceClone.style.width = new StyleLength(new Length(_totalWidth / _bunnyHop - _borderWidth * 2f, LengthUnit.Pixel));
				bunnyHop.Add(bunnyHopPieceClone);
				BunnyHop[i] = bunnyHop[i + 1];
			}
			bunnyHop.Remove(bunnyHopPiece);
		}
	};
};
