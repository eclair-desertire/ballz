using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Explode/Implode and effect nearby objects.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Field")]
    public class Field : AbilityBase {

		[Tooltip("Delays the use of this ability"), SerializeField]
		ActionDelay _actionDelay = new ActionDelay();

		[Tooltip("Radius of the  field"), SerializeField]
		float _radius = 5;

		[Tooltip("Amount of force applied to rigidbodies within the field"), SerializeField]
		float _force = 5;

		[Tooltip("Type of force applied to rigidbodies"), SerializeField]
		ForceMode _forceMode = ForceMode.Force;

		[Tooltip("Multiply Time.deltaTime to the force before applying it to the rigidbodies"), SerializeField, FormerlySerializedAs("_applyDeltaTime")]
		bool _applyDeltaTime;

		[Tooltip("The layers that the rigidbodies are on that should have forces applied to them"), SerializeField]
		LayerMask _fieldMask = -1;

		[Tooltip("The layers that act as obstructions to the field"), SerializeField, FormerlySerializedAs("_raycastCheckMask")]
		LayerMask _raycastObstructionMask = -1;

		public ForceEvent OnFieldHit;

		#region Properties
		/// <summary>
		/// Delays the use of this ability.
		/// </summary>
		public ActionDelay ActionDelay {
			get { return _actionDelay; }
			set { _actionDelay = value; }
		}

		/// <summary>
		/// Multiply Time.deltaTime to the force before applying it to the rigidbodies.
		/// </summary>
		public bool ApplyDeltaTime {
			get { return _applyDeltaTime; }
			set { _applyDeltaTime = value; }
		}

		/// <summary>
		/// The layers that the rigidbodies' colliders are on that should have forces applied to them.
		/// </summary>
		public LayerMask FieldMask {
			get { return _fieldMask; }
			set { _fieldMask = value; }
		}

		/// <summary>
		/// Amount of force applied to rigidbodies within the field.
		/// </summary>
		public float Force {
			get { return _force; }
			set { _force = value; }
		}

		/// <summary>
		/// Type of force applied to rigidbodies.
		/// </summary>
		public ForceMode ForceMode {
			get { return _forceMode; }
			set { _forceMode = value; }
		}

		/// <summary>
		/// Radius of the  field.
		/// </summary>
		public float Radius {
			get { return _radius; }
			set { _radius = value; }
		}

		/// <summary>
		/// The layers that act as obstructions to the field.
		/// </summary>
		public LayerMask RaycastObstructionMask {
			get { return _raycastObstructionMask; }
			set { _raycastObstructionMask = value; }
		}
		#endregion


		protected override void Update() {
			base.Update();

			if(!IsUsing)
				ActionDelay.Update();
			else
				DoAbility();
		}

		/// <summary>
		/// Find rigidbodies and apply explosion force on them.
		/// </summary>
		public override void StartAbility() {
			if(ActionDelay.IsReady && IsReady) {
				DoAbility();
				ActionDelay.Delay();

				IsUsing = true;
			}
		}


		protected virtual void DoAbility() {
			ApplyForces(FindColliders());
		}

		protected virtual Collider[] FindColliders() {
			return Physics.OverlapSphere(transform.position, Radius, FieldMask);
		}

		protected virtual void ApplyForces(Collider[] colliders) {
			float force = Force * (ApplyDeltaTime ? Time.deltaTime : 1);

			foreach(Collider a in colliders)
				if(a.attachedRigidbody && a.attachedRigidbody != Rigidbody)
					if(RaycastObstructionMask != 0) {
						RaycastHit hit;

						if(Physics.Linecast(transform.position, a.attachedRigidbody.transform.position, out hit, RaycastObstructionMask)) {
							if(hit.collider == a) ApplyForce(a, force);
						}
						else if(!RaycastObstructionMask.LayerInMask(a.gameObject)) {
							ApplyForce(a, force);
						}
					}
					else {
						ApplyForce(a, force);
					}
		}

		protected void ApplyForce(Collider collider, float force) {
			collider.attachedRigidbody.AddExplosionForce(force, transform.position, Radius);

			if(OnFieldHit != null) OnFieldHit.Invoke(collider.attachedRigidbody);
		}

		void OnDrawGizmosSelected() {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, Radius);
		}

		/// <summary>
		/// A UnityEvent with a Rigidbody parameter.
		/// </summary>
		[System.Serializable]
		public class ForceEvent : UnityEvent<Rigidbody> { }
	}
}