using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GwambaPrimeAdventure.Data;
namespace GwambaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(IReceptorSignal))]
	internal sealed class Receptor : StateController
	{
		private readonly List<Activator> _usedActivators = new();
		private IReceptorSignal _receptor;
		private ushort _signals = 0;
		private bool _onlyOneActivation = false;
		[Header("Receptor")]
		[SerializeField, Tooltip("The activators that this will receive a signal.")] private Activator[] _activators;
		[SerializeField, Tooltip("If this will receive a signal from specifics or existent objects.")] private string[] _specificsObjects;
		[SerializeField, Tooltip("If this will activate for every activator activated.")] private bool _1X1;
		[SerializeField, Tooltip("If is needed only one activator to activate.")] private bool _oneNeeded;
		[SerializeField, Tooltip("If it will be inactive after one activation")] private bool _oneActivation;
		[SerializeField, Tooltip("If are multiples activators needed to activate.")] private bool _multiplesNeeded;
		[SerializeField, Tooltip("The quantity of multiples activators needed to activate.")] private ushort _quantityNeeded;
		[SerializeField, Tooltip("The amount of time to wait for active after receive the signal.")] private float _timeToActivate;
		private new void Awake()
		{
			base.Awake();
			_receptor = GetComponent<IReceptorSignal>();
			SaveController.Load(out SaveFile saveFile);
			if (_specificsObjects.Length > 0f)
				foreach (string specificObject in _specificsObjects)
					if (saveFile.GeneralObjects.Contains(specificObject))
						Activate();
		}
		private void Activate() => _receptor.Execute();
		private void NormalSignal(Activator signalActivator)
		{
			if (_onlyOneActivation)
				return;
			if (_usedActivators.ToArray() == _activators)
				_usedActivators.Clear();
			if (_1X1)
			{
				foreach (Activator activator1X1 in _activators)
					if (signalActivator == activator1X1 && !_usedActivators.Contains(activator1X1))
					{
						_usedActivators.Add(activator1X1);
						Activate();
						return;
					}
			}
			else if (_multiplesNeeded)
			{
				foreach (Activator activator in _activators)
					if (activator == signalActivator)
						_signals += 1;
				if (_signals >= _quantityNeeded)
				{
					_signals = 0;
					Activate();
				}
			}
			else if (_oneNeeded)
			{
				foreach (Activator activator in _activators)
					if (activator == signalActivator)
					{
						Activate();
						if (_oneActivation)
							_onlyOneActivation = true;
						return;
					}
			}
			else
			{
				foreach (Activator activator in _activators)
					if (activator == signalActivator)
						_signals += 1;
				if (_signals >= _activators.Length)
				{
					_signals = 0;
					Activate();
				}
			}
		}
		internal void ReceiveSignal(Activator signalActivator)
		{
			if (_timeToActivate > 0f)
				StartCoroutine(TimerSignal());
			else
				NormalSignal(signalActivator);
			IEnumerator TimerSignal()
			{
				yield return new WaitTime(this, _timeToActivate);
				NormalSignal(signalActivator);
			}
		}
	};
	internal interface IReceptorSignal
	{
		public void Execute();
	};
};
