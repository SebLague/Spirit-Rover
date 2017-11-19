using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThoughtBubble : MonoBehaviour {


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

	void Awake() {
		roverHead = FindObjectOfType<Rover> ().bubble;
	}

	void Start () {
		cam = Camera.main.transform;
		Deactivate ();
	}

	public void SetHead(Transform t) {
		roverHead = t;
	}

	public void ShowThought(string thought,float duration) {
		if (currentCoroutine != null) {
			StopCoroutine (currentCoroutine);
		}
		currentCoroutine = StartCoroutine (ThoughtProcess (thought,duration));
	}

	IEnumerator ThoughtProcess(string thought, float duration) {
		transform.position = cam.position + cam.forward * offset.z + cam.right * offset.x + cam.up * offset.y;

		bool useBigBubble = thought.Length > 16;
		currentSprites = (useBigBubble) ? largeSprites : smallSprites;
		r.transform.localScale = Vector3.one * ((useBigBubble) ? largeScale : smallScale);
		largeText.text = thought;
		smallText.text = thought;

		largeText.gameObject.SetActive (useBigBubble);
		smallText.gameObject.SetActive (!useBigBubble);
		r.gameObject.SetActive (true);

		float headBelowBubbleDst = Mathf.Clamp(transform.position.y -roverHead.position.y,0,float.MaxValue);
		offsetY = Mathf.Lerp (offset.y, minY, headBelowBubbleDst / 6f);
		//Debug.Log (headBelowBubbleDst + "  " + (headBelowBubbleDst / 6f) + "   " + offsetY);
		float endTime = Time.time + duration;

		foreach (SpriteRenderer dr in dotRenderers) {
			dr.gameObject.SetActive (true);
		}

		while (Time.time < endTime) {
			transform.position = cam.position + cam.forward * offset.z + cam.right * offset.x + cam.up * offsetY;
			transform.LookAt (cam.position);

			dotRenderers [0].transform.position = roverHead.position + cam.up * .5f + cam.right * .5f;
			dotRenderers [dotRenderers.Length-1].transform.position = (useBigBubble) ? dotConnectBig.position : dotConnectSmall.position;
			for (int i = 0; i < dotRenderers.Length; i++) {
				if (i > 0 && i < dotRenderers.Length - 1) {
					float percent = Mathf.InverseLerp (0, dotRenderers.Length - 1, i);
					dotRenderers [i].transform.position = dotRenderers [0].transform.position * (1 - percent) + dotRenderers [dotRenderers.Length - 1].transform.position * percent;
				}
				dotRenderers [i].transform.LookAt (cam.position);
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
