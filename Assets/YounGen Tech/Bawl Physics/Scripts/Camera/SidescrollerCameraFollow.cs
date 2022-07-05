using UnityEngine;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// A simple sidescroller camera controller.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Camera/Sidescroller Camera Follow")]
	public class SidescrollerCameraFollow : MonoBehaviour {
		
		public Transform movementPlane;
		public Rigidbody followRigidbody;

		public float sidewaysSensitivity = 1;
		public float sidewaysMax = 5;
		public float sidewaysMinSpeed = .2f;
		public float height = 2;
		public float distance = 30;
		public float maxDistance = 60;
		public float sensitivity = 1;

		float sideways = 0;
		float sidewaysPosition = 0;

		Camera _camera;
		protected Camera Camera {
			get {
				if(!_camera) _camera = GetComponentInChildren<Camera>();

				return _camera;
			}
		}

		void Update() {
			if(!Camera) return;

			float speed = Mathf.Clamp(followRigidbody.velocity.magnitude, .1f, maxDistance);
			float velocityX = Camera.transform.InverseTransformDirection(followRigidbody.velocity).x;

			if(velocityX > -sidewaysMinSpeed && velocityX < sidewaysMinSpeed)
				if(velocityX >= 0) velocityX = sidewaysMinSpeed;
				else velocityX = -sidewaysMinSpeed;

			sideways = Mathf.Lerp(sideways, Mathf.Clamp(velocityX, -sidewaysMax, sidewaysMax), Mathf.Abs(velocityX) / sidewaysMax);
			sidewaysPosition = Mathf.Lerp(sidewaysPosition, sideways, Time.deltaTime * sidewaysSensitivity);

			Vector3 position = Camera.transform.localPosition;
			position.z = Mathf.Lerp(position.z, -distance - speed, Time.deltaTime * sensitivity);
			Camera.transform.localPosition = position;
		}

		void LateUpdate() {
			if(!Camera) return;

			Vector3 planePoint = movementPlane.InverseTransformPoint(followRigidbody.position);
			planePoint.z = 0;
			planePoint += new Vector3(sidewaysPosition, height, 0);
			transform.position = movementPlane.TransformPoint(planePoint);
		}
	}
}