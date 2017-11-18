using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour {

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
	const float smoothTime = .1f;

	float target;
	float current;
	float smoothV;

	float roverSpeed;
	float circum;

    private void Start()
    {
        //mask = LayerMask.NameToLayer("Collision");
        radius = Mathf.Abs(frontOfWheel.localPosition.y);

        parent = transform.parent;
        initialParentLocalHeight = parent.localPosition.z;
		circum = 2 * Mathf.PI * radius;
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

        if (Physics.Raycast(frontOfWheel.position, -frontOfWheel.forward, out hit, radius,mask))
        {
            if (hit.collider != null)
            {
                grounded = true;
                float desiredWheelCentreHeight = hit.point.y + radius;
                float deltaHeight = desiredWheelCentreHeight - transform.position.y;
                float targetParentLocalHeight = parent.localPosition.z + deltaHeight;

                if (targetParentLocalHeight < initialParentLocalHeight)
                {
					target = initialParentLocalHeight;
                }
                else if (targetParentLocalHeight - initialParentLocalHeight > maxDelta)
                {
					target = initialParentLocalHeight + maxDelta;
                }
                else
                {
					target = targetParentLocalHeight;
                }
			
            }
        }

		target = Mathf.MoveTowards(target, initialParentLocalHeight, Time.fixedDeltaTime * returnToNormalSpeed);
		current = Mathf.SmoothDamp (current, target, ref smoothV, smoothTime);

		parent.localPosition = new Vector3(parent.localPosition.x, parent.localPosition.y, current);

        if (!grounded)
        {
            if (Physics.Raycast(frontOfWheel.position, -frontOfWheel.forward, radius + groundedSkin,mask))
            {
                grounded = true;
            }
        }
    }

    
    private void XXOnDrawGizmos()
    {
        Gizmos.color = (grounded) ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, .5f);
    }
    

}
