using UnityEngine.Events;
using System.Collections;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Ability interface.
	/// </summary>
	public interface IAbility {

		/// <summary>
		/// Is this <see cref="Ability"/> currently being used? Use this if you do not want to enable/disable the component.
		/// </summary>
		bool IsUsing { get; set; }

		/// <summary>
		/// Can this <see cref="Ability"/> be used? If changed, the events will take place the next frame.
		/// </summary>
		bool IsUsable { get; set; }

		/// <summary>
		/// Starts the ability if it is able to.
		/// </summary>
		void StartAbility();

		/// <summary>
		/// Stops the ability if it was being used.
		/// </summary>
		void FinishAbility();

		/// <summary>
		/// Stops the ability and doesn't finish normally.
		/// </summary>
		void CancelAbility();

		/// <summary>
		/// Calls StartAbility, UsingAbility and FinishAbility.
		/// </summary>
		void UseAbilityOnce();
	}
}