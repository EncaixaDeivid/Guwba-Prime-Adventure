using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Tilemap), typeof(TilemapRenderer))]
	internal sealed class PlaceEnemy : EnemyController, IConnector
	{
		private Tilemap _tilemap;
		private TilemapCollider2D _tilemapCollider2D;
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
		public new void Receive(DataConnection data, object additionalData)
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
					yield return new WaitUntil(() => this.isActiveAndEnabled);
				}
				if (appear)
					for (float i = 0f; this._tilemap.color.a < 1f; i += 0.1f)
						yield return Opacity(i);
				else
					for (float i = this._tilemap.color.a; this._tilemap.color.a > 0f; i -= 0.1f)
						yield return Opacity(i);
				this._tilemapCollider2D.enabled = appear;
			}
			EnemyController[] enemies = (EnemyController[])additionalData;
			if (enemies != null)
				foreach (EnemyController enemy in enemies)
					if (enemy != this)
						return;
			if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
				this.StartCoroutine(AppearFade(data.ToggleValue.Value));
			else if (this._reactToDamage && data.StateForm == StateForm.Action)
				this.StartCoroutine(AppearFade(this._tilemap.color.a <= 0f));
		}
	}
};
