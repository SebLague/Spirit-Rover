using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour {
	
	const float lightSmoothTime = 18;

	const float startDelay = 0;
	const float lightRotStartTime = 0 + startDelay;
	const float mainTitleStartTime =2 + startDelay;
	const float titleRiseDuration = 12;
	public bool skipIntro;
	public GameObject[] disable;
	public GameObject[] enable;
	public Transform light;
	public Transform startTitle;
	public Transform endTitle;
	public Text title;
	public Text[] extraText;
	public float maxTitleAlpha;
	public RawImage fadePlane;

	float startTime;
	float titleMoveDst;
	float titleFadePercent;
	float subTitFadePercent;
	float[] subTitFadeOffsets = new float[]{0,1,2f};
	Color[] subTitCols;
	Vector3 lightSmoothV;
	Vector3 targetLightRot;
	bool playingIntro;

	void Start () {

		InitIntro ();

		if (skipIntro) {
			EndIntro ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (playingIntro) {
			if (Time.time - startTime > startDelay) {

				fadePlane.color = Color.Lerp (Color.black, Color.clear, FadePlaneEase(Time.time - startTime - startDelay,3));
				if (Time.time - startTime > lightRotStartTime) {
					light.eulerAngles = Vector3.SmoothDamp (light.eulerAngles, targetLightRot, ref lightSmoothV, lightSmoothTime);
				}

				if (Time.time - startTime > mainTitleStartTime) {
					
					title.color = Color.Lerp (Color.clear, new Color (1, 1, 1, maxTitleAlpha), FadePlaneEase(Time.time - startTime - mainTitleStartTime,6));
					if (Time.time - startTime - mainTitleStartTime < titleRiseDuration) {
						float titleMovePerc = Ease (Time.time - startTime - mainTitleStartTime, titleRiseDuration);
						title.transform.position = Vector3.Lerp (startTitle.position, endTitle.position, titleMovePerc);
					}
				}
				if (Time.time - startTime > mainTitleStartTime + titleRiseDuration + 1) {
					subTitFadePercent += Time.deltaTime * .5f;
					for (int i = 0; i < extraText.Length; i++) {
						extraText [i].color = Color.Lerp (Color.clear, subTitCols [i], subTitFadePercent - subTitFadeOffsets [i]);
					}
				}
			}

			if (Input.GetKeyDown (KeyCode.Return)) {
				EndIntro ();
			}
		}


	}

	void InitIntro() {
		foreach (GameObject g in disable) {
			g.SetActive (false);
		}
		foreach (GameObject g in enable) {
			g.SetActive (true);
		}

		title.gameObject.SetActive (true);
		title.color = Color.clear;
		subTitCols = new Color[extraText.Length];
		for (int i = 0; i < extraText.Length; i++) {
			subTitCols [i] = extraText [i].color;
			extraText[i].color = Color.clear;
			extraText[i].gameObject.SetActive (true);
		}
		titleMoveDst = (startTitle.position - endTitle.position).magnitude;
		targetLightRot = light.transform.eulerAngles;
		light.transform.eulerAngles = new Vector3 (0, -30, 0);
		fadePlane.gameObject.SetActive (true);
		fadePlane.color = Color.black;
		playingIntro = true;
		startTime = Time.time;

	}

	void EndIntro() {
		playingIntro = false;
		foreach (GameObject g in disable) {
			g.SetActive (true);
		}
		fadePlane.color = Color.clear;
		title.gameObject.SetActive (false);
		foreach (Text t in extraText) {
			t.gameObject.SetActive (true);
		}
		foreach (GameObject g in enable) {
			g.SetActive (false);
		}
		light.eulerAngles = targetLightRot;

	}

	float FadePlaneEase(float t, float d) {
		float b= 0;
		float c = 1;
		t /= d;
		return c*t*t*t + b;
	}

	float Ease(float t, float d) {
		float b= 0;
		float c = 1;
		t /= d/2;
		if (t < 1) return c/2*t*t + b;
		t--;
		return -c/2 * (t*(t-2) - 1) + b;
	}
}
