using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour {

    float currHeight;
    public LayerMask mask;
    Transform frontOfWheel;
    float radius;
    Transform parent;
    float initialParentLocalHeight;

    const float maxDelta = .5f;
    const float returnToNormalSpeed = .5f;
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

        if (Physics.Raycast(frontOfWheel.position, -frontOfWheel.forward, out hit, radius))
        {
            if (hit.collider != null)
            {
		
                float desiredWheelCentreHeight = hit.point.y + radius;
                float deltaHeight = desiredWheelCentreHeight - transform.position.y;
                float targetParentLocalHeight = parent.localPosition.z + deltaHeight;

                if (targetParentLocalHeight < initialParentLocalHeight)
                {
                   // print("less");
                    parent.localPosition = new Vector3(parent.localPosition.x, parent.localPosition.y, initialParentLocalHeight);
                }
                else if (targetParentLocalHeight - initialParentLocalHeight > maxDelta)
                {
                    //print("more");
                    parent.localPosition = new Vector3(parent.localPosition.x, parent.localPosition.y, initialParentLocalHeight + maxDelta);
                }
                else
                {
                    //print("set : " + targetParentLocalHeight);
                   // Debug.Log(parent.localPosition.z + "  " + targetParentLocalHeight);
                    parent.localPosition = new Vector3(parent.localPosition.x, parent.localPosition.y, targetParentLocalHeight);
                }

                   // Debug.DrawRay(frontOfWheel.position, -frontOfWheel.forward * (hit.distance), Color.green);
			
            }
        }

        float returnToNormal = Mathf.MoveTowards(parent.localPosition.z, initialParentLocalHeight, Time.fixedDeltaTime * returnToNormalSpeed);
        parent.localPosition = new Vector3(parent.localPosition.x, parent.localPosition.y, returnToNormal);
    }

    public void Raise(float height)
    {
       // Debug.Log("rause");
        //float deltaHeight = height - currHeight;
        //currHeight = height;
        //transform.position += transform.forward * deltaHeight;
    }

}
