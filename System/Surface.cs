using UnityEngine;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
	public sealed class Surface : MonoBehaviour
	{
		[SerializeField] private bool _isScene;
		public bool IsScene => _isScene;
	};
};
