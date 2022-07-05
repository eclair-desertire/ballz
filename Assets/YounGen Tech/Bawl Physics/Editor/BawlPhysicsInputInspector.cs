using UnityEngine;
using UnityEditor;
using System.Collections;

namespace YounGenTech.BawlPhysics {
	[CustomEditor(typeof(BawlPhysicsInput))]
	public class BawlPhysicsInputInspector : Editor {

		GUIStyle labelStyle = null;
		SerializedProperty controls;

		void OnEnable() {
			controls = serializedObject.FindProperty("controls");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			if(labelStyle == null) {
				labelStyle = new GUIStyle(GUI.skin.label);
				labelStyle.richText = true;
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty("_detectMouseLock"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_canControl"));

			if(EditorGUILayout.PropertyField(controls)) {
				EditorGUI.indentLevel++;
				controls.arraySize = EditorGUILayout.IntField("Size", controls.arraySize);

				foreach(SerializedProperty control in controls) {
					EditorGUILayout.BeginVertical(GUI.skin.box);
					{
						ControlGUI(control);
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUI.indentLevel--;

			}

			serializedObject.ApplyModifiedProperties();

			//base.OnInspectorGUI();
		}

		void ControlGUI(SerializedProperty control) {
			EditorGUILayout.BeginHorizontal();
			bool open = EditorGUILayout.PropertyField(control);
			EditorGUILayout.PropertyField(control.FindPropertyRelative("_enabled"), new GUIContent(""), GUILayout.Width(30));
			EditorGUILayout.EndHorizontal();

			if(open) {
				EditorGUI.indentLevel++;
				{
					EditorGUILayout.PropertyField(control.FindPropertyRelative("_name"));

					EditorGUILayout.Space();

					EditorGUILayout.Toggle("Control Active", control.FindPropertyRelative("_controlActive").boolValue);

					SerializedProperty controlType = control.FindPropertyRelative("_controlType");
					EditorGUILayout.PropertyField(controlType);

					switch(controlType.enumValueIndex) {
						case (int)ControlInputType.InputManager:
							EditorGUILayout.PropertyField(control.FindPropertyRelative("_buttonName"));

							EditorGUILayout.Space();

							SerializedProperty controlAxis = control.FindPropertyRelative("_controlAxis");
							SerializedProperty axisAdd = control.FindPropertyRelative("_axisAdd");
							SerializedProperty axisMultiplier = control.FindPropertyRelative("_axisMultiplier");
							SerializedProperty invert = control.FindPropertyRelative("_invert");

							EditorGUI.indentLevel--;
							EditorGUILayout.BeginVertical(GUI.skin.box);
							{
								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.FlexibleSpace();
									float controlAxisOutput = (invert.boolValue ? -(controlAxis.floatValue + axisAdd.floatValue) : (-controlAxis.floatValue + axisAdd.floatValue)) * axisMultiplier.floatValue;

									GUILayout.Label("Axis(" + controlAxisOutput.ToString("F2") + ")", GUILayout.ExpandWidth(false));
									GUILayout.FlexibleSpace();
								}
								EditorGUILayout.EndHorizontal();

								EditorGUILayout.Slider(control.FindPropertyRelative("_controlAxis").floatValue, -1, 1);
								EditorGUILayout.PropertyField(axisAdd);
								EditorGUILayout.PropertyField(axisMultiplier);
								EditorGUILayout.PropertyField(invert);
							}
							EditorGUILayout.EndVertical();
							EditorGUI.indentLevel++;

							break;

						case (int)ControlInputType.KeyCode:
							EditorGUILayout.PropertyField(control.FindPropertyRelative("_key"));
							break;
					}

					EditorGUILayout.Space();

					SerializedProperty eventType = control.FindPropertyRelative("_eventType");
					eventType.isExpanded = EditorGUILayout.Foldout(eventType.isExpanded, "Events");

					if(eventType.isExpanded) {
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField(eventType);
						EditorGUI.indentLevel--;

						EditorGUI.indentLevel--;
						if(eventType.enumValueIndex == (int)ControlEventType.OnControl || eventType.enumValueIndex == (int)ControlEventType.OnControlAndOnToggle) {
							SerializedProperty allowedButtonEvents = control.FindPropertyRelative("_allowedButtonEvents");

							EditorGUILayout.BeginVertical(GUI.skin.box);
							{
								GUILayout.Label("<b>Control Events</b>", labelStyle);

								EditorGUILayout.BeginVertical(GUI.skin.box);
								EditorGUILayout.PropertyField(allowedButtonEvents.FindPropertyRelative("axis"));
								EditorGUILayout.PropertyField(control.FindPropertyRelative("onControlAxis"));
								EditorGUILayout.EndVertical();

								EditorGUILayout.BeginVertical(GUI.skin.box);
								EditorGUILayout.PropertyField(allowedButtonEvents.FindPropertyRelative("hold"));
								EditorGUILayout.PropertyField(control.FindPropertyRelative("onControlHold"));
								EditorGUILayout.EndVertical();

								EditorGUILayout.BeginVertical(GUI.skin.box);
								EditorGUILayout.PropertyField(allowedButtonEvents.FindPropertyRelative("down"));
								EditorGUILayout.PropertyField(control.FindPropertyRelative("onControlDown"));
								EditorGUILayout.EndVertical();

								EditorGUILayout.BeginVertical(GUI.skin.box);
								EditorGUILayout.PropertyField(allowedButtonEvents.FindPropertyRelative("up"));
								EditorGUILayout.PropertyField(control.FindPropertyRelative("onControlUp"));
								EditorGUILayout.EndVertical();
							}
							EditorGUILayout.EndVertical();
						}

						if(eventType.enumValueIndex == (int)ControlEventType.OnToggle || eventType.enumValueIndex == (int)ControlEventType.OnControlAndOnToggle) {
							EditorGUILayout.BeginVertical(GUI.skin.box);
							{
								GUILayout.Label("<b>Toggle Events</b>", labelStyle);
								
								EditorGUILayout.PropertyField(control.FindPropertyRelative("_toggleState"));

								EditorGUILayout.BeginVertical(GUI.skin.box);
								EditorGUILayout.PropertyField(control.FindPropertyRelative("onToggleState"));
								EditorGUILayout.EndVertical();

								EditorGUILayout.BeginVertical(GUI.skin.box);
								EditorGUILayout.PropertyField(control.FindPropertyRelative("onToggleStateTrue"));
								EditorGUILayout.EndVertical();

								EditorGUILayout.BeginVertical(GUI.skin.box);
								EditorGUILayout.PropertyField(control.FindPropertyRelative("onToggleStateFalse"));
								EditorGUILayout.EndVertical();
							}
							EditorGUILayout.EndVertical();
						}
						EditorGUI.indentLevel++;
					}
				}
				EditorGUI.indentLevel--;
			}
		}
	}
}