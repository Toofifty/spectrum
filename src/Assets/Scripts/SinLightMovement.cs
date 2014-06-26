using UnityEngine;
using System.Collections;

public class SinLightMovement : MonoBehaviour {

	Vector3 oldPos;
	float count = 1;
	
	public float speed = 1;
	public float mult = 1;
	public bool xAxis = true;
	public bool yAxis = false;
	public bool zAxis = false;

	// MonoBehaviour
	void Start () {
		oldPos = transform.position;
	}
	
	// MonoBehaviour
	void Update () {
		float add = mult * Mathf.Sin (count);
		float x = oldPos.x;
		float y = oldPos.y;
		float z = oldPos.z;
		if (xAxis) {
			x += add;
		} 
		if (yAxis) {
			y += add;
		} 
		if (zAxis) {
			z += add;
		}
		transform.position = new Vector3 (x, y, z);
		
		count += speed * Time.deltaTime * 60;
		//Debug.Log (Time.time + " | " + count);
	}
}