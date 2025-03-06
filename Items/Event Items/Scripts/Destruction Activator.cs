using UnityEngine;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent]
	internal sealed class DestructionActivator : Activator
	{
		[SerializeField] private bool _activate;
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!this._activate)
				return;
			this.Activation();
		}
	};
};