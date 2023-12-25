using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script kicks into action when promoting a pawn (specifically when gameState is 2)

// promote to costume	costume	piece id
// blank				0		0
// white queen			1		5
// white knight			2		3
// white rook			3		2
// white bishop			4		4
// black queen			5		-5
// black knight			6		-3
// black rook			7		-2
// black bishop			8		-4

public class PromoterScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
	public Sprite[] spriteArray;
	
	int currentOption;
	
	int GetNumber()
	{
		switch (currentOption)
		{
			case 1:
				return 5;
			case 2:
				return 3;
			case 3:
				return 2;
			case 4:
				return 4;
			case 5:
				return -5;
			case 6:
				return -3;
			case 7:
				return -2;
			case 8:
				return -4;
			case 9:
				return -6;
			default:
				// shouldn't hit this
				return 0;
		}
	}
	
	// Start is called before the first frame update
    void Start()
    {
        currentOption = 0;
    }

    // Update is called once per frame
    void Update()
    {
		// call the boss
		GameObject boss = GameObject.Find("Boss");
		BossScript bossScript = boss.GetComponent<BossScript>();	
		
		// should always have the correct sprite
		spriteRenderer.sprite = spriteArray[currentOption];
		
		if (bossScript.GetGameState() == 2)
		{
			// game state 2 means we are promoting a pawn
			
			// we should be showing something, fix if we're not
			if (currentOption == 0)
			{
				if (bossScript.GetTargetSquare() > 30)
				{
					// a white pawn
					currentOption = 1;
				} else {
					// a black pawn
					currentOption = 5;
				}
			}
			
			// if we select one with the down key then confirm that choice
			if ((Input.GetKeyDown("down")) && (currentOption != 0))
			{
				// get the boss to promote the pawn
				bossScript.PromotePawn(GetNumber());
				// go back to the normal gameState
				bossScript.SetGameStateToOne();
				// hide
				currentOption = 0;
			}
			
			// scroll the options
			if ((Input.GetKeyDown("w")) && (currentOption%4 != 1))
			{
				currentOption--;
			}
			if ((Input.GetKeyDown("s")) && (currentOption%4 != 0))
			{
				currentOption++;
			}
		}
    }
}