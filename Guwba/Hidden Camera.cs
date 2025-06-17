using UnityEngine;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	internal sealed class HiddenCamera : StateController
	{
		private new void Awake()
		{
			base.Awake();
			for (ushort i = 0; i < this.transform.childCount; i++)
				this.transform.GetChild(i).gameObject.SetActive(false);
		}
	};
};