using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Roll up on a magnetized object with this ability on and be free to move around all over it.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Magnet")]
	public class Magnet : AbilityBase {

		[Tooltip("Delays the use of this ability"), SerializeField]
		ActionDelay _actionDelay = new ActionDelay();

		[Tooltip("If enabled, when the ball is a certain distance (radius * 1.1) away from the joint the joint will be destroyed"), SerializeField]
		bool _allowBreak = true;

		[Tooltip("Like gravity, but towards the surface normals of what you are touching"), SerializeField]
		float _magnetPower = 98.1f;

		[Tooltip("The distance from the collision point that the magnet joint will be broken"), SerializeField]
		float _breakDistanceFromSurface = .05f;
		
		[Tooltip("Colliders on these layers are magnets"), SerializeField]
		LayerMask _magnetLayer = -1;

		public MagnetEvent OnChangeMagnetization;

		Collider hitCollider;
		ConfigurableJoint configurableJoint;
		MagnetInfoOverride magnetInfo;
		Transform magnetPivot;

		List<ContactPoint> contactPoints = new List<ContactPoint>();
		RaycastHit raycastHit;
		bool jointWasCreated;
		Vector3 localAttachPoint;

		#region Properties
		/// <summary>
		/// Delays the use of this ability.
		/// </summary>
		public ActionDelay ActionDelay {
			get { return _actionDelay; }
			set { _actionDelay = value; }
		}

		/// <summary>
		/// If enabled, when the ball is a certain distance (radius * 1.1) away from the joint the joint will be destroyed.
		/// </summary>
		public bool AllowBreak {
			get { return _allowBreak; }
			set { _allowBreak = value; }
		}

		/// <summary>
		/// The distance from the collision point that the magnet joint will be broken.
		/// </summary>
		public float BreakDistanceFromSurface {
			get { return _breakDistanceFromSurface; }
			set { _breakDistanceFromSurface = value; }
		}

		/// <summary>
		/// Colliders on these layers are magnets.
		/// </summary>
		public LayerMask MagnetLayer {
			get { return _magnetLayer; }
			set { _magnetLayer = value; }
		}

		/// <summary>
		/// Like gravity, but towards the surface normals of what you are touching.
		/// </summary>
		public float MagnetPower {
			get { return _magnetPower; }
			set {
				_magnetPower = value;

				if(configurableJoint)
					configurableJoint.linearLimitSpring = new SoftJointLimitSpring() { spring = MagnetPower };
			}
		}
		#endregion


		void Awake() {
			magnetPivot = new GameObject("Magnet Pivot", typeof(Rigidbody)).transform;

			Rigidbody rigidbody = magnetPivot.GetComponent<Rigidbody>();

			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
		}

		protected override void Update() {
			base.Update();

			if(!IsUsing)
				ActionDelay.Update();
			else {
				if(contactPoints.Count > 0) {
					float lowestDistance = Mathf.Infinity;
					bool collided = false;
					RaycastHit hit;
					RaycastHit lowestHit = new RaycastHit();

					foreach(ContactPoint a in contactPoints) {
						Ray ray = new Ray(transform.position, a.point - transform.position);

						if(ray.direction != Vector3.zero && a.otherCollider.Raycast(ray, out hit, Mathf.Infinity)) {
							float distance = hit.distance;

							if(distance < lowestDistance) {
								lowestHit = hit;
								lowestDistance = distance;

								collided = true;
							}
						}
					}

					if(collided) {
						raycastHit = lowestHit;
						hitCollider = lowestHit.collider;
						localAttachPoint = hitCollider.transform.InverseTransformPoint(raycastHit.point);

						magnetInfo = hitCollider.GetComponent<MagnetInfoOverride>();
						magnetPivot.transform.position = raycastHit.point;

						if(!configurableJoint) {
							jointWasCreated = true;
							Rigidbody.useGravity = false;

							CreateJoint();
						}
					}
				}
			}
		}

		void FixedUpdate() {
			if(configurableJoint)
				if(!IsUsing || !hitCollider || ((magnetInfo ? magnetInfo.AllowBreak : AllowBreak) && !WithinDistance(magnetPivot.position))) {
					Destroy(configurableJoint);
				}
				else {
					float power = (magnetInfo ? magnetInfo.MagnetPower : MagnetPower);

					if(configurableJoint.linearLimitSpring.spring != power)
						configurableJoint.linearLimitSpring = new SoftJointLimitSpring() { spring = power };

					magnetPivot.transform.position = hitCollider.transform.TransformPoint(localAttachPoint);
				}

			if(jointWasCreated && !configurableJoint) {
				jointWasCreated = false;
				Rigidbody.useGravity = true;
				hitCollider = null;
				magnetInfo = null;

				if(OnChangeMagnetization != null) OnChangeMagnetization.Invoke(false);
			}

			contactPoints.Clear();
		}

		void OnCollisionStay(Collision hit) {
			foreach(ContactPoint a in hit.contacts)
				if(IsInMagnetLayer(hit.gameObject))
					contactPoints.Add(a);
		}

		protected override void OnValidate() {
			base.OnValidate();

			MagnetPower = MagnetPower;
		}

		/// <summary>
		/// Start this ability.
		/// </summary>
		public override void StartAbility() {
			if(ActionDelay.IsReady && IsReady) {
				ActionDelay.Delay();

				IsUsing = true;
			}
		}

		/// <summary>
		/// Finish this ability.
		/// </summary>
		public override void FinishAbility() {
			if(IsUsing) {
				base.FinishAbility();
				DestroyJoint();
			}
		}

		/// <summary>
		/// Cancel this ability.
		/// </summary>
		public override void CancelAbility() {
			if(IsUsing) {
				base.CancelAbility();
				DestroyJoint();
			}
		}

		void DestroyJoint() {
			if(configurableJoint) {
				Destroy(configurableJoint);

				jointWasCreated = false;
				Rigidbody.useGravity = true;
				hitCollider = null;
				magnetInfo = null;

				if(OnChangeMagnetization != null) OnChangeMagnetization.Invoke(false);
			}
		}

		void CreateJoint() {
			configurableJoint = magnetPivot.gameObject.AddComponent<ConfigurableJoint>();
			configurableJoint.autoConfigureConnectedAnchor = false;
			configurableJoint.anchor = Vector3.zero;
			configurableJoint.connectedAnchor = Vector3.zero;
			configurableJoint.connectedBody = Rigidbody;
			configurableJoint.xMotion = ConfigurableJointMotion.Limited;
			configurableJoint.yMotion = ConfigurableJointMotion.Limited;
			configurableJoint.zMotion = ConfigurableJointMotion.Limited;

			configurableJoint.linearLimit = new SoftJointLimit() { limit = .01f };
			configurableJoint.linearLimitSpring = new SoftJointLimitSpring() { spring = MagnetPower };

			if(OnChangeMagnetization != null) OnChangeMagnetization.Invoke(true);
		}

		bool WithinDistance(Vector3 point) {
			float distance = Mathf.Max(Vector3.Distance(point, Rigidbody.position) - GetComponent<SphereCollider>().radius, 0);
			
			return distance < BreakDistanceFromSurface || Mathf.Approximately(distance, BreakDistanceFromSurface);
		}

		bool IsInMagnetLayer(GameObject gameObject) {
			return MagnetLayer.LayerInMask(gameObject);
		}

		/// <summary>
		/// A UnityEvent with a bool parameter.
		/// </summary>
		[System.Serializable]
		public class MagnetEvent : UnityEvent<bool> { }
	}
}