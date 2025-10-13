using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Tilemap), typeof(TilemapRenderer), typeof(CompositeCollider2D))]
	internal sealed class PlaceEnemy : EnemyProvider, IConnector
	{
		private Tilemap _tilemap;
		private TilemapCollider2D _tilemapCollider2D;
		[Header("Interactions")]
		[SerializeField, Tooltip("If anything can be hurt.")] private bool _hurtEveryone;
		[SerializeField, Tooltip("If this enemy will react to any damage taken.")] private bool _reactToDamage;
		private new void Awake()
		{
			base.Awake();
			this._tilemap = this.GetComponent<Tilemap>();
			this._tilemapCollider2D = this.GetComponent<TilemapCollider2D>();
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
					color = this._tilemap.color;
					color.a = alpha;
					this._tilemap.color = color;
					yield return new WaitForEndOfFrame();
					yield return new WaitUntil(() => this.isActiveAndEnabled && !this._rigidybody.IsSleeping());
				}
				if (appear)
					for (float i = 0f; this._tilemap.color.a < 1f; i += 0.1f)
						yield return Opacity(i);
				else
					for (float i = 1f; this._tilemap.color.a > 0f; i -= 0.1f)
						yield return Opacity(i);
				this._tilemapCollider2D.enabled = appear;
			}
			if ((EnemyProvider[])additionalData != null)
				foreach (EnemyProvider enemy in (EnemyProvider[])additionalData)
					if (enemy != this)
						return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				this.StartCoroutine(AppearFade(data.ToggleValue.Value));
			else if (data.StateForm == StateForm.Action && this._reactToDamage)
				this.StartCoroutine(AppearFade(this._tilemap.color.a <= 0f));
		}
	};
};
