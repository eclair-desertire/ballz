using UnityEngine;
using UnityEngine.Events;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Tilt controls for mobile.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Input/Mobile Input")]
	public class BawlPhysicsMobileInput : MonoBehaviour {

		[SerializeField]
		bool _isControlling = true;

		[SerializeField]
		Vector2 _centerAngleOffset;

		[SerializeField]
		Vector2 _fullTiltAngle = new Vector2(25, 25);

		public InputEvent onInputUpdated;

		#region Properties
		/// <summary>
		/// Offset the angle of the mobile device's rotation angle.
		/// </summary>
		public Vector2 CenterAngleOffset {
			get { return _centerAngleOffset; }
			set { _centerAngleOffset = value; }
		}

		/// <summary>
		/// The amount of tilt used for input.
		/// </summary>
		public Vector2 FullTiltAngle {
			get { return _fullTiltAngle; }
			set { _fullTiltAngle = value; }
		}

		/// <summary>
		/// Is the input updating active.
		/// </summary>
		public bool IsControlling {
			get { return _isControlling; }
			set { _isControlling = value; }
		}
		#endregion

		void Update() {
			if(!IsControlling) return;

			Vector3 direction = Vector3.zero;
			Vector3 input = Input.acceleration;
			Vector3 angle = Vector3.zero;

			angle.x = Mathf.Atan2(input.z, -input.y) * Mathf.Rad2Deg + CenterAngleOffset.x;
			angle.y = Mathf.Atan2(input.x, -input.y) * Mathf.Rad2Deg + CenterAngleOffset.y;

			direction.x = Mathf.InverseLerp(-FullTiltAngle.y, FullTiltAngle.y, angle.y) * 2 - 1;
			direction.y = -(Mathf.InverseLerp(-FullTiltAngle.x, FullTiltAngle.x, angle.x) * 2 - 1);
			direction = Vector3.ClampMagnitude(direction, 1);

			if(onInputUpdated != null) onInputUpdated.Invoke(direction);
		}

		/// <summary>
		/// Set the center pitch angle offset to the mobile device's current pitch angle.
		/// </summary>
		public void SetCenterAngleOffsetX() {
			_centerAngleOffset.x = Mathf.Atan2(Input.acceleration.z, -Input.acceleration.y) * Mathf.Rad2Deg;
        }

		/// <summary>
		/// Set the center roll angle offset to the mobile device's current roll angle.
		/// </summary>
		public void SetCenterAngleOffsetY() {
			_centerAngleOffset.y = Mathf.Atan2(Input.acceleration.x, -Input.acceleration.y) * Mathf.Rad2Deg;
		}

		/// <summary>
		/// A UnityEvent with a Vector2 parameter.
		/// </summary>
		[System.Serializable]
		public class InputEvent : UnityEvent<Vector2> { }
	}
}