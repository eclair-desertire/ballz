using UnityEngine;
using System.Collections;

namespace YounGenTech {
	public static class LayerMaskExtensions {
		public static bool LayerInMask(this LayerMask layerMask, int layer) {
			return (layerMask & (1 << layer)) > 0;
		}
		public static bool LayerInMask(this LayerMask layerMask, GameObject gameObject) {
			return (layerMask & (1 << gameObject.layer)) > 0;
		}
	}
}