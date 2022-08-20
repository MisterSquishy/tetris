using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] tetrominos;

    void Start()
    {  
        spawnNext();
    }

    void Update()
    {
        
    }

    public void spawnNext() {
        int i = Random.Range(0, tetrominos.Length);
        Instantiate(tetrominos[i],
                    transform.position,
                    Quaternion.identity);
    }
}
