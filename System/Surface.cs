using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Tilemap), typeof(TilemapRenderer))]
	[RequireComponent(typeof(TilemapCollider2D))]
	public sealed class Surface : MonoBehaviour, ILoader
	{
		private Tilemap _tilemap;
		private void Awake() => _tilemap = GetComponent<Tilemap>();
		public IEnumerator Load() { yield return null; }
		internal Tile CheckForTile(Vector2 originPosition) => _tilemap.GetTile<Tile>(_tilemap.WorldToCell(originPosition));
	};
};
