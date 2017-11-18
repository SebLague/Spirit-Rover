using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	private void OnCollisionEnter(Collision collision)
	{
        Debug.Log("HIT " + collision.gameObject.name);
        if (collision.gameObject.GetComponent<Wheel>())
        {
            Debug.Log("weel");
            collision.gameObject.GetComponent<Wheel>().Raise(.3f);
        }
	}

	private void OnTriggerEnter(Collider collision)
	{
		Debug.Log("HIT " + collision.gameObject.name);
		if (collision.gameObject.GetComponent<Wheel>())
		{
			Debug.Log("weel");
			collision.gameObject.GetComponent<Wheel>().Raise(.3f);
		}
	}
}
