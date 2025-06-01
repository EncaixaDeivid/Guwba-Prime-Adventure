using UnityEngine;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent]
	internal sealed class DestructionActivator : Activator
	{
		[SerializeField, Tooltip("If this activator will be turned on after the destruction.")] private bool _activate;
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!this._activate)
				return;
			this.Activation();
		}
	};
};
