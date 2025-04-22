using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using System.Collections;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(PolygonCollider2D), typeof(Receptor))]
	internal sealed class HidenPlace : StateController, Receptor.IReceptor
	{
		private Tilemap _tilemap;
		private Collider2D[] _colliders;
		private Light2DBase _selfLight;
		private bool _appearCompleted = false;
		private bool _fadeCompleted = false;
		[SerializeField] private Light2DBase _followLight;
		[SerializeField] private bool _isReceptor;
		[SerializeField] private bool _fadeActivation;
		[SerializeField] private bool _hasColliders;
		[SerializeField] private bool _hasFollowLight;
		[SerializeField] private float _timeToFadeAgain;
		[SerializeField] private float _timeToAppearAgain;
		private new void Awake()
		{
			base.Awake();
			this._tilemap = this.GetComponentInParent<Tilemap>();
			this._colliders = this.GetComponentsInParent<Collider2D>(true);
			this._selfLight = this.GetComponent<Light2DBase>();
		}
		private IEnumerator Appear()
		{
			for (float i = 0f; i < 1f; i += 0.1f)
			{
				yield return new WaitForEndOfFrame();
				yield return new WaitUntil(() => this.enabled);
				this._tilemap.color = new Color(this._tilemap.color.r, this._tilemap.color.g, this._tilemap.color.b, i);
			}
			this._tilemap.color = new Color(this._tilemap.color.r, this._tilemap.color.g, this._tilemap.color.b, 1f);
			if (this._hasColliders)
				foreach (Collider2D collider in this._colliders)
					collider.enabled = true;
			this._appearCompleted = true;
		}
		private IEnumerator Fade()
		{
			for (float i = 1f; i > 0f; i -= 0.1f)
			{
				yield return new WaitForEndOfFrame();
				yield return new WaitUntil(() => this.enabled);
				this._tilemap.color = new Color(this._tilemap.color.r, this._tilemap.color.g, this._tilemap.color.b, i);
			}
			this._tilemap.color = new Color(this._tilemap.color.r, this._tilemap.color.g, this._tilemap.color.b, 0f);
			if (this._hasColliders)
				foreach (Collider2D collider in this._colliders)
					collider.enabled = false;
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
					this._selfLight.enabled = true;
					this._followLight.enabled = true;
					this.StartCoroutine(Light());
					IEnumerator Light()
					{
						while (this._followLight.enabled)
						{
							this._followLight.transform.position = GuwbaTransformer<CommandGuwba>.Position;
							yield return new WaitForFixedUpdate();
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
				}
			}
		}
	};
};
