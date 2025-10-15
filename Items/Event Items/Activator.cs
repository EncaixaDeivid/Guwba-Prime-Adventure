using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[RequireComponent(typeof(Transform))]
	internal abstract class Activator : StateController
	{
		private Animator _animator;
		private readonly int _isOn = Animator.StringToHash("IsOn");
		private readonly int _use = Animator.StringToHash("Use");
		private readonly int _useAgain = Animator.StringToHash("UseAgain");
		private bool _used = false;
		private bool _usedOne = false;
		private bool _usable = true;
		[Header("Activator")]
		[SerializeField, Tooltip("The receptors that will receive the signal.")] private Receptor[] _receptors;
		[SerializeField, Tooltip("The activator only can be activeted one time.")] private bool _oneActivation;
		[SerializeField, Tooltip("If this object have been activeted before it will always be activeted.")] private bool _saveOnSpecifics;
		protected bool Usable => _usable;
		private new void Awake()
		{
			base.Awake();
			_animator = GetComponent<Animator>();
			SaveController.Load(out SaveFile saveFile);
			if (_saveOnSpecifics && saveFile.generalObjects.Contains(gameObject.name))
				Activation();
		}
		private void OnEnable()
		{
			if (_animator)
				_animator.SetFloat(_isOn, 1f);
		}
		private void OnDisable()
		{
			if (_animator)
				_animator.SetFloat(_isOn, 0f);
		}
		protected void Activation()
		{
			if (_oneActivation && _usedOne)
				return;
			_used = !_used;
			if (_oneActivation)
				_usable = false;
			if (_animator)
				if (_used)
					_animator.SetTrigger(_use);
				else
					_animator.SetTrigger(_useAgain);
			foreach (Receptor receptor in _receptors)
				if (receptor)
					receptor.ReceiveSignal(this);
			_usedOne = true;
			SaveController.Load(out SaveFile saveFile);
			if (_saveOnSpecifics && !saveFile.generalObjects.Contains(gameObject.name))
			{
				saveFile.generalObjects.Add(gameObject.name);
				SaveController.WriteSave(saveFile);
			}
		}
	};
};
