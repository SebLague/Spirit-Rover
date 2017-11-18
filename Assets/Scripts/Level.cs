using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public GameObject[] levels;
    public GameObject currentLevel;
    public int levelIndex;

    public void ResetLevel()
    {
        LoadLevel(levelIndex);
    }

    public void LoadLevel(int i)
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }

        currentLevel = Instantiate(levels[i], Vector3.zero, Quaternion.identity, transform);
    }

}
