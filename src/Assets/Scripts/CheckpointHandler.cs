using UnityEngine;
using System.Collections;

public class CheckpointHandler : MonoBehaviour {

	public Transform target;
	public float hoverSpeed = 0.1f;
	
	float count = 1;
	float oldY = 100;
	
	void Start () {
		Debug.Log ("Chockpoint system initialized.");
	}
	
	// MonoBehaviour
	void Update () {
		if (Input.GetKeyDown (KeyCode.C)) {
			transform.position = target.position;
			oldY = target.position.y;
			count = 1;
			Debug.Log ("New checkpoint: " + transform.position.ToString ());
		}
		
		Vector3 tp = transform.position;
		transform.position = new Vector3 (tp.x, oldY + Mathf.Sin (count) / 20, tp.z);
		count += hoverSpeed * Time.deltaTime * 60;
		
	}
}