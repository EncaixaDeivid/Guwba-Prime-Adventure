using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using System.Collections;
using GuwbaPrimeAdventure.Effects;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(PolygonCollider2D), typeof(Receptor))]
	internal sealed class HidenPlace : StateController, Receptor.IReceptor
	{
		private Tilemap _tilemap;
		private Light2DBase _selfLight;
		private bool _appearCompleted = false, _fadeCompleted = false;
		[SerializeField] private Light2DBase _followLight;
		[SerializeField] private GameObject _shadowObject;
		[SerializeField] private bool _isReceptor, _fadeActivation, _hasShadow, _hasFollowLight;
		[SerializeField] private float _timeToFadeAgain, _timeToAppearAgain;
		private new void Awake()
		{
			base.Awake();
			this._tilemap = this.GetComponentInParent<Tilemap>();
			this._selfLight = this.GetComponent<Light2DBase>();
		}
		private IEnumerator Appear()
		{
			if (this._hasShadow)
				this._shadowObject.SetActive(false);
			for (float i = 0f; i < 1f; i += 0.1f)
			{
				yield return new WaitForEndOfFrame();
				yield return new WaitUntil(() => this.enabled);
				this._tilemap.color = new Color(this._tilemap.color.r, this._tilemap.color.g, this._tilemap.color.b, i);
			}
			this._tilemap.color = new Color(this._tilemap.color.r, this._tilemap.color.g, this._tilemap.color.b, 1f);
			this._appearCompleted = true;
		}
		private IEnumerator Fade()
		{
			if (this._hasShadow)
				this._shadowObject.SetActive(true);
			for (float i = 1f; i > 0f; i -= 0.1f)
			{
				yield return new WaitForEndOfFrame();
				yield return new WaitUntil(() => this.enabled);
				this._tilemap.color = new Color(this._tilemap.color.r, this._tilemap.color.g, this._tilemap.color.b, i);
			}
			this._tilemap.color = new Color(this._tilemap.color.r, this._tilemap.color.g, this._tilemap.color.b, 0f);
			this._fadeCompleted = true;
		}
		public void ActivationEvent()
		{
			if (this._fadeActivation && this._timeToAppearAgain > 0f)
				this.StartCoroutine(FadeTimed());
			else if (this._fadeActivation)
				this.StartCoroutine(this.Fade());
			else if (this._timeToFadeAgain > 0f)
				this.StartCoroutine(AppearTimed());
			else
				this.StartCoroutine(this.Appear());
			IEnumerator AppearTimed()
			{
				this.StartCoroutine(this.Appear());
				yield return new WaitUntil(() => this._appearCompleted && this.enabled);
				yield return new WaitTime(this, this._timeToFadeAgain);
				this.StartCoroutine(this.Fade());
			}
			IEnumerator FadeTimed()
			{
				this.StartCoroutine(this.Fade());
				yield return new WaitUntil(() => this._fadeCompleted && this.enabled);
				yield return new WaitTime(this, this._timeToAppearAgain);
				this.StartCoroutine(this.Appear());
			}
		}
		public void DesactivationEvent()
		{
			if (this._fadeActivation)
				this.StartCoroutine(this.Appear());
			else
				this.StartCoroutine(this.Fade());
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!this._isReceptor && GuwbaTransformer<CommandGuwba>.EqualObject(other.gameObject))
			{
				this.StartCoroutine(this.Fade());
				if (this._hasFollowLight)
				{
					EffectsController.SetGlobalLight(false);
					this._selfLight.enabled = true;
					this._followLight.enabled = true;
					this.StartCoroutine(Light());
					IEnumerator Light()
					{
						while (this._followLight.enabled)
						{
							this._followLight.transform.position = GuwbaTransformer<CommandGuwba>.Position;
							yield return new WaitForEndOfFrame();
							yield return new WaitUntil(() => this.enabled);
						}
					}
				}
			}
		}
		private void OnTriggerExit2D(Collider2D other)
		{
			if (!this._isReceptor && GuwbaTransformer<CommandGuwba>.EqualObject(other.gameObject))
			{
				this.StartCoroutine(this.Appear());
				if (this._hasFollowLight)
				{
					this._selfLight.enabled = false;
					this._followLight.enabled = false;
					EffectsController.SetGlobalLight(true);
				}
			}
		}
	};
};