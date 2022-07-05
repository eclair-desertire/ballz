using UnityEngine;
using UnityEngine.Events;

namespace YounGenTech.BawlPhysics {
    [AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Extras/Grapple Modules/Find Grapple Point")]
    public class FindGrapplePoint : GrappleModule {

		public float searchDistance = 20;
		public float maxViewportDistance = .01f;
		public Vector2 viewportPosition = new Vector2(.5f, .5f);

		public bool dontHighlightCurrentGrappleTarget = true;
		public bool searchEveryFrame = true;

		public SearchEvent onChangeGrapplePoint;

		#region Properties
		public Grapple Grapple { get; private set; }

		/// <summary>
		/// Nearest grapple point to this object.
		/// </summary>
		public GrapplePoint NearestGrapplePoint { get; private set; }
		#endregion

		void Awake() {
			Grapple = GetComponent<Grapple>();
		}

		void Update() {
			if(searchEveryFrame)
				FindNearestGrapplePoint();
		}

		public void StartGrappleWithGrapplePoint(int grappleTargetIndex = 0) {
			if(NearestGrapplePoint && Grapple)
				Grapple.StartAbility(NearestGrapplePoint.transform, Vector3.zero, Space.Self, grappleTargetIndex);
		}

		public void FindNearestGrapplePoint() {
			GrapplePoint nearest = null;
			float maxDistance = searchDistance * searchDistance;
			float closestViewportDistance = maxViewportDistance;
			Plane[] frustum = null;

			foreach(GrapplePoint grapplePoint in GrapplePoint.grapplePoints) {
				if(dontHighlightCurrentGrappleTarget)
					if(Grapple && Grapple.IsTargetting(grapplePoint.transform)) continue;

				float distance = (transform.position - grapplePoint.transform.position).sqrMagnitude;

				if(distance <= maxDistance) {
					Renderer renderer = grapplePoint.GetComponent<Renderer>();
					Bounds bounds = renderer ? renderer.bounds : new Bounds(transform.position, Vector3.zero);

					if(frustum == null)
						frustum = GeometryUtility.CalculateFrustumPlanes(Camera.main);

					if(GeometryUtility.TestPlanesAABB(frustum, bounds)) {
						float viewportDistance = ((Vector2)Camera.main.WorldToViewportPoint(grapplePoint.transform.position) - viewportPosition).sqrMagnitude;
						
						if(viewportDistance < closestViewportDistance)
							if(viewportDistance < closestViewportDistance) {
								nearest = grapplePoint;
								closestViewportDistance = viewportDistance;
							}
					}
				}
			}

			if(nearest != NearestGrapplePoint)
				if(onChangeGrapplePoint != null)
					onChangeGrapplePoint.Invoke(nearest ? nearest.transform : null);

			NearestGrapplePoint = nearest;
		}

		/// <summary>
		/// A UnityEvent with a Transform parameter.
		/// </summary>
		[System.Serializable]
		public class SearchEvent : UnityEvent<Transform> { }
	}
}