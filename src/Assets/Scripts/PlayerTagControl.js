#pragma strict

var target : Transform;
var height : float = 1;
var username : String = "null";

function Update () {
	DoMovement();
}

function DoMovement() {
	transform.position = target.position + Vector3(0, height, 0);
	username = GameObject.Find("Server").GetComponent(NetworkManager).userText;
	this.GetComponent(TextMesh).text = username;
}