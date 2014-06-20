#pragma strict

private var mainX : float;
var speed : float = 1;
var mult : float = 1;
private var count : float = 1;

function Start () {
	mainX = transform.position.x;
}

function Update () {
	transform.position.x = mainX + mult * Mathf.Sin( count );
	count += speed;
}