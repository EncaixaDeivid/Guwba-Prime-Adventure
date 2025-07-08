using UnityEngine;
using GuwbaPrimeAdventure.Data;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[RequireComponent(typeof(Transform), typeof(Rigidbody2D), typeof(Collider2D))]
	internal abstract class BossController : StateController, IConnector
	{
		protected Rigidbody2D _rigidybody;
		protected Collider2D _collider;
		protected readonly Sender _sender = Sender.Create();
		protected short _movementSide = 1;
		private static bool _isDeafeted = false;
		[Header("Boss Controller")]
		[SerializeField, Tooltip("The bosses to send messages.")] private BossController[] _bossesToSend;
		[SerializeField, Tooltip("The layer mask to identify the ground.")] protected LayerMask _groundLayer;
		[SerializeField, Tooltip("The layer mask to identify the target of the attacks.")] protected LayerMask _targetLayerMask;
		[Header("Boss Stats")]
		[SerializeField, Tooltip("The size of the ground identifier.")] private float _groundSize;
		[SerializeField, Tooltip("The amount of speed to move the boss.")] protected ushort _movementSpeed;
		[SerializeField, Tooltip("The maount of damage to hit the target.")] private ushort _damage;
		[SerializeField, Tooltip("If the boss will move firstly to the left.")] protected bool _invertMovementSide;
		[SerializeField, Tooltip("If this boss will not do damage.")] private bool _noDealDamage;
		[Header("Boss Events")]
		[SerializeField, Tooltip("If this boss will react to any damage taken.")] protected bool _reactToDamage;
		[SerializeField, Tooltip("If this boss will start a trancision.")] private bool _isTransitioner;
		[SerializeField, Tooltip("If this boss have any dialog to start after his death.")] private bool _haveDialog;
		public PathConnection PathConnection => PathConnection.Boss;
		protected new void Awake()
		{
			base.Awake();
			this._rigidybody = this.GetComponent<Rigidbody2D>();
			this._collider = this.GetComponent<Collider2D>();
			this._sender.SetToWhereConnection(PathConnection.Boss);
			this._sender.SetAdditionalData(this._bossesToSend);
			this._movementSide = (short)(this._invertMovementSide ? -1f : 1f);
			Sender.Include(this);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			Sender.Exclude(this);
		}
		protected bool SurfacePerception()
		{
			Vector2 point = new(this.transform.position.x, this.transform.position.y - this._collider.bounds.extents.y - this._groundSize / 2f);
			Vector2 size = new(this._collider.bounds.size.x - 0.025f, this._groundSize);
			return Physics2D.OverlapBox(point, size, this.transform.eulerAngles.z, this._groundLayer);
		}
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!this._noDealDamage && collision.TryGetComponent<IDamageable>(out var damageable))
				damageable.Damage(this._damage);
		}
		public void Receive(DataConnection data, object additionalData)
		{
			if (data.StateForm == StateForm.Disable && data.ToggleValue.HasValue && !_isDeafeted)
			{
				if (data.ToggleValue.Value)
				{
					_isDeafeted = true;
					SaveController.Load(out SaveFile saveFile);
					SettingsController.Load(out Settings settings);
					ushort sceneIndex = (ushort)(ushort.Parse($"{this.gameObject.scene.name[^1]}") - 1f);
					if (!saveFile.deafetedBosses[sceneIndex])
					{
						saveFile.deafetedBosses[sceneIndex] = true;
						SaveController.WriteSave(saveFile);
					}
					if (settings.dialogToggle && this._haveDialog)
						this.GetComponent<IInteractable>().Interaction();
					else if (this._isTransitioner)
						this.GetComponent<Transitioner>().Transicion();
				}
				this.enabled = false;
			}
		}
	};
};
