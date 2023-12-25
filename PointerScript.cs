using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is the selector the player moves with the wasd keys

public class PointerScript : MonoBehaviour
{
    int x, y = 0;
	
	// Start is called before the first frame update.
    void Start()
    {
        // go to the bottom left corner
		transform.position = new Vector3(-3f, -3f, transform.position.z);
    }

    // Update is called once per frame.
    void Update()
    {
		// call the boss
		GameObject boss = GameObject.Find("Boss");
		BossScript bossScript = boss.GetComponent<BossScript>();
		
		int targetSquare = bossScript.GetTargetSquare();
		
        // only controllable in regular gameplay (gameState is 1)
		if (bossScript.GetGameState() == 1)
		{
			// move around: update the target square and do the maths later
			if ((Input.GetKeyDown("w")) && (targetSquare < 56))
			{
				bossScript.AugmentTargetSquare(8);
			}
			if ((Input.GetKeyDown("a")) && (targetSquare%8 != 0))
			{
				bossScript.AugmentTargetSquare(-1);
			}
			if ((Input.GetKeyDown("s")) && (targetSquare > 7))
			{
				bossScript.AugmentTargetSquare(-8);
			}
			if ((Input.GetKeyDown("d")) && (targetSquare%8 != 7))
			{
				bossScript.AugmentTargetSquare(1);
			}
			
			// select a square. up means pick it up, down means put it down
			if (Input.GetKeyDown("up"))
			{
				bossScript.PickUp();
			}
			
			if (Input.GetKeyDown("down"))
			{
				bossScript.PutDown();
			}
		}
		
		// update the position on the coordinate axes
		targetSquare = bossScript.GetTargetSquare();
		
		x = bossScript.GetTargetSquare()%8 - 3;
		y = bossScript.GetTargetSquare()/8 - 3;
		
		transform.position = new Vector3((float)x, (float)y, transform.position.z);
	}
}
