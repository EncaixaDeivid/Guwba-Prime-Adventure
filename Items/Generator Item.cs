using UnityEngine;
using System.Collections.Generic;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	internal sealed class GeneratorItem : StateController
	{
		private readonly List<GameObject> _itemsGenerated = new();
		private float _timeGeneration = 0f;
		private bool _continueGeneration = true;
		private bool _stopGenerate = false;
		[Header("Generation Statistics")]
		[SerializeField, Tooltip("The item to be generated.")] private GameObject _generatedItem;
		[SerializeField, Tooltip("The amount of items that have to be generated.")] private ushort _quantityToGenerate;
		[SerializeField, Tooltip("The amount of time to waits to generation.")] private float _generationTime;
		[SerializeField, Tooltip("If the quantity of the generation is limited.")] private bool _especifiedGeneration;
		[SerializeField, Tooltip("If the items generated are to be keeped in existence.")] private bool _existentItems;
		private void Update()
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
