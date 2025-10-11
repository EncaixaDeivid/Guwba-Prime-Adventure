using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace GuwbaPrimeAdventure
{
	[System.Serializable]
	public class SceneField
	{
#if UNITY_EDITOR
		[SerializeField, Tooltip("The scene to be handled.")] private SceneAsset _sceneAsset;
#endif
		[SerializeField, Tooltip("The name of the scene.")] private string _sceneName;
		public string SceneName => this._sceneName;
		public static implicit operator string(SceneField sceneObject) => sceneObject.SceneName;
	};
#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(SceneField))]
	public class SceneFieldProperty : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, GUIContent.none, property);
			SerializedProperty sceneAsset = property.FindPropertyRelative("_sceneAsset");
			SerializedProperty sceneName = property.FindPropertyRelative("_sceneName");
			position =  EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			if (sceneAsset != null)
			{
				sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
				if (sceneAsset.objectReferenceValue != null)
					sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
			}
			EditorGUI.EndProperty();
		}
	};
#endif
};
