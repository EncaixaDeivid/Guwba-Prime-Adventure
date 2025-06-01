using UnityEngine;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal abstract class BossProp : StateController
	{
		protected Collider2D _collider;
		private SaveFile _saveFile;
		protected bool _useDestructuion = false;
		[Header("Boss Prop"), SerializeField] protected LayerMask _groundLayer;
		[SerializeField] protected LayerMask _targetLayerMask;
		[SerializeField] private bool _destructBoss;
		[SerializeField] private bool _saveOnSpecifics;
		[SerializeField] protected bool _indexReact;
		[SerializeField] protected ushort _indexEvent;
		private new void Awake()
		{
			base.Awake();
			this._collider = this.GetComponent<Collider2D>();
			SaveController.Load(out this._saveFile);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!this._useDestructuion)
				return;
			if (this._saveOnSpecifics && !this._saveFile.generalObjects.Contains(this.gameObject.name))
				this._saveFile.generalObjects.Add(this.gameObject.name);
			if (this._destructBoss)
				Sender.Create().SetFromConnection(PathConnection.Boss).SetToWhereConnection(PathConnection.Boss).SetBossType(BossType.All)
					.SetConnectionState(ConnectionState.Disable).SetToggle(true).Send();
		}
	};
};
