using UnityEngine;
using UnityEngine.UIElements;
namespace GwambaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class LevelGateHud : MonoBehaviour
	{
		private UIDocument _document;
		internal Button Level { get; private set; }
		internal Button Boss { get; private set; }
		internal Button Scenes { get; private set; }
		private void Awake()
		{
			_document = GetComponent<UIDocument>();
			VisualElement root = _document.rootVisualElement;
			Level = root.Q<Button>(nameof(Level));
			Boss = root.Q<Button>(nameof(Boss));
			Scenes = root.Q<Button>(nameof(Scenes));
		}
		internal PanelSettings SetPanelSettings(PanelSettings panel) => _document.panelSettings = panel;
	};
};
