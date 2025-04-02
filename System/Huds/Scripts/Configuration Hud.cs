using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	public sealed class ConfigurationHud : MonoBehaviour
	{
		private static ConfigurationHud _instance;
		private GroupBox _confirmation, _buttons;
		private Button _outLevel, _yes, _no, _saveGame, _close;
		[SerializeField] private string _confirmationGroup, _outLevelButton, _yesButton, _noButton, _buttonsGroup, _saveGameButton, _closeButton;
		public GroupBox Confirmation => this._confirmation;
		public GroupBox Buttons => this._buttons;
		public Button OutLevel => this._outLevel;
		public Button Yes => this._yes;
		public Button No => this._no;
		public Button SaveGame => this._saveGame;
		public Button Close => this._close;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.0001f);
				return;
			}
			_instance = this;
			UIDocument hudDocument = this.GetComponent<UIDocument>();
			this._confirmation = hudDocument.rootVisualElement.Q<GroupBox>(this._confirmationGroup);
			this._outLevel = hudDocument.rootVisualElement.Q<Button>(this._outLevelButton);
			this._yes = hudDocument.rootVisualElement.Q<Button>(this._yesButton);
			this._no = hudDocument.rootVisualElement.Q<Button>(this._noButton);
			this._buttons = hudDocument.rootVisualElement.Q<GroupBox>(this._buttonsGroup);
			this._saveGame = hudDocument.rootVisualElement.Q<Button>(this._saveGameButton);
			this._close = hudDocument.rootVisualElement.Q<Button>(this._closeButton);
		}
	};
};
