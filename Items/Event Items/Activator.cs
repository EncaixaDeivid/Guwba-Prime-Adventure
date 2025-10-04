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
		private int _use;
		private int _useAgain;
		[Header("Activator")]
		[SerializeField, Tooltip("The receptors that will receive the signal.")] private Receptor[] _receptors;
		[SerializeField, Tooltip("The activator only can be activeted one time.")] private bool _oneActivation;
		[SerializeField, Tooltip("If this object have been activeted before it will always be activeted.")] private bool _saveOnSpecifics;
		[Header("Animation")]
		[SerializeField, Tooltip("Animation parameter.")] private string _useAnimation;
		[SerializeField, Tooltip("Animation parameter.")] private string _useAgainAnimation;
		protected bool Usable => this._usable;
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
			this._use = Animator.StringToHash(this._useAnimation);
			this._useAgain = Animator.StringToHash(this._useAgainAnimation);
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
			if (this._animator)
			{
				this._used = !this._used;
				if (this._used)
					this._animator.SetTrigger(this._use);
				else
					this._animator.SetTrigger(this._useAgain);
				if (this._oneActivation)
				{
					this._usable = false;
				}
			}
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
