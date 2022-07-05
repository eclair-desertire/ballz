using UnityEngine;
using System.Collections.Generic;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// A place to spawn in a level.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Level/SpawnPoint")]
	public class SpawnPoint : MonoBehaviour {

		static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

		void OnEnable() {
			spawnPoints.Add(this);
		}

		void OnDisable() {
			spawnPoints.Remove(this);
		}

		/// <summary>
		/// Instantiates an object at this spawn point.
		/// </summary>
		public T InstantiateHere<T>(T original) where T : Object {
			return Instantiate(original, transform.position, transform.rotation) as T;
		}

		/// <summary>
		/// Sets the position to this spawn point.
		/// </summary>
		public void MoveHere(Transform transform, bool copyRotation = false) {
			ResetAbilties(transform);

			transform.position = this.transform.position;

			Rigidbody rigidbody = transform.GetComponent<Rigidbody>();

			if(rigidbody) {
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
			}

			if(copyRotation)
				transform.rotation = this.transform.rotation;
		}

		/// <summary>
		/// Sets the position to this spawn point.
		/// </summary>
		public void MoveHere(GameObject gameObject, bool copyRotation = false) {
			MoveHere(gameObject.transform, copyRotation);
		}

		/// <summary>
		/// Sets the position to this spawn point.
		/// </summary>
		public void MoveHere(Component component, bool copyRotation = false) {
			MoveHere(component.transform, copyRotation);
		}

		/// <summary>
		/// Sets the position of the transform to this spawn point.
		/// </summary>
		/// <param name="transform"></param>
		public void MoveToPosition(Transform transform) {
			ResetAbilties(transform);

			transform.position = this.transform.position;

			Rigidbody rigidbody = transform.GetComponent<Rigidbody>();

			if(rigidbody) {
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
			}
		}

		void ResetAbilties(Transform transform) {
			foreach(AbilityBase ability in transform.GetComponents<AbilityBase>())
				ability.CancelAbility();
		}

		/// <summary>
		/// Picks a random spawnpoint to move the transform to.
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="copyRotation"></param>
		public static void MoveToRandom(Transform transform, bool copyRotation = false) {
			if(spawnPoints.Count == 0) return;

			SpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

			spawnPoint.MoveHere(transform, copyRotation);
		}
	}
}