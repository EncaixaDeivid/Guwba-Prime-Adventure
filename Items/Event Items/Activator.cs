using UnityEngine;
using System.Collections;
using GwambaPrimeAdventure.Connection;
namespace GwambaPrimeAdventure.Item.EventItem
{
	[RequireComponent(typeof(Transform))]
	internal abstract class Activator : StateController, ILoader
	{
		private Animator _animator;
		private readonly int IsOn = Animator.StringToHash(nameof(IsOn));
		private readonly int Use = Animator.StringToHash(nameof(Use));
		private readonly int UseAgain = Animator.StringToHash(nameof(UseAgain));
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
		}
		private void OnEnable()
		{
			if (_animator)
				_animator.SetFloat(IsOn, 1F);
		}
		private void OnDisable()
		{
			if (_animator)
				_animator.SetFloat(IsOn, 0F);
		}
		public IEnumerator Load()
		{
			SaveController.Load(out SaveFile saveFile);
			if (_saveOnSpecifics && saveFile.GeneralObjects.Contains(name))
				Activation();
			yield return null;
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
					_animator.SetTrigger(Use);
				else
					_animator.SetTrigger(UseAgain);
			foreach (Receptor receptor in _receptors)
				if (receptor)
					receptor.ReceiveSignal(this);
			_usedOne = true;
			SaveController.Load(out SaveFile saveFile);
			if (_saveOnSpecifics && !saveFile.GeneralObjects.Contains(name))
			{
				saveFile.GeneralObjects.Add(name);
				SaveController.WriteSave(saveFile);
			}
		}
	};
};
