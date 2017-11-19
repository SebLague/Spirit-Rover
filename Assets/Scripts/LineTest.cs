using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTest : MonoBehaviour {

	public Transform[] t;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 c = Geometry.ClosestPointOnLineSeg (new Vector2(t [0].position.x,t [0].position.z), new Vector2(t [1].position.x,t [1].position.z),new Vector2(t [2].position.x,t [2].position.z));
		Debug.DrawLine(t [0].position, t [1].position, Color.green);
		Debug.DrawLine (t [2].position, new Vector3(c.x,t[0].position.y,c.y), Color.red);

	}

}
