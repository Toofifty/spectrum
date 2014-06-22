#pragma strict

var boxSize : Vector3 = Vector3(10, 5, 10);
var target : Transform;
private var visible = false;
private var dLTF : Vector3;
private var dRBB : Vector3;

function Start() {
	dLTF.x = transform.position.x - (boxSize.x / 2);
	dRBB.x = transform.position.x + (boxSize.x / 2);
	
	dLTF.y = transform.position.y + (boxSize.y / 2);
	dRBB.y = transform.position.y - (boxSize.y / 2);
	
	dLTF.z = transform.position.z - (boxSize.z / 2);
	dRBB.z = transform.position.z + (boxSize.z / 2);
	
	if (!visible) {
		renderer.material.color.a = 0;
	}
}

function Update() {
	if (target != null ) {
		if (gameObject.GetComponent(TextMesh).text == '"Music For Manatees" - Kevin MacLeod') {
			Debug.Log(target.position + " | " + dLTF + " - " + dRBB + " V:" + visible);
		}
		if (target.position.x >= dLTF.x 
			&& target.position.y <= dLTF.y 
			&& target.position.z >= dLTF.z 
			&& target.position.x <= dRBB.x 
			&& target.position.y >= dRBB.y 
			&& target.position.z <= dRBB.z) {
			// Within bounding box
			Debug.Log("Within bounding box!");
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

function FadeIn() {
	iTween.FadeTo(gameObject, {"alpha":0.5});
}

function FadeOut() {
	iTween.FadeTo(gameObject, {"alpha":0});
}