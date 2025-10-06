using UnityEngine;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(CircleCollider2D))]
	[RequireComponent(typeof(Transitioner), typeof(IInteractable))]
	internal sealed class LevelGoal : StateController
	{
		[Header("Scene Interactions")]
		[SerializeField, Tooltip("If this will go direct to the boss.")] private string _goToBoss;
		[SerializeField, Tooltip("If theres a dialog after the goal.")] private bool _enterInDialog;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!GuwbaCentralizer.EqualObject(other.gameObject))
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
			if (this._enterInDialog && settings.DialogToggle)
				this.GetComponent<IInteractable>().Interaction();
			else if (sceneIndex - 1 >= 0f && !saveFile.deafetedBosses[sceneIndex - 1])
				this.GetComponent<Transitioner>().Transicion(this._goToBoss);
			else
				this.GetComponent<Transitioner>().Transicion();
		}
	};
};
