using UnityEngine;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(CircleCollider2D), typeof(TransitionController), typeof(IInteractable))]
	internal sealed class LevelGoal : StateController
	{
		[SerializeField] private string _goToBoss;
		[SerializeField] private bool _enterInDialog, _saveOnSpecifics;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!GuwbaTransformer<CommandGuwba>.EqualObject(other.gameObject))
				return;
			short sceneIndex = short.Parse($"{this.gameObject.scene.name[^1]}");
			if (!DataFile.LevelsCompleted[sceneIndex])
				DataFile.LevelsCompleted[sceneIndex] = true;
			if (this._saveOnSpecifics)
				DataFile.GeneralObjects.Add(this.gameObject.name);
			if (sceneIndex - 1 >= 0f && !DataFile.DeafetedBosses[sceneIndex - 1])
				if (this._enterInDialog)
					this.GetComponent<IInteractable>().Interaction();
				else
					this.GetComponent<TransitionController>().Transicion(this._goToBoss);
			else
				this.GetComponent<TransitionController>().Transicion();
		}
	};
};
