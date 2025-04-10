using UnityEngine;
namespace GuwbaPrimeAdventure
{
	public abstract class ControllerConnector : MonoBehaviour
	{
		protected void Awake<ControllerInstance>() where ControllerInstance : ControllerConnector
		{
			ControllerConnector instance = this.GetComponent<ControllerInstance>();
			if (!instance || instance is not MenuController && instance is not ConfigurationController && instance is not DeathScreenController)
				Destroy(this.gameObject, 0.001f);
		}
		protected void Connect<Controller>() where Controller : ControllerConnector
		{
			Controller controller = this.GetComponent<Controller>();
			if (controller)
				controller.Event();
		}
		protected abstract void Event();
	};
};
