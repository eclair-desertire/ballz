using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

namespace YounGenTech {
	/// <summary>
	/// Used for actions that require a timed delay
	/// </summary>
	[Serializable]
	public class ActionDelay : ActionDelayBase {
		/// <summary>
		/// Called when this action is ready.
		/// </summary>
		[UnityEngine.Serialization.FormerlySerializedAs("OnActionReady")]
		public UnityEvent onActionReady;

		public ActionDelay() { onActionReady = new UnityEvent(); }
		public ActionDelay(float delay) : base(delay) { }
		public ActionDelay(float delay, float time) : base(delay, time) { }

		protected override void OnReady() {
			if(onActionReady != null) onActionReady.Invoke();
		}
	}

	public class ActionDelay<T> : ActionDelayBase {
		[Header("Delay Values")]
		[SerializeField, Tooltip("Current value")]
		T _value;
		[SerializeField, Tooltip("The value will be set to this default value when time runs out")]
		T _defaultValue;

		/// <summary>
		/// Called when this action is ready.
		/// </summary>
		[UnityEngine.Serialization.FormerlySerializedAs("OnActionReady")]
		public UnityEvent onActionReady;

		#region Properties
		/// <summary>
		/// The value that Value will be set when this action is ready.
		/// </summary>
		public T DefaultValue {
			get { return _defaultValue; }
			set { _defaultValue = value; }
		}

		/// <summary>
		/// Current value.
		/// </summary>
		public T Value {
			get { return _value; }
			set { _value = value; }
		}
		#endregion

		public ActionDelay() { onActionReady = new UnityEvent(); }
		public ActionDelay(float delay) : base(delay) { }
		public ActionDelay(float delay, float time) : base(delay, time) { }
		public ActionDelay(T value, T defaultValue, float delay)
			: base(delay) {
			Value = value;
			DefaultValue = defaultValue;
		}
		public ActionDelay(T value, T defaultValue, float delay, float time)
			: base(delay, time) {
			Value = value;
			DefaultValue = defaultValue;
		}

		protected override void OnReady() {
			if(onActionReady != null) onActionReady.Invoke();
		}

		/// <summary>
		/// Set the current value.
		/// </summary>
		/// <param name="value">The value to set.</param>
		public virtual void SetValue(T value) {
			Value = value;
		}

		/// <summary>
		/// Set the value to default.
		/// </summary>
		public override void Reset() {
			base.Reset();

			Value = DefaultValue;
		}
	}
}