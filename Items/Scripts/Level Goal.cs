using UnityEngine;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	[RequireComponent(typeof(CircleCollider2D), typeof(Transitioner), typeof(IInteractable))]
	internal sealed class LevelGoal : StateController
	{
		[SerializeField] private string _goToBoss;
		[SerializeField] private bool _enterInDialog, _saveOnSpecifics;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!GuwbaTransformer<CommandGuwba>.EqualObject(other.gameObject))
				return;
			SaveController.Load(out SaveFile saveFile);
			SettingsController.Load(out Settings settings);
			ushort sceneIndex = ushort.Parse($"{this.gameObject.scene.name[^1]}");
			if (!saveFile.levelsCompleted[sceneIndex - 1])
			{
				saveFile.levelsCompleted[sceneIndex - 1] = true;
				SaveController.WriteSave(saveFile);
			}
			if (this._saveOnSpecifics && !saveFile.generalObjects.Contains(this.gameObject.name))
			{
				saveFile.generalObjects.Add(this.gameObject.name);
				SaveController.WriteSave(saveFile);
			}
			if (this._enterInDialog && settings.dialogToggle)
				this.GetComponent<IInteractable>().Interaction();
			else if (sceneIndex - 1 >= 0f && !saveFile.deafetedBosses[sceneIndex - 1])
				this.GetComponent<Transitioner>().Transicion(this._goToBoss);
			else
				this.GetComponent<Transitioner>().Transicion();
		}
	};
};
