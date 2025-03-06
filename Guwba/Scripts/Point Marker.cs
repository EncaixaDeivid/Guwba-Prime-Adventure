using UnityEngine;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	internal sealed class PointMarker : StateController
	{
		private static Vector2 _checkpointIndex = new();
		private static string _lastScene = "";
		private static bool _useCheckpoint = true, _checkScene = true;
		[SerializeField] private string _levelSelectorScene;
		[SerializeField] private ushort _selfIndex;
		private void Start()
		{
			if (this.gameObject.scene.name == this._levelSelectorScene && SaveFileData.LastLevelEntered != "")
				if (ushort.Parse($"{SaveFileData.LastLevelEntered[^1]}") == this._selfIndex)
					GuwbaTransformer<CommandGuwba>.Position = this.transform.position;
			if (_lastScene == this.gameObject.scene.name && _useCheckpoint)
			{
				_useCheckpoint = false;
				GuwbaTransformer<CommandGuwba>.Position = _checkpointIndex;
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
			if (GuwbaTransformer<CommandGuwba>.EqualObject(other.gameObject))
				_checkpointIndex = this.transform.position;
		}
	};
};