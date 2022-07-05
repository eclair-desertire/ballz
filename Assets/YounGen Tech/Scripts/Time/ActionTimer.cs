using UnityEngine;

namespace YounGenTech {
	/// <summary>
	/// Fires an event after a set amount of time. It can be set to repeat.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Scripts/Time/Action Timer")]
	public class ActionTimer : MonoBehaviour {

		public bool delayOnStart;
		public bool restartOnReady = true;

		public ActionDelay action;

		void Start() {
			if(delayOnStart)
				action.Delay();
		}

		void Update() {
			action.Update();

			if(action.IsReady && restartOnReady)
				action.Delay();
		}
	}
}