using UnityEngine;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(CircleCollider2D))]
	internal sealed class QuestionPuller : StateController, IConnector
	{
		private SpriteRenderer _spriteRenderer;
		public PathConnection PathConnection => PathConnection.Item;
		private new void Awake()
		{
			base.Awake();
			(this._spriteRenderer = this.GetComponent<SpriteRenderer>()).enabled = false;
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
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
		public void Receive(DataConnection data, object additionalData)
		{
			Transform parentTransform = (Transform)additionalData;
			if (parentTransform && parentTransform == this.transform.parent && data.StateForm == StateForm.Action && data.ToggleValue.HasValue)
				this._spriteRenderer.enabled = data.ToggleValue.Value;
		}
	};
};
