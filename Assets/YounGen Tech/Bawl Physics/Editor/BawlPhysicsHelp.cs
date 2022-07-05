using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System;

namespace YounGenTech.BawlPhysics {
	public class BawlPhysicsHelp : EditorWindow {
		//const string QuickStartFoldoutPref = "BawlPhysics-Help-QuickStartFoldout";
		//const string PrefabsFoldoutPref = "BawlPhysics-Help-PrefabsFoldout";

		static GUIStyle _wordWrappedLabel = null;
		static GUIStyle WordWrappedLabel {
			get {
				if(_wordWrappedLabel == null) {
					_wordWrappedLabel = GUI.skin.label;
					_wordWrappedLabel.wordWrap = true;
					_wordWrappedLabel.richText = true;
				}

				return _wordWrappedLabel;
			}
		}
		static string[] toolbar = new string[] { "Scenes", "Docs", "Prefabs", "Abilities", "Collision", "Effects", "Input", "Support" };

		public Texture2D surfaceDetectionImage;

		int toolbarSelected = 0;
		Vector2 scrollbar = new Vector2();

		[MenuItem("Window/Bawl Physics Help")]
		static void Init() {
			GetWindow<BawlPhysicsHelp>("Bawl Physics", true);
		}

		void OnGUI() {
			toolbarSelected = GUILayout.Toolbar(toolbarSelected, toolbar, GUILayout.ExpandWidth(false));

			scrollbar = EditorGUILayout.BeginScrollView(scrollbar);
			{
				switch(toolbar[toolbarSelected]) {
					case "Abilities": AbilitiesGUI(); break;
					case "Collision": CollisionGUI(); break;
					case "Effects": EffectsGUI(); break;
					case "Input": InputGUI(); break;
					case "Prefabs": PrefabsGUI(); break;
					case "Scenes": ScenesGUI(); break;
					case "Docs": ScriptingDocumentationGUI(); break;
					case "Support": SupportGUI(); break;
				}
			}
			EditorGUILayout.EndScrollView();
		}

		//void QuickStartGUI() {
		//	prefabsInfo.DrawGUI();
		//}

		void AbilitiesGUI() {
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Boost", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Abilities/Boost.cs"));

				EditorGUILayout.LabelField("Launch the ball. There are many customizable options for charging and launching.", WordWrappedLabel);
				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("<size=14><b>How to use</b></size>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>StartAbility()</b> begins the charging process (if charging is enabled).", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>FinishAbility()</b> will apply the boost and end the ability.", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>CancelAbility()</b> ends the ability without applying boost.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Field", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Abilities/Field.cs"));

				EditorGUILayout.LabelField("Applies a force to nearby rigidbodies to push/pull them.", WordWrappedLabel);
				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("<size=14><b>How to use</b></size>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>StartAbility()</b> will begin using the ability. Forces will be applied on the first <b>StartAbility()</b> call and each frame that the ability is in use.", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>FinishAbility</b> and <b>CancelAbility()</b> end the ability in the same way. No special actions happen.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Grapple", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Abilities/Grapple.cs"));

