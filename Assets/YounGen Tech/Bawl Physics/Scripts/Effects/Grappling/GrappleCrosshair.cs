using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Crosshair UI for the <see cref="Grapple"/> ability. For use with the Canvas UI elements.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Effects/Grapple Crosshair"), RequireComponent(typeof(RectTransform), typeof(Image))]
	public class GrappleCrosshair : MonoBehaviour {

		[Tooltip("The grapple ability that should be located on the player")]
		public Grapple grapple;

		[Tooltip("The color of the crosshair based on whether the grapple can be used or not.\nLeft Color = You can't grapple\nRight Color = You can grapple")]
		public Gradient colorScale;

		[Tooltip("How fast the color should change from left to right")]
		public float transitionSpeed = 1;
		float transitionTime;

		public GrappleTargetType grappleType = GrappleTargetType.Raycast;

		public Transform CurrentGrapplePoint { get; set; }

		public FindGrapplePoint findInViewport;

		RectTransform rectTransform;
		Canvas parentCanvas;

		void Awake() {
			rectTransform = GetComponent<RectTransform>();
		}

		void Start() {
			parentCanvas = GetComponentInParent<Canvas>();
		}

		void Update() {
			if(!grapple) return;

			if(findInViewport)
				if(parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay) {
					Vector2 viewportPosition = parentCanvas.transform.InverseTransformPoint(transform.position);//RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);

					viewportPosition.x = (viewportPosition.x + Screen.width * .5f) / Screen.width;
					viewportPosition.y = (viewportPosition.y + Screen.height * .5f) / Screen.height;

					findInViewport.viewportPosition = viewportPosition;
				}

			bool hit = false;

			if(grapple.IsUsable && grapple.ActionDelay.IsReady)
				hit = CurrentGrapplePoint;

			//switch(grappleType) {
			//	case GrappleTargetType.Raycast:
			//		//hit = grapple.Raycast(Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0)));
			//		break;

			//	case GrappleTargetType.GrapplePoint:
			//		hit = CurrentGrapplePoint;
			//		break;
			//}

			float transitionDirection = hit ? 1 : -1;

			transitionTime = Mathf.Clamp01(transitionTime - Time.unscaledDeltaTime * transitionDirection * transitionSpeed);
			rectTransform.GetComponent<Image>().color = colorScale.Evaluate(1 - transitionTime);
		}

		public enum GrappleTargetType {
			Raycast,
			GrapplePoint
		}
	}
}