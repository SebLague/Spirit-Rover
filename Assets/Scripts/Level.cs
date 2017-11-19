using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public Transform roverSpawn;
	public Transform camStart;

	public Transform test;
	public Transform[] waypoints;
	Vector2[] path;

	void Awake() {
		path = new Vector2[waypoints.Length];
		for (int i = 0; i < waypoints.Length; i++) {
			path [i] = new Vector2 (waypoints [i].position.x, waypoints [i].position.z);
		}
	}
	/*
	void Update() {
		for (int i = 0; i < path.Length - 1; i++) {
			Debug.DrawLine (waypoints [i].position, waypoints [i + 1].position, Color.green);
			Vector2 test2D = new Vector2 (test.position.x, test.position.z);
			Debug.Log(GetDstToEnd(test2D) + "  straight: " + (test2D-path[waypoints.Length-1]).magnitude );
		}
	}
*/
	public float GetDstToEnd(Vector2 roverPos) {
		Vector2 p = Vector2.zero;
		float bestDst = float.MaxValue;
		int bestIndex = 0;
		for (int i = 0; i < path.Length-1; i++) {
			Vector2 pointOnSeg = Geometry.ClosestPointOnLineSeg (path [i], path [i + 1], roverPos);
			float dst = (pointOnSeg - roverPos).magnitude;
			if (dst < bestDst) {
				bestDst = dst;
				bestIndex = i;
				p = pointOnSeg;
			}
		}

		float remDst = (p-path[bestIndex+1]).magnitude;
		for (int i = bestIndex+1; i < path.Length-1; i++) {
			remDst += (path [i] - path [i + 1]).magnitude;
		}
		return remDst;
	}
}
