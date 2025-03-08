using UnityEngine;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator))]
	internal sealed class GeneratorItem : StateController
	{
		private readonly List<GameObject> _itemsGenerated = new();
		private float _timeGeneration = 0f;
		private bool _continueGeneration = true, _stopGenerate = false;
		[SerializeField] private GameObject _generatedItem;
		[SerializeField] private ushort _quantityToGenerate;
		[SerializeField] private float _generationTime;
		[SerializeField] private bool _especifiedGeneration, _existentItems;
		private void FixedUpdate() // Generation
		{
			if (this._stopGenerate)
				return;
			if (this._continueGeneration)
				if (this._timeGeneration > 0f)
					this._timeGeneration -= Time.deltaTime;
				else if (this._timeGeneration <= 0f)
				{
					this._timeGeneration = this._generationTime;
					this._itemsGenerated.Add(Instantiate(this._generatedItem, this.transform.position, this.transform.rotation));
				}
			if (this._existentItems && !this._especifiedGeneration)
			{
				this._itemsGenerated.RemoveAll(item => !item);
				this._continueGeneration = this._quantityToGenerate != this._itemsGenerated.Count;
			}
			else if (this._especifiedGeneration && !this._existentItems && this._quantityToGenerate == this._itemsGenerated.Count)
				this._stopGenerate = true;
		}
	};
};
