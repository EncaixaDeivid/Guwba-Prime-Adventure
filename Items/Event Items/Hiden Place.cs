using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(PolygonCollider2D), typeof(Receptor))]
	internal sealed class HidenPlace : StateController, Receptor.IReceptor
	{
		private Tilemap _tilemap;
		private TilemapCollider2D _collider;
		private Light2DBase _selfLight;
		private readonly Sender _sender = Sender.Create();
		private bool _activation = false;
		[Header("Hidden Place")]
		[SerializeField, Tooltip("The light that will follow Guwba when he enter.")] private Light2DBase _followLight;
		[SerializeField, Tooltip("The hidden object to reveal.")] private HiddenObject _hiddenObject;
		[SerializeField, Tooltip("If this object will receive a signal.")] private bool _isReceptor;
		[SerializeField, Tooltip("If the appearance/fade will be instantly.")] private bool _instantly;
		[SerializeField, Tooltip("If the activation of the receive signal will fade the place.")] private bool _fadeActivation;
		[SerializeField, Tooltip("If the place has any inferior collider.")] private bool _haveColliders;
		[SerializeField, Tooltip("If the place has any hidden objects.")] private bool _haveHidden;
		[SerializeField, Tooltip("If theres a follow light.")] private bool _hasFollowLight;
		[SerializeField, Tooltip("The amount o time to fade/appear again after the activation.")] private float _timeToFadeAppearAgain;
		private new void Awake()
		{
			base.Awake();
			this._tilemap = this.GetComponentInParent<Tilemap>();
			this._collider = this.GetComponentInParent<TilemapCollider2D>();
			this._selfLight = this.GetComponent<Light2DBase>();
			this._sender.SetToWhereConnection(PathConnection.System);
			this._sender.SetStateForm(StateForm.Action);
			this._sender.SetAdditionalData(this._hiddenObject);
			this._activation = !this._fadeActivation;
		}
		private IEnumerator Fade(bool appear)
		{
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
					this._followLight.transform.position = CentralizableGuwba.Position;
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => this.enabled);
				}
			}
			if (this._haveHidden)
			{
				this._sender.SetToggle(appear);
				this._sender.Send();
			}
			if (this._instantly)
			{
				Color color = this._tilemap.color;
				color.a = appear ? 1f : 0f;
				this._tilemap.color = color;
			}
			else
			{
				if (appear)
					for (float i = 0f; this._tilemap.color.a < 1f; i += 0.1f)
						yield return OpacityLevel(i);
				else
					for (float i = 1f; this._tilemap.color.a > 0f; i -= 0.1f)
						yield return OpacityLevel(i);
			}
			IEnumerator OpacityLevel(float alpha)
			{
				yield return new WaitForEndOfFrame();
				yield return new WaitUntil(() => this.enabled);
				Color color = this._tilemap.color;
				color.a = alpha;
				this._tilemap.color = color;
			}
			if (this._haveColliders)
				this._collider.enabled = appear;
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
			if (!this._isReceptor && CentralizableGuwba.EqualObject(other.gameObject))
				this.StartCoroutine(this.Fade(false));
		}
		private void OnTriggerExit2D(Collider2D other)
		{
			if (!this._isReceptor && CentralizableGuwba.EqualObject(other.gameObject))
				this.StartCoroutine(this.Fade(true));
		}
	};
};
