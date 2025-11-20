using UnityEngine;
using System.Collections;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	public sealed class Surface : MonoBehaviour, ILoader
	{
		[SerializeField] private bool _isScene;
		public bool IsScene => _isScene;
		public IEnumerator Load() => null;
	};
};