				EditorGUILayout.LabelField("Grapple to multiple targets that can be set using custom scripts and pre-made <b>Grapple Modules</b>. Set up grapple targets in the ability or through custom <b>Grapple Module</b>s.", WordWrappedLabel);
				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("<size=14><b>How to use</b></size>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>StartAbility()</b> enables all the grapple targets that have been set. <b>Note:</b> You can setup multiple grapple targets before starting the ability.", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>FinishAbility()</b> and <b>CancelAbility()</b> will both end the ability, disable the grapple targets and reset the target variable of the grapple target.", WordWrappedLabel);
				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("You can start the <b>Grapple</b> using <b>Raycast Grapple</b> and <b>Find Grapple Point</b> which are <b>Grapple Modules</b>.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Jump", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Abilities/Jump.cs"));

				EditorGUILayout.LabelField("Jump as many times as you want! Also wall-jumping can be done with this ability.", WordWrappedLabel);
				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("<size=14><b>How to use</b></size>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>StartAbility()</b> will start the jump and end the ability. The jump count will go back to 0 when you touch a surface.", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>FinishAbility()</b> you don't have to worry about calling. It is called automatically from within <b>StartAbility()</b>.", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>CancelAbility()</b> does nothing.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Magnet", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Abilities/Magnet.cs"));

				EditorGUILayout.LabelField("Stick to surfaces (only sticks to one surface at a time)! It uses a joint to keep you stuck to the surface. You can use the <b>Magnet Info Override</b> component on the surface to override the power of the magnet.", WordWrappedLabel);
				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("<size=14><b>How to use</b></size>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>StartAbility()</b> starts the ability, turns off the rigidbody's gravity, creates a joint then begins automatically searching for magnetic surfaces to stick to.", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>FinishAbility()</b> and <b>CancelAbility()</b> end the ability, set the rigidbody's gravity back on and destroys the joint.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Roll", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Abilities/Roll.cs"));

				EditorGUILayout.LabelField("Rolling movement with a few extras.", WordWrappedLabel);
				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("<size=14><b>How to use</b></size>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>StartAbility(...)</b> and any of its variations, or <b>StartAbilityHorizontalAxis()</b> and <b>StartAbilityVerticalAxis()</b> will calculate (based on input, surface normals and orientation) the torque and start the ability.", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>FinishAbility()</b> or <b>CancelAbility</b> will stop the ability but will not reset the torque input. Torque input will only be applied when the ability is being used.", WordWrappedLabel);
				EditorGUILayout.LabelField("The horizontal axis is the local rotation x axis\nThe vertical axis is the local rotation z axis", WordWrappedLabel);
				EditorGUILayout.LabelField("If you wish, the torque can be set manually using <b>SetTorque(Vector3)</b>.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Sprint", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Abilities/Sprint.cs"));

				EditorGUILayout.LabelField("It's a fairly simple ability that does exactly what you'd expect a sprint ability to do. It requires the <b>Roll</b> ability.", WordWrappedLabel);
				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("<size=14><b>How to use</b></size>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>StartAbility()</b> adds the sprint speed to the roll speed.", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>FinishAbility()</b> and <b>CancelAbility()</b> end the ability and subtract the sprint speed from the roll speed.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();
		}

		void CollisionGUI() {
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Bawl Physics Surface Detection", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Collision/BawlPhysicsSurfaceDetection.cs"));

				Rect surfaceDetectionRect = GUILayoutUtility.GetRect(479, 342);

				GUI.color = Color.white;
				GUI.DrawTexture(surfaceDetectionRect, surfaceDetectionImage, ScaleMode.ScaleToFit, false);

				EditorGUILayout.LabelField("Calculates an average surface normal from all of the collisions in the Surface Detection Mask. When the normals are updated, the <b>On Updated Average Normals</b> event will fired. A few of the abilities require their surface normals to be set or else they will default to using up as their surface normal. This is done for performance reasons. ", WordWrappedLabel);
				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("These are the abilities that require their surface normals to be set via their <b>SetSurfaceNormals()</b> function.", WordWrappedLabel);
				EditorGUILayout.LabelField("-<b>Boost</b>", WordWrappedLabel);
				EditorGUILayout.LabelField("-<b>Jump</b>", WordWrappedLabel);
				EditorGUILayout.LabelField("-<b>Roll</b>", WordWrappedLabel);

				EditorGUILayout.LabelField("If you wish to use your own method of collision detection then feel free to inherit from <b>CollisionDetectionBase</b> and customize it as you see fit.", WordWrappedLabel);

			}
			EditorGUILayout.EndVertical();
		}

		void EffectsGUI() {
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Bawl Alpha Clipper", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Effects/BawlAlphaClipper.cs"));

				EditorGUILayout.LabelField("This will control the transparency of the ball when the camera gets close to it. You can set the regular material that will be used when it is not near and the transparent material to be used when it is. The transparency amount will be set based on the distance from the camera to the center of the ball.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Bawl Physics Sound Controller", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Effects/BawlPhysicsSoundController.cs"));

				if(GUILayout.Button("Bawl Physics Effects", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Effects/BawlPhysicsEffects.cs"));

				EditorGUILayout.HelpBox("BawlPhysicsSoundController and BawlPhysicsEffects were only meant to be used with the Fancy Ball and were not designed for use in every type of game.They were created for demoing the ball and Bawl Physics level.", MessageType.Warning);
			}
			EditorGUILayout.EndVertical();
		}

		void InputGUI() {
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Bawl Physics Input", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Input/BawlPhysicsInput.cs"));

				EditorGUILayout.HelpBox("This input system was only intended for temporary (but quick) prototyping use and was not tested for performance.", MessageType.Warning);
				EditorGUILayout.LabelField("With this input system you can hook up functions from other scripts to fire when different input events happen.", WordWrappedLabel);

				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("These are the same as using the functions from the Input class like GetAxis(), GetButton(), GetKey()", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>-OnControlAxis(float)</b>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>-OnControlHold</b>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>-OnControlDown</b>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>-OnControlUp</b>", WordWrappedLabel);

				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("You can set a control to toggle a boolean value instead of just getting input events.", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>-OnToggleState(boolean)</b>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>-OnToggleStateTrue</b>", WordWrappedLabel);
				EditorGUILayout.LabelField("<b>-OnToggleStateFalse</b>", WordWrappedLabel);

				EditorGUILayout.LabelField("", WordWrappedLabel);
				EditorGUILayout.LabelField("You can either get input from the Input Manager or specify a KeyCode.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Bawl Physics Mobile Input", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Input/BawlPhysicsMobileInput.cs"));

				EditorGUILayout.LabelField("Tilt controls for mobile. Hook up the <b>OnInputUpdated</b> event into the <b>Roll</b> ability's <b>SetAbility(Vector2)</b> function to get the ball rollin'!", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Bawl Physics Simple Input", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scripts/Input/BawlPhysicsSimpleInput.cs"));

				EditorGUILayout.LabelField("Example input script showing how you can start the <b>Roll</b> and <b>Jump</b> abilities from script.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();
		}

		void PrefabsGUI() {
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Basic Ball Prefab", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Prefabs/Bawls/Basic Bawl.prefab"));

				EditorGUILayout.LabelField("It doesn't contain any special graphics or effects. It has a Roll and Jump ability along with the Bawl Physics Input and Bawl Physics Surface Detection scripts to get you started.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Fancy Ball Prefab", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Prefabs/Bawls/Fancy Bawl.prefab"));

				EditorGUILayout.LabelField("All of the abilities are on it, hooked up and ready to go.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Camera Prefab", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Sample Assets/Cameras/Prefabs/Free Look Camera Rig.prefab"));

				EditorGUILayout.LabelField("All that's left is to drag in your favorite camera controller or you can use this one from Unity's own Sample Assets", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();
		}

		void ScenesGUI() {
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Bawl Physics Level", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Scenes/Bawl Physics Level.unity"));

				EditorGUILayout.LabelField("A sandbox level to test out all of the Bawl Physics abilities! There's a magnet tower to test the Magnet ability, a bunch of metal trees to test the Grapple, a large world (and a loop) to test the Sprint and Boost, physics rocks to test the Field ability, lots of things to Jump off of and literally EVERYTHING to Roll over.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();
		}

		void ScriptingDocumentationGUI() {
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				if(GUILayout.Button("Scripting Documentation", GUILayout.ExpandWidth(false)))
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/YounGen Tech/Bawl Physics/Documentation/Bawl Physics 2.0 Scripting Documentation.chm"));

				EditorGUILayout.LabelField("Find out how the code is meant to work! Tip: You can also hover over variables in the inspector for tooltips.", WordWrappedLabel);
			}
			EditorGUILayout.EndVertical();
		}

		void SupportGUI() {
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Website - ", GUILayout.ExpandWidth(false));

				if(GUILayout.Button("http://www.youngentech.com", GUILayout.ExpandWidth(false)))
					Application.OpenURL("http://www.youngentech.com");
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Forum Thread - ", GUILayout.ExpandWidth(false));

				if(GUILayout.Button("http://forum.unity3d.com/threads/80548/", GUILayout.ExpandWidth(false)))
					Application.OpenURL("http://forum.unity3d.com/threads/80548/");
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Email (bugs, questions or comments!) - ", GUILayout.ExpandWidth(false));

				if(GUILayout.Button("support@youngentech.com", GUILayout.ExpandWidth(false)))
					Application.OpenURL("http://youngentech.com/contact");
			}
			GUILayout.EndHorizontal();
		}
	}
}