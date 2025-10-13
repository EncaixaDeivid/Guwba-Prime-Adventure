using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[RequireComponent(typeof(Transform))]
	internal abstract class Activator : StateController
	{
		private Animator _animator;
		private bool _used = false;
		private bool _usedOne = false;
		private bool _usable = true;
		private readonly int _use = Animator.StringToHash("Use");
		private readonly int _useAgain = Animator.StringToHash("UseAgain");
		[Header("Activator")]
		[SerializeField, Tooltip("The receptors that will receive the signal.")] private Receptor[] _receptors;
		[SerializeField, Tooltip("The activator only can be activeted one time.")] private bool _oneActivation;
		[SerializeField, Tooltip("If this object have been activeted before it will always be activeted.")] private bool _saveOnSpecifics;
		protected bool Usable => this._usable;
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
			SaveController.Load(out SaveFile saveFile);
			if (this._saveOnSpecifics && saveFile.generalObjects.Contains(this.gameObject.name))
				this.Activation();
		}
		private void OnEnable()
		{
			if (this._animator)
				this._animator.enabled = true;
		}
		private void OnDisable()
		{
			if (this._animator)
				this._animator.enabled = false;
		}
		protected void Activation()
		{
			if (this._oneActivation && this._usedOne)
				return;
			this._used = !this._used;
			if (this._oneActivation)
				this._usable = false;
			if (this._animator)
				if (this._used)
					this._animator.SetTrigger(this._use);
				else
					this._animator.SetTrigger(this._useAgain);
			foreach (Receptor receptor in this._receptors)
				if (receptor)
					receptor.ReceiveSignal(this);
			this._usedOne = true;
			SaveController.Load(out SaveFile saveFile);
			if (this._saveOnSpecifics && !saveFile.generalObjects.Contains(this.gameObject.name))
			{
				saveFile.generalObjects.Add(this.gameObject.name);
				SaveController.WriteSave(saveFile);
			}
		}
	};
};
