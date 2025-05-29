using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class TransicionHud : MonoBehaviour
	{
		private static TransicionHud _instance;
		[Header("Elements"), SerializeField] private string _rootElement;
		[SerializeField] private string _loadingBarProgress;
		internal VisualElement RootVisualElement { get; private set; }
		internal ProgressBar LoadingBar { get; private set; }
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.RootVisualElement = root.Q<VisualElement>(this._rootElement);
			this.LoadingBar = root.Q<ProgressBar>(this._loadingBarProgress);
		}
	};
};
