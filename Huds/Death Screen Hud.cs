using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class DeathScreenHud : MonoBehaviour
	{
		private static bool _isExistent;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _textLabel;
		[SerializeField, Tooltip("User interface element.")] private string _curtainVisual;
		[SerializeField, Tooltip("User interface element.")] private string _continueButton;
		[SerializeField, Tooltip("User interface element.")] private string _outLevelButton;
		[SerializeField, Tooltip("User interface element.")] private string _gameOverButton;
		internal Label Text { get; private set; }
		internal VisualElement Curtain { get; private set; }
		internal Button Continue { get; private set; }
		internal Button OutLevel { get; private set; }
		internal Button GameOver { get; private set; }
		private void Awake()
		{
			if (_isExistent)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_isExistent = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.Text = root.Q<Label>(this._textLabel);
			this.Curtain = root.Q<VisualElement>(this._curtainVisual);
			this.Continue = root.Q<Button>(this._continueButton);
			this.OutLevel = root.Q<Button>(this._outLevelButton);
			this.GameOver = root.Q<Button>(this._gameOverButton);
		}
	};
};
