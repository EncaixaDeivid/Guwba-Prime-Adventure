using UnityEngine;
using UnityEngine.UIElements;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class TransicionHud : MonoBehaviour
	{
		private static TransicionHud _instance;
		[Header("Elements")]
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
			RootElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>(nameof(RootElement));
			LoadingBar = RootElement.Q<ProgressBar>(_loadingBarProgress);
		}
	};
};
