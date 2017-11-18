using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour {

    float currHeight;

    private void Start()
    {
        
    }

    public void Raise(float height)
    {
        Debug.Log("rause");
        float deltaHeight = height - currHeight;
        currHeight = height;
        transform.position += transform.forward * deltaHeight;
    }

}
