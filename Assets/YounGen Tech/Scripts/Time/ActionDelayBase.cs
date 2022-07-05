using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace YounGenTech {
	[Serializable]
	public class ActionDelayBase {
		[Tooltip("The amount (in seconds) to delay"), SerializeField, FormerlySerializedAs("delay")]
		float _timerDelay;

		[Tooltip("The time that is left"), SerializeField, FormerlySerializedAs("time")]
		float _timer;

		[Tooltip("DeltaTime - Timer is effected by Time.timeScale\nUnscaledDeltaTime - Timer counts down in realtime"), SerializeField, FormerlySerializedAs("deltaTimeType")]
		DeltaTimeType _timerType = DeltaTimeType.UnscaledDeltaTime;

		#region Properties
		/// <summary>
		/// Has the timer reached 0?
		/// </summary>
		public bool IsReady {
			get { return Timer == 0; }
		}

		/// <summary>
		/// The normalized timer value between 0-1.
		/// </summary>
		public float NormalizedTime {
			get { return Timer / TimerDelay; }
			set { Timer = Mathf.InverseLerp(0, TimerDelay, value); }
		}

		/// <summary>
		/// Time left until this action is ready.
		/// </summary>
		public float Timer {
			get { return _timer; }
			set { _timer = value; }
		}

		/// <summary>
		/// How much the action should be delayed when calling Delay().
		/// </summary>
		public float TimerDelay {
			get { return _timerDelay; }
			set { _timerDelay = value; }
		}

		/// <summary>
		/// Whether the timer should use Time.deltaTime or Time.unscaledDeltaTime to count down. If DeltaTime is chosen, the timer will be effected by Time.timeScale.
		/// </summary>
		public DeltaTimeType TimerType {
			get { return _timerType; }
			set { _timerType = value; }
		}
		#endregion

		public ActionDelayBase() { }
		public ActionDelayBase(float delay) {
			TimerDelay = delay;
		}
		public ActionDelayBase(float delay, float time)
			: this(delay) {
			Timer = time;
		}

		/// <summary>
		/// Delay this action.
		/// </summary>
		public void Delay() {
			Timer = TimerDelay;
		}

		/// <summary>
		/// Delay this action.
		/// </summary>
		/// <param name="delay">Delay amount (in seconds)</param>
		public void Delay(float delay) {
			Timer = delay;
		}

		protected virtual void OnReady() { }

		/// <summary>
		/// Reset the timer to zero.
		/// </summary>
		public virtual void Reset() {
			_timer = 0;
		}

		/// <summary>
		/// Update the timer. (Must be called manually as it does not use Unity's MonoBehaviour system)
		/// </summary>
		/// <returns>The timer has reached zero on this update.</returns>
		public virtual bool Update() {
			if(Timer > 0) {
				Timer = Mathf.Max(Timer - (TimerType == DeltaTimeType.DeltaTime ? Time.deltaTime : Time.unscaledDeltaTime), 0);

				if(Timer == 0) {
					OnReady();

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Time.deltaTime or Time.unscaledDeltaTime
		/// </summary>
		public enum DeltaTimeType {
			DeltaTime,
			UnscaledDeltaTime
		}
	}
}