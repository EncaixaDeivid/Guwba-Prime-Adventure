using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Linq;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Tilemap), typeof(TilemapRenderer))]
	[RequireComponent(typeof(TilemapCollider2D))]
	public sealed class Surface : MonoBehaviour, ILoader
	{
		private Tilemap _tilemap;
		private void Awake() => _tilemap = GetComponent<Tilemap>();
		public IEnumerator Load() { yield return null; }
		internal bool CheckForTile(Tile[] tiles, Vector2 originPosition) => tiles.Contains(_tilemap.GetTile(_tilemap.WorldToCell(originPosition)));
	};
};
