using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GwambaPrimeAdventure.Data;
namespace GwambaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(IReceptorSignal))]
	internal sealed class Receptor : StateController, ILoader
	{
		private readonly List<Activator> _usedActivators = new();
		private Activator _signalActivator;
		private IReceptorSignal _receptor;
		private ushort _signals = 0;
		private float _signalTimer = 0f;
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
		}
		public IEnumerator Load()
		{
			SaveController.Load(out SaveFile saveFile);
			if (_specificsObjects.Length > 0f)
				foreach (string specificObject in _specificsObjects)
					if (saveFile.GeneralObjects.Contains(specificObject))
						Activate();
			yield return null;
		}
		private void Update()
		{
			if (_signalTimer > 0f)
				if ((_signalTimer -= Time.deltaTime) <= 0f)
					NormalSignal();
		}
		private void Activate() => _receptor.Execute();
		private void NormalSignal()
		{
			if (_onlyOneActivation)
				return;
			if (_usedActivators.ToArray() == _activators)
				_usedActivators.Clear();
			if (_1X1)
			{
				foreach (Activator activator1X1 in _activators)
					if (_signalActivator == activator1X1 && !_usedActivators.Contains(activator1X1))
					{
						_usedActivators.Add(activator1X1);
						Activate();
						return;
					}
			}
			else if (_multiplesNeeded)
			{
				foreach (Activator activator in _activators)
					if (activator == _signalActivator)
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
					if (activator == _signalActivator)
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
					if (activator == _signalActivator)
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
			_signalActivator = signalActivator;
			if (_timeToActivate > 0f)
				_signalTimer = _timeToActivate;
			else
				NormalSignal();
		}
	};
	internal interface IReceptorSignal
	{
		public void Execute();
	};
};
