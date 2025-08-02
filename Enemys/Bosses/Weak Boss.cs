using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(Transitioner))]
	[RequireComponent(typeof(IInteractable))]
	internal sealed class WeakBoss : BossController, IDamageable
	{
		private bool _blockDamage = false;
		private bool _useDestructuion = false;
		[Header("Weak Boss")]
		[SerializeField, Tooltip("The vitality of the main boss.")] private short _vitality;
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private ushort _biggerDamage;
		[SerializeField, Tooltip("The amount of time of wait to deal damage in the boss after damaging it.")] private float _timeToDamage;
		[SerializeField, Tooltip("The amount of stun that this boss can resists.")] private float _stunResistance;
		[SerializeField, Tooltip("The index to a event to a boss make.")] private ushort _indexEvent;
		[SerializeField, Tooltip("If this boss has a index atribute to use.")] private bool _hasIndex;
		[SerializeField, Tooltip("If this boss will destroy the main boss after it's destruction.")] private bool _destructBoss;
		[SerializeField, Tooltip("If this boss will be saved as already existent object.")] private bool _saveOnSpecifics;
		public float StunResistance => this._stunResistance;
		private new void Awake()
		{
			base.Awake();
			this._sender.SetStateForm(StateForm.Action);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!this._useDestructuion || this._vitality > 0f)
				return;
			SaveController.Load(out SaveFile saveFile);
			if (this._saveOnSpecifics && !saveFile.generalObjects.Contains(this.gameObject.name))
				saveFile.generalObjects.Add(this.gameObject.name);
			if (this._destructBoss)
			{
				this._sender.SetStateForm(StateForm.Disable);
				this._sender.SetToggle(true);
				this._sender.Send();
			}
		}
		public bool Damage(ushort damage)
		{
			if (!this._blockDamage && damage >= this._biggerDamage)
			{
				this.StartCoroutine(Timer());
				IEnumerator Timer()
				{
					this._blockDamage = true;
					yield return new WaitTime(this, this._timeToDamage);
					this._blockDamage = false;
				}
				this._vitality -= (short)damage;
				if (this._reactToDamage)
					if (this._hasIndex)
					{
						this._sender.SetNumber(this._indexEvent);
						this._sender.Send();
					}
					else
						this._sender.Send();
				if (this._vitality <= 0)
				{
					this._useDestructuion = true;
					Destroy(this.gameObject);
				}
				return true;
			}
			return false;
		}
	};
};
