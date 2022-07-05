using UnityEngine;
using System.Collections;

namespace YounGenTech.BawlPhysics {
	[AddComponentMenu("YounGen Tech/Bawl Physics/Effects/Emit Once")]
	public class EmitOnce : MonoBehaviour {

		public int emitCount = 15;

		void Awake() {
			ParticleSystem system = GetComponent<ParticleSystem>();

			if(system) system.Emit(emitCount);
		}
	}
}