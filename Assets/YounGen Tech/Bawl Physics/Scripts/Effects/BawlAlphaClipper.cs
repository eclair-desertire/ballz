using UnityEngine;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Handles transitioning the ball between opaque and transparent when the camera gets near the ball so you can see through it.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Effects/Alpha Clipper")]
    public class BawlAlphaClipper : MonoBehaviour {

		public LayerMask clipMask = -1;

		public Material normalMaterial;
		public Material clippedMaterial;

		[Tooltip("Allow the clip alpha to start lower than 1"), Range(0, 1)]
		public float startClippedAlpha = 1;

		[Tooltip("The point at which the object is fully opaque")]
		public float clipStart = 3;
		[Tooltip("The point at which the object is fully transparent")]
		public float clipEnd = 2;

		public Renderer[] renderers;

		bool clipped = false;

		void LateUpdate() {
			if(!Camera.main) {
				if(clipped) {
					foreach(Renderer a in renderers) {
						Color color = a.material.color;
						color.a = 1;
						a.material.color = color;

						a.material = normalMaterial;
					}

					clipped = false;
				}

				return;
			}

			float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
			RaycastHit hit;
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));

			if(distance <= clipStart && Physics.Raycast(ray, out hit, Mathf.Infinity, clipMask))
				clipped = hit.collider == GetComponent<Collider>();
			else
				clipped = false;

			if(clipEnd > clipStart) clipStart = clipEnd;

			float alpha = clipped ? Mathf.InverseLerp(clipEnd, clipStart, distance) : 1;

			foreach(Renderer a in renderers) {
				a.material = clipped ? clippedMaterial : normalMaterial;

				if(clipped) {
					Color color = a.material.color;
					color.a = alpha;
					a.material.color = color;
				}
			}
		}

		void OnValidate() {
			if(clipEnd > clipStart) clipStart = clipEnd;
		}

		[ContextMenu("Find Renderers")]
		void FindRenderers() {
			renderers = GetComponentsInChildren<Renderer>();
		}
	}
}