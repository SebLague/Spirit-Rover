using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

	public RawImage fadePlane;
	public Text gameOverText;
	public GameObject hide;
	public Text pressEnter;
	bool gameOver;
	float localTime;
	// Use this for initialization
	void Start () {
		Rover.OnWin += OnGameOver;
		gameOverText.gameObject.SetActive (false);
	}

	void Update() {
		if (gameOver) {
			localTime += Time.deltaTime;
			float p1 = Mathf.Clamp01(localTime/6f);
			float p2 = Mathf.Clamp01(Ease (localTime, 6));
			float p3 = Mathf.Clamp01(Ease (localTime, 6));
			gameOverText.color = Color.Lerp(Color.clear,Color.white,p1);
			gameOverText.rectTransform.localScale = Vector3.one * Mathf.Lerp (.7f, 1, p2);
			fadePlane.color = Color.Lerp (Color.clear, new Color (0, 0, 0, .5f), p3);
			pressEnter.color = Color.Lerp (Color.clear, new Color(1,1,1,.8f), Mathf.Clamp01 ((localTime - 6) / 6f));
			if (localTime > 3) {
				if (Input.GetKeyDown (KeyCode.Return)) {
					PlayAgain ();
				}
			}
		}
	}

	void PlayAgain() {
		hide.SetActive (true);
		localTime = 0;
		gameOver = false;
		gameOverText.gameObject.SetActive (false);
		fadePlane.color = Color.clear;
		pressEnter.gameObject.SetActive (false);
		FindObjectOfType<Console> ().Upload ();
	}
	
	void OnGameOver() {
		hide.SetActive (false);
		pressEnter.gameObject.SetActive (true);
		pressEnter.color = Color.clear;
		gameOverText.gameObject.SetActive (true);
		gameOverText.color = Color.clear;
		fadePlane.gameObject.SetActive (true);
		fadePlane.color = Color.clear;
		gameOver = true;
	}

	float Ease(float t, float d) {
		float b= 0;
		float c = 1;
		t /= d;
		return c*t*t*t + b;
	}
}
