using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace YounGenTech {
	public static class EditorExtensions {
		#region SerializedObject and SerializedProperty
		public static void SerializedArrayGUI(this SerializedProperty property) {
			property.ArrayAddRemoverGUI(-1, GUILayout.Width(100));

			for(int i = 0; i < property.arraySize; i++) {
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i), new GUIContent(""));
					property.ArrayAddRemoverGUI(GUILayout.Width(60));
				}
				EditorGUILayout.EndHorizontal();
			}

			if(property.arraySize > 0)
				property.ArrayAddRemoverGUI(GUILayout.Width(100));
		}

		public static void ArrayAddRemoverGUI(this SerializedProperty property, params GUILayoutOption[] options) {
			if(!property.isArray) return;

			switch(GUILayout.Toolbar(property.arraySize > 0 ? -1 : 1, new string[] { "+", "-" }, options)) {
				case 0:
					property.arraySize++;
					break;
				case 1:
					if(property.arraySize > 0)
						property.arraySize--;

					break;
			}
		}
		public static void ArrayAddRemoverGUI(this SerializedProperty property, int index, params GUILayoutOption[] options) {
			if(!property.isArray) return;

			switch(GUILayout.Toolbar(property.arraySize > 0 && index < property.arraySize ? -1 : 1, new string[] { "+", "-" }, options)) {
				case 0:
					property.InsertArrayElementAtIndex(index + 1);
					break;
				case 1:
					if(property.arraySize > 0 && index < property.arraySize)
						property.DeleteArrayElementAtIndex(index);

					break;
			}
		}
		#endregion

		#region ScriptableObject

		#endregion

		#region Regular
		public static void ObjectListGUI<T>(this List<T> list, params GUILayoutOption[] options) where T : Object {
			switch(GUILayout.Toolbar(list.Count > 0 ? -1 : 1, new string[] { "+", "-" }, options)) {
				case 0:
					list.Add(null);
					break;
				case 1:
					if(list.Count > 0)
						list.RemoveAt(list.Count - 1);

					break;
			}
		}

		public static void ObjectListGUI<T>(this List<T> list, int index, params GUILayoutOption[] options) where T : Object {
			switch(GUILayout.Toolbar(list.Count > 0 && index < list.Count ? -1 : 1, new string[] { "+", "-" }, options)) {
				case 0:
					list.Insert(index + 1, null);
					break;
				case 1:
					if(list.Count > 0 && index < list.Count)
						list.RemoveAt(index);
					break;
			}
		}
		#endregion
	}
}