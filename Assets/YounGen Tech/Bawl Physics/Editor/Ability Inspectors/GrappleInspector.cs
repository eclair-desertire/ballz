using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace YounGenTech.BawlPhysics {
	[CustomEditor(typeof(Grapple))]
	public class GrappleInspector : Editor {
		const string AddGrappleModuleFoldoutPref = "BawlPhysics-AddGrappleModuleFoldout";
		const string GrappleModuleFoldoutPref = "BawlPhysics-GrappleModuleFoldout";
		const string CombineModulesPref = "BawlPhysics-CombineModules";

		Grapple multiGrapple;
		GUIStyle grappleTargetLabel;

		AnimBool grappleModuleFade;
		AnimBool grappleModuleTypesFade;
		Vector2 grappleModuleTypesScroll;
		List<string> grappleModuleTypes;
		string[] grappleModuleTypeNames;
		Dictionary<GrappleModule, Editor> grappleModules = new Dictionary<GrappleModule, Editor>();

		void OnEnable() {
			multiGrapple = target as Grapple;

			grappleModuleFade = new AnimBool(EditorPrefs.GetBool(GrappleModuleFoldoutPref, false));
			grappleModuleFade.valueChanged.AddListener(Repaint);

			grappleModuleTypes = GetEnumerableOfType<GrappleModule>();
			grappleModuleTypeNames = grappleModuleTypes.ToArray();

			for(int i = 0; i < grappleModuleTypes.Count; i++) {
				Type type = Type.GetType(grappleModuleTypes[i]);
				grappleModuleTypeNames[i] = type.Name;
			}

			grappleModuleTypesFade = new AnimBool(EditorPrefs.GetBool(AddGrappleModuleFoldoutPref, false));
			grappleModuleTypesFade.valueChanged.AddListener(Repaint);
		}

		void OnDisable() {
			foreach(Editor editor in grappleModules.Values)
				DestroyImmediate(editor, true);

			grappleModules.Clear();
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if(grappleTargetLabel == null) {
				grappleTargetLabel = new GUIStyle(GUI.skin.label);
				grappleTargetLabel.richText = true;
			}

			serializedObject.Update();

			if(Application.isPlaying) {
				for(int i = 0; i < multiGrapple.MaxGrappleTargets; i++) {
					Grapple.GrappleTarget grappleTarget = multiGrapple.GetTarget(i);

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("<b>Grapple Target " + (i + 1) + "</b>", grappleTargetLabel, GUILayout.ExpandWidth(false));
						EditorGUILayout.ToggleLeft("Enabled", grappleTarget.Enabled);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUI.indentLevel++;
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField("Target", GUILayout.ExpandWidth(false));
							EditorGUILayout.ObjectField(grappleTarget.Target, typeof(Transform), true);
						}
						EditorGUILayout.EndHorizontal();

						Vector3 localAttachPoint = EditorGUILayout.Vector3Field("Local Attach Point", grappleTarget.LocalAttachPoint);
						if(GUI.changed) grappleTarget.LocalAttachPoint = localAttachPoint;

						GUI.enabled = false;
						EditorGUILayout.Vector3Field("World Attach Point", grappleTarget.WorldAttachPoint);
						GUI.enabled = true;

						grappleTarget.Spring = EditorGUILayout.FloatField("Spring", grappleTarget.Spring);
						grappleTarget.Damper = EditorGUILayout.FloatField("Damper", grappleTarget.Damper);
						grappleTarget.Bounciness = EditorGUILayout.FloatField("Bounciness", grappleTarget.Bounciness);
						grappleTarget.AddLength = EditorGUILayout.FloatField("Add Length", grappleTarget.AddLength);
						grappleTarget.Length = EditorGUILayout.FloatField("Length", grappleTarget.Length);
					}
					EditorGUI.indentLevel--;
				}
			}

			serializedObject.ApplyModifiedProperties();

			bool oldCombineModules = EditorPrefs.GetBool(CombineModulesPref, true);
			bool combineModules = EditorGUILayout.ToggleLeft(new GUIContent("Show Grapple Modules Here", "Stores the Grapple Modules in this component box so the object doesn't become cluttered with Grapple Module components"), oldCombineModules);

			if(oldCombineModules != combineModules) {
				EditorPrefs.SetBool(CombineModulesPref, combineModules);

				if(!combineModules) {
					foreach(KeyValuePair<GrappleModule, Editor> module in grappleModules) {
						module.Key.hideFlags = HideFlags.None;
						DestroyImmediate(module.Value, true);
					}

					grappleModules.Clear();
				}
			}

			if(combineModules) {
				GUI.backgroundColor = new Color(.5f, .5f, 1);
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					GUI.backgroundColor = Color.white;
					grappleModuleFade.target = EditorPrefs.GetBool(GrappleModuleFoldoutPref, false);
					EditorGUI.indentLevel++;
					grappleModuleFade.target = EditorGUILayout.Foldout(grappleModuleFade.target, (grappleModuleFade.target ? "Hide" : "Show") + " Attached Grapple Modules");
					EditorGUI.indentLevel--;

					EditorPrefs.SetBool(GrappleModuleFoldoutPref, grappleModuleFade.target);

					if(EditorGUILayout.BeginFadeGroup(grappleModuleFade.faded)) {
						grappleModuleTypesFade.target = EditorPrefs.GetBool(AddGrappleModuleFoldoutPref, false);

						EditorGUILayout.BeginVertical(GUI.skin.box);
						{
							int selectedType = EditorGUILayout.Popup("Add Module", -1, grappleModuleTypeNames);

							if(selectedType > -1) {
								Type type = Type.GetType(grappleModuleTypes[selectedType]);
								Undo.RegisterCreatedObjectUndo(multiGrapple.gameObject.AddComponent(type), "Added " + type.Name);
							}
						}
						EditorGUILayout.EndVertical();

						foreach(GrappleModule module in multiGrapple.GetComponents<GrappleModule>())
							if(!grappleModules.ContainsKey(module)) {
								grappleModules[module] = CreateEditor(module);
								module.hideFlags = HideFlags.HideInInspector;
							}

						EditorGUI.indentLevel++;
						{
							GrappleModule removeModule = null;

							foreach(KeyValuePair<GrappleModule, Editor> module in grappleModules) {
								Editor editor = module.Value;

								EditorGUILayout.BeginVertical(GUI.skin.box);
								{
									EditorGUILayout.BeginHorizontal();
									{
										if(GUILayout.Button("X", GUILayout.ExpandWidth(false)))
											removeModule = module.Key;

										EditorGUILayout.LabelField("<color=white><b>" + module.Key.GetType().Name + "</b></color>", grappleTargetLabel, GUILayout.ExpandWidth(false));
										module.Key.enabled = EditorGUILayout.ToggleLeft(module.Key.enabled ? "Enabled" : "Disabled", module.Key.enabled, GUILayout.Width(80));
									}
									EditorGUILayout.EndHorizontal();

									if(removeModule != module.Key) {
										EditorGUI.indentLevel++;
										{
											editor.OnInspectorGUI();
										}
										EditorGUI.indentLevel--;
									}
								}
								EditorGUILayout.EndVertical();
							}

							if(removeModule) {
								DestroyImmediate(grappleModules[removeModule], true);
								grappleModules.Remove(removeModule);

								if(Application.isPlaying)
									Destroy(removeModule);
								else
									Undo.DestroyObjectImmediate(removeModule);
							}
						}
						EditorGUI.indentLevel--;
					}
					EditorGUILayout.EndFadeGroup();
				}
				EditorGUILayout.EndVertical();
			}
		}

		public static List<string> GetEnumerableOfType<T>() where T : class {
			List<string> objects = new List<string>();

			foreach(Type type in
				Assembly.GetAssembly(typeof(T)).GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)))) {
				objects.Add(type.AssemblyQualifiedName.ToString());
			}

			objects.Sort();
			return objects;
		}
	}
}