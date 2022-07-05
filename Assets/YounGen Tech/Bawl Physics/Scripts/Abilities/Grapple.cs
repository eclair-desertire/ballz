using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Target an object and swing away. The joint will move along with the object that it is attached to.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Grapple"), DisallowMultipleComponent]
	public class Grapple : AbilityBase {
		//***Starting a grapple too close to the ball will cause weird physics effects.
		[SerializeField, Tooltip("Delays the use of this ability")]
		ActionDelay _actionDelay = new ActionDelay();

		[SerializeField]
		int _maxGrappleTargets = 1;

		[SerializeField]
		float _defaultSpring;

		[SerializeField]
		float _defaultDamper;

		[SerializeField]
		float _defaultBounciness;

		[SerializeField]
		float _defaultAddLength;

		List<GrappleTarget> targets = new List<GrappleTarget>();

		public GrappleEvent onGrappleChanged;

		#region Properties
		/// <summary>
		/// Delays the use of this ability.
		/// </summary>
		public ActionDelay ActionDelay {
			get { return _actionDelay; }
			set { _actionDelay = value; }
		}

		/// <summary>
		/// Default length to add on to the existing length of the grapple joint.
		/// </summary>
		public float DefaultAddLength {
			get { return _defaultAddLength; }
			set { _defaultAddLength = value; }
		}

		/// <summary>
		/// Default bounciness value for the grapple joint.
		/// </summary>
		public float DefaultBounciness {
			get { return _defaultBounciness; }
			set { _defaultBounciness = value; }
		}

		/// <summary>
		/// Default damper value for the grapple joint.
		/// </summary>
		public float DefaultDamper {
			get { return _defaultDamper; }
			set { _defaultDamper = value; }
		}

		/// <summary>
		/// Default spring value for the grapple joint.
		/// </summary>
		public float DefaultSpring {
			get { return _defaultSpring; }
			set { _defaultSpring = value; }
		}

		public int MaxGrappleTargets {
			get { return _maxGrappleTargets; }
			set {
				_maxGrappleTargets = value;

				if(targets.Count < MaxGrappleTargets) {
					GrappleTarget[] addTargets = new GrappleTarget[MaxGrappleTargets - targets.Count];

					for(int i = 0; i < addTargets.Length; i++) {
						addTargets[i] = new GrappleTarget(gameObject);

						addTargets[i].AddLength = DefaultAddLength;
						addTargets[i].Bounciness = DefaultBounciness;
						addTargets[i].Damper = DefaultDamper;
						addTargets[i].Spring = DefaultSpring;

						addTargets[i].OnGrappleChanged.AddListener(onGrappleChanged.Invoke);
					}

					targets.AddRange(addTargets);
				}
				else if(targets.Count > MaxGrappleTargets) {
					for(int i = MaxGrappleTargets - 1; i < targets.Count; i++)
						targets[i].Destroy();

					targets.RemoveRange(targets.Count - 1, targets.Count - MaxGrappleTargets);
				}
			}
		}
		#endregion

		public GrappleTarget this[int index] {
			get { return targets != null ? targets[index] : null; }
		}

		void Awake() {
			MaxGrappleTargets = MaxGrappleTargets;
			onGrappleChanged.AddListener(s => FinishIfNotTargeting(s));
		}

		protected override void Update() {
			base.Update();

			UpdateGrappleTargets();
			ActionDelay.Update();
		}

		void OnDrawGizmos() {
			Gizmos.color = Color.red;

			foreach(GrappleTarget target in targets) {
				if(target.Target) {
					Gizmos.DrawLine(transform.position, target.Target.TransformPoint(target.LocalAttachPoint));
				}
			}
		}

		protected override void OnValidate() {
			base.OnValidate();

			if(Application.isPlaying)
				MaxGrappleTargets = MaxGrappleTargets;
		}

		/// <summary>
		/// Creates a joint to swing on if a target has been set and the ability is ready.
		/// </summary>
		public override void StartAbility() {
			if(ActionDelay.IsReady && IsReady) {
				ActionDelay.Delay();

				foreach(GrappleTarget grappleTarget in targets)
					grappleTarget.Enabled = true;

				IsUsing = true;
			}
		}

		/// <summary>
		/// Set the target, its attach point and start the ability.
		/// </summary>
		public void StartAbility(Transform target, Vector3 attachPoint, Space attachSpace, int grappleTargetIndex = 0) {
			if(ActionDelay.IsReady)
				if(target) {
					SetTarget(target, attachPoint, attachSpace, grappleTargetIndex);
					StartAbility();
				}
		}

		/// <summary>
		/// Stop grappling.
		/// </summary>
		public override void FinishAbility() {
			if(IsUsing) {
				base.FinishAbility();

				if(!IsUsing)
					StopGrapple();
			}
		}

		/// <summary>
		/// Cancel the ability.
		/// </summary>
		public override void CancelAbility() {
			base.CancelAbility();
			StopGrapple();
		}

		public void ClearTarget(int grappleTargetIndex) {
			targets[grappleTargetIndex].Target = null;
			targets[grappleTargetIndex].Enabled = false;

			if(!IsTargetting())
				FinishAbility();
		}
		public void ClearTargets() {
			foreach(GrappleTarget grappleTarget in targets) {
				grappleTarget.Target = null;
				grappleTarget.Enabled = false;
			}
		}

		void FinishIfNotTargeting(GrappleTarget grappleTarget) {
			if(grappleTarget != null && grappleTarget.Target)
				ActionDelay.Delay();
			else if(!IsTargetting())
				FinishAbility();
		}

		void UpdateGrappleTargets() {
			foreach(GrappleTarget a in targets)
				a.Update();
		}

		/// <summary>
		/// Disables each grapple target and removes the target
		/// </summary>
		void StopGrapple() {
			foreach(GrappleTarget grappleTarget in targets) {
				grappleTarget.Enabled = false;
				grappleTarget.Target = null;
			}
		}

		/// <summary>
		/// Sets the grapple target.
		/// </summary>
		/// <param name="target">The Transform that the grapple point will be on. If it has a Rigidbody, the joint will set it as the connectedBody.</param>
		//// <param name="attachPoint">Offset of the grapple point from the target.</param>
		//// <param name="attachPointSpace">Select whether the given attachPoint is in world space or local space.</param>
		public GrappleTarget SetTarget(Transform target, int grappleTargetIndex = -1) {
			if(targets.Count == 0 || grappleTargetIndex > targets.Count) {
				Debug.LogError("SetTarget index out of range");
				return null;
			}

			GrappleTarget grappleTarget = grappleTargetIndex < 0 ? GetTarget() : targets[grappleTargetIndex];

			if(grappleTarget.Target != target) {
				grappleTarget.Bounciness = DefaultBounciness;
				grappleTarget.Damper = DefaultDamper;
				grappleTarget.Spring = DefaultSpring;
				grappleTarget.Target = target;
				grappleTarget.RecalculateLength();
				grappleTarget.AddLength = DefaultAddLength;

				if(IsUsing) grappleTarget.Enabled = true;
			}

			return grappleTarget;
		}

		public GrappleTarget SetTarget(Transform target, Vector3 attachPoint, Space attachSpace, int grappleTargetIndex = -1) {
			GrappleTarget grappleTarget = SetTarget(target, grappleTargetIndex);

			if(grappleTarget != null)
				if(attachSpace == Space.Self)
					grappleTarget.LocalAttachPoint = attachPoint;
				else
					grappleTarget.WorldAttachPoint = attachPoint;

			return grappleTarget;
		}

		public GrappleTarget GetTarget(int grappleTargetIndex) {
			return targets[grappleTargetIndex];
		}

		/// <summary>
		/// Get a GrappleTarget with the specific Transform target.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public GrappleTarget GetTarget(Transform target = null) {
			foreach(GrappleTarget grappleTarget in targets)
				if(grappleTarget.Target == target) return grappleTarget;

			return null;
		}

		/// <summary>
		/// Check if a specific Transform is being targeted by this ability.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public bool IsTargetting(Transform target) {
			if(!target) return false;

			for(int i = 0; i < targets.Count; i++)
				if(targets[i].Target == target) return true;

			return false;
		}

		/// <summary>
		/// Check if there are any grapple targets targetting anything.
		/// </summary>
		public bool IsTargetting() {
			for(int i = 0; i < targets.Count; i++)
				if(targets[i].Target) return true;

			return false;
		}

		void RemoveTarget(int grappleTargetIndex) {
			if(targets.Count > 0 && grappleTargetIndex > 0 && grappleTargetIndex < targets.Count) {
				targets[grappleTargetIndex].Destroy();
				targets.RemoveAt(grappleTargetIndex);
			}
		}

		[System.Serializable]
		public class GrappleTarget : IGrappleLinearLimit {
			bool _enabled;
			float _addLength;
			float _length;
			Vector3 _localAttachPoint;
			Transform _target;
			Rigidbody _targetRigidbody;
			SphereCollider _sphereCollider;

			public GrappleEvent OnGrappleChanged;

			#region Properties
			/// <summary>
			/// Amount to add to the length of the joint.
			/// </summary>
			public float AddLength {
				get { return _addLength; }
				set {
					_addLength = Mathf.Max(value, -Length);

					SoftJointLimit linearLimit = Joint.linearLimit;
					linearLimit.limit = Mathf.Max(AddLength + Length, Mathf.Epsilon);
					Joint.linearLimit = linearLimit;
				}
			}

			/// <summary>
			/// Bounciness of the joint.
			/// </summary>
			public float Bounciness {
				get { return Joint.linearLimit.bounciness; }
				set {
					SoftJointLimit linearLimit = Joint.linearLimit;
					linearLimit.bounciness = value;
					Joint.linearLimit = linearLimit;
				}
			}

			/// <summary>
			/// Damper of the joint.
			/// </summary>
			public float Damper {
				get { return Joint.linearLimitSpring.damper; }
				set {
					SoftJointLimitSpring linearLimitSpring = Joint.linearLimitSpring;
					linearLimitSpring.damper = value;
					Joint.linearLimitSpring = linearLimitSpring;
				}
			}

			/// <summary>
			/// Enables/disables the joint's physical effects on the target and player.
			/// </summary>
			public bool Enabled {
				get { return _enabled; }
				set {
					if(value != Enabled) {
						_enabled = value;

						if(Target && OnGrappleChanged != null)
							OnGrappleChanged.Invoke(this);
					}

					if(Target) {
						Joint.xMotion = Joint.yMotion = Joint.zMotion =
						(value ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free);
					}
				}
			}

			/// <summary>
			/// Joint that controls the grapple physics.
			/// </summary>
			public ConfigurableJoint Joint { get; private set; }

			/// <summary>
			/// Transform that the Joint will use as the connected body if the target does not have one.
			/// </summary>
			Transform JointObject { get; set; }

			/// <summary>
			/// Length of the joint.
			/// </summary>
			public float Length {
				get { return _length; }
				set {
					_length = value;
					_addLength = Mathf.Max(_addLength, -Length);

					SoftJointLimit linearLimit = Joint.linearLimit;
					linearLimit.limit = Mathf.Max(AddLength + Length, Mathf.Epsilon);
					Joint.linearLimit = linearLimit;
				}
			}

			/// <summary>
			/// Pivot point relative to the target.
			/// </summary>
			public Vector3 LocalAttachPoint {
				get { return _localAttachPoint; }
				set {
					_localAttachPoint = value;

					if(Target)
						JointObject.position = Target.transform.TransformPoint(LocalAttachPoint);

					Joint.connectedAnchor = TargetRigidbody ? _localAttachPoint : Vector3.zero;
				}
			}

			/// <summary>
			/// The ConfigurableJoint will be added to this object.
			/// </summary>
			public GameObject Source { get; private set; }

			SphereCollider Collider {
				get {
					if(!_sphereCollider) _sphereCollider = Joint ? Joint.GetComponent<SphereCollider>() : null;
					return _sphereCollider;
				}
			}

			/// <summary>
			/// Springiness of the joint.
			/// </summary>
			public float Spring {
				get { return Joint.linearLimitSpring.spring; }
				set {
					SoftJointLimitSpring linearLimitSpring = Joint.linearLimitSpring;
					linearLimitSpring.spring = value;
					Joint.linearLimitSpring = linearLimitSpring;
				}
			}

			/// <summary>
			/// Grapple target. Use LocalAttachPoint to define the pivot point of the grapple.
			/// </summary>
			public Transform Target {
				get { return _target; }
				set {
					_target = value;

					if(Target && TargetRigidbody) {
						if(Enabled && Joint.connectedBody != TargetRigidbody && OnGrappleChanged != null) {
							OnGrappleChanged.Invoke(this);
							Debug.Log("Changed1");
						}

						Joint.connectedBody = TargetRigidbody;
						Joint.connectedAnchor = LocalAttachPoint;
					}
					else {
						if(JointObject) {
							Rigidbody jointObjectRigidbody = JointObject.GetComponent<Rigidbody>();

							if(Enabled && OnGrappleChanged != null)
								OnGrappleChanged.Invoke(this);

							Joint.connectedBody = jointObjectRigidbody;
							Joint.connectedAnchor = Vector3.zero;
						}
					}

					if(Target) {
						Joint.xMotion = Joint.yMotion = Joint.zMotion =
						(Enabled ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free);
					}
					else
						Joint.xMotion = Joint.yMotion = Joint.zMotion = ConfigurableJointMotion.Free;
				}
			}

			/// <summary>
			/// Rigidbody of the target, if it has one.
			/// </summary>
			public Rigidbody TargetRigidbody {
				get {
					if(Target)
						if(!_targetRigidbody || _targetRigidbody.transform != Target)
							_targetRigidbody = Target.GetComponent<Rigidbody>();

					return _targetRigidbody;
				}
			}

			/// <summary>
			/// The position of the local attach point in worldspace.
			/// </summary>
			public Vector3 WorldAttachPoint {
				get { return Target ? Target.TransformPoint(LocalAttachPoint) : LocalAttachPoint; }
				set { LocalAttachPoint = Target ? Target.InverseTransformPoint(value) : value; }
			}
			#endregion

			public GrappleTarget(GameObject source) {
				OnGrappleChanged = new GrappleEvent();

				Source = source;

				CreateJoint();
			}
			public GrappleTarget(GameObject source, Transform target, Vector3 attachPoint, Space space = Space.Self) : this(source) {
				Target = target;

				if(space == Space.Self)
					LocalAttachPoint = attachPoint;
				else
					WorldAttachPoint = attachPoint;

				RecalculateLength();
			}

			public void Destroy() {
				Object.Destroy(JointObject);
				Object.Destroy(Joint);
			}

			public void Update() {
				if(Target) {
					JointObject.position = Target.transform.TransformPoint(LocalAttachPoint);
				}
				else {
					if(Joint.connectedBody) Joint.connectedBody = null;

					if((Joint.xMotion & Joint.yMotion & Joint.zMotion) != ConfigurableJointMotion.Free)
						Joint.xMotion = Joint.yMotion = Joint.zMotion = ConfigurableJointMotion.Free;
				}
			}

			void CreateJoint() {
				JointObject = new GameObject("Grapple Joint", typeof(Rigidbody)).transform;
				JointObject.GetComponent<Rigidbody>().isKinematic = true;

				Joint = Source.AddComponent<ConfigurableJoint>();

				Joint.hideFlags = HideFlags.HideAndDontSave;
				Joint.autoConfigureConnectedAnchor = false;
				Joint.anchor = Vector3.zero;
				Joint.connectedAnchor = Vector3.zero;
				Joint.axis = Vector3.zero;
				Joint.secondaryAxis = Vector3.zero;
				Joint.enableCollision = true;
			}

			/// <summary>
			/// Sets the length based on the distance from the target to the source.
			/// </summary>
			public void RecalculateLength() {
				Length = Target ? Vector3.Distance(Source.transform.position, Target.TransformPoint(LocalAttachPoint)) : 0;
			}
		}

		public interface IGrappleLinearLimit {
			float AddLength { get; set; }
			float Bounciness { get; set; }
			float Damper { get; set; }
			float Length { get; set; }
			float Spring { get; set; }
		}

		/// <summary>
		/// A UnityEvent with a GrappleTarget parameter.
		/// </summary>
		[System.Serializable]
		public class GrappleEvent : UnityEvent<GrappleTarget> { }
	}
}