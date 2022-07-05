using UnityEngine;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Highlights the selected grapple point with a sprite.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Effects/Highlight Grapple Point")]
	public class HighlightGrapplePoint : MonoBehaviour {

		[SerializeField]
		Sprite _highlightSprite;

		public Transform SpriteTransform { get; private set; }

		public void SelectGrapplePoint(Transform grapplePoint) {
			SelectGrapplePoint(grapplePoint ? grapplePoint.GetComponent<GrapplePoint>() : null);
		}

		public void SelectGrapplePoint(GrapplePoint grapplePoint) {
			if(!SpriteTransform) {
				GameObject go = new GameObject("Highlight Sprite", typeof(SpriteRenderer));

				go.layer = LayerMask.NameToLayer("GrapplePoint");
				go.GetComponent<SpriteRenderer>().sprite = _highlightSprite;
				SpriteTransform = go.transform;
			}

			SpriteTransform.SetParent(grapplePoint ? grapplePoint.transform : null, false);
			SpriteTransform.gameObject.SetActive(grapplePoint);
		}
	}
}