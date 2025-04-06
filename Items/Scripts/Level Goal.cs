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
			ushort sceneIndex = ushort.Parse($"{this.gameObject.scene.name[^1]}");
			if (!SaveController.LevelsCompleted[sceneIndex - 1])
				SaveController.LevelsCompleted[sceneIndex - 1] = true;
			if (this._saveOnSpecifics && !SaveController.GeneralObjects.Contains(this.gameObject.name))
				SaveController.GeneralObjects.Add(this.gameObject.name);
			if (this._enterInDialog && SettingsController.DialogToggle)
				this.GetComponent<IInteractable>().Interaction();
			else if (sceneIndex - 1 >= 0f && !SaveController.DeafetedBosses[sceneIndex - 1])
				this.GetComponent<TransitionController>().Transicion(this._goToBoss);
			else
				this.GetComponent<TransitionController>().Transicion();
		}
	};
};
