using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Collider2D))]
	internal sealed class InteractiveActivator : Activator, IInteractable
	{
		public void Interaction()
		{
			if (this.Usable)
			{
				this.Activation();
				if (!this.Usable)
				{
					Sender sender = Sender.Create();
					sender.SetStateForm(StateForm.State);
					sender.SetToggle(false);
					sender.SetAdditionalData(this.gameObject);
					sender.Send(PathConnection.Hud);
				}
			}
		}
	};
};
