using UnityEngine;
using UnityEngine.UIElements;
namespace GwambaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(MeshRenderer), typeof(UIDocument))]
	internal sealed class LevelGateHud : MonoBehaviour
	{
		[SerializeField, Tooltip("The material used to show the UI.")] private Material _uiMaterial;
		internal Button Level { get; private set; }
		internal Button Boss { get; private set; }
		internal Button Scenes { get; private set; }
		private void Awake()
		{
			VisualElement root = GetComponent<UIDocument>().rootVisualElement;
			Level = root.Q<Button>(nameof(Level));
			Boss = root.Q<Button>(nameof(Boss));
			Scenes = root.Q<Button>(nameof(Scenes));
			GetComponent<MeshRenderer>().material = _uiMaterial;
			transform.localScale = new Vector3(_uiMaterial.mainTexture.width / WorldBuild.PIXELS_PER_UNIT, _uiMaterial.mainTexture.height / WorldBuild.PIXELS_PER_UNIT);
		}
	};
};
