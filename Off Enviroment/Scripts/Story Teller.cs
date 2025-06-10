using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
namespace GuwbaPrimeAdventure.OffEnviroment
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	internal sealed class StoryTeller : MonoBehaviour
	{
		private StorySceneHud _storySceneHud;
		private ushort _imageIndex = 0;
		[Header("Scene Object")]
		[SerializeField, Tooltip("The object that handles the hud of the story scene.")] private StorySceneHud _storySceneHudObject;
		[SerializeField, Tooltip("The object that carry the scene settings.")] private SceneObject _sceneObject;
		private void UpdateImage()
		{
			this._imageIndex = (ushort)(this._imageIndex < this._sceneObject.BackgroundImages.Length - 1 ? this._imageIndex + 1 : 0);
			Texture2D texture = this._sceneObject.BackgroundImages[this._imageIndex].Image;
			this._storySceneHud.SceneImage.style.backgroundImage = Background.FromTexture2D(texture);
		}
		private IEnumerator FadeImage(bool appear)
		{
			if (appear)
				for (float i = 0f; this._storySceneHud.SceneImage.style.opacity.value < 1f; i += 0.1f)
					yield return this._storySceneHud.SceneImage.style.opacity = i;
			else
				for (float i = 1f; this._storySceneHud.SceneImage.style.opacity.value > 0f; i -= 0.1f)
					yield return this._storySceneHud.SceneImage.style.opacity = i;
		}
		internal void ShowScene()
		{
			this._storySceneHud = Instantiate(this._storySceneHudObject, this.transform);
			Texture2D texture = this._sceneObject.BackgroundImages[this._imageIndex].Image;
			this._storySceneHud.SceneImage.style.backgroundImage = Background.FromTexture2D(texture);
			this.StartCoroutine(this.FadeImage(true));
		}
		internal IEnumerator NextSlide()
		{
			yield return this.FadeImage(false);
			this.UpdateImage();
			yield return this.FadeImage(true);
			while (this._sceneObject.BackgroundImages[this._imageIndex].OffDialog)
			{
				yield return new WaitForSeconds(this._sceneObject.BackgroundImages[this._imageIndex].TimeToDesapear);
				yield return this.FadeImage(false);
				this.UpdateImage();
				yield return this.FadeImage(true);
			}
		}
		internal void CloseScene() => this.StartCoroutine(this.FadeImage(false));
	};
};
