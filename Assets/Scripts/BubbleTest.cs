using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleTest : MonoBehaviour {

	public Sprite[] s;
	public float t;
	SpriteRenderer r;
	int i;
	float nextT;

	// Use this for initialization
	void Start () {
		r = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextT) {
			nextT =Time.time + t;
			r.sprite = s [i];
			i++;
			i %= s.Length;
		}
	}
}
