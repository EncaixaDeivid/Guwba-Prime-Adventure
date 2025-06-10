using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class LevelGateHud : MonoBehaviour
	{
		private static LevelGateHud _instance;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _rootElementGroup;
		[SerializeField, Tooltip("User interface element.")] private string _levelButton;
		[SerializeField, Tooltip("User interface element.")] private string _bossButton;
		[SerializeField, Tooltip("User interface element.")] private string _scenesButton;
		[SerializeField, Tooltip("User interface element.")] private string _lifeLabel;
		[SerializeField, Tooltip("User interface element.")] private string _coinLabel;
		internal VisualElement RootElement { get; private set; }
		internal Button Level { get; private set; }
		internal Button Boss { get; private set; }
		internal Button Scenes { get; private set; }
		internal Label Life { get; private set; }
		internal Label Coin { get; private set; }
		private void Awake()
		{
			if (_instance)
				Destroy(_instance.gameObject);
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.RootElement = root.Q<VisualElement>(this._rootElementGroup);
			this.Level = root.Q<Button>(this._levelButton);
			this.Boss = root.Q<Button>(this._bossButton);
			this.Scenes = root.Q<Button>(this._scenesButton);
			this.Life = root.Q<Label>(this._lifeLabel);
			this.Coin = root.Q<Label>(this._coinLabel);
		}
	};
};
