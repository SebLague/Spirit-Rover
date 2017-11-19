using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtGen : MonoBehaviour {

	const float intialDelayBeforeRandomThoughts = 5;
	const int numThoughtsBeforeRepeating = 20;
	const float minTimeBetweenThoughts = 5;
	const float maxTimeBetweenThoughts = 10;
	readonly Vector2 delayBetweenChainedThoughtsMinMax = new Vector2(1,2f);

	public Thought[] beginThoughts;
	public Thought[] successThoughts;
	public Thought[] endOfCommandThoughts;
	public Thought[] toppleThoughts;
	public Thought[] stuckThoughts;
	public Thought[] accThoughts;
	public Thought[] brakeThoughts;
	public Thought[] turnThoughts;
	public Thought[] randomThoughts;
	public Thought[] stuckPleas;
	public Thought[] toppledPleas;

	bool hasBegun;
	Coroutine trainOfThought;
	bool isThinking;
	float nextThinkTime;

	ThoughtBubble bubble;
	Queue<Thought> lastNThoughts = new Queue<Thought>();
	float nextAllowedRandomThoughtTime;

	void Start() {
		
		bubble = FindObjectOfType<ThoughtBubble> ();
		Rover.OnCommandRunForFirstTime += OnCommandUsed;
		Rover.OnCommandsFinished += OnCommandsFinished;
		Rover.OnStuck += OnStuck;
		Rover.OnTopple += OnTopple;
		Rover.OnBegin += Rover_OnBegin;
		Console.OnHelpMenuOpen += ClearThought ;
	}
		

	void ShowThought(Thought thought, bool interrupt = false) {
		if (!lastNThoughts.Contains (thought)) {
			lastNThoughts.Enqueue (thought);
			if (lastNThoughts.Count > numThoughtsBeforeRepeating) {
				lastNThoughts.Dequeue ();
			}

			if (!Console.helpMenuOpen) {
				if (HappyForNewThought () || interrupt) {
					if (trainOfThought != null) {
						StopCoroutine (trainOfThought);
					}
					trainOfThought = StartCoroutine (TrainOfThought (thought));
				}
			}
		}
	}

	void ClearThought() {
		isThinking = false;
		if (trainOfThought != null) {
			StopCoroutine (trainOfThought);
		}
		bubble.Clear ();
	}

	void Update() {
		if (hasBegun) {
			if (HappyForNewThought ()) {
				if (Rover.instance.isToppled) {
					ShowThought(toppledPleas[Random.Range(0,toppledPleas.Length)]);
				}
				else if (Rover.instance.isStuck ) {
					ShowThought(stuckPleas[Random.Range(0,stuckPleas.Length)]);
				} else {
					if (Time.time > nextAllowedRandomThoughtTime) {
						ShowThought (GetRandomThought ());
					}
				}
			}
		}
	}

	bool HappyForNewThought() {
		return !isThinking && (Time.time > nextThinkTime) && !Console.helpMenuOpen;
	}

	IEnumerator TrainOfThought(Thought thought) {
		isThinking = true;
		string[] allTexts = new string[]{ thought.text };
		if (thought.continuation != null && thought.continuation.Length >0) {
			allTexts = new string[thought.continuation.Length + 1];
			allTexts [0] = thought.text;
			for (int i = 0; i < thought.continuation.Length; i++) {
				allTexts [i + 1] = thought.continuation [i];
			}
		}

		for (int i = 0; i < allTexts.Length; i++) {
			string text = allTexts [i];
			float duration = 2.5f + text.Length / 16f;
			bubble.ShowThought (text,duration);
			float delay = Random.Range (delayBetweenChainedThoughtsMinMax.x, delayBetweenChainedThoughtsMinMax.y);
			if (i == allTexts.Length - 1) {
				delay = 0;
			}
			yield return new WaitForSeconds (duration + delay);
		}
		isThinking = false;
		nextThinkTime = Time.time + Random.Range (minTimeBetweenThoughts, maxTimeBetweenThoughts);
		nextAllowedRandomThoughtTime = nextThinkTime + 3;
	}


	Thought GetRandomThought() {
		int randInd = Random.Range (0, randomThoughts.Length);
		for (int i = 0; i < randomThoughts.Length; i++) {
			
			Thought r = randomThoughts [(randInd+i)%randomThoughts.Length];
			if (!lastNThoughts.Contains(r)) {
				return r;
			}
		}
	
		return randomThoughts [0];
	}
	

	void Rover_OnBegin ()
	{
		nextAllowedRandomThoughtTime = Time.time + intialDelayBeforeRandomThoughts;
		hasBegun = true;
		ClearThought ();

		float skipChance = 70;
		if (DontSkip (skipChance)) {
			ShowThought(beginThoughts[Random.Range(0,beginThoughts.Length)]);
		}
	}

	void OnCommandUsed(Command command) {
		float skipChance = 70;
		if (DontSkip (skipChance)) {
			if (command.commandType == Command.CommandType.Accelerate) {
				ShowThought(accThoughts[Random.Range(0,accThoughts.Length)]);
			}
			else if (command.commandType == Command.CommandType.Brake) {
				ShowThought(brakeThoughts[Random.Range(0,brakeThoughts.Length)]);
			}
			else if (command.commandType == Command.CommandType.Left || command.commandType == Command.CommandType.Right) {
				ShowThought(turnThoughts[Random.Range(0,turnThoughts.Length)]);
			}
		}
	}

	void OnCommandsFinished() {
		float skipChance = 70;
		if (DontSkip (skipChance)) {
			ShowThought(endOfCommandThoughts[Random.Range(0,endOfCommandThoughts.Length)]);
		}
	}

	void OnStuck() {
		ShowThought (stuckThoughts [Random.Range (0,stuckThoughts.Length)],true);
	}

	void OnTopple() {
		ShowThought (toppleThoughts [Random.Range (0,toppleThoughts.Length)],true);
	}

	bool DontSkip(float skipChance) {
		return Random.Range (0.01f, 100) > skipChance;
	}

}

[System.Serializable]
public class Thought {
	public string text;
	public string[] continuation;

}