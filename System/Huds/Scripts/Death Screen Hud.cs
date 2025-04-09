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
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this._text = root.Q<Label>(this._textLabel);
			this._continue = root.Q<Button>(this._continueButton);
			this._outLevel = root.Q<Button>(this._outLevelButton);
			this._gameOver = root.Q<Button>(this._gameOverButton);
		}
	};
};
