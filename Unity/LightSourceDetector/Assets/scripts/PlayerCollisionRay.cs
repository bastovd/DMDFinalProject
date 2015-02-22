using UnityEngine;
using System.Collections;

public class PlayerCollisionRay : MonoBehaviour {

	GameObject m_camera;
	RaycastHit hit;

	// Use this for initialization
	void Start () {
		m_camera = GameObject.Find ("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			if (hit.transform.tag == "targetSphere") {
				//print ("I am looking at " + hit.transform.name);
				GameObject targetObj = GameObject.Find (hit.transform.name);
				TargetSphere targetScript = targetObj.GetComponent<TargetSphere>();
				int id = targetScript.m_Id;
				for (int i = 0; i < Puzzle_Behavior.NUM_PUZZLES; i ++) {
					GameObject controlPoint = GameObject.Find ("Point" + (i+1));
					ControlPoint pointScript = controlPoint.GetComponent<ControlPoint>();
					if (pointScript.m_Id == id) {
						float dist = Vector3.Distance(transform.position, controlPoint.transform.position);
						if (dist <= pointScript.m_radius) {
							print ("hit " + hit.transform.name);
							int playerLoc = GameObject.Find ("Player").GetComponent<PlayerScript>().currLoc;
							if (playerLoc < id) {
								GameObject.Find ("Player").GetComponent<PlayerScript>().Progress();
							}
						}
					}
				}
			}
		} else {
		//	print("I'm looking at nothing!");
		}
	}
}
