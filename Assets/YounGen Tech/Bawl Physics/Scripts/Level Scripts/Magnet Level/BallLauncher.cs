using UnityEngine;
using System.Collections;

namespace YounGenTech.BawlPhysics {
	[AddComponentMenu("")]
	public class BallLauncher : MonoBehaviour {

		public ActionDelay launcher = new ActionDelay(3);
		public ActionDelay beepTimer = new ActionDelay(1);

		public Transform launcherCameraTarget;
		public Transform launchPosition;
		public Transform indicator;

		public float force = 1;

		GameObject target;
		AbstractTargetFollower cameraFollower;
		float indicatorAngle;
		float currentAngle;

		void Start() {
			currentAngle = 45;
			indicatorAngle = 45;
			indicator.localRotation = Quaternion.Euler(0, 0, indicatorAngle);

			beepTimer.onActionReady.AddListener(GetComponent<AudioSource>().Play);
		}

		void Update() {
			if(target)
				if(beepTimer.Update()) beepTimer.Delay();

			if(launcher.Update())
				Launch();

			if(!launcher.IsReady) {
				indicatorAngle = Mathf.Lerp(45, -45, 1 - launcher.NormalizedTime);
				currentAngle = Mathf.Lerp(currentAngle, indicatorAngle + (indicatorAngle - currentAngle) * (500 * (((Random.value * 2) - 1) * (1 - launcher.NormalizedTime))), Time.deltaTime * 8);

				indicator.localRotation = Quaternion.Euler(0, 0, currentAngle);

				currentAngle = indicatorAngle;
			}
			else {
				if(indicatorAngle < 45) {
					indicatorAngle -= (indicatorAngle - 45) * Time.deltaTime;

					if(indicatorAngle > 44.75f) {
						currentAngle = 45;
						indicatorAngle = 45;
						indicator.localRotation = Quaternion.Euler(0, 0, indicatorAngle);
					}
					else
						indicator.localRotation = Quaternion.Slerp(indicator.localRotation, Quaternion.Euler(0, 0, indicatorAngle), Time.deltaTime * 2);
				}
			}
		}

		void OnTriggerStay(Collider hit) {
			if(!target)
				if(hit.CompareTag("Player")) {
					target = hit.attachedRigidbody.gameObject;

					StartLaunchTimer();
				}
		}

		public void Launch() {
			target.transform.position = launchPosition.position;

			Rigidbody rigidbody = target.GetComponent<Rigidbody>();

			rigidbody.velocity = launchPosition.forward * force;
			rigidbody.angularVelocity = Vector3.zero;

			if(cameraFollower)
				cameraFollower.SetTarget(target.transform);

			target.SetActive(true);

			target = null;
			cameraFollower = null;
			beepTimer.Reset();
		}

		void StartLaunchTimer() {
			StartCoroutine("StartLaunchTimerCoroutine");
		}

		IEnumerator StartLaunchTimerCoroutine() {
			if(launcherCameraTarget) {
				GameObject cameraRig = GameObject.FindWithTag("Main Camera Rig");

				if(cameraRig) {
					cameraFollower = cameraRig.GetComponent<AbstractTargetFollower>();

					if(cameraFollower) {
						cameraFollower.SetTarget(launcherCameraTarget);

						FreeLookCam freeLook = cameraFollower as FreeLookCam;

						if(freeLook) {
							freeLook.TiltAngle = launcherCameraTarget.eulerAngles.x;
							freeLook.LookAngle = launcherCameraTarget.eulerAngles.y;
						}
					}
				}
			}

			yield return null;

			target.SetActive(false);

			launcher.Delay();
			beepTimer.Delay();
		}
	}
}