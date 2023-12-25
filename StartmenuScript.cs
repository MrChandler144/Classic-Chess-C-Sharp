using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartmenuScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
	public Sprite[] spriteArray;
	
	public AudioSource source;
	public AudioClip clip;
	
	int costume;		// 1 is home(start), 2 is home(tutorial), 3 is home(credits), 4 is tutorial, 5 is credits, 6 is end screen
	bool soundHasPlayed;
	
	// Start is called before the first frame update
    void Start()
    {
        costume=1;
		soundHasPlayed = false;
    }
	
	public int GetCostume()
	{
		return costume;
	}

    // Update is called once per frame
    void Update()
    {
        // get ready to talk to the boss
		GameObject boss = GameObject.Find("Boss");
		BossScript bossScript = boss.GetComponent<BossScript>();
		
		// now manage all the various game states
		int gameState = bossScript.GetGameState();
		
		switch (gameState)
		{
			case -1:	// start menu
				spriteRenderer.sprite = spriteArray[costume];
				
				// go up and down in main menu
				if (Input.GetKeyDown("w") && ((costume == 2) || (costume == 3)))
				{
					costume--;
				}
				if (Input.GetKeyDown("s") && ((costume == 1) || (costume == 2)))
				{
					costume++;
				}
				
				// start the game
				if (Input.GetKeyDown("up") && (costume == 1))
				{
					bossScript.SetGameStateToOne();
				}
				
				// navigate to and from tutorial
				if (Input.GetKeyDown("up") && (costume == 2))
				{
					costume = 4;
				}
				if (Input.GetKeyDown("down") && (costume == 4))
				{
					costume = 2;
				}
				
				// navigate to and from credits
				if (Input.GetKeyDown("up") && (costume == 3))
				{
					costume = 5;
				}
				if (Input.GetKeyDown("down") && (costume == 5))
				{
					costume = 3;
				}
				break;
			case 4:		// white won
				spriteRenderer.sprite = spriteArray[6];
				break;
			case 5:		// black won
				spriteRenderer.sprite = spriteArray[7];
				break;
			case 6:		// stalemate
				spriteRenderer.sprite = spriteArray[8];
				break;
			default:	// menu not needed
				spriteRenderer.sprite = spriteArray[0];
				break;
		}
		
		if ((gameState == 6) && (!soundHasPlayed))
		{
			source.Play();
			soundHasPlayed = true;
		}
		
		if (gameState != 6)
		{
			soundHasPlayed = false;
			source.Stop();
		}
    }
}