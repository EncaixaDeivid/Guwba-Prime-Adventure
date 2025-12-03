using UnityEngine;
using UnityEngine.Tilemaps;
using System;
namespace GwambaPrimeAdventure
{
	[CreateAssetMenu(fileName = "Surface Sound", menuName = "Surface/Sound", order = 0)]
	internal class SurfaceSound : ScriptableObject
   {
		[field: SerializeField] internal TilesSound[] TilesSounds { get; private set; }
   };
	[Serializable]
	internal struct TilesSound
	{
		[field: SerializeField] internal Tile[] Tiles { get; private set; }
		[field: SerializeField] internal AudioSource Source { get; private set; }
	};
};