using UnityEngine;
using System.Collections;

public class GUICamera : MonoBehaviour {

	public float transitionTime = 10.0f;
	
	public bool moveRotation = false;

	Vector3 original;
	Vector3 p;
	Vector3 r;

	void Start () {
		float distance = 2000 / Screen.width;
		p = transform.position + new Vector3 (0, 0, distance);
		if (moveRotation) {
			r = transform.rotation.eulerAngles;
		}
		original = p;
	}
	
	void Update () {
		float mX = Mathf.Min (Mathf.Max (Input.mousePosition.x, 0.0f), Screen.width);
		float mY = Mathf.Min (Mathf.Max (Input.mousePosition.y, 0.0f), Screen.height);
	
		float xpos = mX - Screen.width / 2;
		float ypos = mY - Screen.height / 2;
		
		float distance = 4000 / Screen.width;
		if (moveRotation) {
			transform.rotation = Quaternion.Euler (r + new Vector3 (ypos / 40, xpos / 20, 0));
			//transform.position = p + new Vector3 (0, 0, distance);
			transform.position = p + new Vector3 (xpos / 100, ypos / 100, distance);	
			//transform.Rotate (-xpos / 2000, distance, ypos / 4000);
		} else {
			transform.position = p + new Vector3 (-xpos / 2000, ypos / 4000, distance);			
		}
	}
	
	public void ShiftTo (int id) {
		// 0 : Main
		// 1 : Level select (left)
		// 2 : Start Server (up)
		// 3 : Server hosts (down)
		// 4 : Options (right)
		switch (id) {
		case 0:
			StartCoroutine (LerpCamera (p, original, transitionTime));
			break;
		case 1:
			StartCoroutine (LerpCamera (p, original + new Vector3 (16f, 0f, 0f), transitionTime));
			break;
		case 2:
			StartCoroutine (LerpCamera (p, original + new Vector3 (0f, 16f, 0f), transitionTime));
			break;
		case 3:
			StartCoroutine (LerpCamera (p, original + new Vector3 (0f, -16f, 0f), transitionTime));
			break;
		case 4:
			StartCoroutine (LerpCamera (p, original + new Vector3 (-16f, 0f, 0f), transitionTime));
			break;
		}
	}
	
	IEnumerator LerpCamera (Vector3 start, Vector3 end, float duration) {
		float t = 0.0f;
		float rate = 1.0f / duration;
		while (t <= 1) {
			t += Time.deltaTime * rate;
			p = Vector3.Lerp(start, end, (-Mathf.Cos(Mathf.PI * t) + 1)/2);
			yield return true;
		}
	}
}
