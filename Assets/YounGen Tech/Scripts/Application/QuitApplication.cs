using UnityEngine;
using System.Collections;

namespace YounGenTech {
	/// <summary>
	/// Quits the application when a button is pressed.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Scripts/Application/Quit Application")]
	public class QuitApplication : MonoBehaviour {

		public KeyCode quitKey = KeyCode.Escape;

		void Update() {
			if(Input.GetKeyDown(quitKey))
				Quit();
		}

		public void Quit() {
			Application.Quit();
		}
	}
}