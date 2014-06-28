using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	string typeName = "SpectrumMP";
	string joinIP = "localhost";
	public HostData[] hostList = {};
	List<string> chatList = new List<string> ();
	GameManager gameManager;
	
	public string displayName = "";
	public GameObject playerPF;
	public bool showUI = true;
	public int hostPort = 62010;
	
	//public GUIStyle style;
	public GUIStyle chatStyle;
	
	// MonoBehaviour
	void Start () {
		MasterServer.ipAddress = "121.214.63.194";
		MasterServer.port = 23466;
		Network.natFacilitatorIP = "121.214.63.194";
		Network.natFacilitatorPort = 23466;
		gameManager = (GameManager)GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ();
	}

	Object SpawnPlayer () {
		return Network.Instantiate (playerPF, new Vector3 (0, 20, 0),
									Quaternion.identity, 0);
	}
	
	public void StartServer (string gameName) {
		Network.InitializeServer (4, hostPort, !Network.HavePublicAddress ());
		MasterServer.RegisterHost (typeName, gameName);
	}
	
	public void StartPrivate () {
	}
	
	public void ConnectTo (string address, int port) {
		Network.Connect (address, port);
	}
	
	// MonoBehaviour
	void OnServerInitialized () {
		SpawnPlayer ();
		SendGameMessage ("Game hosted on port " + hostPort);
	}
	
	// MonoBehaviour
	void OnGUI () {
		if (showUI) {
			if (Network.isServer) {
				if (GUI.Button (new Rect (10, 55, 100, 20), "Close server")) {
					Network.Disconnect ();
				}
				GUI.Box (new Rect (10, 80, 100, 20), "Kick players");
				for (int i = 0; i < Network.connections.Length; i++) {
					if (GUI.Button (new Rect(10, 105 + 25*i, 100, 20), Network.connections[i].ipAddress)) {
						Network.CloseConnection (Network.connections[i], true);
					}
				}
			} else {
				if (GUI.Button (new Rect (10, 55, 100, 20), "Disconnect")) {
					Network.Disconnect ();
				}
			}
			
			GUI.Box( new Rect (10, 30, 100, 20), "Username");
			
			for (int i = 0; i < chatList.Count; i++) {
				int j = chatList.Count - (i + 1);
				string message = chatList[i];
				GUI.Label (new Rect (10, Screen.height - (25 * (j + 1)) - 10, 300, 25), message, chatStyle);
			}
		}
	}
	
	public void RefreshHostList () {
		MasterServer.RequestHostList (typeName);
	}
	
	// MonoBehaviour
	void OnMasterServerEvent(MasterServerEvent msEvent) {
		if (msEvent == MasterServerEvent.HostListReceived) {
			hostList = MasterServer.PollHostList ();
		}
	}
	
	public void JoinServer (HostData hostData) {
		Network.Connect (hostData);
	}
	
	// MonoBehaviour
	void OnConnectedToServer () {
		SpawnPlayer ();
	}
	
	// MonoBehaviour
	void OnPlayerConnected (NetworkPlayer player) {
		SendGameMessage (player.ipAddress + " joined. " + NumberPlayers(true));
	}
	
	// MonoBehaviour
	void OnPlayerDisconnected (NetworkPlayer player) {
		Network.RemoveRPCs (player);
		Network.DestroyPlayerObjects (player);
		SendGameMessage (player.ipAddress + " left. " + NumberPlayers(false));
	}
	
	// MonoBehaviour
	void OnDisconnectedFromServer (NetworkDisconnection info) {
		if (Network.isServer) {
			SendGameMessage ("Local server connection lost.");
		} else {
			if (info == NetworkDisconnection.LostConnection) {
				SendGameMessage ("Lost connection to the server.");
			} else {
				SendGameMessage ("Disconnected from the server.");
			}
		}
		gameManager.RestartGame ();
	}
	
	// MonoBehaviour
	void OnFailedToConnect (NetworkConnectionError error) {
		SendGameMessage ("Could not connect to server.");
		SendGameMessage (error.ToString ());
		gameManager.ChangeObjectLight ("JoinIPButton", new Color (1.0f, 0.5f, 0.5f, 1.0f));
		
	}
	
	string NumberPlayers (bool add) {
		int plusOne = add ? 1 : 0;
		return (Network.connections.Length + plusOne) + "/" + Network.maxConnections;
	}
	
	public void SendGameMessage (string message) {
		chatList.Add (message);
		Debug.Log (FormatTime (Time.time) + " " + message);
		if (chatList.Count > 6) {
			chatList.RemoveAt (chatList.Count - 1);
		}
	}
	
	string FormatTime (float time) {
		int intTime = (int)time;
		int minutes = intTime / 60;
		int seconds = intTime % 60;
		int fraction = (int)time * 10;
		fraction = fraction % 10;
		
		var timeText = minutes.ToString("00") + ":";
		timeText += seconds.ToString("00") + ":";
		timeText += fraction.ToString("00");
		return timeText;
	}	
	
	public void setUI (bool active) {
		showUI = active;
	}
}

