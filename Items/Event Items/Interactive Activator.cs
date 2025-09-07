using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent]
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
					sender.SetToWhereConnection(PathConnection.Item);
					sender.SetStateForm(StateForm.Action);
					sender.SetToggle(false);
					sender.SetAdditionalData(this.gameObject);
					sender.Send();
				}
			}
		}
	};
};
