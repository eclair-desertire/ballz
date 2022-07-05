using UnityEngine;
using UnityEngine.UI;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Updates UI elements based on the <see cref="Roll"/> ability variables.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/UI/Roll Modifier UI")]
	public class RollModifierUI : AbilityModifierUI {

		public Toggle inputIsDirectionElement;

		protected override void Update() {
			base.Update();

			if(!(ability is Roll)) {
				enabled = false;
				Debug.LogWarning("Ability needs to be set to the Roll ability.");
			}

			inputIsDirectionElement.isOn = (ability as Roll).InputIsDirection;
		}
	}
}