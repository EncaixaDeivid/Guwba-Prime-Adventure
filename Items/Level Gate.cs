using UnityEngine;
using UnityEngine.UIElements;
namespace GwambaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(MeshRenderer), typeof(UIDocument))]
	internal sealed class LevelGateHud : MonoBehaviour
	{
		internal Button Level { get; private set; }
		internal Button Boss { get; private set; }
		internal Button Scenes { get; private set; }
		private void Awake()
		{
			VisualElement root = GetComponent<UIDocument>().rootVisualElement;
			Level = root.Q<Button>(nameof(Level));
			Boss = root.Q<Button>(nameof(Boss));
			Scenes = root.Q<Button>(nameof(Scenes));
		}
	};
};
