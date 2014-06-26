using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHandler : MonoBehaviour {

	// Public
	public float acceleration = 16;
	public float airAcceleration = 8;
	public float maxVelocity = 8; // MUST be positive!
	public float jumpHeight = 5;
	public float fallDistance = -30;
	public List<Vector3> subPositions = new List<Vector3> ();
	public int currSub = 0;
	public bool noZ = true;
	public bool showUI = true;
	public GUIStyle namePlate;
	public bool keepMomentum = false;
	public GameObject cpPF;

	// Private
	//Vector3 currSubPosition;
	Vector3 namePlatePos;
	Vector3 checkpointPos;
	string localName = "";
	bool isFalling = false;
	PowerUp pu;
	
	// MonoBehaviour
	void Start () {
		CreateCheckpoint (rigidbody.position);
		//currSubPosition = subPositions[0];
		pu = new PowerUp ();
	}
	
	// MonoBehaviour
	void Update () {
		if (networkView.isMine) {
			LocalMovement ();
		} else {
			SyncMovement ();
		}
	}
	
	// MonoBehaviour
	void FixedUpdate () {
		isFalling = !Physics.Raycast(transform.position, -Vector3.up, 0.5f);
	}

	void LocalMovement () {
		float movement = Input.GetAxis ("Horizontal") * (isFalling ? airAcceleration : acceleration);
		movement *= Time.deltaTime;
		if (isFalling) {
			movement *= airAcceleration;
		}
		AddXVelocity (movement, maxVelocity);
		
		HandleInput ();
		
		// Ensure no Z movement when noZ is enabled
		if (noZ) {
			Vector3 rv = rigidbody.velocity;
			Vector3 rp = rigidbody.position;
			rigidbody.velocity = new Vector3 (rv.x, rv.y, 0);
			rigidbody.position = new Vector3 (rp.x, rp.y, 0);
		}
		
		if (rigidbody.position.y < fallDistance) {
			if (currSub == 0) { currSub++; }
			RestartSub ();
		}
	}
	
	void HandleInput () {
		if ((Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W) 
			 || Input.GetKey (KeyCode.Space)) && !isFalling) {
			Vector3 rv = rigidbody.velocity;
			rigidbody.velocity = new Vector3 (rv.x, jumpHeight, rv.z);
		}
		
		if (Input.GetKeyDown(KeyCode.Backspace)) {
			RestartSub ();
		}
		
		if (Input.GetKeyDown(KeyCode.Return)) {
			TeleCheckpoint ();
		}
		
		if (Input.GetKeyDown(KeyCode.C)) {
			CreateCheckpoint (rigidbody.position);
		}
		
		if (Input.GetKeyDown(KeyCode.F1)) {
			GameObject server = GameObject.FindGameObjectWithTag("Server");
			NetworkManager nm = (NetworkManager)server.GetComponent ("NetworkManager");
			if (showUI) {
				showUI = false;
				nm.setUI (false);
			} else {
				showUI = true;
				nm.setUI (true);
			}
		}
	}
	
	public void SetPosition (Vector3 pos) {
		rigidbody.position = pos;
		if (!keepMomentum) {
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			rigidbody.isKinematic = true;
			rigidbody.isKinematic = false;
		}
	}
	
	public void RestartSub () {
		SetPosition (subPositions[currSub]);
	}
	
	public void TeleCheckpoint () {
		SetPosition (checkpointPos);
	}
	
	public void CreateCheckpoint (Vector3 position) {
		checkpointPos = position;
	}

	public void AddXVelocity (float delta, float max) {
		Vector3 rv = rigidbody.velocity;
		rv.x += delta;
		if (Mathf.Abs (rv.x) > max) {
			rv.x = rv.x >= 0 ? max : -max;
		}
		SetVelocity (rv.x, rv.y, rv.z);
	}

	public void AddYVelocity (float delta, float max) {
		Vector3 rv = rigidbody.velocity;
		rv.y += delta;
		if (Mathf.Abs (rv.y) > max) {
			rv.y = rv.y >= 0 ? max : -max;
		}
		SetVelocity (rv.x, rv.y, rv.z);
	}
	
	public void AddZVelocity (float delta, float max) {
		Vector3 rv = rigidbody.velocity;
		rv.z += delta;
		if (Mathf.Abs (rv.z) > max) {
			rv.z = rv.z >= 0 ? max : -max;
		}
		SetVelocity (rv.x, rv.y, rv.z);
	}
	
	public void SetVelocity (float x, float y, float z) {
		rigidbody.velocity = new Vector3 (x, y, z);
	}
	
	// MonoBehaviour
	void OnGUI () {
		if (showUI) {
			if (networkView.isMine) {
				localName = GUI.TextField(new Rect(10, 10, 100, 20), localName, 30);
				networkView.RPC ("ChangePlayerTag", RPCMode.OthersBuffered, localName);
			}
			namePlatePos = Camera.main.WorldToScreenPoint (gameObject.transform.position);
			GUI.Label (new Rect(namePlatePos.x - 100, Screen.height - namePlatePos.y + 10, 200, 50), localName, namePlate);
		}
	}
	
	
	/**
	  *   = POWERUPS =
	  */
	  
	public void ActivatePowerup (int id, Color color) {
		pu.SetId (id);
		pu.SetColor (color, gameObject);
	}
	
	/**
	  *   = NETWORKING =
	  */
	
	float lastSyncTime = 0;
	float syncDelay = 0;
	float syncTime = 0;
	Vector3 syncStartPos = Vector3.zero;
	Vector3 syncEndPos = Vector3.zero;
	
	public Rigidbody SyncMovement () {
		syncTime += Time.deltaTime;
		rigidbody.position = Vector3.Lerp(syncStartPos, syncEndPos, syncTime / syncDelay);
		return rigidbody;
	}
	
	// MonoBehaviour
	public void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info) {
		
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		if (stream.isWriting) {
			syncPosition = rigidbody.position;
			stream.Serialize(ref syncPosition);
			
			syncVelocity = rigidbody.velocity;
			stream.Serialize(ref syncVelocity);
		} else {
			
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			
			syncTime = 0;
			syncDelay = Time.time - lastSyncTime;
			lastSyncTime = Time.time;
			
			syncEndPos = syncPosition + syncVelocity * syncDelay;
			syncStartPos = rigidbody.position;
		}
	}
	
	// MonoBehaviour
	public void OnNetworkInstantiate (NetworkMessageInfo info) {
		if (networkView.isMine) {
			Camera cam = Camera.main;
			CameraHandler ch = (CameraHandler)cam.GetComponent("CameraHandler");
			ch.target = transform;
			Object checkpoint = Network.Instantiate(cpPF, transform.position, transform.rotation, 0);
			GameObject tempg = (GameObject) checkpoint;
			CheckpointHandler cph = (CheckpointHandler)tempg.GetComponent("CheckpointHandler");
			cph.target = transform;
			foreach (GameObject fadeObj in GameObject.FindGameObjectsWithTag("Fade")) {
				((FadeObject)fadeObj.GetComponent("FadeObject")).target = transform;
			}
			
		}
	}
	
	[RPC]
	void ChangePlayerTag (string tag) {
		localName = tag;
	}
}

