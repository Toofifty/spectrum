#pragma strict

function Awake() {
	if (!networkView.isMine) {
		Debug.Log("Object isn't mine");
	} else {
		Debug.Log("Object is mine");
	}
}