using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour {

    public GameObject roverPrefab;
    public Level[] levels;
    public Level currentLevel;
    public int levelIndex;
  
    private void Start()
    {
        ResetLevel();
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
		Transform roverSpawn = currentLevel.roverSpawn;
		Rover r = Instantiate(roverPrefab, roverSpawn.position, roverSpawn.rotation, roverSpawn).GetComponent<Rover>();
		FindObjectOfType<CamFollow> ().SetNewLevel (currentLevel.camStart, r);
    }

 

}