public class PowerUp {

	public int id = 0;
	public Color color = new Color (0, 0, 0, 0);
	
	public void SetId (int i) {
		id = i;
	}
	
	public void SetColor (Color c, GameObject gameObject) {
		color = c;
		gameObject.renderer.material.SetVector ("_RimColor", color);
		gameObject.GetComponent<ParticleSystem> ().renderer.material.SetVector ("_RimColor", color);
	}
}

public class PlayerNetworker {

	float lastSyncTime = 0;
	float syncDelay = 0;
	float syncTime = 0;
	Vector3 syncStartPos = Vector3.zero;
	Vector3 syncEndPos = Vector3.zero;
	NetworkView nv;
	
	public PlayerNetworker (NetworkView networkView) {
		nv = networkView;
	}

	public Rigidbody SyncMovement (Rigidbody rigidbody) {
		syncTime += Time.deltaTime;
		rigidbody.position = Vector3.Lerp(syncStartPos, syncEndPos, syncTime / syncDelay);
		return rigidbody;
	}
	
	public void OSNV (BitStream stream, NetworkMessageInfo info, Rigidbody rigidbody) {
		
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		if (stream.isWriting) {
			syncPosition = rigidbody.position;
			stream.Serialize(ref syncPosition);
			
			syncVelocity = rigidbody.velocity;
			stream.Serialize(ref syncVelocity);
		} else {
			
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			
			syncTime = 0;
			syncDelay = Time.time - lastSyncTime;
			lastSyncTime = Time.time;
			
			syncEndPos = syncPosition + syncVelocity * syncDelay;
			syncStartPos = rigidbody.position;
		}
	}
	
	public void ONI (NetworkMessageInfo info, Transform transform) {
		if (nv.isMine) {
			Camera cam = Camera.main;
			CameraHandler ch = (CameraHandler)cam.GetComponent("CameraHandler");
			ch.target = transform;
			foreach (GameObject fadeObj in GameObject.FindGameObjectsWithTag("Fade")) {
				((FadeObject)fadeObj.GetComponent("FadeObject")).target = transform;
			}
			
		}
	}
}