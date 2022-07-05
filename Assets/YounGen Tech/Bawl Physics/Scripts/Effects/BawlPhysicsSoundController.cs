using UnityEngine;
using System.Collections;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Handles the example sound effects for the ball.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Effects/Sound Controller"), RequireComponent(typeof(Rigidbody))]
	public class BawlPhysicsSoundController : MonoBehaviour {

		[HideInInspector]
		public Roll roll;

		[HideInInspector]
		public Sprint sprint;

		[HideInInspector]
		public Jump jump;

		[HideInInspector]
		public Boost boost;

		[HideInInspector]
		public Magnet magnet;

		[HideInInspector]
		public bool isColliding;

		[Header("Rolling Sound Properties"), Space(5)]
		public AudioSource rollingSound;
		public float startRollSoundVelocity = .1f;
		public float rollingPitchMin = .8f;
		public float rollingPitchMax = 1.2f;
		public float rollingVelocityPitchMultiplier = 1;
		public float rollingVelocityVolumeMultiplier = 1;

		[Header("Collision Sound Properties"), Space(5)]
		public AudioSource collideSound;
		public float playCollideSoundAboveForce = .1f;

		void OnEnable() {
			GetAbilityComponents();
		}

		public void GetAbilityComponents() {
			if(!roll) roll = GetComponent<Roll>();
			if(!sprint) sprint = GetComponent<Sprint>();
			if(!jump) jump = GetComponent<Jump>();
			if(!boost) boost = GetComponent<Boost>();
			if(!magnet) magnet = GetComponent<Magnet>();
		}

		void Update() {
			float angularVelocity = GetComponent<Rigidbody>().angularVelocity.magnitude;
			//bool velocityIsZero = Mathf.Approximately(angularVelocity, 0);

			if(rollingSound) {
				rollingSound.volume = Mathf.Lerp(0, 1, angularVelocity * rollingVelocityVolumeMultiplier * (isColliding ? 1 : 0));
				rollingSound.pitch = Mathf.Lerp(rollingPitchMin, rollingPitchMax, angularVelocity * rollingVelocityPitchMultiplier * Time.timeScale);
			}
		}

		//void LateUpdate() {
		void FixedUpdate() {
			isColliding = false;
		}

		void OnCollisionEnter(Collision hit) {
			if(collideSound)
				if(hit.relativeVelocity.magnitude > playCollideSoundAboveForce)
					collideSound.Play();
		}

		void OnCollisionStay() {
			isColliding = true;
		}
	}
}