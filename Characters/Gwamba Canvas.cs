using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using NaughtyAttributes;
namespace GwambaPrimeAdventure.Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class GwambaCanvas : MonoBehaviour
	{
		private static GwambaCanvas _instance;
		[field: SerializeField, BoxGroup("Visual"), ColorUsage(true, true), Tooltip("The color of Gwamba's vitality bar background."), Space(WorldBuild.FIELD_SPACE_LENGTH * 2F)]
		internal Color BackgroundColor { get; private set; }
		[field: SerializeField, BoxGroup("Visual"), ColorUsage(true, true), Tooltip("The color of Gwamba's vitality bar border.")] internal Color BorderColor { get; private set; }
		[field: SerializeField, BoxGroup("Visual"), ColorUsage(true, true), Tooltip("The color of Gwamba's stun resistance bar.")] internal Color StunResistanceColor
		{ get; private set; }
		[field: SerializeField, BoxGroup("Visual"), ColorUsage(true, true), Tooltip("The color of Gwamba's bunny hop bar.")] internal Color BunnyHopColor { get; private set; }
		[field: SerializeField, BoxGroup("Visual"), ColorUsage(true, true), Tooltip("The color of Gwamba's vitality bar missing vitality piece.")]
		internal Color MissingColor { get; private set; }
		[SerializeField, BoxGroup("Visual"), Tooltip("The total of vitality that Gwamba have.")] private ushort _vitality;
		[SerializeField, BoxGroup("Visual"), Tooltip("The total of recover vitality that Gwamba have.")] private ushort _recoverVitality;
		[SerializeField, BoxGroup("Visual"), Tooltip("The total of stun resistance that Gwamba have.")] private ushort _stunResistance;
		[SerializeField, BoxGroup("Visual"), Tooltip("The total of bunny hop that Gwamba have.")] private ushort _bunnyHop;
		[SerializeField, BoxGroup("Visual"), Min(0F), Tooltip("The total width of Gwamba's vitality bar.")] private float _totalWidth;
		[SerializeField, BoxGroup("Visual"), Min(0F), Tooltip("The norder width of Gwamba's vitality bar.")] private float _borderWidth;
		[Space(WorldBuild.FIELD_SPACE_LENGTH * 2F)]
		[SerializeField, BoxGroup("Elements"), Tooltip("User interface element.")] private string _vitalityVisual;
		[SerializeField, BoxGroup("Elements"), Tooltip("User interface element.")] private string _vitalityPieceVisual;
		[SerializeField, BoxGroup("Elements"), Tooltip("User interface element.")] private string _recoverVitalityVisual;
		[SerializeField, BoxGroup("Elements"), Tooltip("User interface element.")] private string _recoverVitalityPieceVisual;
		[SerializeField, BoxGroup("Elements"), Tooltip("User interface element.")] private string _stunResistanceVisual;
		[SerializeField, BoxGroup("Elements"), Tooltip("User interface element.")] private string _stunResistancePieceVisual;
		[SerializeField, BoxGroup("Elements"), Tooltip("User interface element.")] private string _bunnyHopVisual;
		[SerializeField, BoxGroup("Elements"), Tooltip("User interface element.")] private string _bunnyHopPieceVisual;
		[SerializeField, BoxGroup("Elements"), Tooltip("User interface element.")] private string _fallDamageTextObject;
		[SerializeField, BoxGroup("Elements"), Tooltip("User interface element.")] private string _lifeTextObject;
		[SerializeField, BoxGroup("Elements"), Tooltip("User interface element.")] private string _coinTextObject;
		internal VisualElement RootElement { get; private set; }
		internal VisualElement[] Vitality { get; private set; }
		internal VisualElement[] RecoverVitality { get; private set; }
		internal VisualElement[] StunResistance { get; private set; }
		internal VisualElement[] BunnyHop { get; private set; }
		internal Label FallDamageText { get; private set; }
		internal Label LifeText { get; private set; }
		internal Label CoinText { get; private set; }
		private void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, WorldBuild.MINIMUM_TIME_SPACE_LIMIT);
				return;
			}
			_instance = this;
			RootElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>(nameof(RootElement));
			Vitality = new VisualElement[_vitality];
			RecoverVitality = new VisualElement[_recoverVitality];
			StunResistance = new VisualElement[_stunResistance];
			BunnyHop = new VisualElement[_bunnyHop];
			FallDamageText = RootElement.Q<Label>(_fallDamageTextObject);
			LifeText = RootElement.Q<Label>(_lifeTextObject);
			CoinText = RootElement.Q<Label>(_coinTextObject);
		}
		internal IEnumerator StartUI()
		{
			VisualElement vitality = RootElement.Q<VisualElement>(_vitalityVisual);
			vitality.style.width = new StyleLength(new Length(_totalWidth, LengthUnit.Pixel));
			VisualElement vitalityPiece = RootElement.Q<VisualElement>(_vitalityPieceVisual);
			for (ushort i = 0; i < _vitality; i++)
			{
				VisualElement vitalityPieceClone = new() { name = vitalityPiece.name };
				vitalityPieceClone.style.backgroundColor = new StyleColor(BackgroundColor);
				vitalityPieceClone.style.borderBottomColor = new StyleColor(BorderColor);
				vitalityPieceClone.style.borderLeftColor = new StyleColor(BorderColor);
				vitalityPieceClone.style.borderRightColor = new StyleColor(BorderColor);
				vitalityPieceClone.style.borderTopColor = new StyleColor(BorderColor);
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
			for (ushort i = 0; i < _recoverVitality; i++)
			{
				VisualElement recoverVitalityPieceClone = new() { name = recoverVitalityPiece.name };
				recoverVitalityPieceClone.style.backgroundColor = new StyleColor(MissingColor);
				recoverVitalityPieceClone.style.width = new StyleLength(new Length(_totalWidth / _recoverVitality - _borderWidth * 2F, LengthUnit.Pixel));
				recoverVitality.Add(recoverVitalityPieceClone);
				RecoverVitality[i] = recoverVitality[i + 1];
			}
			recoverVitality.Remove(recoverVitalityPiece);
			VisualElement stunResistance = RootElement.Q<VisualElement>(_stunResistanceVisual);
			stunResistance.style.width = new StyleLength(new Length(_totalWidth, LengthUnit.Pixel));
			VisualElement stunResistancePiece = RootElement.Q<VisualElement>(_stunResistancePieceVisual);
			for (ushort i = 0; i < _stunResistance; i++)
			{
				VisualElement stunResistancePieceClone = new() { name = stunResistancePiece.name };
				stunResistancePieceClone.style.backgroundColor = new StyleColor(StunResistanceColor);
				stunResistancePieceClone.style.width = new StyleLength(new Length(_totalWidth / _stunResistance - _borderWidth * 2F, LengthUnit.Pixel));
				stunResistance.Add(stunResistancePieceClone);
				StunResistance[i] = stunResistance[i + 1];
			}
			stunResistance.Remove(stunResistancePiece);
			VisualElement bunnyHop = RootElement.Q<VisualElement>(_bunnyHopVisual);
			bunnyHop.style.width = new StyleLength(new Length(_totalWidth, LengthUnit.Pixel));
			VisualElement bunnyHopPiece = RootElement.Q<VisualElement>(_bunnyHopPieceVisual);
			for (ushort i = 0; i < _bunnyHop; i++)
			{
				VisualElement bunnyHopPieceClone = new() { name = bunnyHopPiece.name };
				bunnyHopPieceClone.style.backgroundColor = new StyleColor(MissingColor);
				bunnyHopPieceClone.style.width = new StyleLength(new Length(_totalWidth / _bunnyHop - _borderWidth * 2F, LengthUnit.Pixel));
				bunnyHop.Add(bunnyHopPieceClone);
				BunnyHop[i] = bunnyHop[i + 1];
			}
			bunnyHop.Remove(bunnyHopPiece);
			yield return null;
		}
	};
};
