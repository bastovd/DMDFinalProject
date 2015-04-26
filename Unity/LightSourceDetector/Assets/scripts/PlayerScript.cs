using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public int currLoc = 0;
	private bool canProgress = false;
	private float speed = 0.5f;

	// Use this for initialization
	void Start () {
		GameObject loc0 = GameObject.Find ("Location1_0_0");
		transform.position = loc0.transform.position;
		Progress ();
	}

	public void translateAlongPath() {
		GameObject loc = GameObject.Find ("Location" + ((int) (Levels.level+1)) + "_" + currLoc + "_" + 0);
		GameObject path = GameObject.Find("Path" + ((int) (Levels.level+1)) + "_" + currLoc);
		int path_length = path.transform.childCount;

		print (currLoc + " translate along path");

		Vector3[] movement_path = (Vector3[]) Levels.PATHS_LIST[currLoc];
		iTween.PutOnPath(this.gameObject,movement_path,0.0f);
		iTween.MoveTo(this.gameObject, iTween.Hash("path",movement_path,
		                                           "time",6,
		                                           "loopType","none",
		                                           "easeType","linear",
		                                           "orientToPath",true,
		                                           "oncomplete", "checkEndLevel",
		                                           "oncompletetarget",this.gameObject));
		currLoc ++;
	}

	public void checkEndLevel() {
		if (currLoc == Levels.NUM_WALKS[(int)Levels.level]) {
			transform.rotation = new Quaternion(0,0,0,1);
			currLoc++;
			Progress();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (canProgress) {
			print ("CAN PROGRESS " + currLoc);
			if (currLoc > Levels.NUM_WALKS[(int)Levels.level]) {
				currLoc = 0;
				GameObject.Find("scene").GetComponent<Levels>().nextLevel();
			}
			else {
				translateAlongPath();
			}
			canProgress = false;
		}
	}

	public void Progress() {
		//currLoc ++;
		canProgress = true;
	}

	void OnGUI() {
		GUI.Label (new Rect (10, 100, 200, 200), currLoc.ToString ());
	}
}
