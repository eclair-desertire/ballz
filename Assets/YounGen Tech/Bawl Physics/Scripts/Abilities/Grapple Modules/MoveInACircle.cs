using UnityEngine;

public class MoveInACircle : MonoBehaviour {

	public float radius = 1;

	Vector3 startPosition;

	void Awake() {
		startPosition = transform.position;
	}

	void Update() {
		Vector3 offset = new Vector3();

		offset.x = Mathf.Sin(Time.time);
		offset.z = Mathf.Cos(Time.time);
		offset *= radius;

		transform.position = startPosition + offset;
	}
}