using UnityEngine;
namespace GwambaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D))]
    internal sealed class OutOfBounds : MonoBehaviour
    {
		private void OnTriggerEnter2D(Collider2D collision) => Destroy(collision.gameObject);
	};
};