using UnityEngine;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Grapple to the grapplePoint when entering a trigger on this GameObject.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Trigger/Grapple Trigger")]
	public class GrappleTrigger : MonoBehaviour {
		public GrapplePoint grapplePoint;

		void OnTriggerEnter(Collider collider) {
			if(!grapplePoint) return;

			Grapple grapple = collider.GetComponent<Grapple>();

			if(grapple)
				grapplePoint.GrappleHere(grapple);
		}
	}
}