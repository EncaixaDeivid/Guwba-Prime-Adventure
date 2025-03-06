using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class LevelGateHud : MonoBehaviour
	{
		private static LevelGateHud _instance;
		private GroupBox _baseElement;
		private Button _level, _boss;
		private Label _lifeText, _coinText;
		[SerializeField] private string _baseElementGroup, _levelButton, _bossButton, _lifeLabel, _coinLabel;
		internal GroupBox BaseElement => this._baseElement;
		internal Button Level => this._level;
		internal Button Boss => this._boss;
		internal Label Life => this._lifeText;
		internal Label Coin => this._coinText;
		private void Awake()
		{
			if (_instance)
				Destroy(_instance.gameObject);
			_instance = this;
			UIDocument hudDocument = this.GetComponent<UIDocument>();
			this._baseElement = hudDocument.rootVisualElement.Q<GroupBox>(this._baseElementGroup);
			this._level = hudDocument.rootVisualElement.Q<Button>(this._levelButton);
			this._boss = hudDocument.rootVisualElement.Q<Button>(this._bossButton);
			this._lifeText = hudDocument.rootVisualElement.Q<Label>(this._lifeLabel);
			this._coinText = hudDocument.rootVisualElement.Q<Label>(this._coinLabel);
		}
	};
};