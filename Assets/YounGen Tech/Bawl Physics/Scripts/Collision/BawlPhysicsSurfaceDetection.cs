using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Handles calculating the average normals based on all of the current collision points. It will fire the <see cref="onUpdatedAverageNormals"/> event when the normals have changed which can be used to call SetSurfaceNormals (must be setup manually) in a few of the Bawl Physics abilities. Currently the only abilities that require their surface normals to be set are, <see cref="Roll"/>, <see cref="Jump"/> and <see cref="Boost"/>.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Collision/Surface Detection")]
	public class BawlPhysicsSurfaceDetection : CollisionDetectionBase {

		[Tooltip("The max surface normal angle that will be detected. -90 = Pointing Down, 90 = Pointing Up"), Range(-90, 90), SerializeField]
		float _maxSurfaceAngle = -90;

		public NormalDetectionEvent onUpdatedAverageNormals;

		#region Properties
		/// <summary>
		/// The max surface normal angle that will be detected. -90 = Pointing Down, 90 = Pointing Up
		/// </summary>
		public float MaxSurfaceAngle {
			get { return _maxSurfaceAngle; }
			set { _maxSurfaceAngle = value; }
		}
		#endregion

		protected override void Update() {
			if(IsColliding) {
				int count = 0;
				Vector3 averageNormals = Vector3.zero;

				foreach(ContactPoint a in contactPoints) {
					RaycastHit hit;
					Ray ray = new Ray(transform.position, a.point - transform.position);

					if(ray.direction != Vector3.zero && a.otherCollider.Raycast(ray, out hit, Mathf.Infinity)) {
						averageNormals += hit.normal;
						count++;
					}
				}

				if(count > 0) //<- Checking the count in the case that the raycasts miss somehow
					averageNormals /= count;

				StickyNormals.Value = averageNormals;

				if(StickyNormals.Value == Vector3.zero) StickyNormals.Reset();
			}
			else
				StickyNormals.Reset();

			if(IsColliding && StickyNormals.Value != Vector3.zero)
				if(onUpdatedAverageNormals != null)
					onUpdatedAverageNormals.Invoke(StickyNormals.Value);

			StickyNormals.Update();
		}

		protected override void FireOnCollided(Collision hit) {
			foreach(ContactPoint a in hit.contacts)
				if(surfaceDetectionMask.LayerInMask(a.otherCollider.gameObject))
					if(a.normal.y * 90 >= MaxSurfaceAngle) {
						contactPoints.Add(a);

						if(onCollided != null) onCollided.Invoke(hit);
					}
		}

		void OnDrawGizmosSelected() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(transform.position, StickyNormals.Value);
		}

		/// <summary>
		/// A UnityEvent with a Vector3 parameter.
		/// </summary>
		[Serializable]
		public class NormalDetectionEvent : UnityEvent<Vector3> { }
	}
}