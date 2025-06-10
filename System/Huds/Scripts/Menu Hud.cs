using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class MenuHud : MonoBehaviour
	{
		private static MenuHud _instance;
		[Header("Element")]
		[SerializeField, Tooltip("User interface element.")] private string _buttonsGroup;
		[SerializeField, Tooltip("User interface element.")] private string _savesGroup;
		[SerializeField, Tooltip("User interface element.")] private string _playButton;
		[SerializeField, Tooltip("User interface element.")] private string _configurationsButton;
		[SerializeField, Tooltip("User interface element.")] private string _quitButton;
		[SerializeField, Tooltip("User interface element.")] private string _backButton;
		[SerializeField, Tooltip("User interface element.")] private string _saveNameTextField;
		[SerializeField, Tooltip("User interface element.")] private string _loadButton;
		[SerializeField, Tooltip("User interface element.")] private string _deleteButton;
		internal GroupBox Buttons { get; private set; }
		internal GroupBox Saves { get; private set; }
		internal Button Play { get; private set; }
		internal Button Configurations { get; private set; }
		internal Button Quit { get; private set; }
		internal Button Back { get; private set; }
		internal TextField[] SaveName { get; private set; }
		internal Button[] Load { get; private set; }
		internal Button[] Delete { get; private set; }
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.Buttons = root.Q<GroupBox>(this._buttonsGroup);
			this.Play = root.Q<Button>(this._playButton);
			this.Configurations = root.Q<Button>(this._configurationsButton);
			this.Quit = root.Q<Button>(this._quitButton);
			this.Saves = root.Q<GroupBox>(this._savesGroup);
			this.Back = root.Q<Button>(this._backButton);
			this.SaveName = new TextField[4];
			for (ushort i = 0; i < this.SaveName.Length; i++)
				this.SaveName[i] = root.Q<TextField>(this._saveNameTextField + $"{i + 1f}");
			this.Load = new Button[4];
			for (ushort i = 0; i < this.Load.Length; i++)
				this.Load[i] = root.Q<Button>(this._loadButton + $"{i + 1f}");
			this.Delete = new Button[4];
			for (ushort i = 0; i < this.Delete.Length; i++)
				this.Delete[i] = root.Q<Button>(this._deleteButton + $"{i + 1f}");
		}
	};
};
