using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class TransicionHud : MonoBehaviour
	{
		private static bool _isExistent;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _rootElement;
		[SerializeField, Tooltip("User interface element.")] private string _loadingBarProgress;
		internal VisualElement RootVisualElement { get; private set; }
		internal ProgressBar LoadingBar { get; private set; }
		private void Awake()
		{
			if (_isExistent)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_isExistent = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.RootVisualElement = root.Q<VisualElement>(this._rootElement);
			this.LoadingBar = root.Q<ProgressBar>(this._loadingBarProgress);
		}
	};
};
