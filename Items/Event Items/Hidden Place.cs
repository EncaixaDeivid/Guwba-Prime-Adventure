using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using System.Collections;
using GwambaPrimeAdventure.Connection;
using GwambaPrimeAdventure.Character;
namespace GwambaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Tilemap), typeof(TilemapRenderer))]
	[RequireComponent(typeof(TilemapCollider2D), typeof(CompositeCollider2D), typeof(Surface)), RequireComponent(typeof(Light2DBase), typeof(Receptor))]
	internal sealed class HiddenPlace : StateController, IReceptorSignal
	{
		private Tilemap _tilemap;
		private TilemapCollider2D _collider;
		private Light2DBase _selfLight;
		private Light2DBase _followLight;
		private readonly Sender _sender = Sender.Create();
		private bool _activation = false;
		private bool _follow = false;
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
			_tilemap = GetComponent<Tilemap>();
			_collider = GetComponent<TilemapCollider2D>();
			_selfLight = GetComponent<Light2DBase>();
			_followLight = GetComponentInChildren<Light2DBase>();
			_sender.SetStateForm(StateForm.State);
			_sender.SetAdditionalData(_hiddenObject);
			_activation = !_fadeActivation;
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			EffectsController.OffGlobalLight(_selfLight);
		}
		private IEnumerator Fade(bool appear)
		{
			bool onFirst = false;
			if (_otherPlace)
				if (onFirst = _otherPlace._appearFirst && _otherPlace._activation)
					yield return StartCoroutine(_otherPlace.Fade(true));
				else if (onFirst = _otherPlace._fadeFirst && !_otherPlace._activation)
					yield return StartCoroutine(_otherPlace.Fade(false));
			if (_isReceptor)
				_activation = !_activation;
			if (appear)
				EffectsController.OffGlobalLight(_selfLight);
			else
				EffectsController.OnGlobalLight(_selfLight);
			if (_hasFollowLight)
				_follow = !appear;
			void HaveHidden()
			{
				if (_haveHidden)
				{
					_sender.SetToggle(appear);
					_sender.Send(PathConnection.System);
				}
			}
			if (_instantly)
			{
				Color color = _tilemap.color;
				color.a = appear ? 1f : 0f;
				_tilemap.color = color;
				HaveHidden();
			}
			else
			{
				if (appear)
				{
					HaveHidden();
					for (float i = 0f; _tilemap.color.a < 1f; i += 0.1f)
						yield return OpacityLevel(i);
				}
				else
				{
					HaveHidden();
					for (float i = 1f; _tilemap.color.a > 0f; i -= 0.1f)
						yield return OpacityLevel(i);
				}
			}
			IEnumerator OpacityLevel(float alpha)
			{
				yield return new WaitUntil(() => isActiveAndEnabled);
				Color color = _tilemap.color;
				color.a = alpha;
				_tilemap.color = color;
			}
			if (_haveColliders)
				_collider.enabled = appear;
			if (_otherPlace && !onFirst)
				if (!_otherPlace._appearFirst && _otherPlace._activation)
					StartCoroutine(_otherPlace.Fade(true));
				else if (!_otherPlace._fadeFirst && !_otherPlace._activation)
					StartCoroutine(_otherPlace.Fade(false));
		}
		private void FixedUpdate()
		{
			if (_follow)
				_followLight.transform.position = GwambaStateMarker.Localization;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!_isReceptor && GwambaStateMarker.EqualObject(other.gameObject))
				StartCoroutine(Fade(false));
		}
		private void OnTriggerExit2D(Collider2D other)
		{
			if (!_isReceptor && GwambaStateMarker.EqualObject(other.gameObject))
				StartCoroutine(Fade(true));
		}
		public void Execute()
		{
			if (_timeToFadeAppearAgain > 0f)
				StartCoroutine(FadeTimed(_activation));
			else
				StartCoroutine(Fade(_activation));
			IEnumerator FadeTimed(bool appear)
			{
				yield return Fade(appear);
				yield return new WaitTime(this, _timeToFadeAppearAgain);
				StartCoroutine(Fade(!appear));
			}
		}
	};
};
