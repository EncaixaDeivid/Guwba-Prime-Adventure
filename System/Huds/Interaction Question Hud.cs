using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class InteractionQuestionHud : MonoBehaviour
	{
		[Header("Element")]
		[SerializeField, Tooltip("User interface element.")] private string _rootElement;
		[Header("Stats")]
		[SerializeField, Tooltip("The size of the element.")] private float _size;
		internal VisualElement RootVisualElement { get; private set; }
		private void Awake()
		{
			this.RootVisualElement = this.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>(this._rootElement);
			this.RootVisualElement.style.width = this._size;
			this.RootVisualElement.style.height = this._size * 2f;
		}
	};
};