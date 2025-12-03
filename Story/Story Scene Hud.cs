using UnityEngine;
using UnityEngine.UIElements;
namespace GwambaPrimeAdventure.Story
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class StorySceneHud : MonoBehaviour
	{
		private static StorySceneHud _instance;
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _sceneImageVisual;
		internal VisualElement SceneImage { get; private set; }
		private void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, WorldBuild.MINIMUM_TIME_SPACE_LIMIT);
				return;
			}
			_instance = this;
			SceneImage = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>(_sceneImageVisual);
		}
	};
};
