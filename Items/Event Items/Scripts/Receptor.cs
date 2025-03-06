using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	internal sealed class Receptor : StateController
	{
		private readonly List<Activator> _usedActivators = new();
		private IReceptor _receptor;
		private ushort _signals = 0;
		private bool _intercalate = true, _onlyOneActivation = false;
		[SerializeField] private Activator[] _activators;
		[SerializeField] private string[] _specificsObjects;
		[SerializeField] private bool _1X1, _intercalateEvents, _oneNeeded, _oneActivation;
		[SerializeField] private float _timeToActivate;
		private new void Awake()
		{
			base.Awake();
			this._receptor = this.GetComponent<IReceptor>();
			if (this._specificsObjects.Length > 0f)
				foreach (string specificObject in this._specificsObjects)
					if (SaveFileData.GeneralObjects.Contains(specificObject))
						this.Activate();
		}
		private void Activate()
		{
			if (this._intercalate)
			{
				this._intercalate = !this._intercalateEvents;
				this._receptor.ActivationEvent();
			}
			else if (this._intercalateEvents && !this._intercalate)
			{
				this._intercalate = true;
				this._receptor.DesactivationEvent();
			}
		}
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
			public void ActivationEvent();
			public void DesactivationEvent();
		};
	};
};