using UnityEngine;
using System.Collections;

namespace YounGenTech.BawlPhysics {
	[AddComponentMenu("YounGen Tech/Bawl Physics/Effects/Toggle Image Effects")]
    public class ToggleImageEffects : MonoBehaviour {

		public KeyCode toggleKey = KeyCode.T;
		bool toggle = true;

		public MonoBehaviour[] imageEffects;

		void Update() {
			if(Input.GetKeyDown(toggleKey)) {
				toggle = !toggle;

				foreach(MonoBehaviour a in imageEffects)
					if(a) a.enabled = toggle;
			}
		}
	}
}