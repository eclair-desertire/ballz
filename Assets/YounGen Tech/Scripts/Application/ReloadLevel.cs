using UnityEngine;
using System.Collections;

namespace YounGenTech {
	/// <summary>
	/// Reloads the current level when a buton is pressed.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Scripts/Application/Reload Level")]
	public class ReloadLevel : MonoBehaviour {

		public KeyCode reloadButton = KeyCode.R;

		void Update() {
			if(Input.GetKeyDown(reloadButton))
				Reload();
		}

		public void Reload() {
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}