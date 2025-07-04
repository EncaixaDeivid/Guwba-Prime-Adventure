using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(IReceptor))]
	internal sealed class Receptor : StateController
	{
		private readonly List<Activator> _usedActivators = new();
		private IReceptor _receptor;
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
			this._receptor = this.GetComponent<IReceptor>();
			SaveController.Load(out SaveFile saveFile);
			if (this._specificsObjects.Length > 0f)
				foreach (string specificObject in this._specificsObjects)
					if (saveFile.generalObjects.Contains(specificObject))
						this.Activate();
		}
		private void Activate() => this._receptor.Execute();
		private void NormalSignal(Activator signalActivator)
		{
			if (this._onlyOneActivation)
				return;
			if (this._usedActivators.ToArray() == this._activators)
				this._usedActivators.Clear();
			if (this._1X1)
			{
				foreach (Activator activator1X1 in this._activators)
					if (signalActivator == activator1X1 && !this._usedActivators.Contains(activator1X1))
					{
						this._usedActivators.Add(activator1X1);
						this.Activate();
						return;
					}
			}
			else if (this._multiplesNeeded)
			{
				foreach (Activator activator in this._activators)
					if (activator == signalActivator)
						this._signals += 1;
				if (this._signals >= this._quantityNeeded)
				{
					this._signals = 0;
					this.Activate();
				}
			}
			else if (this._oneNeeded)
			{
				foreach (Activator activator in this._activators)
					if (activator == signalActivator)
					{
						this.Activate();
						if (this._oneActivation)
							this._onlyOneActivation = true;
						return;
					}
			}
			else
			{
				foreach (Activator activator in this._activators)
					if (activator == signalActivator)
						this._signals += 1;
				if (this._signals >= this._activators.Length)
				{
					this._signals = 0;
					this.Activate();
				}
			}
		}
		internal void ReceiveSignal(Activator signalActivator)
		{
			if (this._timeToActivate > 0f)
				this.StartCoroutine(TimerSignal());
			else
				this.NormalSignal(signalActivator);
			IEnumerator TimerSignal()
			{
				yield return new WaitTime(this, this._timeToActivate);
				this.NormalSignal(signalActivator);
			}
		}
		internal interface IReceptor
		{
			public void Execute();
		};
	};
};
