using UnityEngine;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D), typeof(Receptor))]
	internal sealed class DestructiveObject : StateController, IReceptorSignal, IDestructible
	{
		private readonly Sender _sender = Sender.Create();
		[Header("Destructive Object")]
		[SerializeField, Tooltip("If there a object that will be instantiate after the destruction of ")]
		private HiddenObject _hiddenObject;
		[SerializeField, Tooltip("The vitality of this object before it destruction.")] private short _vitality;
		[SerializeField, Tooltip("The amount of damage that this object have to receive real damage.")] private short _biggerDamage;
		[SerializeField, Tooltip("If this object will be destructed on collision with another object.")] private bool _destroyOnCollision;
		[SerializeField, Tooltip("If this object will be saved as already existent object.")] private bool _saveOnSpecifics;
		public short Health => _vitality;
		private new void Awake()
		{
			base.Awake();
			_sender.SetStateForm(StateForm.State);
			_sender.SetAdditionalData(_hiddenObject);
			_sender.SetToggle(true);
			SaveController.Load(out SaveFile saveFile);
			if (_saveOnSpecifics && saveFile.generalObjects.Contains(gameObject.name))
				Destroy(gameObject, 1e-3f);
		}
		public void Execute()
		{
			if (_hiddenObject)
				_sender.Send(PathConnection.System);
			SaveObject();
			Destroy(gameObject);
		}
		private void SaveObject()
		{
			SaveController.Load(out SaveFile saveFile);
			if (_saveOnSpecifics && !saveFile.generalObjects.Contains(gameObject.name))
			{
				saveFile.generalObjects.Add(gameObject.name);
				SaveController.WriteSave(saveFile);
			}
		}
		private void DestroyOnCollision()
		{
			if (_destroyOnCollision)
			{
				if (_hiddenObject)
					_sender.Send(PathConnection.System);
				SaveObject();
				Destroy(gameObject);
			}
		}
		private void OnCollisionEnter2D(Collision2D collision) => DestroyOnCollision();
		private void OnTriggerEnter2D(Collider2D collision) => DestroyOnCollision();
		public bool Hurt(ushort damage)
		{
			if (damage < _biggerDamage || _vitality <= 0f)
				return false;
			if ((_vitality -= (short)damage) <= 0f)
			{
				if (_hiddenObject)
					_sender.Send(PathConnection.System);
				SaveObject();
				Destroy(gameObject);
			}
			return true;
		}
		public void Stun(ushort stunStength, float stunTime) { }
	};
};
