using UnityEngine;
using UnityEngine.Events;

public class FloatToNormalized : MonoBehaviour {

	[SerializeField]
	float min;

	[SerializeField]
	float max;

	public TextEvent OnConvert;

	public void Convert(float x) {
		if(OnConvert != null) OnConvert.Invoke(Mathf.InverseLerp(min, max, x));
	}

	/// <summary>
	/// A UnityEvent with a float parameter.
	/// </summary>
	[System.Serializable]
	public class TextEvent : UnityEvent<float> { }
}