using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Collider2D))]
	internal sealed class InteractiveActivator : Activator, IInteractable
	{
		private readonly Sender _sender = Sender.Create();
		public void Interaction()
		{
			if (this.Usable)
			{
				this.Activation();
				if (!this.Usable)
				{
					this._sender.SetStateForm(StateForm.State);
					this._sender.SetToggle(false);
					this._sender.SetAdditionalData(this.gameObject);
					this._sender.Send(PathConnection.Hud);
				}
			}
		}
	};
};
