using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	public sealed class TransicionHud : MonoBehaviour
	{
		private static TransicionHud _instance;
		private GroupBox _baseElement;
		[SerializeField] private string _baseElementGroup;
		public GroupBox BaseElement => this._baseElement;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			this._baseElement = this.GetComponent<UIDocument>().rootVisualElement.Q<GroupBox>(this._baseElementGroup);
		}
	};
};
