using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Item.EventItem
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Collider2D), typeof(Receptor))]
	internal sealed class DestructiveObject : StateController, Receptor.IReceptor, IDamageable
	{
		[SerializeField] private GameObject _hiddenObject;
		[SerializeField] private short _vitality, _biggerDamage;
		[SerializeField] private bool _destroyOnCollision, _saveObject, _saveOnDestruction;
		public ushort Health => (ushort)this._vitality;
		private new void Awake()
		{
			base.Awake();
			SaveController.Load(out SaveFile saveFile);
			if (this._saveObject && saveFile.generalObjects.Contains(this.gameObject.name))
				Destroy(this.gameObject, 0.001f);
		}
		public void ActivationEvent()
		{
			if (this._hiddenObject)
				Instantiate(this._hiddenObject, this.transform.position, this._hiddenObject.transform.rotation);
			this.SaveObject();
			Destroy(this.gameObject);
		}
		public void DesactivationEvent() => this.ActivationEvent();
		private void SaveObject()
		{
			SaveController.Load(out SaveFile saveFile);
			if (this._saveObject && !saveFile.generalObjects.Contains(this.gameObject.name))
			{
				saveFile.generalObjects.Add(this.gameObject.name);
				SaveController.WriteSave(saveFile);
			}
		}
		private void DestroyOnCollision()
		{
			if (this._destroyOnCollision)
			{
				if (this._hiddenObject)
					Instantiate(this._hiddenObject, this.transform.position, this._hiddenObject.transform.rotation);
				this.SaveObject();
				Destroy(this.gameObject);
			}
		}
		public bool Damage(ushort damage)
		{
			if (this._vitality <= 0f)
				return false;
			if (damage < this._biggerDamage)
				return false;
			this._vitality -= (short)damage;
			if(this._vitality <= 0f)
			{
				if(this._hiddenObject)
					Instantiate(this._hiddenObject, this.transform.position, this._hiddenObject.transform.rotation);
				this.SaveObject();
				Destroy(this.gameObject);
			}
			return true;
		}
		private void OnCollisionEnter2D(Collision2D collision) => this.DestroyOnCollision();
		private void OnTriggerEnter2D(Collider2D collision) => this.DestroyOnCollision();
	};
};
