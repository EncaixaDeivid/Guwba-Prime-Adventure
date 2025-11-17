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
		private IEnumerator _appearFade;
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
			if (_appearFade is not null)
				_appearFade = null;
			Sender.Exclude(this);
		}
		private void Update() => _appearFade?.MoveNext();
		public void Receive(DataConnection data, object additionalData)
		{
			if (additionalData != null || additionalData is EnemyProvider[] || additionalData as EnemyProvider[] != null || (additionalData as EnemyProvider[]).Length > 0)
				foreach (EnemyProvider enemy in additionalData as EnemyProvider[])
					if (enemy == this)
					{
						if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
							_appearFade = AppearFade(data.ToggleValue.Value);
						else if (data.StateForm == StateForm.Event && _reactToDamage)
							_appearFade = AppearFade(_tilemap.color.a <= 0f);
						IEnumerator AppearFade(bool appear)
						{
							Color color;
							IEnumerator Opacity(float alpha)
							{
								color = _tilemap.color;
								color.a = alpha;
								_tilemap.color = color;
								yield return null;
							}
							if (appear)
								for (float i = 0f; _tilemap.color.a < 1f; i += 0.1f)
								{
									yield return Opacity(i);
									yield return new WaitWhile(() => IsStunned);
								}
							else
								for (float i = 1f; _tilemap.color.a > 0f; i -= 0.1f)
								{
									yield return Opacity(i);
									yield return new WaitWhile(() => IsStunned);
								}
							_tilemapCollider.enabled = appear;
						}
						return;
					}
		}
	};
};
