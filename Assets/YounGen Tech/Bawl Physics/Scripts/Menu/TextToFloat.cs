using UnityEngine;
using UnityEngine.Events;

public class TextToFloat : MonoBehaviour {

	public TextEvent OnConvert;

	public void Convert(string str) {
		float n;

		if(float.TryParse(str, out n))
			if(OnConvert != null) OnConvert.Invoke(n);
	}

	/// <summary>
	/// A UnityEvent with a float parameter.
	/// </summary>
	[System.Serializable]
	public class TextEvent : UnityEvent<float> { }
}