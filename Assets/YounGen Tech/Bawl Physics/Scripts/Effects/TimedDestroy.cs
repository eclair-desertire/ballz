using UnityEngine;
using System.Collections;

namespace YounGenTech.BawlPhysics {
    [AddComponentMenu("YounGen Tech/Bawl Physics/Effects/Timed Destroy")]
    public class TimedDestroy : MonoBehaviour {

		public float time;

		void Awake() {
			Destroy(gameObject, time);
		}
	}
}