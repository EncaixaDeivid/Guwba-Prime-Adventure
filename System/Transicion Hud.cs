using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class TransicionHud : MonoBehaviour
	{
		private static TransicionHud _instance;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _rootElement;
		[SerializeField, Tooltip("User interface element.")] private string _loadingBarProgress;
		internal VisualElement RootElement { get; private set; }
		internal ProgressBar LoadingBar { get; private set; }
		private void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
			VisualElement root = GetComponent<UIDocument>().rootVisualElement;
			RootElement = root.Q<VisualElement>(_rootElement);
			LoadingBar = root.Q<ProgressBar>(_loadingBarProgress);
		}
	};
};
