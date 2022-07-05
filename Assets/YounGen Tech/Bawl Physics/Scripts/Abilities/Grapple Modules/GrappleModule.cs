using UnityEngine;

namespace YounGenTech.BawlPhysics {
	[AddComponentMenu("")]
	public class GrappleModule : MonoBehaviour {
		Grapple _grapple;

		public Grapple GrappleAbility {
			get {
				if(!_grapple) _grapple = GetComponent<Grapple>();
				return _grapple;
			}
		}
	}
}