using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace YounGenTech.BawlPhysics {
	[AddComponentMenu("YounGen Tech/Bawl Physics/Effects/World Icon"), ExecuteInEditMode, RequireComponent(typeof(SpriteRenderer))]
	public class WorldIcon : MonoBehaviour {

		[SerializeField]
		LayerMask _raycastMask = -1;

		[SerializeField]
		float _visibleDistance = 1;

		SpriteRenderer _spriteRenderer;

		public VisibilityEvent onChangeVisibility;

		#region Properties
		/// <summary>
		/// Can this icon be obstructed by anything.
		/// </summary>
		public LayerMask RaycastMask {
			get { return _raycastMask; }
			set { _raycastMask = value; }
		}

		/// <summary>
		/// The distance that this icon will be completely in visible.
		/// </summary>
		public float VisibleDistance {
			get { return _visibleDistance; }
			set { _visibleDistance = value; }
		}

		public SpriteRenderer SpriteRendererComponent {
			get { return _spriteRenderer; }
			private set { _spriteRenderer = value; }
		}
		#endregion

		void Awake() {
			SpriteRendererComponent = GetComponent<SpriteRenderer>();
		}

		void OnWillRenderObject() {
			Color color = SpriteRendererComponent.color;

			if(Camera.current.transform.InverseTransformPoint(transform.position).z > 0) {
				if(RaycastMask != 0 && Physics.Linecast(transform.position, Camera.current.transform.position, RaycastMask))
					color.a = 0;
				else
					color.a = Mathf.Clamp01(VisibleDistance - Vector3.Distance(transform.position, Camera.current.transform.position));
			}
			else
				color.a = 0;

			if(SpriteRendererComponent.color.a != color.a)
				if(onChangeVisibility != null) onChangeVisibility.Invoke(color.a > 0);

			SpriteRendererComponent.color = color;

			transform.rotation = Camera.current.transform.rotation;
		}

		/// <summary>
		///  A UnityEvent with a bool parameter.
		/// </summary>
		[System.Serializable]
		public class VisibilityEvent : UnityEvent<bool> { }
	}
}