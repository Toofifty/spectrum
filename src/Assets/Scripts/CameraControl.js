#pragma strict

var target : Transform = null;
var distance : float = -10;
var lift = 0;

function Update () {
	DoMovement();
}

function DoMovement() {
	var zoom : float = Input.GetAxis( "Mouse ScrollWheel" );
	distance += 5 * zoom;
	if ( distance < -20 ) {
		distance = -20;
	} else if ( distance > -5 ) {
		distance = -5;
	}
	transform.position = target.position + Vector3( 0, lift, distance );
}