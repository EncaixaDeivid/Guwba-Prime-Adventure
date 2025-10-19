using UnityEngine;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Character;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(CircleCollider2D))]
	[RequireComponent(typeof(Transitioner), typeof(IInteractable))]
	internal sealed class LevelGoal : StateController
	{
		private static LevelGoal _instance;
		[Header("Scene Interactions")]
		[SerializeField, Tooltip("If this will go direct to the boss.")] private SceneField _goToBoss;
		[SerializeField, Tooltip("If theres a dialog after the goal.")] private bool _enterInDialog;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		private new void Awake()
		{
			if (_instance)
			{
				Destroy(gameObject, 1e-3f);
				return;
			}
			_instance = this;
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!GuwbaStateMarker.EqualObject(other.gameObject))
				return;
			SaveController.Load(out SaveFile saveFile);
			SettingsController.Load(out Settings settings);
			ushort sceneIndex = ushort.Parse($"{gameObject.scene.name[^1]}");
			if (!saveFile.levelsCompleted[sceneIndex - 1])
			{
				saveFile.levelsCompleted[sceneIndex - 1] = true;
				SaveController.WriteSave(saveFile);
			}
			if (_saveOnSpecifics && !saveFile.generalObjects.Contains(gameObject.name))
			{
				saveFile.generalObjects.Add(gameObject.name);
				SaveController.WriteSave(saveFile);
			}
			if (_enterInDialog && settings.DialogToggle)
				GetComponent<IInteractable>().Interaction();
			else if (sceneIndex - 1 >= 0f && !saveFile.deafetedBosses[sceneIndex - 1])
				GetComponent<Transitioner>().Transicion(_goToBoss);
			else
				GetComponent<Transitioner>().Transicion();
		}
	};
};
