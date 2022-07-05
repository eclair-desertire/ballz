using UnityEngine;
using UnityEditor;
using System.Collections;

namespace YounGenTech.BawlPhysics {
	[CustomEditor(typeof(GrappleModule), true)]
	public class GrappleModuleInspector : Editor {

		public override void OnInspectorGUI() {
			serializedObject.Update();

			SerializedProperty property = serializedObject.GetIterator();

			property.Next(true);
			while(property.NextVisible(false))
				if(property.name != "m_Script")
					EditorGUILayout.PropertyField(property, true);

			serializedObject.ApplyModifiedProperties();
		}
	}
}