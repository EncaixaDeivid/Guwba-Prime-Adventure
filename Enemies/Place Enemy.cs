using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D)), RequireComponent(typeof(CompositeCollider2D))]
	internal sealed class PlaceEnemy : EnemyProvider, IConnector
	{
		private Tilemap _tilemap;
		private TilemapCollider2D _tilemapCollider;
		[Header("Interactions")]
		[SerializeField, Tooltip("If anything can be hurt.")] private bool _hurtEveryone;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _reactToDamage;
		private new void Awake()
		{
			base.Awake();
			_tilemap = GetComponent<Tilemap>();
			_tilemapCollider = GetComponent<TilemapCollider2D>();
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			IEnumerator AppearFade(bool appear)
			{
				Color color;
				IEnumerator Opacity(float alpha)
				{
					color = _tilemap.color;
					color.a = alpha;
					_tilemap.color = color;
					yield return new WaitForEndOfFrame();
					yield return new WaitUntil(() => isActiveAndEnabled && !IsStunned);
				}
				if (appear)
					for (float i = 0f; _tilemap.color.a < 1f; i += 0.1f)
						yield return Opacity(i);
				else
					for (float i = 1f; _tilemap.color.a > 0f; i -= 0.1f)
						yield return Opacity(i);
				_tilemapCollider.enabled = appear;
			}
			if ((EnemyProvider[])additionalData != null)
				foreach (EnemyProvider enemy in (EnemyProvider[])additionalData)
					if (enemy != this)
						return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				StartCoroutine(AppearFade(data.ToggleValue.Value));
			else if (data.StateForm == StateForm.Action && _reactToDamage)
				StartCoroutine(AppearFade(_tilemap.color.a <= 0f));
		}
	};
};
