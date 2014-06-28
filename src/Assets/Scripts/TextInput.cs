using UnityEngine;
using System.Collections;

public class TextInput : MonoBehaviour {

	public string text = "untitled";
	public int maxLength = 10;
	
	TextMesh tm;
	bool selected = false;

	void Start () {
		tm = gameObject.GetComponent<TextMesh> ();
		text = tm.text;
		renderer.material.color = new Color (0f, 0f, 0f, 0.5f);
	}
	
	void Update () {
	}
	
	void OnGUI () {
		
		if (selected && (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Escape))) {
			selected = false;
			renderer.material.color -= new Color (0f, 0f, 0f, 0.5f);
		}
		
		if (selected) {
			GUI.SetNextControlName ("hiddenText");
			GUI.FocusControl ("hiddenText");
			text = GUI.TextField (new Rect (90, -100, 200, 25), text, maxLength);
			tm.text = text;
		}
	}
	
	public void SetSelected (bool sel) {
		selected = sel;
		renderer.material.color += new Color (0f, 0f, 0f, 0.5f);
	}
}
