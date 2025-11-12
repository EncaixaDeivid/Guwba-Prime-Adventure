using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
namespace GwambaPrimeAdventure.Story
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	internal sealed class StoryTeller : MonoBehaviour
	{
		private StorySceneHud _storySceneHud;
		private ushort _imageIndex = 0;
		[Header("Scene Objects")]
		[SerializeField, Tooltip("The object that handles the hud of the story scene.")] private StorySceneHud _storySceneHudObject;
		[SerializeField, Tooltip("The object that carry the scene settings.")] private SceneObject _sceneObject;
		private IEnumerator FadeImage(bool appear)
		{
			if (appear)
				for (float i = 0f; _storySceneHud.SceneImage.style.opacity.value < 1f; i += 0.1f)
					yield return _storySceneHud.SceneImage.style.opacity = i;
			else
				for (float i = 1f; _storySceneHud.SceneImage.style.opacity.value > 0f; i -= 0.1f)
					yield return _storySceneHud.SceneImage.style.opacity = i;
		}
		internal void ShowScene()
		{
			_storySceneHud = Instantiate(_storySceneHudObject, transform);
			_storySceneHud.SceneImage.style.backgroundImage = Background.FromTexture2D(_sceneObject.SceneComponents[_imageIndex].Image);
			StartCoroutine(FadeImage(true));
		}
		internal IEnumerator NextSlide()
		{
			yield return FadeImage(false);
			_imageIndex = (ushort)(_imageIndex < _sceneObject.SceneComponents.Length - 1 ? _imageIndex + 1 : 0);
			_storySceneHud.SceneImage.style.backgroundImage = Background.FromTexture2D(_sceneObject.SceneComponents[_imageIndex].Image);
			yield return FadeImage(true);
			while (_sceneObject.SceneComponents[_imageIndex].OffDialog)
			{
				yield return new WaitForSeconds(_sceneObject.SceneComponents[_imageIndex].TimeToDesapear);
				if (_sceneObject.SceneComponents[_imageIndex].JumpToNext)
					yield return NextSlide();
			}
		}
		internal void CloseScene() => StartCoroutine(FadeImage(false));
	};
};
