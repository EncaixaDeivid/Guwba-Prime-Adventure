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
		private Collider2D[] _colliders;
		private Light2DBase _selfLight;
		[SerializeField] private Light2DBase _followLight;
		[SerializeField] private bool _isReceptor;
		[SerializeField] private bool _fadeActivation;
		[SerializeField] private bool _hasColliders;
		[SerializeField] private bool _hasFollowLight;
		[SerializeField] private float _timeToFadeAppearAgain;
		private new void Awake()
		{
			base.Awake();
			this._tilemap = this.GetComponentInParent<Tilemap>();
			this._colliders = this.GetComponentsInParent<Collider2D>(true);
			this._selfLight = this.GetComponent<Light2DBase>();
		}
		private IEnumerator Fade(bool appear)
		{
			if (appear)
			{
				this._selfLight.enabled = false;
				EffectsController.OnOffGlobalLight(true);
			}
			else
			{
				EffectsController.OnOffGlobalLight(false);
				this._selfLight.enabled = true;
			}
			if (this._hasFollowLight && !appear)
				this.StartCoroutine(FollowLight());
			IEnumerator FollowLight()
			{
				while (!appear)
				{
					this._followLight.transform.position = GuwbaAstral<CommandGuwba>.Position;
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => this.enabled);
				}
			}
			if (appear)
				for (float i = 0f; this._tilemap.color.a < 1f; i += 0.1f)
					yield return OpacityLevel(i);
			else
				for (float i = 1f; this._tilemap.color.a > 0f; i -= 0.1f)
					yield return OpacityLevel(i);
			IEnumerator OpacityLevel(float alpha)
			{
				yield return new WaitForEndOfFrame();
				yield return new WaitUntil(() => this.enabled);
				this._tilemap.color = new Color(this._tilemap.color.r, this._tilemap.color.g, this._tilemap.color.b, alpha);
			}
			if (this._hasColliders)
				foreach (Collider2D collider in this._colliders)
					collider.enabled = appear;
		}
		public void ActivationEvent()
		{
			if (this._timeToFadeAppearAgain > 0f)
				this.StartCoroutine(FadeTimed(!this._fadeActivation));
			else
				this.StartCoroutine(this.Fade(!this._fadeActivation));
			IEnumerator FadeTimed(bool appear)
			{
				yield return this.Fade(appear);
				yield return new WaitTime(this, this._timeToFadeAppearAgain);
				this.StartCoroutine(this.Fade(!appear));
			}
		}
		public void DesactivationEvent()
		{
			if (this._fadeActivation)
				this.StartCoroutine(this.Fade(true));
			else
				this.StartCoroutine(this.Fade(false));
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!this._isReceptor && GuwbaAstral<CommandGuwba>.EqualObject(other.gameObject))
				this.StartCoroutine(this.Fade(false));
		}
		private void OnTriggerExit2D(Collider2D other)
		{
			if (!this._isReceptor && GuwbaAstral<CommandGuwba>.EqualObject(other.gameObject))
				this.StartCoroutine(this.Fade(true));
		}
	};
};
