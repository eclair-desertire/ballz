using UnityEngine;

namespace YounGenTech.BawlPhysics {
	[AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Extras/Grapple Modules/Extra Jump On Grapple")]
    public class ExtraJumpOnGrapple : GrappleModule {

		CollisionDetectionBase CollisionDetection { get; set; }

		void Awake() {
			CollisionDetection = GetComponent<CollisionDetectionBase>();

			GetComponent<Grapple>().onGrappleChanged.AddListener(OnGrapple);
		}

		public void OnGrapple(Grapple.GrappleTarget grappleTarget) {
			if(enabled && grappleTarget.Target && grappleTarget.Enabled && !CollisionDetection.IsColliding) {
				Jump jump = GetComponent<Jump>();

				if(jump) jump.ExtendJump();
			}
		}
	}
}