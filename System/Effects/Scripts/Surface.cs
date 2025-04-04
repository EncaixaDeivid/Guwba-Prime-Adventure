using UnityEngine;
namespace GuwbaPrimeAdventure.Effects
{
	public sealed class Surface : StateController
	{
		[SerializeField] private bool _isScene;
		public bool IsScene => this._isScene;
	};
};
