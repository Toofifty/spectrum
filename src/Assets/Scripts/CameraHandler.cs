using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour {

	public Transform target;
	public float distance = -10;
	public float maxDistance = -5;
	public float minDistance = -20;
	public float lift = 0;

	void Update () {
		FollowPlayer ();
	}

	void FollowPlayer () {
		float zoom = Input.GetAxis ("Mouse ScrollWheel");
		distance += zoom;
		if (distance > maxDistance) {
			distance = maxDistance;
		} else if (distance < minDistance) {
			distance = minDistance;
		}
		transform.position = target.position + new Vector3 (0, lift, distance);
	}

	public void SetTarget (Transform t) {
		target = t;
	}
}
