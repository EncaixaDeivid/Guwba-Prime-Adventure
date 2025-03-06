using UnityEngine;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(CircleCollider2D), typeof(TransitionController))]
	internal sealed class LevelGoal : StateController
	{
		[SerializeField] private string _goToBoss;
		[SerializeField] private bool _saveOnSpecifics;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!GuwbaTransformer<CommandGuwba>.EqualObject(other.gameObject))
				return;
			short sceneIndex = short.Parse($"{this.gameObject.scene.name[^1]}");
			if (!SaveFileData.LevelsCompleted[sceneIndex])
				SaveFileData.LevelsCompleted[sceneIndex] = true;
			if (this._saveOnSpecifics)
				SaveFileData.GeneralObjects.Add(this.gameObject.name);
			if (sceneIndex - 1 >= 0f && !SaveFileData.DeafetedBosses[sceneIndex - 1])
				this.GetComponent<TransitionController>().Transicion(this._goToBoss);
			else
				this.GetComponent<TransitionController>().Transicion();
		}
	};
};