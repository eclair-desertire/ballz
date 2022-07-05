using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Simple collision detection. Stores the first contact point's surface normal and fires an event while colliding.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Collision/Collision Detection Base"), RequireComponent(typeof(Rigidbody)),RequireComponent(typeof(SphereCollider))]
    public class CollisionDetectionBase : MonoBehaviour {

		[Tooltip("The surfaces that will contribute to the surface normals.")]
		public LayerMask surfaceDetectionMask = -1;
		public CollisionDetectionEvent onCollided;

		protected List<ContactPoint> contactPoints = new List<ContactPoint>();

		[SerializeField]
		SurfaceStickDelay _stickyNormals = new SurfaceStickDelay(.2f);

		#region Properties
		/// <summary>
		/// Count of all points this object is in contact with.
		/// </summary>
		public int ContactPointCount {
			get { return contactPoints.Count; }
		}

		/// <summary>
		/// Is the ball colliding with anything, using the <see cref="surfaceDetectionMask"/>.
		/// </summary>
		public bool IsColliding { get; protected set; }

		/// <summary>
		/// Is the ball colliding with anything, ignoring the <see cref="surfaceDetectionMask"/>.
		/// </summary>
		public bool IsCollidingAny { get; protected set; }

		/// <summary>
		/// Stores the current surface normal if colliding. If not colliding, a timer will count down and the surface normal will be set to a default normal upon reaching 0.
		/// </summary>
		public SurfaceStickDelay StickyNormals {
			get { return _stickyNormals; }
			set { _stickyNormals = value; }
		}
		#endregion

		protected virtual void Update() {
			if(IsColliding)
				StickyNormals.Value = contactPoints[0].normal;

			StickyNormals.Update();
		}

		protected virtual void FixedUpdate() {
			contactPoints.Clear();
			IsColliding = false;
			IsCollidingAny = false;
		}

		protected virtual void OnCollisionStay(Collision hit) {
			FireOnCollided(hit);

			if(contactPoints.Count > 0) {
				IsColliding = true;
				StickyNormals.Delay();
			}

			IsCollidingAny = true;
		}

		/// <summary>
		/// Decide how the <see cref="onCollided"/> event is called using the given Collision information.
		/// </summary>
		protected virtual void FireOnCollided(Collision hit) {
			foreach(ContactPoint a in hit.contacts)
				if(surfaceDetectionMask.LayerInMask(a.otherCollider.gameObject)) {
					contactPoints.Add(a);

					if(onCollided != null) onCollided.Invoke(hit);
				}
		}

		public ContactPoint GetContactPoint(int index) {
			return contactPoints[index];
		}

		public IEnumerable GetContactPoints() {
			foreach(ContactPoint a in contactPoints)
				yield return a;
		}

		/// <summary>
		/// A UnityEvent with a Collision parameter.
		/// </summary>
		[Serializable]
		public class CollisionDetectionEvent : UnityEvent<Collision> { }
	}
}