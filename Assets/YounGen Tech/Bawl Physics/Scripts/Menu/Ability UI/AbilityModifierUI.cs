using UnityEngine;
using UnityEngine.UI;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Updates UI elements based on <see cref="AbilityBase"/> variables.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/UI/Ability Modifier UI")]
	public class AbilityModifierUI : MonoBehaviour {

		public AbilityBase ability;

		public Toggle isUsingElement;
		public Toggle isUsableElement;

		protected virtual void Update() {
			isUsingElement.isOn = ability.IsUsing;
			isUsableElement.isOn = ability.IsUsable;
		}
	}
}