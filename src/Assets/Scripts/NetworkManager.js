#pragma strict
#pragma implicit
#pragma downcast

private var typeName = "SpectrumMP";
private var gameName = "Untitled";
private var hostList = [];
private var player : GameObject;
private var joinIP : String = "localhost";
private var chatList = Array();
var userText : String = "";
var playerPrefab : GameObject;
var showUI = true;

function Start() {
	MasterServer.ipAddress = "121.214.63.194";
	MasterServer.port = 23466;
	Network.natFacilitatorIP = "121.214.63.194";
	Network.natFacilitatorPort = 23466;
}

function SpawnPlayer() {
	player = Network.Instantiate(playerPrefab, Vector3(0, 20, 0), Quaternion.identity, 0);
}

function StartServer () {
	Network.InitializeServer(4, 62010, !Network.HavePublicAddress());
	MasterServer.RegisterHost(typeName, gameName);
}

function OnServerInitialized() {
	SpawnPlayer();
	SendGameMessage("Game hosted on port 62010");
}

function OnGUI() {
	if (showUI) {
		if (!Network.isClient && !Network.isServer) {
			gameName = GUI.TextField(Rect(200, 100, 100, 20), gameName, 20);
			joinIP = GUI.TextField(Rect(200, 130, 100, 20), joinIP, 20);
			if (GUI.Button(Rect(100, 100, 100, 20), "Open Server")) {
				SendGameMessage("Creating server '" + gameName + "'...");
				StartServer();
			}
			
			if (GUI.Button(Rect(100, 160, 200, 20), "Refresh Hosts")) {
				SendGameMessage("Refreshing hosts...");
				RefreshHostList();
			}
			
			if (GUI.Button(Rect(100, 130, 100, 20), "Join IP")) {
				SendGameMessage("Joining IP " + joinIP + "...");
				Network.Connect(joinIP, 62010);
			}
			
			if (hostList.length >= 0) {
				for (var i = 0; i < hostList.length; i++) {
					var hData : HostData = hostList[i];
					if (GUI.Button(Rect(100, 190 + (30 * i), 200, 20), hData.gameName + " | " + hData.connectedPlayers + "/4")) {
						SendGameMessage("Joining host " + hData.gameName + "...");
						JoinServer(hData);
					}
				} 
			}
		} else if (Network.isServer) {
			if (GUI.Button(Rect(0, 45, 100, 20), "Close server")) {
				Network.Disconnect();
			}
			GUI.Box(Rect(0, 70, 100, 20), "Kick players");
			for (i = 0; i < Network.connections.Length; i++) {
				if (GUI.Button(Rect(0, 95 + 25*i, 100, 20), Network.connections[i].ipAddress)) {
					Network.CloseConnection(Network.connections[i], true);
				}
			}
		} else {
			if (GUI.Button(Rect(0, 45, 100, 20), "Disconnect")) {
				Network.Disconnect();
			}
		}
		GUI.Box(Rect(0, 20, 100, 20), "Username");
		
		for (i = 0; i < chatList.length; i++) {
			j = chatList.length - (i+1);
			var message : String = chatList[j];
			GUI.Label(Rect(0, Screen.height - (25 * (i+1)), 300, 25), message);
		}
	}
}

function RefreshHostList() {
	MasterServer.RequestHostList(typeName);
}

function OnMasterServerEvent(msEvent : MasterServerEvent) {
	if (msEvent == MasterServerEvent.HostListReceived) {
		hostList = MasterServer.PollHostList();
	}
}

function JoinServer(hostData : HostData) {
	Network.Connect(hostData);
}

function OnConnectedToServer() {
	SpawnPlayer();
}

function OnPlayerConnected(player: NetworkPlayer) {
	SendGameMessage("Player joined the game." + NumberPlayers(true));
}

function OnPlayerDisconnected(player: NetworkPlayer) {
	Network.RemoveRPCs(player);
	Network.DestroyPlayerObjects(player);
	SendGameMessage("Player left the game." + NumberPlayers(false));
}

function OnDisconnectedFromServer(info : NetworkDisconnection) {
	if (Network.isServer) {
		SendGameMessage("Local server connection disconnected");
	} else {
		if (info == NetworkDisconnection.LostConnection) {
			SendGameMessage("Lost connection to the server.");
		} else {
			SendGameMessage("Disconnected from the server.");
		}
	}
}

function OnFailedToConnect(error: NetworkConnectionError) {
	SendGameMessage("Could not connect to server:");
	SendGameMessage(error.ToString());
}
	
function NumberPlayers(add : boolean) {
	var plusOne : int = 0;
	if (add) { plusOne = 1; }
	var str : String = (Network.connections.Length + plusOne + "/" + Network.maxConnections);
	return str;
}

function SendGameMessage(msg : String) {
	chatList.push(msg);
	Debug.Log(FormatTime(Time.time) + " " + msg);
	if (chatList.length > 6) {
		chatList.shift();
	}
}

function FormatTime(time : float) {
	var intTime : int = time;
	var minutes : int = intTime / 60;
	var seconds : int = intTime % 60;
	var fraction : int = time * 10;
	fraction = fraction % 10;
	
	var timeText = minutes.ToString("00") + ":";
	timeText += seconds.ToString("00") + ":";
	timeText += fraction.ToString("00");
	return timeText;
}