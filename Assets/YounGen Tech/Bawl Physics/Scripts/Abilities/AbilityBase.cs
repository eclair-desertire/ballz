using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace YounGenTech.BawlPhysics {
	public abstract class AbilityBase : MonoBehaviour, IAbility {

		/// <summary>
		/// Is this <see cref="Ability"/> currently being used? Use this if you do not want to enable/disable the component.
		/// </summary>
		[SerializeField]
		protected bool _isUsable = true;

		/// <summary>
		/// Can this <see cref="Ability"/> be used? If changed, the events will take place the next frame.
		/// </summary>
		[SerializeField, Disabled]
		protected bool _isUsing;

		/// <summary>
		/// Fire events when started.
		/// </summary>
		public AbilityEvent onStartAbility;

		/// <summary>
		/// Fire events when during use.
		/// </summary>
		public AbilityEvent onUsingAbility;

		/// <summary>
		/// Fire events when successfully finished.
		/// </summary>
		public AbilityEvent onFinishAbility;

		/// <summary>
		/// Fire events when canceled.
		/// </summary>
		public AbilityEvent onCancelAbility;

		Rigidbody _rigidbody;
		CollisionDetectionBase _collisionDetection;

		#region Properties
		/// <summary>
		/// Bawl Physics collision detection component.
		/// </summary>
		public CollisionDetectionBase CollisionDetection {
			get {
				if(!_collisionDetection) {
					_collisionDetection = GetComponent<CollisionDetectionBase>();

					if(!_collisionDetection) {
						Debug.LogWarning("No CollisionDetectionBase component was found. Please add a BawlPhysicsSurfaceDetection component or your own component that inherits from CollisionDetectionBase. Disabling ability.");
						enabled = false;
					}
				}

				return _collisionDetection;
			}
		}

		/// <summary>
		/// Is this ability usable and not currently being used already.
		/// </summary>
		public bool IsReady {
			get { return IsUsable && !IsUsing; }
		}

		/// <summary>
		/// Is the ability able to be used.
		/// </summary>
		public bool IsUsable {
			get { return enabled ? _isUsable : false; }
			set {
				_isUsable = enabled ? value : false;

				if(!IsUsable && IsUsing)
					IsUsing = false;
			}
		}

		/// <summary>
		/// Is the ability in use.
		/// </summary>
		public bool IsUsing {
			get { return _isUsing; }
			set {
				if(value != IsUsing)
					if(value) {	//Starting the ability
						if(IsUsable) {
							InvokeStartEvent();
							_isUsing = true;
						}
					}
					else {   //Finishing the ability
						if(IsUsable)
							InvokeFinishEvent(); //Don't call the finish event if the ability isn't supposed to be usable
						else
							CancelAbility();

						_isUsing = false;
					}
			}
		}

		/// <summary>
		/// Rigidbody component.
		/// </summary>
		protected Rigidbody Rigidbody {
			get {
				if(!_rigidbody) _rigidbody = GetComponent<Rigidbody>();
				return _rigidbody;
			}
		}
		#endregion

		protected virtual void OnDisable() {
			FinishAbility();
		}

		protected virtual void Update() {
			if(IsUsing)
				UsingAbility();
		}

		protected virtual void OnValidate() {
			if(!IsUsable && IsUsing)
				IsUsing = false;
		}

		/// <summary>
		/// Invokes the OnStartAbility event.
		/// </summary>
		protected void InvokeStartEvent() {
			if(onStartAbility != null) onStartAbility.Invoke(this);
		}

		/// <summary>
		/// Invokes the OnUsingAbility event.
		/// </summary>
		protected void InvokeUsingEvent() {
			if(onUsingAbility != null) onUsingAbility.Invoke(this);
		}

		/// <summary>
		/// Invokes the OnFinishAbility event.
		/// </summary>
		protected void InvokeFinishEvent() {
			if(onFinishAbility != null) onFinishAbility.Invoke(this);
		}

		/// <summary>
		/// Invokes the OnCancelAbility event.
		/// </summary>
		protected void InvokeCancelEvent() {
			if(onCancelAbility != null) onCancelAbility.Invoke(this);
		}

		/// <summary>
		/// Starts the ability if it is able to.
		/// </summary>
		public virtual void StartAbility() {
			IsUsing = true;
		}

		/// <summary>
		/// Stops the ability if it was being used.
		/// </summary>
		public virtual void FinishAbility() {
			IsUsing = false;
		}

		/// <summary>
		/// Stops the ability and doesn't finish normally.
		/// </summary>
		public virtual void CancelAbility() {
			if(IsUsing) {
				InvokeCancelEvent();

				_isUsing = false;
			}
		}

		/// <summary>
		/// The ability is being used each frame.
		/// </summary>
		void UsingAbility() {
			if(IsUsing) InvokeUsingEvent();
		}

		/// <summary>
		/// Calls StartAbility, UsingAbility and FinishAbility.
		/// </summary>
		public virtual void UseAbilityOnce() {
			UseAbilityOnce(false);
		}

		/// <summary>
		/// Can start a coroutine that calls the StartAbility, UsingAbility and FinishAbility events. Waits one frame before firing FinishAbility. Or it can call them all in one frame.
		/// </summary>
		public virtual void UseAbilityOnce(bool useCoroutine) {
			if(IsUsable && !IsUsing)
				if(useCoroutine)
					StartCoroutine("UseAbilityCoroutine");
				else {
					StartAbility();

					if(IsUsing) {
						UsingAbility();
						FinishAbility();
					}
				}
		}

		/// <summary>
		/// Calls StartAbility, UsingAbility then FinishAbility over the course of 3 frames.
		/// </summary>
		protected virtual IEnumerator UseAbilityCoroutine() {
			if(IsReady) {
				StartAbility();

				if(IsUsing) {
					//yield return new WaitForEndOfFrame();
					UsingAbility();

					yield return new WaitForEndOfFrame();
					FinishAbility();
				}
			}
		}

		/// <summary>
		/// A UnityEvent with an AbilityBase parameter.
		/// </summary>
		[System.Serializable]
		public class AbilityEvent : UnityEvent<AbilityBase> { }
	}
}