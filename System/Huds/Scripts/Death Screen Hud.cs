using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	public sealed class DeathScreenHud : MonoBehaviour
	{
		private static DeathScreenHud _instance;
		private Label _text;
		private Button _continue, _outLevel, _gameOver;
		[SerializeField] private string _textLabel, _continueButton, _outLevelButton, _gameOverButton;
		public Label Text => this._text;
		public Button Continue => this._continue;
		public Button OutLevel => this._outLevel;
		public Button GameOver => this._gameOver;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			UIDocument hudDocument = this.GetComponent<UIDocument>();
			this._text = hudDocument.rootVisualElement.Q<Label>(this._textLabel);
			this._continue = hudDocument.rootVisualElement.Q<Button>(this._continueButton);
			this._outLevel = hudDocument.rootVisualElement.Q<Button>(this._outLevelButton);
			this._gameOver = hudDocument.rootVisualElement.Q<Button>(this._gameOverButton);
		}
	};
};
