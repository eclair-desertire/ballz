using UnityEngine;
using UnityEngine.Events;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Used to switch between third person and sidescrolling cameras in the Bawl Physics level.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Trigger/Platformer Wall Trigger")]
	public class PlatformerWallTrigger : MonoBehaviour {

		public string triggererTag = "Player";

		public TriggerEvent OnTriggerChanged;
		public TriggerEvent OnTriggerChangedOpposite;
		public UnityEvent OnPlatformedEntered;
		public UnityEvent OnPlatformedExited;

		void OnTriggerEnter(Collider hit) {
			if(hit.CompareTag(triggererTag)) {
				if(OnPlatformedEntered != null) OnPlatformedEntered.Invoke();
				if(OnTriggerChanged != null) OnTriggerChanged.Invoke(true);
				if(OnTriggerChangedOpposite != null) OnTriggerChangedOpposite.Invoke(false);
			}
		}

		void OnTriggerExit(Collider hit) {
			if(hit.CompareTag(triggererTag)) {
				if(OnPlatformedExited != null) OnPlatformedExited.Invoke();
				if(OnTriggerChanged != null) OnTriggerChanged.Invoke(false);
				if(OnTriggerChangedOpposite != null) OnTriggerChangedOpposite.Invoke(true);
			}
		}

		/// <summary>
		/// A UnityEvent with a bool parameter.
		/// </summary>
		[System.Serializable]
		public class TriggerEvent : UnityEvent<bool> { }
	}
}