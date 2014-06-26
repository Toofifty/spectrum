using UnityEngine;
using System.Collections;

public class FadeObject : MonoBehaviour {

	public Vector3 boxSize = new Vector3 (10, 5, 10);
	public Transform target;
	public bool visible = false;
	
	Vector3 dLTF;
	Vector3 dRBB;

	void Start () {
		dLTF.x = transform.position.x - (boxSize.x / 2);
		dRBB.x = transform.position.x + (boxSize.x / 2);
		
		dLTF.y = transform.position.y + (boxSize.y / 2);
		dRBB.y = transform.position.y - (boxSize.y / 2);
		
		dLTF.z = transform.position.z - (boxSize.z / 2);
		dRBB.z = transform.position.z + (boxSize.z / 2);
		
		if (!visible) {
			Color mat = renderer.material.color;
			renderer.material.color = new Color (mat.r, mat.g, mat.b, 0);
		}
	}

	void Update () {
		if (target != null) {
			if (target.position.x >= dLTF.x 
				&& target.position.y <= dLTF.y 
				&& target.position.z >= dLTF.z 
				&& target.position.x <= dRBB.x 
				&& target.position.y >= dRBB.y 
				&& target.position.z <= dRBB.z) {
				if (!visible) {
					FadeIn();
					visible = true;
				}
			} else {
				if (visible) {
					FadeOut();
					visible = false;
				}
			}
		}
	}
	
	void FadeIn() {
		iTween.FadeTo(gameObject, iTween.Hash("alpha", 1));
	}
	
	void FadeOut() {
		iTween.FadeTo(gameObject, iTween.Hash("alpha", 0));
	}
}

