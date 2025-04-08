using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[RequireComponent(typeof(Transform))]
	internal abstract class Activator : StateController
	{
		private Animator _animator;
		private SaveFile _saveFile;
		private bool _useOneActivation = false;
		[SerializeField] private Receptor[] _receptors;
		[SerializeField] private string _use, _useOne;
		[SerializeField] private bool _oneActivation, _saveObject;
		private new void Awake()
		{
			base.Awake();
			this._animator = this.GetComponent<Animator>();
			SaveController.Load(out this._saveFile);
			if (this._saveObject && this._saveFile.generalObjects.Contains(this.gameObject.name))
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
			if (this._saveObject && !this._saveFile.generalObjects.Contains(this.gameObject.name))
			{
				this._saveFile.generalObjects.Add(this.gameObject.name);
				SaveController.WriteSave(this._saveFile);
			}
		}
	};
};
