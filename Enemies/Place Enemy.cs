using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D)), RequireComponent(typeof(CompositeCollider2D))]
	internal sealed class PlaceEnemy : EnemyProvider, IConnector
	{
		private Tilemap _tilemap;
		private TilemapCollider2D _tilemapCollider;
		private IEnumerator _appearFadeEvent;
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
			if (_appearFadeEvent is not null)
				_appearFadeEvent = null;
			Sender.Exclude(this);
		}
		private void Update() => _appearFadeEvent?.MoveNext();
		public void Receive(MessageData message)
		{
			if (message.AdditionalData != null && message.AdditionalData is EnemyProvider[] && (message.AdditionalData as EnemyProvider[]).Length > 0)
				foreach (EnemyProvider enemy in message.AdditionalData as EnemyProvider[])
					if (enemy && enemy == this)
					{
						if (message.Format == MessageFormat.State && message.ToggleValue.HasValue)
							_appearFadeEvent = AppearFade(message.ToggleValue.Value);
						else if (message.Format == MessageFormat.Event && _reactToDamage)
							_appearFadeEvent = AppearFade(_tilemap.color.a <= 0f);
						IEnumerator AppearFade(bool appear)
						{
							Color color = _tilemap.color;
							if (appear)
								for (float i = 0f; _tilemap.color.a < 1f; i += 1e-1f)
								{
									yield return new WaitUntil(() => isActiveAndEnabled && !IsStunned);
									color.a = i;
									_tilemap.color = color;
									yield return null;
								}
							else
								for (float i = 1f; _tilemap.color.a > 0f; i -= 1e-1f)
								{
									yield return new WaitUntil(() => isActiveAndEnabled && !IsStunned);
									color.a = i;
									_tilemap.color = color;
									yield return null;
								}
							_tilemapCollider.enabled = appear;
							_appearFadeEvent = null;
						}
						return;
					}
		}
	};
};
