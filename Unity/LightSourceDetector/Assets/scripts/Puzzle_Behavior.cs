using UnityEngine;
using System.Collections;

public class Puzzle_Behavior : MonoBehaviour {

	private Vector3 m_initPos = Vector3.zero;
	public static int NUM_PUZZLES = 4;
	GameObject[] puzzle_spheres = new GameObject[NUM_PUZZLES];
	// Use this for initialization
	void Start () {
		for (int i = 0; i < puzzle_spheres.Length; i++) {
			GameObject puzzle_sphere = 
				(GameObject) GameObject.Instantiate(Resources.Load ("Sphere"),m_initPos, Quaternion.identity);
			puzzle_sphere.renderer.enabled = false;
			puzzle_sphere.name = "sphere" + i;
			puzzle_spheres[i] = puzzle_sphere;
		}
	}
	
	// Update is called once per frame
	void Update () {
		GameObject m_sphere = null;
		for (int i = 0; i < NUM_PUZZLES; i++) {
			m_sphere = puzzle_spheres[i];
			m_sphere.transform.Translate(0,1,0);
		}
	}
}
