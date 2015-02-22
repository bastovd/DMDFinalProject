using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public int currLoc = 0;
	private bool canProgress = false;
	private float speed = 0.5f;
	private bool hasNextStep = false;

	// Use this for initialization
	void Start () {
		GameObject loc0 = GameObject.Find ("Location0");
		transform.position = loc0.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (canProgress) {
			GameObject loc = GameObject.Find ("Location" + currLoc);
			if ((transform.position - loc.transform.position).magnitude > 0.0001f
			    && !hasNextStep) {
				transform.position = Vector3.MoveTowards(transform.position,
				                                         loc.transform.position,
				                                         speed * Time.deltaTime);
			} else {
				GameObject loc1 = GameObject.Find ("Location" + currLoc + "_1");
				if (loc1 != null) {
					hasNextStep = true;
					if ((transform.position - loc1.transform.position).magnitude > 0.0001f) {
						transform.position = Vector3.MoveTowards(transform.position,
						                                         loc1.transform.position,
						                                         speed * Time.deltaTime);
					} else {
						transform.position = loc1.transform.position;
						canProgress = false;
						hasNextStep = false;
					}
				} else {
					transform.position = loc.transform.position;
					canProgress = false;
				}
			}
		}
	}

	public void Progress() {
		currLoc ++;
		canProgress = true;
	}

	void OnGUI() {
		GUI.Label (new Rect (10, 100, 200, 200), currLoc.ToString ());
	}
}
