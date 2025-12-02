using UnityEngine;
using UnityEngine.UIElements;
namespace GwambaPrimeAdventure.Hud
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
		internal VisualElement RootElement { get; private set; }
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
			RootElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>(nameof(RootElement));
			Curtain = RootElement.Q<VisualElement>(_curtainVisual);
			Text = RootElement.Q<Label>(_textLabel);
			Continue = RootElement.Q<Button>(_continueButton);
			OutLevel = RootElement.Q<Button>(_outLevelButton);
			GameOver = RootElement.Q<Button>(_gameOverButton);
		}
	};
};
