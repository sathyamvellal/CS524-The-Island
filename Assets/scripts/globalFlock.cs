using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalFlock : MonoBehaviour {
    public GameObject animalPrefab;
    public GameObject goalPrefab;
    static int numbFlock = 50;
    public static GameObject[] allAnimals = new GameObject[numbFlock];
    public static int tankSize = 5;
    
    public static Vector3 goalPos = Vector3.zero;
	// Use this for initialization
	void Start () {
        for(int i = 0; i<numbFlock; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-tankSize, tankSize),
                                      Random.Range(-tankSize, tankSize),
                                      Random.Range(-tankSize, tankSize));
            allAnimals[i] = (GameObject)Instantiate(animalPrefab, pos, Quaternion.identity);
        }
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Random.Range(0, 10000) < 50)
        {
            goalPos = new Vector3(Random.Range(-tankSize, tankSize),
                                  Random.Range(-tankSize, tankSize),
                                  Random.Range(-tankSize, tankSize));
            goalPrefab.transform.position = goalPos;
        }
    }
}
