using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThoughtBubble : MonoBehaviour {

	const float smoothTime = .2f;

	public Sprite[] smallSprites;
	public Sprite[] largeSprites;
	public Sprite[] dots;
	public SpriteRenderer[] dotRenderers;
	Sprite[] currentSprites;

	public float animTime = .1f;
	public SpriteRenderer r;
	public float smallScale = .3f;
	public float largeScale = .8f;
	public Text smallText;
	public Text largeText;
	public Vector3 offset;
	public float minY = -1.2f;
	public Transform dotConnectBig;
	public Transform dotConnectSmall;
	int animIndex;
	float nextT;
	bool flipX;

	Transform roverHead;
	Transform cam;
	float offsetY;
	Coroutine currentCoroutine;
	Rover rover;


	Vector3 smoothRefV;
	Vector3[] dotSmoothRefV;

	void Awake() {
		roverHead = FindObjectOfType<Rover> ().bubble;
	}

	void Start () {
		cam = Camera.main.transform;
		Deactivate ();
		dotSmoothRefV = new Vector3[dots.Length];

	}

	public void SetRover(Rover t) {
		roverHead = t.bubble;
		rover = t;
	}

	public void ShowThought(string thought,float duration) {
		Clear ();
		currentCoroutine = StartCoroutine (ThoughtProcess (thought,duration));
	}

	IEnumerator ThoughtProcess(string thought, float duration) {
		bool useBigBubble = thought.Length > 16 || (thought.Length > 6 && !thought.Contains(" "));

		transform.position = cam.position + cam.forward * offset.z + cam.right * offset.x + cam.up * offset.y;
		Vector3[] dotPos = GetDotPositions (useBigBubble);
		for (int i = 0; i < dotPos.Length; i++) {
			dotRenderers [i].transform.position = dotPos [i];
			dotRenderers [i].transform.LookAt (cam, cam.up);
		}

		smoothRefV = Vector3.zero;
		for (int i = 0; i < dotSmoothRefV.Length; i++) {
			dotSmoothRefV [i] = Vector3.zero;
		}


		currentSprites = (useBigBubble) ? largeSprites : smallSprites;
		r.transform.localScale = Vector3.one * ((useBigBubble) ? largeScale : smallScale);
		largeText.text = thought;
		smallText.text = thought;

		largeText.gameObject.SetActive (useBigBubble);
		smallText.gameObject.SetActive (!useBigBubble);
		r.gameObject.SetActive (true);

	
		//float headBelowBubbleDst = Mathf.Clamp(transform.position.y -roverHead.position.y,0,float.MaxValue);
		//offsetY = Mathf.Lerp (offset.y, minY, headBelowBubbleDst / 6f);

		//Debug.Log (headBelowBubbleDst + "  " + (headBelowBubbleDst / 6f) + "   " + offsetY);
		float endTime = Time.time + duration;

		foreach (SpriteRenderer dr in dotRenderers) {
			dr.gameObject.SetActive (true);
		}

		while (Time.time < endTime) {
			Vector3 targPos = cam.position + cam.forward * offset.z + cam.right * offset.x + cam.up * offset.y;
			transform.position = Vector3.SmoothDamp (transform.position, targPos, ref smoothRefV, smoothTime);
			transform.LookAt (cam.position,cam.up);

			dotPos = GetDotPositions (useBigBubble);
			for (int i = 0; i < dotPos.Length; i++) {
				dotRenderers [i].transform.position = Vector3.SmoothDamp(dotRenderers [i].transform.position, dotPos[i],ref dotSmoothRefV[i],smoothTime);
				dotRenderers [i].transform.LookAt (cam, cam.up);
			}

			if (Time.time > nextT) {
				for (int i = 0; i < dotRenderers.Length; i++) {
					dotRenderers [i].sprite = dots [Random.Range (0, dots.Length)];
				}
				animIndex++;
				animIndex %= currentSprites.Length;

				nextT = Time.time + animTime;
				r.sprite = currentSprites [animIndex];

				if (animIndex == 0) {
					r.flipX = !r.flipX;
				}
			}
			yield return null;
		}

		Deactivate ();

	}

	Vector3[] GetDotPositions(bool useBigBubble) {
		Vector3[] dotTargetPos = new Vector3[dotRenderers.Length];

		dotTargetPos [0]= roverHead.position + cam.up * .5f + cam.right * .5f;
		dotTargetPos [dotRenderers.Length-1]= (useBigBubble) ? dotConnectBig.position : dotConnectSmall.position;
		for (int i = 0; i < dotRenderers.Length; i++) {
			if (i > 0 && i < dotRenderers.Length - 1) {
				float percent = Mathf.InverseLerp (0, dotRenderers.Length - 1, i);
				dotTargetPos [i]= dotRenderers [0].transform.position * (1 - percent) + dotRenderers [dotRenderers.Length - 1].transform.position * percent;
			}
		}
		return dotTargetPos;
	}

	public void Clear() {
		if (currentCoroutine != null) {
			StopCoroutine (currentCoroutine);
		}
		Deactivate ();

	}

	void Deactivate() {
		r.gameObject.SetActive (false);
		smallText.gameObject.SetActive (false);
		largeText.gameObject.SetActive (false);
		foreach (SpriteRenderer dr in dotRenderers) {
			dr.gameObject.SetActive (false);
		}
	}

}
