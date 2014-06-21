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
	Debug.Log("Server Initialized!");
	SpawnPlayer();
	chatList.push("Game hosted on port 62010");
}

function OnGUI() {
	if (!Network.isClient && !Network.isServer) {
		gameName = GUI.TextField(Rect(200, 100, 100, 20), gameName, 20);
		joinIP = GUI.TextField(Rect(200, 130, 100, 20), joinIP, 20);
		if (GUI.Button(Rect(100, 100, 100, 20), "Open Server")) {
			chatList.push("Creating server '" + gameName + "'...");
			StartServer();
		}
		
		if (GUI.Button(Rect(100, 160, 200, 20), "Refresh Hosts")) {
			chatList.push("Refreshing hosts...");
			RefreshHostList();
		}
		
		if (GUI.Button(Rect(100, 130, 100, 20), "Join IP")) {
			chatList.push("Joining IP " + joinIP + "...");
			Network.Connect(joinIP, 62010);
		}
		
		if (hostList.length >= 0) {
			for (var i = 0; i < hostList.length; i++) {
				var hData : HostData = hostList[i];
				if (GUI.Button(Rect(100, 190 + (30 * i), 200, 20), hData.gameName + " | " + hData.connectedPlayers + "/4")) {
					chatList.push("Joining host " + hData.gameName + "...");
					JoinServer(hData);
				}
			} 
		}
	} else if (Network.isServer) {
		if (GUI.Button(Rect(0, 45, 100, 20), "Close server")) {
			chatList.push("Closed all external connections.");
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
			chatList.push("Disconnected from server.");
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
	chatList.push("Player joined the game." + NumberPlayers(true));
}

function OnPlayerDisconnected(player: NetworkPlayer) {
	Network.RemoveRPCs(player);
	Network.DestroyPlayerObjects(player);
	chatList.push("Player left the game." + NumberPlayers(false));
}

function OnFailedToConnect(error: NetworkConnectionError) {
	chatList.push("Could not connect to server:");
	chatList.push(error);
}
	
function NumberPlayers(add : boolean) {
	var plusOne : int = 0;
	if (add) { plusOne = 1; }
	var str : String = (Network.connections.Length + plusOne + "/" + Network.maxConnections);
	Debug.Log(str);
	return str;
}