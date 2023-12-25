using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this guy drops in the piece, auth and check clones to all 64 squares

public class SpawnerScript : MonoBehaviour
{
    public GameObject pie;
    public GameObject auth;
    public GameObject check;
	
	// Start is called before the first frame update
    void Start()
    {
        // create 64 pieces all in a grid pattern
		int i, j;
		for (i=-3; i<5; i++)
		{
			for (j=-3; j<5; j++)
			{
				Instantiate(pie, new Vector3((float)i, (float)j, 0), transform.rotation);
				Instantiate(auth, new Vector3((float)i, (float)j, 0), transform.rotation);
				Instantiate(check, new Vector3((float)i, (float)j, -1), transform.rotation);
			}
		}
		
		// that's it that's all this does
    }
}
