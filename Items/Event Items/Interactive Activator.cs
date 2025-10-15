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
			if (Usable)
			{
				Activation();
				if (!Usable)
				{
					_sender.SetStateForm(StateForm.State);
					_sender.SetToggle(false);
					_sender.SetAdditionalData(gameObject);
					_sender.Send(PathConnection.Hud);
				}
			}
		}
	};
};
