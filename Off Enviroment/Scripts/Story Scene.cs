using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
namespace GuwbaPrimeAdventure.OffEnviroment
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	internal sealed class StoryScene : MonoBehaviour
	{
		private StorySceneHud _storySceneHud;
		private ushort _imageIndex = 0;
		[SerializeField] private StorySceneHud _storySceneHudObject;
		[SerializeField] private SceneObject _sceneObject;
		[SerializeField] private float _fadeSpeed;
		private IEnumerator FadeImage(short wayToGo)
		{
			wayToGo = (short)(wayToGo > 0 ? 1 : -1);
			for (uint i = 0; i < 1; i++)
				yield return this._storySceneHud.SceneImage.style.opacity = i * wayToGo * this._fadeSpeed * Time.fixedDeltaTime;
		}
		internal void ShowScene()
		{
			this._storySceneHud = Instantiate(this._storySceneHudObject, this.transform);
			Texture2D texture = this._sceneObject.BackgroundImages[this._imageIndex].image;
			this._storySceneHud.SceneImage.style.backgroundImage = Background.FromTexture2D(texture);
			this.StartCoroutine(this.FadeImage(1));
		}
		internal IEnumerator NextSlide()
		{
			yield return this.FadeImage(0);
			this._imageIndex = (ushort)(this._imageIndex < this._sceneObject.BackgroundImages.Length - 1 ? this._imageIndex + 1 : 0);
			yield return this.FadeImage(1);
			if (this._sceneObject.TimeToDesapear > 0f)
			{
				yield return new WaitForSeconds(this._sceneObject.TimeToDesapear);
				yield return this.FadeImage(0);
				this._imageIndex = (ushort)(this._imageIndex < this._sceneObject.BackgroundImages.Length - 1 ? this._imageIndex + 1 : 0);
				yield return this.FadeImage(1);
			}
		}
		internal void CloseScene() => this.StartCoroutine(this.FadeImage(0));
	};
};