using UnityEngine;
using System.Collections;

public class Levels : MonoBehaviour {
	
	public static ArrayList PATHS_LIST;
	public static int NUM_PUZZLES = 2;
	public static int[] NUM_WALKS;
	public static float levelChangeSpeed = 1.0f; 
	//create paths between Locations
	public Vector3[] paths;

	public enum LEVELS {
		LEVEL_1,
		LEVEL_2,
		LEVEL_3
	};
	public static LEVELS level;

	private static Shader transparent_shader;
	private static Shader diffuse_shader;
	// Use this for initialization

	void Awake() {
		disableAllSpheres ();
		setSpheres ();
	}

	void Start () {
		transparent_shader = Shader.Find("Transparent/Diffuse");
		diffuse_shader = Shader.Find("Diffuse");
		NUM_WALKS = new int[3]{3,5,0};
		level = LEVELS.LEVEL_1;
		//paths
		setPaths ();
		initLevels ();
	}

	public void setSpheres() {
		Collider[] targets = GameObject.Find ("Spheres" + (int)(level+1)).GetComponentsInChildren<SphereCollider> ();
		Collider[] sources = GameObject.Find ("Points" + (int)(level+1)).GetComponentsInChildren<SphereCollider> ();
		for (int i = 0; i < targets.Length; i++) {
			targets[i].enabled = true;
			sources[i].enabled = true;
		}
	}

	public void disableAllSpheres() {
		for (int j = 1; j < 3; j++) {
			Collider[] targets = GameObject.Find ("Spheres" + j).GetComponentsInChildren<SphereCollider> ();
			Collider[] sources = GameObject.Find ("Points" + j).GetComponentsInChildren<SphereCollider> ();
			for (int i = 0; i < targets.Length; i++) {
				targets[i].enabled = false;
				sources[i].enabled = false;
			}
		}
	}

	public void setPaths() {
		print ("level " + level);
		//int path_length = 3;
		PATHS_LIST = new ArrayList ();
		//paths = new Vector3[Puzzle_Behavior.NUM_WALKS,path_length];
		GameObject path, loc;
		for (int i = 0; i < NUM_WALKS[(int)level]; i++) {
			path = GameObject.Find("Path" + (int)(level+1) + "_" + i);
			int count = path.transform.childCount;
			paths = new Vector3[count];
			for (int j = 0; j < count; j++) {
				loc = GameObject.Find ("Location" + (int)(level+1) + "_" + i + "_" + j);
				paths[j] = loc.transform.position;
				//print (i+ " " + paths[j]);
			}
			PATHS_LIST.Add(paths);
		}
	}

	public void initLevels() {
		for (int i = 0; i < 3; i++) {
			GameObject level = GameObject.Find ("level" + (int)(i+1));
			Renderer[] l_shaders = level.GetComponentsInChildren<Renderer>();
			if (i != 0) {
				foreach (Renderer shader in l_shaders) {
					shader.material.shader = transparent_shader;
				}
				iTween.FadeTo (level, 0.0f, 0.0f);
			}
		}
	}

	//draw paths
	void OnDrawGizmos() {
		Vector3[] path;
		Color[] colors = new Color[3]{Color.blue, Color.red, Color.cyan};
		if (PATHS_LIST == null) {
			return;
		}
		for (int i = 0, j = 0; i < PATHS_LIST.Count; j++, i++) {
			path = (Vector3[]) PATHS_LIST[i];
			if (j >= colors.Length) {j=0;}
			iTween.DrawPath (path, colors[j]);
		}
	}

	public void nextLevel() {
		if (level == LEVELS.LEVEL_1) {
			GameObject level1 = GameObject.Find ("level1");
			GameObject level2 = GameObject.Find ("level2");
			GameObject levels = GameObject.Find ("Levels");
			translateLevels(level1, level2, levels);
			setPaths();
			disableAllSpheres();
			setSpheres();
			for ( int i = 0; i < PATHS_LIST.Count; i++) {
				Vector3[] path = (Vector3[]) PATHS_LIST[i];
				/*foreach (Transform t in path.GetComponentsInChildren<Transform>()) {
					t.Translate(new Vector3(0.0f, -0.9f, 0.0f));
				}*/
				for (int j = 0; j < path.Length; j++) {
					path[j] = new Vector3(path[j].x, path[j].y-0.9f, path[j].z);
				}
			}
			print (GameObject.Find ("Player").GetComponent<PlayerScript>().currLoc);
		}
		else if (level == LEVELS.LEVEL_2) {
			GameObject level1 = GameObject.Find ("level2");
			GameObject level2 = GameObject.Find ("level3");
			GameObject levels = GameObject.Find ("Levels");
			translateLevels(level1, level2, levels);
			setPaths();
		}
		else if (level == LEVELS.LEVEL_3) {
			
		}
	}

	public void translateLevels(GameObject l1, GameObject l2, GameObject l) {
		GameObject girl = GameObject.Find ("Player");
		iTween.MoveBy (l, iTween.Hash ("time", 3.0f,
		                               "amount", new Vector3 (0.0f, -0.9f, 0.0f), 
		                               "oncomplete", "Progress",
		                               "oncompletetarget", girl));
		iTween.MoveBy (girl, new Vector3 (0.0f, -0.9f, 0.0f), 3.0f);

		//l1.transform.GetComponentInChildren<Renderer>().material.shader;
		Renderer[] l1_shaders = l1.GetComponentsInChildren<Renderer>();
		Renderer[] l2_shaders = l2.GetComponentsInChildren<Renderer>();
		foreach (Renderer shader in l1_shaders) {
			shader.material.shader = transparent_shader;
		}
		iTween.FadeTo (l1, 0.0f, levelChangeSpeed);
		iTween.FadeTo (l2, iTween.Hash("alpha", 1.0f,
		                               "time",levelChangeSpeed,
		                               "delay", levelChangeSpeed,
		                               "oncomplete", "changeToDiffuse",
		                               "oncompletetarget", this.gameObject,
		                               "oncompleteparams", l2_shaders));
		level++;
	}

	public void changeToDiffuse(Renderer[] l2_shaders) {
		foreach (Renderer shader in l2_shaders) {
			shader.material.shader = diffuse_shader;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
