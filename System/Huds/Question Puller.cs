using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D), typeof(IInteractable))]
	internal sealed class QuestionPuller : StateController
	{
		private InteractionQuestionHud _interactionHud;
		[Header("Interaction Stats")]
		[SerializeField, Tooltip("The main camera that is rendering in the scene.")] private Camera _mainCamera;
		[SerializeField, Tooltip("The object that handles the hud of the interactions.")] private InteractionQuestionHud _interactionHudObject;
		[SerializeField, Tooltip("The position the hud will be placed.")] private float _pixelHeigthOffset;
		private new void Awake()
		{
			base.Awake();
			this._interactionHud = Instantiate(this._interactionHudObject, this.transform);
		}
		private void Update()
		{
			if (this._interactionHud.RootVisualElement.style.display == DisplayStyle.None)
				return;
			Vector2 screenPosition = this._mainCamera.WorldToScreenPoint(this.transform.position);
			this._interactionHud.RootVisualElement.style.left = screenPosition.x - this._interactionHud.RootVisualElement.layout.width / 2f;
			this._interactionHud.RootVisualElement.style.top = this._mainCamera.scaledPixelHeight - screenPosition.y - this._pixelHeigthOffset;
		}
		private void OnTriggerEnter2D(Collider2D collision) => this._interactionHud.RootVisualElement.style.display = DisplayStyle.Flex;
		private void OnTriggerExit2D(Collider2D collision) => this._interactionHud.RootVisualElement.style.display = DisplayStyle.None;
	};
};