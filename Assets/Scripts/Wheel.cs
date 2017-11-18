using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour {

	public LineRenderer tracksPrefab;
	public Transform frontOfWheel;
	public Transform mesh;
    public bool grounded;
    float currHeight;
    public LayerMask mask;
    float radius;
    Transform parent;
    float initialParentLocalHeight;

    const float maxDelta = .5f;
    const float returnToNormalSpeed = .5f;
    const float groundedSkin = .1f;
	const float smoothTime = .2f;
	const float trackDst = .15f;
	const float trackRaiseAboveGround = .05f;

	float target;
	float current;
	float smoothV;

	float roverSpeed;
	float circum;

	LineRenderer currentTrack;
	Vector3 lastTrackPoint;
	bool lastTrackPointActive;
	Transform levelT;

    private void Start()
    {
        //mask = LayerMask.NameToLayer("Collision");
        radius = Mathf.Abs(frontOfWheel.localPosition.y);

        parent = transform.parent;
        initialParentLocalHeight = parent.localPosition.z;
		circum = 2 * Mathf.PI * radius;
		levelT = FindObjectOfType<Level> ().transform;
    }

	public void SetSpeed(float s) {
		roverSpeed = s;
	}

    private void FixedUpdate()
    {
		float moveDst = roverSpeed * Time.fixedDeltaTime;
		float revolutions = moveDst / circum;
		mesh.Rotate (Vector3.right, 360*revolutions, Space.Self);
        RaycastHit hit;
        grounded = false;

		if (Physics.Raycast (frontOfWheel.position, -frontOfWheel.forward, out hit, radius + groundedSkin, mask)) {
        
			grounded = true;

			if (frontOfWheel.position.y - hit.point.y <= radius) {
				float desiredWheelCentreHeight = hit.point.y + radius;
				float deltaHeight = desiredWheelCentreHeight - transform.position.y;
				float targetParentLocalHeight = parent.localPosition.z + deltaHeight;

				if (targetParentLocalHeight < initialParentLocalHeight) {
					target = initialParentLocalHeight;
				} else if (targetParentLocalHeight - initialParentLocalHeight > maxDelta) {
					target = initialParentLocalHeight + maxDelta;
				} else {
					target = targetParentLocalHeight;
				}
			}

			if (hit.collider.tag == "Soil") {
				if (lastTrackPointActive) {
					Vector3 newTrackPoint = hit.point + hit.normal * trackRaiseAboveGround;
					if (Vector3.Distance (newTrackPoint, lastTrackPoint) >= trackDst) {
						if (currentTrack == null) {
							currentTrack = Instantiate<LineRenderer> (tracksPrefab, levelT);
							currentTrack.transform.eulerAngles = new Vector3 (-90, 0, 0);
							currentTrack.positionCount = 1;
							currentTrack.SetPosition (0, lastTrackPoint);
						}
						currentTrack.positionCount++;
						currentTrack.SetPosition (currentTrack.positionCount - 1, newTrackPoint);
						lastTrackPoint = newTrackPoint;
					}
				} else {
					lastTrackPoint = hit.point + hit.normal * trackRaiseAboveGround;
					lastTrackPointActive = true;
				}
			} else {
				lastTrackPointActive = false;
				currentTrack = null;
			}
            
		} else {
			lastTrackPointActive = false;
			currentTrack = null;
		}

		target = Mathf.MoveTowards(target, initialParentLocalHeight, Time.fixedDeltaTime * returnToNormalSpeed);
		current = Mathf.SmoothDamp (current, target, ref smoothV, smoothTime);

		parent.localPosition = new Vector3(parent.localPosition.x, parent.localPosition.y, current);


		if (grounded) {

		}
    }

    
    private void XXOnDrawGizmos()
    {
        Gizmos.color = (grounded) ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, .5f);
    }
    

}
