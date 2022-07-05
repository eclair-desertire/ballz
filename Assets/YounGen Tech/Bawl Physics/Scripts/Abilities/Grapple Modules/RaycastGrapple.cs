using UnityEngine;
using UnityEngine.Events;

namespace YounGenTech.BawlPhysics {
	[AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Extras/Grapple Modules/Raycast Grapple"), RequireComponent(typeof(Grapple))]
	public class RaycastGrapple : GrappleModule {

		[SerializeField, Tooltip("Starting point of the raycast. If using Screen or Viewport raycast mode, this should be set to the camera.")]
		Transform _raycastOrigin;

		[SerializeField, Tooltip("Raycast objects in this layer")]
		LayerMask _raycastMask = -1;

		[SerializeField, Tooltip("The distance used when calling the StartAbility functions that use raycasting where the distance is not specified")]
		float _raycastDistance = 40;

		[SerializeField, Tooltip("Direction or screen/viewport origin of the raycast")]
		Vector3 _raycastDirection = new Vector3(.5f, .5f, 0);

		[SerializeField]
		RaycastType _raycastMode = RaycastType.Viewport;

		[SerializeField]
		Transform _currentTarget;

		[SerializeField]
		Vector3 _currentWorldAttachPoint;

		public SearchEvent OnChangeTarget;

		#region Properties
		public Transform CurrentTarget {
			get { return _currentTarget; }
			protected set {
				Transform previousValue = _currentTarget;
				_currentTarget = value;

				if(previousValue != _currentTarget)
					if(OnChangeTarget != null) OnChangeTarget.Invoke(_currentTarget);
			}
		}

		public Vector3 CurrentWorldAttachPoint {
			get { return _currentWorldAttachPoint; }
			protected set { _currentWorldAttachPoint = value; }
		}

		public Grapple Grapple { get; private set; }

		/// <summary>
		/// Starting point of the raycast. If using Screen or Viewport raycast mode, this should be set to the camera.
		/// </summary>
		public RaycastType RaycastMode {
			get { return _raycastMode; }
			set { _raycastMode = value; }
		}

		/// <summary>
		/// Direction or screen/viewport origin of the raycast.
		/// </summary>
		public Vector3 RaycastDirection {
			get { return _raycastDirection; }
			set { _raycastDirection = value; }
		}

		/// <summary>
		/// The distance used when calling the StartAbility functions that use raycasting where the distance is not specified.
		/// </summary>
		public float RaycastDistance {
			get { return _raycastDistance; }
			set { _raycastDistance = value; }
		}

		/// <summary>
		/// Raycast objects in this layer.
		/// </summary>
		public LayerMask RaycastMask {
			get { return _raycastMask; }
			set { _raycastMask = value; }
		}

		/// <summary>
		/// Starting point of the raycast.
		/// </summary>
		public Transform RaycastOrigin {
			get { return _raycastOrigin; }
			set { _raycastOrigin = value; }
		}
		#endregion

		void Awake() {
			Grapple = GetComponent<Grapple>();
			//Grapple.OnStartAbility.AddListener(OnUseGrapple);
		}

		void Update() {
			if(!RaycastOrigin) return;
			CurrentTarget = null;

			RaycastHit hit;
			Ray ray = new Ray(RaycastOrigin.position, RaycastOrigin.InverseTransformDirection(RaycastDirection));

			switch(RaycastMode) {
				default:
					ray = new Ray(RaycastOrigin.position, RaycastOrigin.InverseTransformDirection(RaycastDirection));
					break;

				case RaycastType.Screen:
					{
						Camera camera = RaycastOrigin.GetComponent<Camera>();

						if(camera)
							ray = camera.ScreenPointToRay(RaycastDirection);
					}
					break;
				case RaycastType.Viewport:
					{
						Camera camera = RaycastOrigin.GetComponent<Camera>();

						if(camera)
							ray = camera.ViewportPointToRay(RaycastDirection);
					}
					break;
			}

			if(Physics.Raycast(ray, out hit, RaycastDistance, RaycastMask)) {
				CurrentTarget = hit.transform;
				CurrentWorldAttachPoint = hit.point;
			}
		}

		public void StartGrapple() {
			if(CurrentTarget)
				Grapple.StartAbility(CurrentTarget, CurrentWorldAttachPoint, Space.World);
		}

		public enum RaycastType {
			Screen = 0,
			Viewport = 1,
			Direction = 2
		}

		/// <summary>
		/// A UnityEvent with a Transform parameter.
		/// </summary>
		[System.Serializable]
		public class SearchEvent : UnityEvent<Transform> { }
	}
}