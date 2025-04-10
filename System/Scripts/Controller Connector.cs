using UnityEngine;
namespace GuwbaPrimeAdventure
{
	internal abstract class ControllerConnector : MonoBehaviour
	{
		protected void Connect<Controller>() where Controller : ControllerConnector
		{
			Controller controller = this.GetComponent<Controller>();
			if (controller)
				controller.Event();
		}
		protected abstract void Event();
	};
};