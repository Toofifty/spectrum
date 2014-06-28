using UnityEngine;
using System.Collections;

public class GUIButton : MonoBehaviour {

	public float movement = 0.5f;
	public float maxIntensity = 1.0f;
	public int guiID = 1000;

	bool highLighted = false;
	Vector3 originalPosition;
	float originalIntensity;
	bool useL;
	Light l;
	GameManager gameManager;

	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager> ();
	
		originalPosition = transform.position;
		l = (Light)gameObject.GetComponent<Light> ();
		if (l == null) {
			useL = false;
		} else {
			originalIntensity = l.intensity;
			useL = true;
		}		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnMouseEnter () {
		if (!highLighted) {
			StartCoroutine (DistanceLerp (originalPosition, originalPosition + new Vector3 (0, 0, movement), 0.2f, true));
			highLighted = true;
		}
	}
	
	public void OnMouseExit () {
		if (highLighted) {
			StartCoroutine (DistanceLerp (originalPosition + new Vector3 (0, 0, movement), originalPosition , 0.3f, false));
			highLighted = false;
		}
	}
	
	public void OnMouseUp () {
		gameManager.ActivateGUIButton(guiID, gameObject);
	}
	
	IEnumerator DistanceLerp (Vector3 start, Vector3 end, float duration, bool upIntensity) {
		float t = 0.0f;
		float rate = 1.0f / duration;
		while (t <= 1) {
			t += Time.deltaTime * rate;
			transform.position = Vector3.Lerp(start, end, t);
			if (useL) {
				if (upIntensity) {
					l.intensity = originalIntensity + (maxIntensity - originalIntensity) * t;
				} else {
					l.intensity = maxIntensity + (maxIntensity - originalIntensity) * -t;
				}
			}
			yield return true;
		}
	}
}
