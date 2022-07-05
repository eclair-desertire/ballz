using UnityEngine;

[AddComponentMenu("YounGen Tech/Bawl Physics/Level/Respawn On NavMesh")]
public class RespawnOnNavMesh : MonoBehaviour {

	public Vector3 LastGroundPosition { get; protected set; }
	public float raycastDistance = 1.2f;
	public float respawnHeight = 1;

	void Start() {
		LastGroundPosition = transform.position;
	}

	void Update() {
		UnityEngine.AI.NavMeshHit navMeshHit;

		if(UnityEngine.AI.NavMesh.SamplePosition(transform.position, out navMeshHit, raycastDistance, -1))
			LastGroundPosition = navMeshHit.position + Vector3.up * respawnHeight;
	}

	[ContextMenu("Respawn")]
	public void Respawn() {
		transform.position = LastGroundPosition;

		Rigidbody rigidbody = GetComponent<Rigidbody>();

		if(rigidbody) {
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
	}
}