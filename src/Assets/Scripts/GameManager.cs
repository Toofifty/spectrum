using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject hostPF;

	GUICamera guiCamera;
	NetworkManager networkManager;

	void Start () {
		guiCamera = Camera.main.GetComponent<GUICamera> ();
		networkManager = GameObject.FindGameObjectWithTag ("Network").GetComponent<NetworkManager> ();
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.H)) {
			ActivateGUIButton (0, gameObject);
		}
	}
	
	public void ActivateGUIButton (int id, GameObject gameObject) {
		TextInput ti;
		Debug.Log ("CASE " + id);
		
		// Host numbers
		if (id >= 10 && id <= 17) {
			guiCamera.GetComponent<AudioListener> ().enabled = false;
			networkManager.JoinServer (networkManager.hostList[id - 10]);
			return;
		}
		
		switch (id) {
		case 0:
			Debug.Log ("Returning home from " + gameObject);
			guiCamera.ShiftTo (id);
			break;
		case 1: // MAIN : "PLAY"
			break;
		case 2: // MAIN : "START SERVER"
			guiCamera.ShiftTo (id);
			break;
		case 3: // MAIN : "SERVER HOSTS"
			guiCamera.ShiftTo (id);
			break;
		case 4: // MAIN : "OPTIONS"
			Application.Quit();
			break;
		case 5: // START SERVER : "SERVER NAME"
			GameObject.FindGameObjectWithTag ("ServerNameTextInput").GetComponent<TextInput> ().SetSelected (true);
			break;
		case 6: // START SERVER : "CREATE"
			ti = GameObject.FindGameObjectWithTag ("ServerNameTextInput").GetComponent<TextInput> ();
			ti.SetSelected (false);
			string gameName = ti.text;
			networkManager.SendGameMessage ("Creating server '" + gameName + "'...");
			networkManager.StartServer (gameName);
			break;
		case 7: // SERVER HOSTS : "IP"
			GameObject.FindGameObjectWithTag ("IPTextInput").GetComponent<TextInput> ().SetSelected (true);
			break;
		case 8: // SERVER HOSTS : "JOIN IP"
			ti = GameObject.FindGameObjectWithTag ("IPTextInput").GetComponent<TextInput> ();
			ti.SetSelected (false);
			string[] parts = ti.text.Split(char.Parse(":"));
			int port = (parts.Length > 1 ? int.Parse(parts[1]) : 62010);
			networkManager.SendGameMessage ("Joining IP " + ti.text + "...");
			networkManager.ConnectTo (parts[0], port);
			guiCamera.GetComponent<AudioListener> ().enabled = false;
			break;
		case 9: // SERVER HOSTS : "REFRESH HOSTS"
			networkManager.RefreshHostList ();
			foreach (GameObject go in GameObject.FindGameObjectsWithTag ("HostEntry")) {
				Destroy (go);
			}
			HostData[] hL = networkManager.hostList;
			if (hL.Length == 0) {
				GameObject go = (GameObject)Instantiate (hostPF, new Vector3 (-2.5f, 200.7f, 61.9f), Quaternion.identity);
				go.GetComponent<HostInformation> ().NoHosts ();
				break;
			}
			float x = -2.5f;
			for (int i = 0; i < hL.Length; i++) {
				if (i > 4) {
					if (i > 8) {
						break;
					}
					x = -4.75f;
				} else {
					x = -2.5f;
				}
				GameObject go = (GameObject)Instantiate (hostPF, new Vector3 (-2.5f, 200.7f - (i * 0.6f), 61.9f), Quaternion.identity);
			 	int j = 0;
			 	string ip = "";
				while (j < hL[i].ip.Length) {
					ip = hL[i].ip[j];
					j++;
				}
				go.GetComponent<HostInformation> ().SetInformation (hL[i].gameName, ip + ":" + hL[i].port, hL[i].connectedPlayers + "/" + Network.maxConnections);
				go.GetComponent<GUIButton> ().guiID = 10 + i;
			}
			break;
		// case 10 - 17: // SERVER HOSTS : HOST #(id - 10)
		case 18: // SERVER HOSTS : ">"
			break;
		default: 
			Debug.Log ("GUI Button not assigned for " + gameObject.ToString ());
			break;
		}
	}
	
	public void ChangeObjectLight (string tag, Color color) {
		GameObject gObj = GameObject.FindGameObjectWithTag (tag);
		Light l = gObj.GetComponent<Light> ();
		StartCoroutine (LerpObjectLight (l, color, 1));
	}
	
	IEnumerator LerpObjectLight (Light l, Color end, float duration) {
		float t = 0.0f;
		float rate = 1.0f / duration;
		Color start = l.color;
		while (t <= 1) {
			t += Time.deltaTime * rate;
			l.color = Vector4.Lerp(start, end, (-Mathf.Cos(Mathf.PI * t) + 1)/2);
			yield return true;
		}
	}
	
	public void RestartGame () {
		Application.LoadLevel (0);
	}
}
