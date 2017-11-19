using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour {

	public ParticleSystem dustParticle;
	public LineRenderer tracksPrefab;
	public Transform frontOfWheel;
	public Transform mesh;
    public bool grounded;
    float currHeight;
    public LayerMask mask;
    float radius;
    Transform parent;
    float initialParentLocalHeight;
	public bool isFrontWheel;
    const float maxDelta = .5f;
    const float returnToNormalSpeed = 1f;
    const float groundedSkin = .25f;
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

	ParticleSystem.MainModule dustMainMod;
	ParticleSystem.EmissionModule dustEmmMod;

	float timeNotGrounded;
	float wheelSpeedMultiplier = 1;
	float targetWheelSpeedMultiplier = 1;
	float wheelSpeedMultiplierSmoothV;

	float wheelSlowDownTime;

    private void Start()
    {
        //mask = LayerMask.NameToLayer("Collision");
        radius = Mathf.Abs(frontOfWheel.localPosition.y);

        parent = transform.parent;
        initialParentLocalHeight = parent.localPosition.z;
		circum = 2 * Mathf.PI * radius;
		levelT = FindObjectOfType<Level> ().transform;

		if (dustParticle != null) {
			dustMainMod = dustParticle.main;
			dustEmmMod = dustParticle.emission;
		}

		wheelSlowDownTime = Random.Range (5f, 12f);
    }

	public void SetSpeed(float s) {
		roverSpeed = s;
	}

    private void FixedUpdate()
    {
		float moveDst = roverSpeed * Time.fixedDeltaTime;
		float revolutions = moveDst / circum;
		mesh.Rotate (Vector3.right, 360*revolutions * wheelSpeedMultiplier, Space.Self);
        RaycastHit hit;
        grounded = false;
		timeNotGrounded += Time.fixedDeltaTime;
		if (isFrontWheel) {
			WheelRaise ();
		}

		if (Physics.Raycast (transform.position, -transform.forward, out hit, radius + groundedSkin, mask)) {

			grounded = true;
			timeNotGrounded = 0;

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


		if (dustParticle != null) {
			dustMainMod.startSpeed = 1+roverSpeed;
			dustEmmMod.rateOverTime = (grounded)? roverSpeed * 15:0;
		}

		targetWheelSpeedMultiplier = (timeNotGrounded > 2) ? 0 : 1;
		float smoothT = (targetWheelSpeedMultiplier>.9f)?.5f:wheelSlowDownTime;
		wheelSpeedMultiplier = Mathf.SmoothDamp(wheelSpeedMultiplier,targetWheelSpeedMultiplier,ref wheelSpeedMultiplierSmoothV,smoothT);

    }


	void WheelRaise() {
		RaycastHit hit;
		if (Physics.Raycast (frontOfWheel.position, -frontOfWheel.forward, out hit, radius + groundedSkin, mask)) {
			if (frontOfWheel.position.y - hit.point.y <= radius) {
				float desiredWheelCentreHeight = hit.point.y + radius-.05f;
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
		} 

		target = Mathf.MoveTowards(target, initialParentLocalHeight, Time.fixedDeltaTime * returnToNormalSpeed);
		current = Mathf.SmoothDamp (current, target, ref smoothV, smoothTime);

		parent.localPosition = new Vector3(parent.localPosition.x, parent.localPosition.y, current);

	}
    
    private void OnDrawGizmos()
    {
        Gizmos.color = (grounded) ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, .5f);
    }
    

}
