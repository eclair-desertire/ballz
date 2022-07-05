using UnityEngine;
using System.Collections.Generic;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// The base class for creating and updating a group of controls.
	/// </summary>
	[AddComponentMenu("")]
	public class UserInputController : MonoBehaviour {
		
		/// <summary>
		/// Controls/inputs that you can define.
		/// </summary>
		public Control[] controls;

		protected virtual void Update() {
			foreach(Control control in controls)
				control.UpdateControls();
		}

		/// <summary>
		/// Gets a <see cref="Control"/> from the controls array by name.
		/// </summary>
		/// <param name="name">Name of the control</param>
		public Control GetControl(string name) {
			foreach(Control a in controls)
				if(a.Name == name)
					return a;

			return null;
		}

		/// <summary>
		/// Gets a <see cref="Control"/> from the controls array by index.
		/// </summary>
		/// <param name="index">Index of the control</param>
		public Control GetControl(int index) {
			if(index > -1 && index < controls.Length)
				return controls[index];

			return null;
		}

		public void DisableControl(string name) {
			Control control = GetControl(name);

			if(control != null) control.Enabled = false;
		}
		public void DisableControl(int index) {
			Control control = GetControl(index);

			if(control != null) control.Enabled = false;
		}

		public void EnableControl(string name) {
			Control control = GetControl(name);

			if(control != null) control.Enabled = true;
		}
		public void EnableControl(int index) {
			Control control = GetControl(index);

			if(control != null) control.Enabled = true;
		}
	}
}