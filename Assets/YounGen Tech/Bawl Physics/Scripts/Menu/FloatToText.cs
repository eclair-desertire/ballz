using UnityEngine;
using UnityEngine.Events;

public class FloatToText : MonoBehaviour {

	[SerializeField, Range(0, 7)]
	int decimalPoints = 1;

	public TextEvent OnConvert;

	public void Convert(float x) {
		if(OnConvert != null) OnConvert.Invoke(x.ToString("F" + decimalPoints));
	}

	/// <summary>
	/// A UnityEvent with a string parameter.
	/// </summary>
	[System.Serializable]
	public class TextEvent : UnityEvent<string> { }
}