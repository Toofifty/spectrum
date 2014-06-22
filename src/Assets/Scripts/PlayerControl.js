#pragma strict

// Editable whilst in Unity
var acceleration = 4;
var maxSpeed = 4;
var jumpHeight = 8;
var fallDistance = -10;
var startPositions = [ Vector3( 0, 10, 0 ) , Vector3( 40, 10, 0) , Vector3( 80, 10, 0 ) ];
private var checkPointPosition = startPositions[0];
var currentSubLevel = 0;
var noZ = true;
var tagPrefab : GameObject;
var showUI = true;

// NETWORKING
private var lastSynchronizationTime : float = 0;
private var syncDelay : float = 0;
private var syncTime : float = 0;
private var syncStartPosition : Vector3 = Vector3.zero;
private var syncEndPosition : Vector3 = Vector3.zero;
private var currentTick : int = 0;

private var localName:String = ""; // Holds the local player name
private var namePlatePos : Vector3;
var namePlate:GUIStyle;


private var isFalling = false;

function Start() {
	if (networkView.isMine || !Network.isClient) {
	}
}

// Function run each frame
function Update( ) {
	if (networkView.isMine) {
		DoMovement();
	} else {
		SyncedMovement();
	}
}

function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo) {
	// Wait a bit after joining
	if (currentTick++ <= 5) {
		return;
	}

	var syncPosition : Vector3 = Vector3.zero;
	var syncVelocity : Vector3 = Vector3.zero;
	if (stream.isWriting) {
		// We're uploading the data
		
		// Store current position
		syncPosition = rigidbody.position;
		// Send to server
		stream.Serialize(syncPosition);
		
		// Store current velocity
		syncVelocity = rigidbody.velocity;
		// Send to server
		stream.Serialize(syncVelocity);
	} else {
		// We're receiving the data
		
		// Read position and velocity from server
		
		stream.Serialize(syncPosition);
		stream.Serialize(syncVelocity);
		
		// Time taken for current sync
		syncTime = 0;
		// Find time since last sync
		syncDelay = Time.time - lastSynchronizationTime;
		lastSynchronizationTime = Time.time;
		
		// Where we think the player will be
		syncEndPosition = syncPosition + syncVelocity * syncDelay;
		// Where the is at the moment
		syncStartPosition = rigidbody.position;
	}
}

function SyncedMovement() {
	// Add time taken for current sync
	syncTime += Time.deltaTime;
	// Transition from start > finish positions, align to update rate
	rigidbody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
}

// Assign a camera when connecting to a network
function OnNetworkInstantiate(info : NetworkMessageInfo) {
	if (networkView.isMine) {
		Camera.main.GetComponent(CameraControl).target = transform;
		for (var fadeObj : GameObject in GameObject.FindGameObjectsWithTag("Fade")) {
			fadeObj.GetComponent(FadeObject).target = transform;
		}
		
	}
}

@RPC
function ChangePlayerTag(pTag : String) {
	localName = pTag;
}

function OnGUI() {
	if (showUI) {
		if (networkView.isMine) {
			localName = GUI.TextField(Rect(0, 0, 100, 20), localName, 10);
			networkView.RPC("ChangePlayerTag", RPCMode.OthersBuffered, localName);
		}
		// Place the name plate where the gameObject (player prefab) is
		namePlatePos = Camera.main.WorldToScreenPoint(gameObject.transform.position);  
		GUI.Label(Rect((namePlatePos.x-50), (Screen.height - namePlatePos.y+10), 100, 50), localName, namePlate);  
	}
}

function DoMovement() {
	// Input.GetAxis corresponds to Left/Right, 'A'/'D', Joysticks, etc
	var movement : float = Input.GetAxis( "Horizontal" ) * acceleration;
	// Time.deltaTime ensures speed isn't influenced by framerate
	movement *= Time.deltaTime;
	// A Vector3 object gives Unity a simple direction
	rigidbody.velocity.x += movement;	
	if (rigidbody.velocity.x > maxSpeed) {
		rigidbody.velocity.x = maxSpeed;
	} else if (rigidbody.velocity.x < -maxSpeed) {
		rigidbody.velocity.x = -maxSpeed;
	}
	
	if (noZ) {
		rigidbody.velocity.z = 0;
		rigidbody.position.z = 0;
	}
	
	if ( ( Input.GetKeyDown( KeyCode.UpArrow ) || Input.GetKeyDown( KeyCode.Space ) || Input.GetKeyDown( KeyCode.W ) ) && !isFalling) {
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
	
	if (Input.GetKeyDown(KeyCode.F1)) {
		if (showUI) {
			GameObject.Find("Server").GetComponent(NetworkManager).showUI = false;
			showUI = false;
		} else {
			GameObject.Find("Server").GetComponent(NetworkManager).showUI = true;
			showUI = true;
		}
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

function SetPosition( vec : Vector3 ) {
	transform.position = vec;
	rigidbody.velocity = Vector3.zero;
	rigidbody.angularVelocity = Vector3.zero;
	rigidbody.isKinematic = true;
	rigidbody.isKinematic = false;
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