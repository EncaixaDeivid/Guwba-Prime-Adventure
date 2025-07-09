using UnityEngine;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent]
	internal sealed class InteractiveActivator : Activator, IInteractable
	{
		public void Interaction()
		{
			if (this.Usable)
				this.Activation();
		}
	};
};
