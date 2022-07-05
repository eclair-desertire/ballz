using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace YounGenTech {
	/// <summary>
	/// A special class to define a single customizable input/control.
	/// </summary>
	[System.Serializable]
	public class Control {

		[SerializeField]
		string _name;

		[SerializeField]
		bool _enabled = true;

		[SerializeField]
		ControlInputType _controlType = ControlInputType.KeyCode;

		[SerializeField]
		ButtonPress _buttonPressType = ButtonPress.Axis;

		[Tooltip("Which events should be allowed to be fired"), SerializeField]
		ButtonEventTypes _allowedButtonEvents = new ButtonEventTypes(true, true, true, true);

		[Tooltip("This will be used for either control types and will override the KeyCode if that is used"), SerializeField]
		string _buttonName = "";

		[SerializeField]
		KeyCode _key;

		[Tooltip("Add to the Axis output before it is multiplied. Takes place before inversion too."), SerializeField]
		float _axisAdd;
		[Tooltip("Multiply the Axis output"), SerializeField]
		float _axisMultiplier = 1;

		[Tooltip("Flips the ControlAxis output"), SerializeField]
		bool _invert;

		[Tooltip("OnControl - Only sends OnControl events\nOnToggle - Only sends OnToggle events\nOnControlAndOnToggle - Sends both event types"), SerializeField]
		ControlEventType _eventType = ControlEventType.OnControl;

		[SerializeField]
		bool _toggleState;

		//[Header("Control Events")]
		[UnityEngine.Serialization.FormerlySerializedAs("OnControlAxis")]
		public ControlEvent onControlAxis;
		[UnityEngine.Serialization.FormerlySerializedAs("OnControlHold")]
		public UnityEvent onControlHold;
		[UnityEngine.Serialization.FormerlySerializedAs("OnControlDown")]
		public UnityEvent onControlDown;
		[UnityEngine.Serialization.FormerlySerializedAs("OnControlUp")]
		public UnityEvent onControlUp;

		//[Header("Toggle Events")]
		[Tooltip("Send a toggle event when a button is tapped or every frame when held")]
		[UnityEngine.Serialization.FormerlySerializedAs("OnToggleState")]
		public ToggleEvent onToggleState;
		[Tooltip("Send a toggle event when a button is tapped or every frame when held")]
		[UnityEngine.Serialization.FormerlySerializedAs("OnToggleStateTrue")]
		public UnityEvent onToggleStateTrue;
		[Tooltip("Send a toggle event when a button is tapped or every frame when held")]
		[UnityEngine.Serialization.FormerlySerializedAs("OnToggleStateFalse")]
		public UnityEvent onToggleStateFalse;

		[SerializeField,HideInInspector]
		float _controlAxis;
		[SerializeField, HideInInspector]
		bool _controlActive = false;

		#region Properties
		/// <summary>
		/// Which events should be allowed to be fired.
		/// </summary>
		public ButtonEventTypes AllowedButtonEvents {
			get { return _allowedButtonEvents; }
			set { _allowedButtonEvents = value; }
		}

		/// <summary>
		/// Add to the Axis output before it is multiplied. Takes place before inversion too.
		/// </summary>
		public float AxisAdd {
			get { return _axisAdd; }
			set { _axisAdd = value; }
		}

		/// <summary>
		/// Multiply the Axis output.
		/// </summary>
		public float AxisMultiplier {
			get { return _axisMultiplier; }
			set { _axisMultiplier = value; }
		}

		/// <summary>
		/// This will be used for either control types and will override the KeyCode if that is used.
		/// </summary>
		public string ButtonName {
			get { return _buttonName; }
			set { _buttonName = value; }
		}

		public ButtonPress ButtonPressType {
			get { return _buttonPressType; }
			set { _buttonPressType = value; }
		}

		/// <summary>
		/// Standard control events can be fired.
		/// </summary>
		bool CanDoControlEvent {
			get { return EventType == ControlEventType.OnControl || EventType == ControlEventType.OnControlAndOnToggle; }
		}

		/// <summary>
		/// Toggle control events can be fired.
		/// </summary>
		bool CanDoToggleEvent {
			get { return EventType == ControlEventType.OnToggle || EventType == ControlEventType.OnControlAndOnToggle; }
		}

		/// <summary>
		/// Output of all the other types of controls.
		/// </summary>
		public bool ControlActive {
			get { return _controlActive; }
			set { _controlActive = value; }
		}

		/// <summary>
		/// Output axis of the control/input. The output will be multiplied with AxisMultiplier.
		/// </summary>
		public float ControlAxis {
			get { return (Invert ? -(_controlAxis + AxisAdd) : (_controlAxis + AxisAdd)) * AxisMultiplier; }
			set { _controlAxis = value; }
		}

		public ControlInputType ControlType {
			get { return _controlType; }
			set { _controlType = value; }
		}

		/// <summary>
		/// If enabled, this control will be updated.
		/// </summary>
		public bool Enabled {
			get { return _enabled; }
			set { _enabled = value; }
		}

		/// <summary>
		/// Which control events should be fired.
		/// </summary>
		public ControlEventType EventType {
			get { return _eventType; }
			set { _eventType = value; }
		}

		/// <summary>
		/// Flips the ControlAxis output.
		/// </summary>
		public bool Invert {
			get { return _invert; }
			set { _invert = value; }
		}

		public KeyCode Key {
			get { return _key; }
			set { _key = value; }
		}

		/// <summary>
		/// A name to identify this control with.
		/// </summary>
		public string Name {
			get { return _name; }
			set { _name = value; }
		}

		public bool toggleState {
			get { return _toggleState; }
			set { _toggleState = value; }
		}
		#endregion

		public Control() { }
		public Control(string name, string buttonName) {
			this._name = name;
			this.ButtonName = buttonName;
		}
		public Control(string name, ControlInputType controlType, ButtonPress buttonPressType, string buttonName)
			: this(name, controlType, buttonName) {

			this.ButtonPressType = buttonPressType;
		}
		public Control(string name, ControlInputType controlType, string buttonName)
			: this(name, buttonName) {

			this.ControlType = controlType;
		}
		public Control(string name, ButtonPress buttonPressType, string buttonName)
			: this(name, buttonName) {

			this.ButtonPressType = buttonPressType;
		}
		public Control(string name, ButtonPress buttonPressType, KeyCode keyCode) {
			this._name = name;
			ControlType = ControlInputType.KeyCode;
			this.ButtonPressType = buttonPressType;
			this.Key = keyCode;
		}

		/// <summary>
		/// Updates the control values and events. (Use in Unity's Update function)
		/// </summary>
		public void UpdateControls() {
			if(!Enabled) {
				ControlActive = false;
				ControlAxis = 0;
				return;
			}

			bool previousControlActive = ControlActive;
			bool isUISelected = false;

			if(EventSystem.current && EventSystem.current.currentSelectedGameObject) {
				isUISelected = true;
				ControlActive = false;
			}

			if(!isUISelected)
				if(ControlType == ControlInputType.InputManager) {
					switch(ButtonPressType) {
						case ButtonPress.Axis: ControlAxis = Input.GetAxis(ButtonName); ControlActive = ControlAxis != 0; break;
						case ButtonPress.Hold: ControlActive = Input.GetButton(ButtonName); break;
						case ButtonPress.Tap: ControlActive = Input.GetButtonDown(ButtonName); break;
						case ButtonPress.Release: ControlActive = Input.GetButtonUp(ButtonName); break;
					}
				}
				else {
					if(ButtonName != "") {
						switch(ButtonPressType) {
							case ButtonPress.Axis:
							case ButtonPress.Hold:
								ControlActive = Input.GetKey(ButtonName);
								break;

							case ButtonPress.Tap: ControlActive = Input.GetKeyDown(ButtonName); break;
							case ButtonPress.Release: ControlActive = Input.GetKeyUp(ButtonName); break;
						}
					}
					else {
						switch(ButtonPressType) {
							case ButtonPress.Axis:
							case ButtonPress.Hold:
								ControlActive = Input.GetKey(Key);
								break;

							case ButtonPress.Tap: ControlActive = Input.GetKeyDown(Key); break;
							case ButtonPress.Release: ControlActive = Input.GetKeyUp(Key); break;
						}
					}
				}

			if(ControlActive) {	//ControlActive = True
				if(ControlType == ControlInputType.InputManager)
					if(onControlAxis != null && CanDoControlEvent && AllowedButtonEvents.axis)
						onControlAxis.Invoke(ControlAxis);

				if(onControlHold != null && CanDoControlEvent && AllowedButtonEvents.hold)
					onControlHold.Invoke();

				if(previousControlActive != ControlActive)
					if(AllowedButtonEvents.down) {
						if(onControlDown != null && CanDoControlEvent)
							onControlDown.Invoke();

						DoToggle();
					}

				if(AllowedButtonEvents.hold)
					DoToggle();
			}
			else {
				if(previousControlActive != ControlActive) {
					if(ControlType == ControlInputType.InputManager)
						if(onControlAxis != null && CanDoControlEvent && AllowedButtonEvents.axis)
							onControlAxis.Invoke(0);

					if(onControlUp != null && CanDoControlEvent && AllowedButtonEvents.up)
						onControlUp.Invoke();
				}
			}
		}

		/// <summary>
		/// Get the the axis of the control that ranges from -1 to 1.
		/// </summary>
		public float GetControlAxis() {
			if(ControlType == ControlInputType.InputManager)
				return Input.GetAxis(ButtonName);

			return 0;
		}

		/// <summary>
		/// Get if the control is being held down each frame.
		/// </summary>
		public bool GetControlHold() {
			if(ControlType == ControlInputType.InputManager) {
				if(Input.GetButton(ButtonName))
					return true;
			}
			else {
				if(ButtonName != "") {
					if(Input.GetKey(ButtonName))
						return true;
				}
				else {
					if(Input.GetKey(Key))
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Get if the control is tapped.
		/// </summary>
		public bool GetControlDown() {
			if(ControlType == ControlInputType.InputManager) {
				if(Input.GetButtonDown(ButtonName))
					return true;
			}
			else {
				if(ButtonName != "") {
					if(Input.GetKeyDown(ButtonName))
						return true;
				}
				else {
					if(Input.GetKeyDown(Key))
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Get if the control was released.
		/// </summary>
		public bool GetControlUp() {
			if(ControlType == ControlInputType.InputManager) {
				if(Input.GetButtonUp(ButtonName))
					return true;
			}
			else {
				if(ButtonName != "") {
					if(Input.GetKeyUp(ButtonName))
						return true;
				}
				else {
					if(Input.GetKeyUp(Key))
						return true;
				}
			}

			return false;
		}

		void DoToggle() {
			if(CanDoToggleEvent) {
				toggleState = !toggleState;

				if(onToggleState != null) onToggleState.Invoke(toggleState);

				if(toggleState) {
					if(onToggleStateTrue != null) onToggleStateTrue.Invoke();
				}
				else {
					if(onToggleStateFalse != null) onToggleStateFalse.Invoke();
				}
			}
		}
	}

	public enum ControlInputType {
		/// <summary>
		/// Use a button from the Input Manager.
		/// </summary>
		InputManager = 0,

		/// <summary>
		/// Use a specific button from the keyboard ex. KeyCode.A or "a".
		/// </summary>
		KeyCode = 1,
	}

	public enum ButtonPress {
		/// <summary>
		/// GetAxis - Can only be used with ControlType.InputManager | Will act as ButtonPressType.Hold if ControlType.KeyCode
		/// </summary>
		Axis,

		/// <summary>
		/// GetButton - GetKey
		/// </summary>
		Hold,

		/// <summary>
		/// GetButtonDown - GetKeyDown
		/// </summary>
		Tap,

		/// <summary>
		/// GetButtonUp - GetKeyUp
		/// </summary>
		Release
	}

	public enum ControlEventType {
		/// <summary>
		/// Only sends OnControl events
		/// </summary>
		OnControl = 0,

		/// <summary>
		/// Only sends OnToggle events
		/// </summary>
		OnToggle = 1,

		/// <summary>
		/// Sends both event types
		/// </summary>
		OnControlAndOnToggle = 2
	}

	[System.Serializable]
	public struct ButtonEventTypes {
		/// <summary>
		/// Enable axis input event.
		/// </summary>
		[Tooltip("Enable axis input event")]
		public bool axis;

		/// <summary>
		/// Enable button holding event.
		/// </summary>
		[Tooltip("Enable button holding event")]
		public bool hold;

		/// <summary>
		/// Enable button tap event.
		/// </summary>
		[Tooltip("Enable button tap event")]
		public bool down;

		/// <summary>
		/// Enable button release event.
		/// </summary>
		[Tooltip("Enable button release event")]
		public bool up;

		public ButtonEventTypes(bool axis, bool hold, bool down, bool up) {
			this.axis = axis;
			this.hold = hold;
			this.down = down;
			this.up = up;
		}
	}

	/// <summary>
	/// A UnityEvent with a float parameter.
	/// </summary>
	[System.Serializable]
	public class ControlEvent : UnityEvent<float> { }

	/// <summary>
	/// A UnityEvent with a bool parameter.
	/// </summary>
	[System.Serializable]
	public class ToggleEvent : UnityEvent<bool> { }
}