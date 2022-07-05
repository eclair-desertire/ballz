using UnityEngine;
using System.Collections;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Controls rolling the ball. Use the Orientation Transform object to decide which way is forward (it automatically uses Camera.main as default if nothing is set).
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Roll"), DisallowMultipleComponent]
	public class Roll : AbilityBase {

		[Tooltip("Stores the current surface normal if colliding. If not colliding a timer will count down and the surface normal will be set to a default normal upon reaching 0."), SerializeField]
		SurfaceStickDelay _stickyNormals = new SurfaceStickDelay(Vector3.zero, Vector3.up, .1f);

		[Tooltip("Torque multiplier"), SerializeField]
		float _rollSpeed = 1;

		[Tooltip("Use forces to move when in the air"), SerializeField]
		float _airSpeed = 1;

		[Header("Control Options")]
		[Tooltip("Only flip forward/backward when the surface is at a certain angle\n-90 = Ceiling\n90 = Floor"), Range(-90, 90), SerializeField]
		float _flipForwardBelowAngle = -90;

		[Tooltip("Different handling when the ball is on surface normals that are parallel to the Orientation Transform's viewing plane.\n\nEx. If you are looking parallel to the wall that you are on rather than straight at the wall while pressing up/down, you will spin in place. If enabled, you will properly move up and down the wall.\n\nBasically -\n 3D Movement - On\n 2D Movement - Off"), SerializeField]
		bool _surfaceNormalCompensation = true;

		[Tooltip("How fast the ball is allowed to spin"), SerializeField]
		float _maxAngularVelocity = 200;

		Vector2 _localTorqueInput;

		[Tooltip("Stop a rotation axis from receiving input"), SerializeField]
		LockVector3 _lockRotation;

		[Tooltip("Torque that will be applied to the ball"), SerializeField]
		Vector3 _torque;

		[Tooltip("Automatically reset the torque each physics frame in the FixedUpdate function"), SerializeField]
		bool _resetTorqueInFixedUpdate;

		[Tooltip("Use an alternate method to control the ball. The roll angle will be automatically calculated based on the surface normals and the Orientation Transform's direction."), SerializeField]
		bool _inputIsDirection;

		[Tooltip("The Transform to use for forward movement. If null, the main camera is used instead."), SerializeField]
		Transform _orientationTransform;

		[Tooltip("OrientationTransform will be set to the camera tagged as MainCamera on Awake."), SerializeField]
		bool useCameraAsOrientationOnAwake = true;

		Quaternion _orientation;

		#region Properties
		/// <summary>
		/// Use forces to move in the air.
		/// </summary>
		public float AirSpeed {
			get { return _airSpeed; }
			set { _airSpeed = value; }
		}

		/// <summary>
		/// <para>Only flip forward/backward when the surface is at a certain angle.</para>
		/// <para>-90 = Ceiling</para>
		/// <para>90 = Floor</para>
		/// <para>Usually when on surfaces that are upside-down (ceilings) and you try to roll forward, you will instead roll backwards. Setting this above -90 will flip the forward movement control and you will instead roll forward on ceilings instead of backwards. This behaviour will take into account the rotation of the <see cref="OrientationTransform"/> relative to the surface angle.</para>
		/// </summary>
		public float FlipForwardBelowAngle {
			get { return _flipForwardBelowAngle; }
			set { _flipForwardBelowAngle = value; }
		}

		/// <summary>
		/// Use an alternate method to control the ball. If enabled the roll angle will be automatically calculated based on the surface normals and the <see cref="OrientationTransform"/>'s direction (rotation).
		/// </summary>
		public bool InputIsDirection {
			get { return _inputIsDirection; }
			set { _inputIsDirection = value; }
		}

		/// <summary>
		/// Stop a rotation axis from receiving input.
		/// </summary>
		public LockVector3 LockRotation {
			get { return _lockRotation; }
			set { _lockRotation = value; }
		}

		public Vector2 LocalTorqueInput {
			get { return _localTorqueInput; }
			set { _localTorqueInput = value; }
		}

		/// <summary>
		/// How fast the ball is allowed to spin.
		/// </summary>
		public float MaxAngularVelocity {
			get { return _maxAngularVelocity; }
			set {
				_maxAngularVelocity = value;
				Rigidbody.maxAngularVelocity = _maxAngularVelocity;
			}
		}

		/// <summary>
		/// Determines which way is forward.
		/// </summary>
		public Quaternion Orientation {
			get {
				if(OrientationTransform)
					return OrientationTransform.rotation;
				else
					return _orientation;
			}
			set { _orientation = value; }
		}

		/// <summary>
		/// The Transform to use for forward movement.
		/// </summary>
		public Transform OrientationTransform {
			get { return _orientationTransform; }
			set { _orientationTransform = value; }
		}

		/// <summary>
		/// Automatically reset the torque each physics frame in the FixedUpdate function.
		/// </summary>
		public bool ResetTorqueInFixedUpdate {
			get { return _resetTorqueInFixedUpdate; }
			set { _resetTorqueInFixedUpdate = value; }
		}

		/// <summary>
		/// Torque multiplier.
		/// </summary>
		public float RollSpeed {
			get { return _rollSpeed; }
			set { _rollSpeed = value; }
		}

		/// <summary>
		/// Stores the current surface normal if colliding. If not colliding a timer will count down and the surface normal will be set to a default normal upon reaching 0.
		/// </summary>
		public SurfaceStickDelay StickyNormals {
			get { return _stickyNormals; }
			set { _stickyNormals = value; }
		}

		/// <summary>
		/// <para>Different handling when the ball is on surface normals that are parallel to the <see cref="OrientationTransform"/>'s viewing plane.</para>
		/// <para>(Ex. If you are looking along the wall that you are on rather than straight at the wall while pressing up/down, you will spin in place. If enabled, you will properly move up and down the wall.) </para>
		/// <para>Basically -\n 3D Movement - On\n 2D Movement - Off</para>
		/// </summary>
		public bool SurfaceNormalCompensation {
			get { return _surfaceNormalCompensation; }
			set { _surfaceNormalCompensation = value; }
		}

		/// <summary>
		/// Torque that will be applied to the ball.
		/// </summary>
		public Vector3 Torque {
			get { return _torque; }
			set { _torque = value; }
		}
		#endregion

		void OnEnable() {
			Rigidbody.maxAngularVelocity = MaxAngularVelocity;
		}

		void Awake() {
			if(useCameraAsOrientationOnAwake && Camera.main)
				OrientationTransform = Camera.main.transform;
		}

		protected override void Update() {
			base.Update();

			StickyNormals.Update();
		}

		void FixedUpdate() {
			if(IsUsing) {
				if(StickyNormals.IsReady) {
					Vector3 direction = Quaternion.LookRotation(Orientation * Vector3.right) * new Vector3(-LocalTorqueInput.y, 0, LocalTorqueInput.x);

					Rigidbody.AddForce(direction * AirSpeed);
				}

				Rigidbody.AddTorque(Vector3.ClampMagnitude(Torque, 1) * RollSpeed);

				if(ResetTorqueInFixedUpdate && LocalTorqueInput == Vector2.zero)
					if(Torque != Vector3.zero) ResetTorque();
			}
		}

		void OnCollisionStay() {
			StickyNormals.Delay();
		}

		/// <summary>
		/// Start rolling if the torque is greater than zero.
		/// </summary>
		public override void StartAbility() {
			if(Torque == Vector3.zero) {
				if(IsUsing) CancelAbility();
			}
			else
				base.StartAbility();
		}

		/// <summary>
		/// Roll forward using just the horizontal roll axis.
		/// </summary>
		public void StartAbility(float forward) {
			Vector3 direction = CalculateTorque(new Vector2(0, forward), InputIsDirection);

			SetTorque(direction);
			StartAbility();
		}

		/// <summary>
		/// Set the roll direction.
		/// </summary>
		public void StartAbility(Vector2 analogDirection) {
			Vector3 direction = CalculateTorque(analogDirection, InputIsDirection);

			SetTorque(direction);
			StartAbility();
		}

		/// <summary>
		/// Set the torque.
		/// </summary>
		/// <param name="torque">The amount of torque to be used.</param>
		public void StartAbility(Vector3 torque) {
			SetTorque(torque);
			StartAbility();
		}

		/// <summary>
		/// Set the horizontal axis input without resetting the vertical axis.
		/// </summary>
		public void StartAbilityHorizontalAxis(float horizontal) {
			_localTorqueInput.x = horizontal;

			Torque = CalculateTorque(Vector3.ClampMagnitude(LocalTorqueInput.normalized, 1), InputIsDirection);
			Torque = Vector3.ClampMagnitude(Torque, 1);

			StartAbility();
		}

		/// <summary>
		/// Set the vertical axis input without resetting the horizontal axis.
		/// </summary>
		public void StartAbilityVerticalAxis(float vertical) {
			_localTorqueInput.y = vertical;

			Torque = CalculateTorque(LocalTorqueInput.normalized, InputIsDirection);
			Torque = Vector3.ClampMagnitude(Torque, 1);

			StartAbility();
		}

		/// <summary>
		/// Set the torque.
		/// </summary>
		/// <param name="torque">The amount of torque to be used.</param>
		public void SetTorque(Vector3 torque) {
			Torque = torque;
		}

		/// <summary>
		/// Set the Torque to zero.
		/// </summary>
		[ContextMenu("Reset Torque")]
		public void ResetTorque() {
			Torque = Vector3.zero;
		}

		/// <summary>
		/// Set the current surface normals of StickyNormals if enabled.
		/// </summary>
		public void SetSurfaceNormals(Vector3 normals) {
			if(enabled) StickyNormals.SetValue(normals);
		}

		/// <summary>
		/// Get the direction that the ball is rolling.
		/// </summary>
		public Vector3 GetForwardDirection() {
			return Vector3.Cross(Orientation * Vector3.right, StickyNormals.Value);
		}

		/// <summary>
		/// Gets the direction of the ball based on angular velocity.
		/// </summary>
		public Vector3 GetRollDirection() {
			Vector3 direction = Rigidbody.angularVelocity.normalized;

			if(direction == Vector3.zero || StickyNormals.Value == Vector3.zero)
				return Vector3.zero;

			Quaternion angularDirection = Quaternion.LookRotation(Rigidbody.angularVelocity.normalized, StickyNormals.Value);
			return angularDirection * Vector3.left;
		}

		/// <summary>
		/// Converts an input axis to a torque axis based on the <see cref="OrientationTransform"/>'s direction.
		/// </summary>
		/// <param name="analogInput"></param>
		/// <param name="inputIsDirection">If false, the analogInput is meant to be read as the pitch and roll axis. If true, the analogInput is read as left/right and backward/forward.</param>
		/// <returns>Torque axis which can be used with AddTorque.</returns>
		public Vector3 CalculateTorque(Vector2 analogInput, bool inputIsDirection) {
			Vector3 axis = Vector3.zero;

			if(OrientationTransform) {
				Vector3 normals = LockRotation.SetLockedToZero(StickyNormals.Value);

				if(analogInput != Vector2.zero)
					if(inputIsDirection) {
						axis = new Vector3(-analogInput.x, 0, -analogInput.y);
						axis = Vector3.Cross(Orientation * axis, normals);

						Debug.DrawRay(transform.position, Orientation * axis, Color.red, 0, false);

						return axis;
					}
					else {
						Vector3 right = Orientation * Vector3.right;
						Quaternion look = Quaternion.LookRotation(right, normals);

						if(SurfaceNormalCompensation) {
							float relativeSurfaceX = Vector3.Dot(right, normals);

							look *= Quaternion.Euler(90 * relativeSurfaceX, 0, 0);
						}

						if(CollisionDetection.IsColliding)
							if((Orientation * normals).y * 90 < FlipForwardBelowAngle)
								look *= Quaternion.Euler(0, 180, 180);

						axis = look * new Vector3(analogInput.x, 0, analogInput.y);
					}
			}

			return LockRotation.SetLockedToZero(axis);
		}

		/// <summary>
		/// Toggles whether <see cref="InputIsDirection"/> is on or off.
		/// </summary>
		public void ToggleInputType() {
			InputIsDirection = !InputIsDirection;
		}

		[System.Serializable]
		public struct LockVector3 {
			public bool x;
			public bool y;
			public bool z;

			public LockVector3(bool x, bool y, bool z) {
				this.x = x;
				this.y = y;
				this.z = z;
			}

			public Vector3 SetLockedToZero(Vector3 inputVector) {
				return SetLockedTo(inputVector, Vector3.zero);
			}
			public Vector3 SetLockedTo(Vector3 inputVector, Vector3 setToVector) {
				if(x) inputVector.x = setToVector.x;
				if(y) inputVector.y = setToVector.y;
				if(z) inputVector.z = setToVector.z;

				return inputVector;
			}

			public Vector3 SetUnlockedToZero(Vector3 inputVector) {
				return SetUnlockedTo(inputVector, Vector3.zero);
			}
			public Vector3 SetUnlockedTo(Vector3 inputVector, Vector3 setToVector) {
				if(!x) inputVector.x = setToVector.x;
				if(!y) inputVector.y = setToVector.y;
				if(!z) inputVector.z = setToVector.z;

				return inputVector;
			}
		}
	}
}