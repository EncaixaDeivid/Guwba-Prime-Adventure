using UnityEngine;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(CircleCollider2D))]
	[RequireComponent(typeof(IInteractable))]
	internal sealed class QuestionPuller : StateController
	{
		private SpriteRenderer _spriteRenderer;
		private new void Awake()
		{
			base.Awake();
			(this._spriteRenderer = this.GetComponent<SpriteRenderer>()).enabled = false;
		}
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (GuwbaAstral<CommandGuwba>.EqualObject(collision.gameObject))
				this._spriteRenderer.enabled = true;
		}
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (GuwbaAstral<CommandGuwba>.EqualObject(collision.gameObject))
				this._spriteRenderer.enabled = false;
		}
	};
};
