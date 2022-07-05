using UnityEngine;
using UnityEngine.EventSystems;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// <para>Customizable input script that fires events which you can hook up to functions in other components via the inspector or scripting.</para>
	/// <para>This script is only meant to be for prototyping as it may not support all the inputs that you need.</para>
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Input/Customizable Input"), DisallowMultipleComponent]
	public class BawlPhysicsInput : UserInputController {

		[SerializeField]
		bool _detectMouseLock = true;

		[Tooltip("Enable inputs and button presses"), SerializeField]
		bool _canControl = true;

		#region Properties
		/// <summary>
		/// Enable inputs and button presses.
		/// </summary>
		public bool CanControl {
			get { return _canControl; }
			set {
				bool before = _canControl;

				_canControl = value;

				if(!_canControl && _canControl != before)
					StopAbilities();
			}
		}

		/// <summary>
		/// Automatically lock the mouse when clicking on a part of the screen that doesn't have a UI element in the way.
		/// </summary>
		public bool DetectMouseLock {
			get { return _detectMouseLock; }
			set { _detectMouseLock = value; }
		}
		#endregion

		public BawlPhysicsInput() {
			controls = new Control[] {
				new Control("Horizontal", ControlInputType.InputManager, "Horizontal"),
				new Control("Vertical", ControlInputType.InputManager, "Vertical"),
				new Control("Sprint", ButtonPress.Hold, KeyCode.LeftShift),
				new Control("Jump", ControlInputType.InputManager, "Jump"),
				new Control("Boost", ButtonPress.Hold, KeyCode.LeftShift),
				new Control("Magnet", ButtonPress.Hold, KeyCode.V),
				new Control("Grapple", ButtonPress.Hold, KeyCode.Mouse0)
			};
		}

		protected override void Update() {
			if(!CanControl) return;

			base.Update();

			if(DetectMouseLock && Input.GetMouseButtonDown(0))
				if(GUIUtility.hotControl == 0 && !(EventSystem.current && EventSystem.current.IsPointerOverGameObject())) {
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
				}
		}

		void StopAbilities() {
			foreach(AbilityBase a in GetComponents<AbilityBase>())
				a.CancelAbility();
		}

		void OnValidate() {
			if(!CanControl)
				StopAbilities();
		}
	}
}