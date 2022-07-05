using UnityEngine;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Interacts with the <see cref="Roll"/> ability to raise the roll speed.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Sprint"), DisallowMultipleComponent, RequireComponent(typeof(Roll))]
	public class Sprint : AbilityBase {

		[Tooltip("Delays the use of this ability"), SerializeField]
		ActionDelay _actionDelay = new ActionDelay();

		[Tooltip("The speed that should be added to the Roll ability's speed"), SerializeField]
		public float _sprintSpeed = 10;

		Roll _roll;

		#region Properties
		/// <summary>
		/// Delays the use of this ability.
		/// </summary>
		public ActionDelay ActionDelay {
			get { return _actionDelay; }
			set { _actionDelay = value; }
		}

		/// <summary>
		/// Roll ability
		/// </summary>
		public Roll Roll {
			get {
				if(!_roll) _roll = GetComponent<Roll>();

				return _roll;
			}
		}

		/// <summary>
		/// The speed that should be added to the <see cref="Roll"/> ability's speed
		/// </summary>
		public float SprintSpeed {
			get { return _sprintSpeed; }
			set { _sprintSpeed = value; }
		}
		#endregion


		protected override void Update() {
			base.Update();

			ActionDelay.Update();
		}

		/// <summary>
		/// Raise the <see cref="Roll"/> ability's speed.
		/// </summary>
		public override void StartAbility() {
			if(ActionDelay.IsReady && IsReady) {
				StartSprint();
				ActionDelay.Delay();

				IsUsing = true;
			}
		}

		/// <summary>
		/// Finish the ability by lowering the <see cref="Roll"/> ability's speed.
		/// </summary>
		public override void FinishAbility() {
			if(IsUsing) {
				base.FinishAbility();

				if(!IsUsing)
					StopSprint();
			}
		}

		/// <summary>
		/// Cancel the ability and  lower the <see cref="Roll"/> ability's speed.
		/// </summary>
		public override void CancelAbility() {
			if(IsUsing) {
				base.CancelAbility();

				StopSprint();
			}
		}

		void StartSprint() {
			if(Roll)
				Roll.RollSpeed += _sprintSpeed;
		}

		void StopSprint() {
			if(Roll)
				Roll.RollSpeed -= _sprintSpeed;
		}
	}
}