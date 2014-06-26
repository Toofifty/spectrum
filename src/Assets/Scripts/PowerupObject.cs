using UnityEngine;
using System.Collections;

public class PowerupObject : MonoBehaviour {

	public int power = 0;
	public Color color = new Color (0, 0, 0, 0);
	public Transform burst;
	
	float rotation = 1;
	bool hidden = false;
	bool black = false;

	// MonoBehaviour
	void Start () {
		renderer.material.SetVector ("_RimColor", color);
		Light lightObj = (Light) gameObject.GetComponent ("Light");
		lightObj.color = color;
		if (color == new Color (0, 0, 0, 0)) {
			lightObj.color = new Color (1f, 1f, 1f, 0.5f);
			black = true;
		}
	}

	// MonoBehaviour
	void Update () {
		transform.Rotate (new Vector3 (rotation, rotation, rotation));
	}
	
	// MonoBehaviour
	void OnTriggerEnter (Collider collider) {
		if (hidden) return;
		if (collider.tag == "Player") {
			PlayerHandler ph = (PlayerHandler)collider.gameObject.GetComponent("PlayerHandler");
			ph.ActivatePowerup(power, color);
			GameObject burstAnim = ((Transform)Instantiate(burst, transform.position, transform.rotation)).gameObject;
			Light burstLight = (Light) burstAnim.GetComponent<Light> ();
			burstAnim.renderer.material.SetVector ("_RimColor", black ? new Color (1f, 1f, 1f, 0.5f) : color);
			burstLight.color = black ? new Color (1f, 1f, 1f, 0.5f) : color;
			burstLight.intensity = gameObject.GetComponent<Light> ().intensity;
			Debug.Log ("LIGHT INTENSITY " + light.intensity);
			Destroy(gameObject);
		}
	}
}

