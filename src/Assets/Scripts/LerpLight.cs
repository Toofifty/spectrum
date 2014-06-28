using UnityEngine;
using System.Collections;

public class LerpLight : MonoBehaviour {

	// MonoBehaviour
	void Start () {
		Light l = gameObject.GetComponent<Light> ();
		StartCoroutine (LerpLightIntensity (l, 5f));
	}
	
	IEnumerator LerpLightIntensity (Light l, float duration) {
		float t = 0.0f;
		float rate = 1.0f / duration;
		float orig = l.intensity;
		while (t < 1) {
			t += Time.deltaTime * rate;
			l.intensity = orig - t;
			yield return true;
		}
	}
}

