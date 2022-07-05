using UnityEngine;

namespace YounGenTech.BawlPhysics {
    [AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Extras/Grapple Modules/Change Grapple Length")]
    public class ChangeGrappleLength : GrappleModule {

		[SerializeField, Header("Extra Options"), Tooltip("When the player moves closer to the joint, shorten the joint's length")]
		bool _shortenJointLength;

		[SerializeField, Space(10), Tooltip("Automatically shorten the joint's length overtime")]
		ChangeOvertimeType _changeAddLengthOvertime;

		[SerializeField, Tooltip("The value that will be added to addLength to either shorten or lengthen the joint's length")]
		float _changeAddLengthSpeed = -.75f;

		[SerializeField, Tooltip("When using ChangeAddLengthOvertimeType.AddMultipliedValueWithLength, what should the maximum change be?")]
		float _maximumMultipliedLengthChange = 20;

		#region Properties
		/// <summary>
		/// Shorten the joint's length overtime.
		/// </summary>
		public ChangeOvertimeType ChangeAddLengthOvertime {
			get { return _changeAddLengthOvertime; }
			set { _changeAddLengthOvertime = value; }
		}

		/// <summary>
		/// The value that will be added to addLength to either shorten or lengthen the joint's length.
		/// </summary>
		public float ChangeAddLengthSpeed {
			get { return _changeAddLengthSpeed; }
			set { _changeAddLengthSpeed = value; }
		}

		/// <summary>
		/// When using ChangeAddLengthOvertimeType.AddMultipliedValueWithLength, what should the maximum change be?
		/// </summary>
		public float MaximumMultipliedLengthChange {
			get { return _maximumMultipliedLengthChange; }
			set { _maximumMultipliedLengthChange = value; }
		}

		/// <summary>
		/// When the player moves closer to the joint, shorten the joint's length.
		/// </summary>
		public bool ShortenJointLength {
			get { return _shortenJointLength; }
			set { _shortenJointLength = value; }
		}
		#endregion


		void Update() {
			if(GrappleAbility.IsUsing) {
				for(int i = 0; i < GrappleAbility.MaxGrappleTargets; i++) {
					Grapple.GrappleTarget grappleTarget = GrappleAbility[i];

					if(grappleTarget.Target && grappleTarget.Enabled) {
						switch(ChangeAddLengthOvertime) {
							case ChangeOvertimeType.AddValue:
								AddJointLength(ChangeAddLengthSpeed * grappleTarget.Length, true, i);
								break;

							case ChangeOvertimeType.AddMultipliedLength:
								MultiplyJointLengthOvertime(ChangeAddLengthSpeed, i);
								break;
						}

						if(ShortenJointLength) {
							float distanceFromJoint = Vector3.Distance(grappleTarget.WorldAttachPoint, transform.position);

							if(distanceFromJoint < grappleTarget.Length)
								grappleTarget.Length = distanceFromJoint;
						}
					}
				}
			}
		}

		/// <summary>
		/// Changes the AddLength without letting it go past the length of the swing joint in the negatives.
		/// </summary>
		public void AddJointLength(float amount, int grappleTargetIndex) {
			if(enabled)
				if(grappleTargetIndex < 0) {
					for(int i = 0; i < GrappleAbility.MaxGrappleTargets; i++) {
						Grapple.GrappleTarget grappleTarget = GrappleAbility[i];

						if(grappleTarget.Target && grappleTarget.Enabled)
							grappleTarget.AddLength = Mathf.Max(grappleTarget.AddLength + amount, -grappleTarget.Length);
					}
				}
				else {
					Grapple.GrappleTarget grappleTarget = GrappleAbility[grappleTargetIndex];

					if(grappleTarget.Target && grappleTarget.Enabled)
						grappleTarget.AddLength = Mathf.Max(grappleTarget.AddLength + amount, -grappleTarget.Length);
				}
		}
		public void AddJointLength(float amount) {
			AddJointLength(amount, -1);
        }
		/// <summary>
		/// Changes the AddLength without letting it go past the length of the swing joint in the negatives.
		/// </summary>
		/// <param name="multiplyDeltaTime">Will amount be multiplied by Time.deltaTime.</param>
		public void AddJointLength(float amount, bool multiplyDeltaTime, int grappleTargetIndex = -1) {
			if(multiplyDeltaTime)
				amount *= Time.deltaTime;

			AddJointLength(amount, grappleTargetIndex);
		}

		/// <summary>
		/// Add an amount to AddLength overtime using Time.deltaTime.
		/// </summary>
		/// <param name="amount">Amount to add</param>
		public void AddJointLengthOvertime(float amount, int grappleTargetIndex) {
			AddJointLength(amount, true, grappleTargetIndex);
		}
		public void AddJointLengthOvertime(float amount) {
			AddJointLengthOvertime(amount, -1);
		}

		/// <summary>
		/// Multiplies the joint's length by the amount and Time.deltaTime then adds it to grapple target's AddLength.
		/// </summary>
		/// <param name="amount">Amount to multiply</param>
		public void MultiplyJointLengthOvertime(float amount, int grappleTargetIndex) {
			if(grappleTargetIndex < 0) {
				for(int i = 0; i < GrappleAbility.MaxGrappleTargets; i++) {
					Grapple.GrappleTarget grappleTarget = GrappleAbility[i];

					if(grappleTarget.Target && grappleTarget.Enabled)
						if(amount < 0)
							AddJointLength(Mathf.Min(amount * grappleTarget.Length, -MaximumMultipliedLengthChange), true);
						else
							AddJointLength(Mathf.Min(amount * grappleTarget.Length, MaximumMultipliedLengthChange), true);
				}
			}
			else {
				Grapple.GrappleTarget grappleTarget = GrappleAbility[grappleTargetIndex];

				if(grappleTarget.Target && grappleTarget.Enabled)
					if(amount < 0)
						AddJointLength(Mathf.Min(amount * grappleTarget.Length, -MaximumMultipliedLengthChange), true);
					else
						AddJointLength(Mathf.Min(amount * grappleTarget.Length, MaximumMultipliedLengthChange), true);
			}
		}
		public void MultiplyJointOvertime(float amount) {
			MultiplyJointLengthOvertime(amount, -1);
        }

		public enum ChangeOvertimeType {
			/// <summary>
			/// No math is used.
			/// </summary>
			None,

			/// <summary>
			/// Add the value.
			/// </summary>
			AddValue,

			/// <summary>
			/// Add the (amount * length).
			/// </summary>
			AddMultipliedLength
		}
	}
}