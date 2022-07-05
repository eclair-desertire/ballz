using UnityEngine;
using UnityEngine.Events;

public class OnToggle : MonoBehaviour {

	public UnityEvent OnToggleTrue;
	public UnityEvent OnToggleFalse;

	public void ToggleEvent(bool value) {
		if(value) {
			if(OnToggleTrue != null) OnToggleTrue.Invoke();
		}
		else {
			if(OnToggleFalse != null) OnToggleFalse.Invoke();
		}
	}
}