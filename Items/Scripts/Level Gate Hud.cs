using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class LevelGateHud : MonoBehaviour
	{
		private static LevelGateHud _instance;
		private VisualElement _rootElement;
		private Button _level, _boss;
		private Label _lifeText, _coinText;
		[SerializeField] private string _rootElementGroup, _levelButton, _bossButton, _lifeLabel, _coinLabel;
		internal VisualElement RootElement => this._rootElement;
		internal Button Level => this._level;
		internal Button Boss => this._boss;
		internal Label Life => this._lifeText;
		internal Label Coin => this._coinText;
		private void Awake()
		{
			if (_instance)
				Destroy(_instance.gameObject);
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this._rootElement = root.Q<VisualElement>(this._rootElementGroup);
			this._level = root.Q<Button>(this._levelButton);
			this._boss = root.Q<Button>(this._bossButton);
			this._lifeText = root.Q<Label>(this._lifeLabel);
			this._coinText = root.Q<Label>(this._coinLabel);
		}
	};
};
