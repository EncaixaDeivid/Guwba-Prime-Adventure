using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent, RequireComponent(typeof(Tilemap), typeof(TilemapRenderer))]
	internal sealed class PlaceBoss : BossController, IConnector
	{
		private Tilemap _tilemap;
		private TilemapCollider2D _tilemapCollider2D;
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
				}
				if (appear)
					for (float i = 0f; this._tilemap.color.a < 1f; i += 0.1f)
						yield return Opacity(i);
				else
					for (float i = this._tilemap.color.a; this._tilemap.color.a > 0f; i -= 0.1f)
						yield return Opacity(i);
				this._tilemapCollider2D.enabled = appear;
			}
			BossController[] bosses = (BossController[])additionalData;
			foreach (BossController boss in bosses)
				if (boss == this)
				{
					if (data.StateForm == StateForm.State && data.ToggleValue.HasValue)
						this.StartCoroutine(AppearFade(data.ToggleValue.Value));
					else if (this._reactToDamage && data.StateForm == StateForm.Action)
						this.StartCoroutine(AppearFade(this._tilemap.color.a <= 0f));
					break;
				}
		}
	}
};
