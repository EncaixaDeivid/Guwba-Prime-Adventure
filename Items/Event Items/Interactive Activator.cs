using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent]
	internal sealed class InteractiveActivator : Activator, IInteractable
	{
		private bool _used = false;
		public void Interaction()
		{
			if (this.Usable)
				this.Activation();
			if (this.Usable && !this._used)
			{
				this._used = true;
				Sender sender = Sender.Create();
				sender.SetToWhereConnection(PathConnection.Item);
				sender.SetStateForm(StateForm.Action);
				sender.SetToggle(false);
				sender.SetAdditionalData(this.gameObject);
				sender.Send();
			}
		}
	};
};
