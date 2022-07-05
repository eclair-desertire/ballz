using UnityEngine;
using UnityEngine.Events;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Charge up and then launch forward.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Boost"), DisallowMultipleComponent]
	public class Boost : AbilityBase {

		static PhysicMaterial frictionlessPhysicMaterial;

		[Tooltip("Delays the use of this ability"), SerializeField]
		ActionDelay _actionDelay = new ActionDelay(.2f);

		[Tooltip("Stores the current surface normal if colliding. If not colliding a timer will count down and the surface normal will be set to a default normal upon reaching 0."), SerializeField]
		SurfaceStickDelay _stickyNormals = new SurfaceStickDelay(Vector3.zero, Vector3.up, .2f);

		[Tooltip("The speed of the boost"), SerializeField, Space(5)]
		float _boostPower = 15;

		[Header("Charge Options"), Tooltip("If enabled, the amount of boost power will be based on (Charge Time / Charge Length)"), SerializeField]
		bool _useCharge = true;

		[Tooltip("How long it takes to charge up the boost"), SerializeField]
		float _chargeLength = 1;

		[Tooltip("Current amount of charge time"), SerializeField]
		float _chargeTime;

		[Tooltip("The minimum amount of charge time required to boost"), SerializeField]
		float _minRequiredChargePercent = 0;

		[Tooltip("The angular velocity of the rigidbody will be set to zero upon starting the charge"), SerializeField, Space(5)]
		bool _resetAngularVelocityOnCharge = true;

		[Tooltip("Take into account the rigidbody's angular velocity when calculating the boost direction"), SerializeField]
		bool _angularVelocityIsDirection = true;

		[Tooltip("Ignores chargeLength and chargeTime and uses the rigidbody's angular velocity as the charge power. Boost power then acts as the maximum."), SerializeField]
		bool _angularVelocityIsChargePower = true;

		[Tooltip("Multiply the rigidbody's angular velocity when used as charge power"), SerializeField]
		float _angularVelocityChargeMultiplier = 1;

		[Tooltip("The ability will apply drag and a frictionless material when charging"), SerializeField, Space(5)]
		bool _chargeInPlace = true;

		[Tooltip("The amount of drag applied when charging"), SerializeField]
		float _drag = 1;

		[Tooltip("Only allow charging when colliding"), SerializeField, Space(5)]
		bool _chargeWhileColliding = true;

		[Tooltip("Only allow boosting when colliding"), SerializeField]
		bool _boostWhileColliding;

		[Tooltip("When there are no more collisions, cancel the ability"), SerializeField]
		bool _cancelAbilityOnCollisionExit;

		[Tooltip("The Transform to use for forward movement. If null, the main camera is used instead."), SerializeField, Space(5)]
		Transform _orientationTransform;

		[Tooltip("OrientationTransform will be set to the camera tagged as MainCamera on Awake."), SerializeField]
		bool useCameraAsOrientationOnAwake = true;

		public ChargeEvent OnChargeChanged;

		Quaternion _orientation;
		PhysicMaterial previousPhysicMaterial;

		#region Properties
		/// <summary>
		/// Delays the use of this ability.
		/// </summary>
		public ActionDelay ActionDelay {
			get { return _actionDelay; }
			set { _actionDelay = value; }
		}

		/// <summary>
		/// Take into account the rigidbody's angular velocity when calculating the boost direction.
		/// </summary>
		public bool AngularVelocityIsDirection {
			get { return _angularVelocityIsDirection; }
			set { _angularVelocityIsDirection = value; }
		}

		/// <summary>
		/// Ignores chargeLength and chargeTime and uses the rigidbody's angular velocity as the charge power. Boost power then acts as the maximum.
		/// </summary>
		public bool AngularVelocityIsChargePower {
			get { return _angularVelocityIsChargePower; }
			set { _angularVelocityIsChargePower = value; }
		}

		/// <summary>
		/// Multiply the rigidbody's angular velocity when used as charge power.
		/// </summary>
		public float AngularVelocityChargeMultiplier {
			get { return _angularVelocityChargeMultiplier; }
			set { _angularVelocityChargeMultiplier = value; }
		}

		/// <summary>
		/// The speed of the boost.
		/// </summary>
		public float BoostPower {
			get { return _boostPower; }
			set { _boostPower = value; }
		}

		/// <summary>
		/// Only allow boosting when colliding
		/// </summary>
		public bool BoostWhileColliding {
			get { return _boostWhileColliding; }
			set { _boostWhileColliding = value; }
		}

		/// <summary>
		/// The ability will apply drag and a frictionless material when charging
		/// </summary>
		public bool ChargeInPlace {
			get { return _chargeInPlace; }
			set { _chargeInPlace = value; }
		}

		/// <summary>
		/// Only allow charging when colliding.
		/// </summary>
		public bool ChargeWhileColliding {
			get { return _chargeWhileColliding; }
			set { _chargeWhileColliding = value; }
		}

		/// <summary>
		/// When there are no more collisions, cancel the ability.
		/// </summary>
		public bool CancelAbilityOnCollisionExit {
			get { return _cancelAbilityOnCollisionExit; }
			set { _cancelAbilityOnCollisionExit = value; }
		}

		/// <summary>
		/// Current amount of charge time.
		/// </summary>
		public float ChargeTime {
			get { return _chargeTime; }
			set {
				_chargeTime = value;

				if(OnChargeChanged != null) OnChargeChanged.Invoke(ChargePower);
			}
		}

		/// <summary>
		/// How long it takes to charge up the boost.
		/// </summary>
		public float ChargeLength {
			get { return _chargeLength; }
			set { _chargeLength = value; }
		}

		/// <summary>
		/// Returns a percentage value to act as a range between 0 and boostPower
		/// </summary>
		public float ChargePower {
			get {
				float chargePower = 0;

				if(AngularVelocityIsChargePower)
					chargePower = Mathf.Clamp(Rigidbody.angularVelocity.magnitude * AngularVelocityChargeMultiplier, 0, BoostPower) / BoostPower;
				else
					chargePower = ChargeTime / ChargeLength;

				return chargePower;
			}
		}

		/// <summary>
		/// The amount of drag applied when charging.
		/// </summary>
		public float Drag {
			get { return _drag; }
			set { _drag = value; }
		}

		/// <summary>
		/// The direction of the boost
		/// </summary>
		public Vector3 ForwardBoost {
			get {
				Vector3 forward = Vector3.zero;

				if(AngularVelocityIsDirection && !(Rigidbody.angularVelocity.sqrMagnitude < 9.999999E-11f)) {
					Quaternion angularDirection = Quaternion.LookRotation(Rigidbody.angularVelocity.normalized, StickyNormals.Value);
					forward = angularDirection * Vector3.left;
				}
				else {
					Quaternion look = Quaternion.LookRotation(Orientation * Vector3.right, StickyNormals.Value);

					forward = look * Vector3.forward;
					forward = new Vector3(-forward.z, forward.y, forward.x);

					Debug.DrawRay(transform.position, forward, Color.red, 5, false);
				}

				return forward;
			}
		}

		/// <summary>
		/// The minimum amount of charge time required to boost
		/// </summary>
		public float MinRequiredChargePercent {
			get { return _minRequiredChargePercent; }
			set { _minRequiredChargePercent = value; }
		}

		/// <summary>
		/// Determines which way is forward.
		/// </summary>
		public Quaternion Orientation {
			get {
				if(OrientationTransform)
					return OrientationTransform.rotation;
				else
					return Quaternion.identity;
			}
		}

		/// <summary>
		/// The Transform to use for forward movement.
		/// </summary>
		public Transform OrientationTransform {
			get {
				return _orientationTransform;
			}
			set {
				_orientationTransform = value;
			}
		}

		/// <summary>
		/// The angular velocity of the rigidbody will be set to zero upon starting the charge.
		/// </summary>
		public bool ResetAngularVelocityOnCharge {
			get { return _resetAngularVelocityOnCharge; }
			set { _resetAngularVelocityOnCharge = value; }
		}

		/// <summary>
		/// Stores the current surface normal if colliding. If not colliding a timer will count down and the surface normal will be set to a default normal upon reaching 0.
		/// </summary>
		public SurfaceStickDelay StickyNormals {
			get { return _stickyNormals; }
			set { _stickyNormals = value; }
		}

		/// <summary>
		/// If enabled, the amount of boost power will be based on (Charge Time / Charge Length).
		/// </summary>
		public bool UseCharge {
			get { return _useCharge; }
			set { _useCharge = value; }
		}
		#endregion


		void OnEnable() {
			if(!frictionlessPhysicMaterial) {
				frictionlessPhysicMaterial = new PhysicMaterial("Frictionless");

				frictionlessPhysicMaterial.dynamicFriction = 0;
				frictionlessPhysicMaterial.staticFriction = 0;

				frictionlessPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
				frictionlessPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
			}
		}

		void Awake() {
			if(useCameraAsOrientationOnAwake && Camera.main)
				OrientationTransform = Camera.main.transform;
		}

		protected override void Update() {
			base.Update();

			if(!IsUsing)
				ActionDelay.Update();

			if(CancelAbilityOnCollisionExit && IsUsing && CollisionDetection.StickyNormals.IsReady) {
				CancelAbility();
				SetFrictionlessMaterial(false);
			}

			if(IsUsing) {
				if(ChargeWhileColliding ? CollisionDetection.IsColliding : true)
					if(AngularVelocityIsChargePower) {
						ChargeTime = ChargePower;
					}
					else {
						if(ChargeTime < ChargeLength)
							ChargeTime = Mathf.Min(ChargeTime += Time.deltaTime, ChargeLength);
					}

				if(!ChargeInPlace) {
					if(GetComponent<Collider>().sharedMaterial == frictionlessPhysicMaterial)
						SetFrictionlessMaterial(false);
				}
				else {
					if(GetComponent<Collider>().sharedMaterial != frictionlessPhysicMaterial)
						SetFrictionlessMaterial(true);
				}
			}

			StickyNormals.Update();
		}

		void FixedUpdate() {
			if(IsUsing && CollisionDetection.IsColliding) 
				Rigidbody.velocity -= Rigidbody.velocity * Drag * Time.deltaTime;
		}

		void OnCollisionStay(Collision hit) {
			StickyNormals.Delay();
		}

		/// <summary>
		/// Start charging the boost.
		/// </summary>
		public override void StartAbility() {
			if(ActionDelay.IsReady && IsReady) {
				if(CancelAbilityOnCollisionExit && CollisionDetection.StickyNormals.IsReady)
					return;

				ChargeTime = 0;

				if(ResetAngularVelocityOnCharge)
					Rigidbody.angularVelocity = Vector3.zero;

				SetFrictionlessMaterial(true);

				IsUsing = true;
			}
		}

		/// <summary>
		/// Apply the boost.
		/// </summary>
		public override void FinishAbility() {
			if(IsUsing) {
				if((BoostWhileColliding ? CollisionDetection.IsColliding : true) && ChargePower >= MinRequiredChargePercent) {
					DoBoost();

					base.FinishAbility();
					if(!IsUsing) SetFrictionlessMaterial(false);
				}
				else
					CancelAbility();

				ChargeTime = 0;
			}
		}

		/// <summary>
		/// Cancel the ability without boosting.
		/// </summary>
		public override void CancelAbility() {
			if(IsUsing) {
				base.CancelAbility();

				ChargeTime = 0;
				SetFrictionlessMaterial(false);
			}
		}

		void DoBoost() {
			if(!ActionDelay.IsReady || ChargePower < MinRequiredChargePercent) return;

			Rigidbody.AddForce(ForwardBoost * BoostPower * ChargePower, ForceMode.Impulse);

			ActionDelay.Delay();
		}

		void SetFrictionlessMaterial(bool enable) {
			if(enable) {
				if(GetComponent<Collider>().sharedMaterial != frictionlessPhysicMaterial) {
					previousPhysicMaterial = GetComponent<Collider>().sharedMaterial;
					GetComponent<Collider>().sharedMaterial = frictionlessPhysicMaterial;
				}
			}
			else
				GetComponent<Collider>().sharedMaterial = previousPhysicMaterial;
		}

		/// <summary>
		/// Set the current surface normals of StickyNormals if enabled.
		/// </summary>
		public void SetSurfaceNormals(Vector3 normals) {
			if(enabled) StickyNormals.SetValue(normals);
		}
	}

	/// <summary>
	/// A UnityEvent with a float parameter.
	/// </summary>
	[System.Serializable]
	public class ChargeEvent : UnityEvent<float> { }
}