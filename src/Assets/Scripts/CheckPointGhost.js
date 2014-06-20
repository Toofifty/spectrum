#pragma strict

var target : Transform;
var hoverSpeed : float;
private var count : float = 1;
private var mainY : float = 100;

function Update () {
	// Create checkpoint
	if ( Input.GetKeyDown( KeyCode.C ) ) {
		transform.position = target.position;
		mainY = target.position.y;
		count = 1;
	}
	
	transform.position.y = mainY + Mathf.Sin( count ) / 20;
	count += hoverSpeed;
}