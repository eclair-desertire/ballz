using UnityEngine;
using UnityEngine.Serialization;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Add this component to objects that you wish to have customizable magnetic information for the Magnet ability
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Extras/Magnet/Magnet Info Override")]
	public class MagnetInfoOverride : MonoBehaviour {

		[Tooltip("The magnet ability can disconnect from this surface"), SerializeField, FormerlySerializedAs("allowBreak")]
		bool _allowBreak;

		[Tooltip("The gravity of this surface"), SerializeField, FormerlySerializedAs("magnetPower")]
		float _magnetPower = 98.1f;

		#region Properties
		/// <summary>
		/// The magnet ability can disconnect from this surface.
		/// </summary>
		public bool AllowBreak {
			get { return _allowBreak; }
			set { _allowBreak = value; }
		}

		/// <summary>
		/// The gravity of this surface.
		/// </summary>
		public float MagnetPower {
			get { return _magnetPower; }
			set { _magnetPower = value; }
		}
		#endregion
	}
}