using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class LevelGateHud : MonoBehaviour
	{
		[Header("Elements")]
		[SerializeField, Tooltip("User interface element.")] private string _levelButton;
		[SerializeField, Tooltip("User interface element.")] private string _bossButton;
		[SerializeField, Tooltip("User interface element.")] private string _scenesButton;
		internal Button Level { get; private set; }
		internal Button Boss { get; private set; }
		internal Button Scenes { get; private set; }
		private void Awake()
		{
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.Level = root.Q<Button>(this._levelButton);
			this.Boss = root.Q<Button>(this._bossButton);
			this.Scenes = root.Q<Button>(this._scenesButton);
		}
	};
};
