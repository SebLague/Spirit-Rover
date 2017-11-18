using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour {

    public bool grounded;
    float currHeight;
    public LayerMask mask;
    Transform frontOfWheel;
    float radius;
    Transform parent;
    float initialParentLocalHeight;

    const float maxDelta = .5f;
    const float returnToNormalSpeed = .5f;
    const float groundedSkin = .1f;

    private void Start()
    {
        //mask = LayerMask.NameToLayer("Collision");
        frontOfWheel = transform.GetChild(0);
        radius = Mathf.Abs(frontOfWheel.localPosition.y);

        parent = transform.parent;
        initialParentLocalHeight = parent.localPosition.z;
    }

    private void FixedUpdate()
    {
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
                    parent.localPosition = new Vector3(parent.localPosition.x, parent.localPosition.y, initialParentLocalHeight);
                }
                else if (targetParentLocalHeight - initialParentLocalHeight > maxDelta)
                {
                    parent.localPosition = new Vector3(parent.localPosition.x, parent.localPosition.y, initialParentLocalHeight + maxDelta);
                }
                else
                {
                    parent.localPosition = new Vector3(parent.localPosition.x, parent.localPosition.y, targetParentLocalHeight);
                }
			
            }
        }

        float returnToNormal = Mathf.MoveTowards(parent.localPosition.z, initialParentLocalHeight, Time.fixedDeltaTime * returnToNormalSpeed);
        parent.localPosition = new Vector3(parent.localPosition.x, parent.localPosition.y, returnToNormal);

        if (!grounded)
        {
            if (Physics.Raycast(frontOfWheel.position, -frontOfWheel.forward, radius + groundedSkin,mask))
            {
                grounded = true;
            }
        }
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = (grounded) ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, .5f);
    }
    */

}
