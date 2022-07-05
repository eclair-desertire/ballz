using UnityEngine;
using UnityEngine.Events;

namespace YounGenTech {
	[AddComponentMenu("YounGen Tech/Scripts/Trigger/OnTrigger")]
    public class OnTrigger : MonoBehaviour {

		public LayerMask triggerMask = -1;

		public TriggerEvent OnEnter;
		public TriggerEvent OnStay;
		public TriggerEvent OnExit;

		public void OnTriggerEnter(Collider hit) {

			if(triggerMask.LayerInMask(hit.gameObject.layer))
				if(OnEnter != null) OnEnter.Invoke(hit);
		}

		public void OnTriggerStay(Collider hit) {
			if(triggerMask.LayerInMask(hit.gameObject.layer))
				if(OnStay != null) OnStay.Invoke(hit);
		}

		public void OnTriggerExit(Collider hit) {
			if(triggerMask.LayerInMask(hit.gameObject.layer))
				if(OnExit != null) OnExit.Invoke(hit);
		}

		/// <summary>
		/// A UnityEvent with a Collider parameter.
		/// </summary>
		[System.Serializable]
		public class TriggerEvent : UnityEvent<Collider> { }
	}
}