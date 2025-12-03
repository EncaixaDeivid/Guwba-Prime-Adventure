using UnityEngine;
using UnityEngine.UIElements;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	public sealed class TransicionHud : MonoBehaviour
	{
		private static TransicionHud _instance;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _loadingBarProgress;
		public VisualElement RootElement { get; private set; }
		public ProgressBar LoadingBar { get; private set; }
		private void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, WorldBuild.MINIMUM_TIME_SPACE_LIMIT);
				return;
			}
			_instance = this;
			RootElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>(nameof(RootElement));
			LoadingBar = RootElement.Q<ProgressBar>(_loadingBarProgress);
		}
	};
};
