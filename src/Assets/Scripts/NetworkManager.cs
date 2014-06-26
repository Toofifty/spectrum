using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	string typeName = "SpectrumMP";
	string gameName = "Untitled";
	string joinIP = "localhost";
	HostData[] hostList = {};
	List<string> chatList = new List<string> ();
	
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
	}

	Object SpawnPlayer () {
		return Network.Instantiate (playerPF, new Vector3 (0, 20, 0),
									Quaternion.identity, 0);
	}
	
	void StartServer () {
		Network.InitializeServer (4, hostPort, !Network.HavePublicAddress ());
		MasterServer.RegisterHost (typeName, gameName);
	}
	
	// MonoBehaviour
	void OnServerInitialized () {
		SpawnPlayer ();
		SendGameMessage ("Game hosted on port " + hostPort);
	}
	
	// MonoBehaviour
	void OnGUI () {
		if (showUI) {
			if (!Network.isClient && !Network.isServer) {
				gameName = GUI.TextField (new Rect (210, 110, 100, 20), gameName, 20);
				joinIP = GUI.TextField (new Rect (210, 140, 100, 20), joinIP, 20);
				
				if (GUI.Button (new Rect (110, 110, 100, 20), "Open Server")) {
					SendGameMessage ("Creating server '" + gameName + "'...");
					StartServer ();
				}
				
				if (GUI.Button (new Rect (110, 170, 200, 20), "Refresh Hosts")) {
					SendGameMessage ("Refreshing hosts...");
					RefreshHostList ();
				}
				
				if (GUI.Button (new Rect(110, 140, 100, 20), "Join IP")) {
					SendGameMessage ("Joining IP " + joinIP + "...");
					Network.Connect (joinIP, hostPort);
				}
				
				if (hostList.Length >= 0) {
					for (int i = 0; i < hostList.Length; i++) {
						if (GUI.Button (new Rect (110, 200 + (30 * i), 200, 20), 
										hostList[i].gameName + " | " + hostList[i].connectedPlayers 
										+ "/" + Network.maxConnections)) {
							SendGameMessage ("Joining host " + hostList[i].gameName + "...");
							JoinServer (hostList[i]);
						}
					}
				}
			} else if (Network.isServer) {
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
	
	void RefreshHostList () {
		MasterServer.RequestHostList (typeName);
	}
	
	// MonoBehaviour
	void OnMasterServerEvent(MasterServerEvent msEvent) {
		if (msEvent == MasterServerEvent.HostListReceived) {
			hostList = MasterServer.PollHostList ();
		}
	}
	
	void JoinServer (HostData hostData) {
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
	}
	
	// MonoBehaviour
	void OnFailedToConnect (NetworkConnectionError error) {
		SendGameMessage ("Could not connect to server.");
		SendGameMessage (error.ToString ());
	}
	
	string NumberPlayers (bool add) {
		int plusOne = add ? 1 : 0;
		return (Network.connections.Length + plusOne) + "/" + Network.maxConnections;
	}
	
	void SendGameMessage (string message) {
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

