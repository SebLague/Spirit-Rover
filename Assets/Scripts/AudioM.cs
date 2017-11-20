using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioM : MonoBehaviour {

	public AudioClip[] keyPress;
	public AudioClip[] enterPress;
	AudioSource a;
	static AudioM instance;
	// Use this for initialization
	void Start () {
		instance = this;
		a = GetComponent<AudioSource> ();
	}
	
	public static void PlayKeyPress(bool enterKey) {
		instance.a.PlayOneShot (instance.keyPress [Random.Range (0, instance.keyPress.Length)]);
		/*
		if (enterKey) {
			instance.a.PlayOneShot (instance.enterPress [Random.Range (0, instance.enterPress.Length)]);
		} else {
			
		}
		*/
	}
}
