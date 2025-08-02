using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Light2DBase)), DefaultExecutionOrder(-1)]
	public sealed class EffectsController : StateController
	{
		private static EffectsController _instance;
		private List<Light2DBase> _lightsStack;
		private bool _canStun = true;
		[SerializeField, Tooltip("The renderer of the image.")] private ImageRenderer _imageRenderer;
		private new void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			base.Awake();
			this._lightsStack = new List<Light2DBase>() { this.GetComponent<Light2DBase>() };
		}
		private void PrivateStun(IDamageable damageable, Vector2 stunDirection, float stunStrength, float stopTime, float slowTime)
		{
			if (this._canStun)
			{
				this.StartCoroutine(HitStop());
				if (damageable != null && (damageable as StateController).TryGetComponent<Rigidbody2D>(out var rigidbody))
				{
					float strength = stunStrength - damageable.StunResistance * rigidbody.mass;
					if (strength < 0f)
						strength = 0f;
					rigidbody.AddForce(stunDirection * strength, ForceMode2D.Impulse);
				}
			}
			IEnumerator HitStop()
			{
				this._canStun = false;
				Time.timeScale = slowTime;
				yield return new WaitForSecondsRealtime(stopTime);
				this._canStun = true;
				Time.timeScale = 1f;
			}
		}
		private void PrivateOnGlobalLight(Light2DBase globalLight)
		{
			if (this._lightsStack.Contains(globalLight))
				return;
			foreach (Light2DBase light in this._lightsStack)
				light.enabled = false;
			globalLight.enabled = true;
			this._lightsStack.Add(globalLight);
		}
		private void PrivateOffGlobalLight(Light2DBase globalLight)
		{
			if (!this._lightsStack.Contains(globalLight))
				return;
			foreach (Light2DBase light in this._lightsStack)
				light.enabled = false;
			this._lightsStack.Remove(globalLight);
			this._lightsStack[^1].enabled = true;
		}
		private IImagePool PrivateCreateImageRenderer<Components>(Components components) where Components : class, IImageComponents
		{
			MonoBehaviour rendererObject = components as MonoBehaviour;
			ImageRenderer image = Instantiate(this._imageRenderer);
			image.transform.SetParent(rendererObject.transform, false);
			image.transform.localPosition = components.ImageOffset;
			SpriteRenderer spriteRenderer = image.GetComponent<SpriteRenderer>();
			spriteRenderer.sprite = components.Image;
			return image;
		}
		public static void Stun(IDamageable damageable, Vector2 stunDirection, float stunStrength, float stopTime, float slowTime) =>
			_instance.PrivateStun(damageable, stunDirection, stunStrength, stopTime, slowTime);
		public static void OnGlobalLight(Light2DBase globalLight) => _instance.PrivateOnGlobalLight(globalLight);
		public static void OffGlobalLight(Light2DBase globalLight) => _instance.PrivateOffGlobalLight(globalLight);
		public static IImagePool CreateImageRenderer<Components>(Components components) where Components : class, IImageComponents =>
			_instance.PrivateCreateImageRenderer(components);
	};
};
