using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Story
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class StorySceneHud : MonoBehaviour
	{
		private static bool _isExistent;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _sceneImageVisual;
		internal VisualElement SceneImage { get; private set; }
		private void Awake()
		{
			if (_isExistent)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_isExistent = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.SceneImage = root.Q<VisualElement>(this._sceneImageVisual);
		}
	};
};
