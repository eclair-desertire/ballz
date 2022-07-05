using UnityEngine;
using UnityEngine.UI;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Simple quality menu for Bawl Physics.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/UI/Quality Menu")]
	public class QualityMenu : MonoBehaviour {

		public Toggle[] toggles;

		void Awake() {
			LoadQualitySettings();
			gameObject.SetActive(false);
		}

		public void LoadQualitySettings() {
			QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualityLevel", 5), true);

			Toggle selected = toggles[QualitySettings.GetQualityLevel()];
			selected.group.SetAllTogglesOff();
			selected.isOn = true;
		}

		public void SetQualitySettings(int qualityLevel) {
			PlayerPrefs.SetInt("QualityLevel", qualityLevel);
			PlayerPrefs.Save();

			QualitySettings.SetQualityLevel(qualityLevel, true);
		}
	}
}