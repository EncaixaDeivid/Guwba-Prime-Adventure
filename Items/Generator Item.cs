using UnityEngine;
using System.Collections.Generic;
namespace GwambaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform))]
	internal sealed class GeneratorItem : StateController
	{
		private readonly List<GameObject> _itemsGenerated = new();
		private float _timeGeneration = 0f;
		private bool _continueGeneration = true;
		[Header("Generation Statistics")]
		[SerializeField, Tooltip("The item to be generated.")] private GameObject _generatedItem;
		[SerializeField, Tooltip("The amount of items that have to be generated.")] private ushort _quantityToGenerate;
		[SerializeField, Tooltip("The amount of time to waits to generation.")] private float _generationTime;
		[SerializeField, Tooltip("If the quantity of the generation is limited.")] private bool _especifiedGeneration;
		[SerializeField, Tooltip("If the items generated are to be keeped in existence.")] private bool _existentItems;
		private void Update()
		{
			if (_continueGeneration && _timeGeneration > 0f)
				if ((_timeGeneration -= Time.deltaTime) <= 0f)
				{
					_timeGeneration = _generationTime;
					_itemsGenerated.Add(Instantiate(_generatedItem, transform.position, transform.rotation));
				}
			if (_existentItems && !_especifiedGeneration)
			{
				_itemsGenerated.RemoveAll(item => !item);
				_continueGeneration = _quantityToGenerate != _itemsGenerated.Count;
			}
			else if (_especifiedGeneration && !_existentItems && _quantityToGenerate == _itemsGenerated.Count)
				enabled = false;
		}
	};
};
