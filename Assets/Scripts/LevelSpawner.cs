using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour {

    public GameObject roverPrefab;
    public Level[] levels;
    public Level currentLevel;
    public int levelIndex;
	Rover rover;
	public UnityEngine.UI.Text dstRemTxt;

    private void Start()
    {
		rover = FindObjectOfType<Rover> ();
    }

    public void ResetLevel()
    {
        LoadLevel(levelIndex);
    }

    public void LoadLevel(int i)
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel.gameObject);
        }
		if (FindObjectOfType<Rover>() != null)
		{
            Destroy(FindObjectOfType<Rover>().gameObject);
		}

        currentLevel = Instantiate(levels[i], Vector3.zero, Quaternion.identity, transform);
		currentLevel.enabled = true;
		Transform roverSpawn = currentLevel.roverSpawn;
		rover = Instantiate(roverPrefab, roverSpawn.position, roverSpawn.rotation, roverSpawn).GetComponent<Rover>();
		FindObjectOfType<CamFollow> ().SetNewLevel (currentLevel.camStart, rover);
		FindObjectOfType<ThoughtBubble> ().SetRover (rover);
    }

	void Update() {
		float dst = currentLevel.GetDstToEnd (new Vector2 (rover.transform.position.x, rover.transform.position.z));

		float dstR = Mathf.RoundToInt (dst * 10) / 10f;
		//Debug.Log (dst +  "   " + dstR);
		string dec = string.Format ("{0:0.#}", dstR);
		if (!dec.Contains (".")) {
			dec += ".0";
		}
		dstRemTxt.text = "Dst remaining: " + dec+ "m";
	}
 

}
