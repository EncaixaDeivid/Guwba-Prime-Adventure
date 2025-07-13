using UnityEngine;
namespace GuwbaPrimeAdventure
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	internal sealed class HiddenCamera : MonoBehaviour
	{
		private void Awake()
		{
			for (ushort i = 0; i < this.transform.childCount; i++)
				this.transform.GetChild(i).gameObject.SetActive(false);
		}
	};
};