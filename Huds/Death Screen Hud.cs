using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class DeathScreenHud : MonoBehaviour
	{
		private static DeathScreenHud _instance;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _curtainVisual;
		[SerializeField, Tooltip("User interface element.")] private string _textLabel;
		[SerializeField, Tooltip("User interface element.")] private string _continueButton;
		[SerializeField, Tooltip("User interface element.")] private string _outLevelButton;
		[SerializeField, Tooltip("User interface element.")] private string _gameOverButton;
		internal VisualElement Curtain { get; private set; }
		internal Label Text { get; private set; }
		internal Button Continue { get; private set; }
		internal Button OutLevel { get; private set; }
		internal Button GameOver { get; private set; }
		private void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			VisualElement root = GetComponent<UIDocument>().rootVisualElement;
			Curtain = root.Q<VisualElement>(_curtainVisual);
			Text = root.Q<Label>(_textLabel);
			Continue = root.Q<Button>(_continueButton);
			OutLevel = root.Q<Button>(_outLevelButton);
			GameOver = root.Q<Button>(_gameOverButton);
		}
	};
};
