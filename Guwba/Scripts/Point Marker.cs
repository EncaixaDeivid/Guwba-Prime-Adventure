using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	internal sealed class PointMarker : StateController
	{
		private static Vector2 _checkpointIndex = new();
		private static string _lastScene = "";
		private static bool _useCheckpoint = true;
		private static bool _checkScene = true;
		[SerializeField, Tooltip("The name of the hubby world scene.")] private string _levelSelectorScene;
		[SerializeField, Tooltip("Which point is checked when scene is the level selector.")] private ushort _selfIndex;
		private void Start()
		{
			SaveController.Load(out SaveFile saveFile);
			if (this.gameObject.scene.name == this._levelSelectorScene && saveFile.lastLevelEntered != "")
				if (ushort.Parse($"{saveFile.lastLevelEntered[^1]}") == this._selfIndex)
					GuwbaAstral<CommandGuwba>.Position = this.transform.position;
			if (_lastScene == this.gameObject.scene.name && _useCheckpoint)
			{
				_useCheckpoint = false;
				GuwbaAstral<CommandGuwba>.Position = _checkpointIndex;
			}
			_checkScene = true;
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			_useCheckpoint = true;
			if (_checkScene)
			{
				_checkScene = false;
				_lastScene = this.gameObject.scene.name;
			}
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (GuwbaAstral<CommandGuwba>.EqualObject(other.gameObject))
				_checkpointIndex = this.transform.position;
		}
	};
};
