#pragma strict

// Editable whilst in Unity
var rotationSpeed = 100;
var jumpHeight = 8;
var fallDistance = -10;
var startPositions = [ Vector3( 0, 10, 0 ) , Vector3( 40, 10, 0) , Vector3( 80, 10, 0 ) ];

private var checkPointPosition = startPositions[0];
var currentSubLevel = 0;

private var isFalling = false;

function Update( ) {
	// Input.GetAxis corresponds to Left/Right, 'A'/'D', Joysticks, etc
	var rotation : float = Input.GetAxis( "Horizontal" ) * rotationSpeed;
	// Time.deltaTime ensures speed isn't influenced by framerate
	rotation *= Time.deltaTime;
	// Physics engine allows torques!
	// A Vector3 object gives Unity a simple direction
	rigidbody.AddRelativeTorque( Vector3.back * rotation );
	
	if ( ( Input.GetKeyDown( KeyCode.UpArrow ) || Input.GetKeyDown( KeyCode.Space ) || Input.GetKeyDown( KeyCode.W ) ) && isFalling == false ) {
		rigidbody.velocity.y = jumpHeight;
	}
	isFalling = true;
	
	// Restart
	if ( Input.GetKeyDown( KeyCode.Backspace ) ) {
		SetPosition( GetStartPosition( GetLevel() ) );
		rigidbody.velocity.y = -10;
	}
	
	// Checkpoints
	if ( Input.GetKeyDown( KeyCode.Return ) ) {
		SetPosition( checkPointPosition + Vector3( 0, .5, 0));
	}
	
	// Create checkpoint
	if ( Input.GetKeyDown( KeyCode.C ) ) {
		checkPointPosition = transform.position;
	}
	

	
	if (transform.position.y <= fallDistance) {
		if ( GetLevel() == 0 ) { IncLevel(); }
		SetPosition( GetStartPosition( GetLevel() ) );
		Debug.Log( GetLevel() );
	}
}

function OnCollisionStay( ) {
	isFalling = false;
}

function SetPosition( vec ) {
	transform.position = vec;/*
	rigidbody.velocity = Vector3.zero;
	rigidbody.angularVelocity = Vector3.zero;
	rigidbody.isKinematic = true;
	rigidbody.isKinematic = false;*/
}

function SetLevel( ID : int ) {
	currentSubLevel = ID;
}

function GetLevel( ) {
	return currentSubLevel;
}

function GetStartPosition( lID : int ) {
	return startPositions[lID];
}

function IncLevel( ) {
	currentSubLevel += 1;
	return currentSubLevel;
}