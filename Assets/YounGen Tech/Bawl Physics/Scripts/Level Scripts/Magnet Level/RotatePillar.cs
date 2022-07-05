using UnityEngine;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Used to rotate a section of the magnet tower in the Bawl Physics level.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Level/Rotate Pillar")]
	public class RotatePillar : MonoBehaviour {

		public float angleIncrements = 90;
		public float rotationSpeed = 1;

		Quaternion rotation;

		[SerializeField]
		float currentAngle = 0;

		void Awake() {
			rotation = transform.localRotation;
		}

		void Update() {
			float angleBetween = Quaternion.Angle(transform.localRotation, rotation);

			if(angleBetween >= .1f)
				transform.localRotation = Quaternion.RotateTowards(transform.localRotation, rotation, rotationSpeed * Time.deltaTime);
		}

		public void IncrementRotation() {
			currentAngle = Mathf.Floor(WrapAngle(currentAngle + angleIncrements) / angleIncrements) * angleIncrements;

			rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
		}

		static float WrapAngle(float angle) {
			float value = angle %= 360;

			if(value < 0) angle += 360;

			return angle;
		}
	}
}