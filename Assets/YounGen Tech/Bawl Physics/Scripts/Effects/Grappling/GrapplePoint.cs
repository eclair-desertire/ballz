using System.Collections.Generic;
using UnityEngine;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// A point in 3D space in which to grapple. Simple as that.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Level/Grapple Point")]
	public class GrapplePoint : MonoBehaviour {
		public static readonly List<GrapplePoint> grapplePoints = new List<GrapplePoint>();

		void OnEnable() {
			grapplePoints.Add(this);
		}

		void OnDisable() {
			grapplePoints.Remove(this);
		}

		/// <summary>
		/// Starts the ball's ability at this point.
		/// </summary>
		public void GrappleHere(Grapple grapple) {
			grapple.StartAbility(transform, transform.position, Space.World);
		}
	}
}