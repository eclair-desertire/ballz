using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class FreeLookCam : PivotBasedCameraRig {
	// This script is designed to be placed on the root object of a camera rig,
	// comprising 3 gameobjects, each parented to the next:

	// 	Camera Rig
	// 		Pivot
	// 			Camera

	[SerializeField]
	private float moveSpeed = 1f;      // How fast the rig will move to keep up with the target's position.
	[Range(0f, 10f)]
	[SerializeField]
	private float turnSpeed = 1.5f;    // How fast the rig will rotate from user input.
	[SerializeField]
	private float turnSmoothing = 0.1f;// How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
	[SerializeField]
	private float tiltMax = 75f;       // The maximum value of the x axis rotation of the pivot.
	[SerializeField]
	private float tiltMin = 45f;       // The minimum value of the x axis rotation of the pivot.
	[SerializeField]
	private bool lockCursor = false;   // Whether the cursor should be hidden and locked.

	[SerializeField]
	private float lookAngle;                            // The rig's y axis rotation.
	[SerializeField]
	private float tiltAngle;                            // The pivot's x axis rotation.

	private const float LookDistance = 100f;            // How far in front of the pivot the character's look target is.
	private float smoothX = 0;
	private float smoothY = 0;
	private float smoothXvelocity = 0;
	private float smoothYvelocity = 0;

	public float LookAngle {
		get { return lookAngle; }
		set {
			lookAngle = value;

			// Rotate the rig (the root object) around Y axis only:
			transform.rotation = Quaternion.Euler(0f, LookAngle, 0f);
		}
	}

	public float TiltAngle {
		get { return tiltAngle; }
		set {
			tiltAngle = value;

			// Tilt input around X is applied to the pivot (the child of this object)
			pivot.localRotation = Quaternion.Euler(TiltAngle, 0f, 0f);
		}
	}

	protected override void Awake() {
		base.Awake();
		// Lock or unlock the cursor.
		Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !lockCursor;

		// find the camera in the object hierarchy
		cam = GetComponentInChildren<Camera>().transform;
		pivot = cam.parent;

		//lookAngle = transform.eulerAngles.y;
		//tiltAngle = pivot.transform.eulerAngles.x;
	}

	protected override void Update() {
		base.Update();

		if(Input.GetKeyUp(KeyCode.Escape)) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		HandleRotationMovement();

		if(lockCursor && Input.GetMouseButtonUp(0)) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	void OnDisable() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	protected override void FollowTarget(float deltaTime) {
		// Move the rig towards target position.
		transform.position = Vector3.Lerp(transform.position, target.position, deltaTime * moveSpeed);
	}

	void HandleRotationMovement() {
		// Read the user input
#if CROSS_PLATFORM_INPUT
		var x = CrossPlatformInput.GetAxis ("Mouse X");
		var y = CrossPlatformInput.GetAxis ("Mouse Y");
#else
		var x = Input.GetAxis("Mouse X");
		var y = Input.GetAxis("Mouse Y");
#endif
		if(
			Cursor.lockState == CursorLockMode.None
		//!Screen.lockCursor ||
		//!(GUIUtility.hotControl == 0 &&
		//(EventSystem.current ? !EventSystem.current.IsPointerOverGameObject() : true))
		//(EventSystemManager.currentSystem ? !EventSystemManager.currentSystem.IsPointerOverEventSystemObject() : true))
		) {
			x = 0;
			y = 0;
		}

		// smooth the user input
		if(turnSmoothing > 0) {
			smoothX = Mathf.SmoothDamp(smoothX, x, ref smoothXvelocity, turnSmoothing);
			smoothY = Mathf.SmoothDamp(smoothY, y, ref smoothYvelocity, turnSmoothing);
		}
		else {
			smoothX = x;
			smoothY = y;
		}

		// Adjust the look angle by an amount proportional to the turn speed and horizontal input.
		LookAngle += smoothX * turnSpeed;

#if MOBILE_INPUT
		// For tilt input, we need to behave differently depending on whether we're using mouse or touch input:
		// on mobile, vertical input is directly mapped to tilt value, so it springs back automatically when the look input is released
		// we have to test whether above or below zero because we want to auto-return to zero even if min and max are not symmetrical.
		if (y>0) TiltAngle = Mathf.Lerp(0,-tiltMin, smoothY);
		if (y<=0) TiltAngle = Mathf.Lerp (0,tiltMax, -smoothY);
#else
		float angle = TiltAngle;

		// on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
		angle -= smoothY * turnSpeed;

		// and make sure the new value is within the tilt range
		TiltAngle = Mathf.Clamp(angle, -tiltMin, tiltMax);
#endif
	}

}