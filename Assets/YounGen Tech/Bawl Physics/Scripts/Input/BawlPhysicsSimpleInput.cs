using UnityEngine;

namespace YounGenTech.BawlPhysics {

	/// <summary>
	/// Simple input for rolling and jumping.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Input/Simple Input")]
	public class BawlPhysicsSimpleInput : MonoBehaviour {

		Jump _jump;
		Roll _roll;

		public Jump JumpAbility {
			get {
				if(!_jump) _jump = GetComponent<Jump>();

				return _jump;
			}
		}

		public Roll RollAbility {
			get {
				if(!_roll) _roll = GetComponent<Roll>();

				return _roll;
			}
		}

		void Update() {
			if(RollAbility) {
				RollAbility.StartAbilityHorizontalAxis(Input.GetAxis("Horizontal"));
				RollAbility.StartAbilityVerticalAxis(Input.GetAxis("Vertical"));
			}

			if(JumpAbility) {
				if(Input.GetButtonDown("Jump"))
					JumpAbility.StartAbility();
			}
		}
	}
}