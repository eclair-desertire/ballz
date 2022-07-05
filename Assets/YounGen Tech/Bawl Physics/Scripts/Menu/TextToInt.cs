using UnityEngine;
using UnityEngine.Events;

public class TextToInt : MonoBehaviour {

	public TextEvent OnConvert;

	public void Convert(string str) {
		int n;

		if(int.TryParse(str, out n))
			if(OnConvert != null) OnConvert.Invoke(n);
	}

	/// <summary>
	/// A UnityEvent with a int parameter.
	/// </summary>
	[System.Serializable]
	public class TextEvent : UnityEvent<int> { }
}