using UnityEngine;
using System;
using System.Collections;

namespace YounGenTech {
	/// <summary>
	/// Stores a collision normal value for a set amount of time before reseting to a default value. Used, for example, when you want to give a player extra time to jump after leaving the ground.
	/// </summary>
	[Serializable]
	public class SurfaceStickDelay : ActionDelay<Vector3> {
		public SurfaceStickDelay() { }
		public SurfaceStickDelay(float delay)
			: base(delay) { }
		public SurfaceStickDelay(float delay, float time)
			: base(delay, time) { }
		public SurfaceStickDelay(Vector3 normals, Vector3 defaultNormals, float delay)
			: base(normals, defaultNormals, delay) { }
		public SurfaceStickDelay(Vector3 normals, Vector3 defaultNormals, float delay, float time)
			: base(normals, defaultNormals, delay, time) { }

		protected override void OnReady() {
			Reset();
		}

		public void Reset(Vector3 defaultNormals) {
			Value = defaultNormals;
		}
	}
}