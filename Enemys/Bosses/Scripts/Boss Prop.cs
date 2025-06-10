using UnityEngine;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[RequireComponent(typeof(Transform), typeof(Collider2D))]
	internal abstract class BossProp : StateController
	{
		protected Collider2D _collider;
		protected bool _useDestructuion = false;
		[Header("Boss Prop")]
		[SerializeField, Tooltip("The layer mask to identify the ground.")] protected LayerMask _groundLayer;
		[SerializeField, Tooltip("The layer mask to identify the target of the attacks.")] protected LayerMask _targetLayerMask;
		[Header("Prop Events")]
		[SerializeField, Tooltip("If this prop will destroy the mais boss after use.")] private bool _destructBoss;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		[SerializeField, Tooltip("If this prop will make a reaction at an index.")] protected bool _indexReact;
		[SerializeField, Tooltip("The index to a event to a boss make.")] protected ushort _indexEvent;
		protected new void Awake()
		{
			base.Awake();
			this._collider = this.GetComponent<Collider2D>();
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!this._useDestructuion)
				return;
			SaveController.Load(out SaveFile saveFile);
			if (this._saveOnSpecifics && !saveFile.generalObjects.Contains(this.gameObject.name))
				saveFile.generalObjects.Add(this.gameObject.name);
			if (this._destructBoss)
			{
				Sender sender = Sender.Create();
				sender.SetToWhereConnection(PathConnection.Boss).SetAdditionalData(BossType.Controller);
				sender.SetConnectionState(ConnectionState.Disable).Send();
			}
		}
	};
};
