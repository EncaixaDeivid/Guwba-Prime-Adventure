using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Tilemap), typeof(TilemapRenderer))]
	[RequireComponent(typeof(TilemapCollider2D), typeof(CompositeCollider2D), typeof(Surface))]
	[RequireComponent(typeof(Light2DBase), typeof(Receptor))]
	internal sealed class HiddenPlace : StateController, Receptor.IReceptorSignal
	{
		private Tilemap _tilemap;
		private TilemapCollider2D _collider;
		private Light2DBase _selfLight;
		private Light2DBase _followLight;
		private readonly Sender _sender = Sender.Create();
		private bool _activation = false;
		[Header("Hidden Place")]
		[SerializeField, Tooltip("Other hidden place to activate.")] private HiddenPlace _otherPlace;
		[SerializeField, Tooltip("The hidden object to reveal.")] private HiddenObject _hiddenObject;
		[SerializeField, Tooltip("If this object will receive a signal.")] private bool _isReceptor;
		[SerializeField, Tooltip("If the other hidden place will appear first.")] private bool _appearFirst;
		[SerializeField, Tooltip("If the other hidden place will fade first.")] private bool _fadeFirst;
		[SerializeField, Tooltip("If this object will appear-fade instantly.")] private bool _instantly;
		[SerializeField, Tooltip("If the activation of the receive signal will fade the place.")] private bool _fadeActivation;
		[SerializeField, Tooltip("If the place has any inferior collider.")] private bool _haveColliders;
		[SerializeField, Tooltip("If the place has any hidden objects.")] private bool _haveHidden;
		[SerializeField, Tooltip("If theres a follow light.")] private bool _hasFollowLight;
		[SerializeField, Tooltip("The amount o time to fade/appear again after the activation.")] private float _timeToFadeAppearAgain;
		private new void Awake()
		{
			base.Awake();
			this._tilemap = this.GetComponent<Tilemap>();
			this._collider = this.GetComponent<TilemapCollider2D>();
			this._selfLight = this.GetComponent<Light2DBase>();
			this._followLight = this.GetComponentInChildren<Light2DBase>();
			this._sender.SetStateForm(StateForm.State);
			this._sender.SetAdditionalData(this._hiddenObject);
			this._activation = !this._fadeActivation;
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			EffectsController.OffGlobalLight(this._selfLight);
		}
		private IEnumerator Fade(bool appear)
		{
			bool onFirst = false;
			if (this._otherPlace)
				if (onFirst = this._otherPlace._appearFirst && this._otherPlace._activation)
					yield return this.StartCoroutine(this._otherPlace.Fade(true));
				else if (onFirst = this._otherPlace._fadeFirst && !this._otherPlace._activation)
					yield return this.StartCoroutine(this._otherPlace.Fade(false));
			if (this._isReceptor)
				this._activation = !this._activation;
			if (appear)
				EffectsController.OffGlobalLight(this._selfLight);
			else
				EffectsController.OnGlobalLight(this._selfLight);
			if (this._hasFollowLight && !appear)
				this.StartCoroutine(FollowLight());
			IEnumerator FollowLight()
			{
				while (!appear)
				{
					this._followLight.transform.position = GuwbaCentralizer.Position;
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => this.isActiveAndEnabled);
				}
			}
			void HaveHidden()
			{
				if (this._haveHidden)
				{
					this._sender.SetToggle(appear);
					this._sender.Send(PathConnection.System);
				}
			}
			if (this._instantly)
			{
				Color color = this._tilemap.color;
				color.a = appear ? 1f : 0f;
				this._tilemap.color = color;
				HaveHidden();
			}
			else
			{
				if (appear)
				{
					HaveHidden();
					for (float i = 0f; this._tilemap.color.a < 1f; i += 0.1f)
						yield return OpacityLevel(i);
				}
				else
				{
					HaveHidden();
					for (float i = 1f; this._tilemap.color.a > 0f; i -= 0.1f)
						yield return OpacityLevel(i);
				}
			}
			IEnumerator OpacityLevel(float alpha)
			{
				yield return new WaitForEndOfFrame();
				yield return new WaitUntil(() => this.isActiveAndEnabled);
				Color color = this._tilemap.color;
				color.a = alpha;
				this._tilemap.color = color;
			}
			if (this._haveColliders)
				this._collider.enabled = appear;
			if (this._otherPlace && !onFirst)
				if (!this._otherPlace._appearFirst && this._otherPlace._activation)
					this.StartCoroutine(this._otherPlace.Fade(true));
				else if (!this._otherPlace._fadeFirst && !this._otherPlace._activation)
					this.StartCoroutine(this._otherPlace.Fade(false));
		}
		public void Execute()
		{
			if (this._timeToFadeAppearAgain > 0f)
				this.StartCoroutine(FadeTimed(this._activation));
			else
				this.StartCoroutine(this.Fade(this._activation));
			IEnumerator FadeTimed(bool appear)
			{
				yield return this.Fade(appear);
				yield return new WaitTime(this, this._timeToFadeAppearAgain);
				this.StartCoroutine(this.Fade(!appear));
			}
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!this._isReceptor && GuwbaCentralizer.EqualObject(other.gameObject))
				this.StartCoroutine(this.Fade(false));
		}
		private void OnTriggerExit2D(Collider2D other)
		{
			if (!this._isReceptor && GuwbaCentralizer.EqualObject(other.gameObject))
				this.StartCoroutine(this.Fade(true));
		}
	};
};
