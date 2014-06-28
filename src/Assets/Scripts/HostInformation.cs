using UnityEngine;
using System.Collections;

public class HostInformation : MonoBehaviour {

	GameObject hostText;
	GameObject ipText;
	GameObject playersText;
	
	Vector3 ipPosition;
	Vector3 playersPosition;
	
	void Start () {
		hostText =  transform.FindChild ("HostName").gameObject;
		ipText = transform.FindChild ("HostIP").gameObject;
		playersText = transform.FindChild ("HostPlayers").gameObject;
		
		ipText.renderer.material.color = new Color (0, 0, 0, 0);
		playersText.renderer.material.color = new Color (0, 0, 0, 0);
		
		ipPosition = ipText.transform.position;
		playersPosition = playersText.transform.position;
	}

	void OnMouseEnter () {
		StartCoroutine (LerpText (ipText, new Color (0, 0, 0, 1), ipPosition, ipPosition + new Vector3 (0, -0.5f, 0), 0.5f));
		StartCoroutine (LerpText (playersText, new Color (0, 0, 0, 1), playersPosition, playersPosition + new Vector3 (-2, 0, 0), 0.5f));
	}
	
	void OnMouseExit () {
		StartCoroutine (LerpText (ipText, new Color (0, 0, 0, 0), ipPosition + new Vector3 (0, -0.5f, 0), ipPosition, 0.5f));
		StartCoroutine (LerpText (playersText,  new Color (0, 0, 0, 0), playersPosition + new Vector3 (-2, 0, 0), playersPosition, 0.5f));
	}
	
	IEnumerator LerpText (GameObject obj, Color cend, Vector3 pstart, Vector3 pend, float duration) {
		float t = 0.0f;
		float rate = 1.0f / duration;
		Material r = obj.renderer.material;
		Color cstart = r.color;
		while (t <= 1) {
			t += Time.deltaTime * rate;
			r.color = Vector4.Lerp(cstart, cend, t);
			obj.transform.position = Vector3.Lerp(pstart, pend, t);
			yield return true;
		}
	}
	
	public void NoHosts () {	
		GetComponent<Light> ().color = new Color (1f, 0.5f, 0.5f);
		SetInformation ("None found", "None", "N/A");
	}
	
	public void SetInformation (string hostName, string ip, string players) {
		Start ();
		ipText.GetComponent<TextMesh> ().text = ip;
		playersText.GetComponent<TextMesh> ().text = players;
		hostText.GetComponent<TextMesh> ().text = hostName;
	}
}
