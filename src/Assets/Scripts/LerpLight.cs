using UnityEngine;
using System.Collections;

public class LerpLight : MonoBehaviour {

	// MonoBehaviour
	void Start () {
		Debug.Log ("BURST CREATED");
		Light l = gameObject.GetComponent<Light> ();
		StartCoroutine (LerpLightIntensity (l, 5f));
	}
	
	IEnumerator LerpLightIntensity (Light l, float duration) {
		Debug.Log ("Lerping light!");
		float t = 0.0f;
		float rate = 1.0f / duration;
		float orig = l.intensity;
		Debug.Log ("rate " + rate);
		while (t < 1) {
			t += Time.deltaTime * rate;
			l.intensity = orig - t;
			Debug.Log (l.intensity + "l t" + t);
			yield return true;
		}
	}
}

