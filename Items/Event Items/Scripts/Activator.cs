using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[RequireComponent(typeof(Transform))]
	internal abstract class Activator : StateController
	{
		private Animator _animator;
		private bool _useOneActivation = false;
		[SerializeField] private Receptor[] _receptors;
		[SerializeField] private string _use, _useOne;
		[SerializeField] private bool _oneActivation, _saveObject;
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
			SaveController.Load(out SaveFile saveFile);
			if (this._saveObject && saveFile.generalObjects.Contains(this.gameObject.name))
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
			if (this._oneActivation && this._useOneActivation)
				return;
			if (this._animator)
			{
				this._animator.SetTrigger(this._use);
				if (this._oneActivation)
					this._animator.SetBool(this._useOne, true);
			}
			foreach (Receptor receptor in this._receptors)
				if (receptor)
					receptor.ReceiveSignal(this);
			this._useOneActivation = true;
			SaveController.Load(out SaveFile saveFile);
			if (this._saveObject && !saveFile.generalObjects.Contains(this.gameObject.name))
			{
				saveFile.generalObjects.Add(this.gameObject.name);
				SaveController.WriteSave(saveFile);
			}
		}
	};
};
