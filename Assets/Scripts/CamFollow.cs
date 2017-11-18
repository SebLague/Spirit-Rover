using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamFollow : MonoBehaviour {

	//public float optimalDistance
	public Transform lookPos;
	Rover r;

	void Update() {
		SetLookPos ();
	}

	public void SetNewLevel(Transform start, Rover rover) {
		r = rover;
		SetLookPos ();

		CinemachineBrain b = GetComponent<CinemachineBrain> ();
		b.ActiveVirtualCamera.LookAt = lookPos;
		b.ActiveVirtualCamera.Follow = rover.transform;

	}

	void SetLookPos() {
		if (r != null) {
			lookPos.position = r.transform.position;
		}
	}
}
