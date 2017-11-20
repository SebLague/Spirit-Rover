using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamFollow : MonoBehaviour {

	//public float optimalDistance
	public Transform target;
	public Vector3 offset;
	public Transform lookPos;
	CinemachineBrain brain;
	Rover r;

	void LateUpdate() {

	}

	void Update() {
		//SetLookPos ();
	}

	public void SetNewLevel(Transform start, Rover rover) {
		r = rover;
		SetLookPos ();

		brain = GetComponent<CinemachineBrain> ();
		brain.ActiveVirtualCamera.LookAt = r.head;
		brain.ActiveVirtualCamera.Follow = rover.camFollowPos;

	}

	void SetLookPos() {
		if (r != null) {
			lookPos.position = r.transform.position;
		}
	}
}
