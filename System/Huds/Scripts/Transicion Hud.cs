using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	public sealed class TransicionHud : MonoBehaviour
	{
		private static TransicionHud _instance;
		private VisualElement _rootVisualElement;
		private ProgressBar _loadingBar;
		[Header("Elements"), SerializeField] private string _rootElement;
		[SerializeField] private string _loadingBarProgress;
		[SerializeField] private float _appearRate;
		public VisualElement RootVisualElement => this._rootVisualElement;
		public ProgressBar LoadingBar => this._loadingBar;
		public float ApearRate => this._appearRate;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this._rootVisualElement = root.Q<VisualElement>(this._rootElement);
			this._loadingBar = root.Q<ProgressBar>(this._loadingBarProgress);
		}
	};
};
