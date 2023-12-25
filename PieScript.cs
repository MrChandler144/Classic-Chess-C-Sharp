using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is the actual pixel art piece display

public class PieScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
	public Sprite[] spriteArray;
    
	int place;								// which array spot it correlates to
	int arrayValue;							// the number you're meant to be, pulled from Boss
	
	int GetCostumeNumber(int arrayValue)
	{
		switch (arrayValue)
		{
			case 0:			// empty
				return 0;
			case 1:			// white pawn
				return 1;
			case 2:			// white rook
				return 2;
			case 3:			// white knight
				return 3;
			case 4:			// white bishop
				return 4;
			case 5:			// white queen
				return 5;
			case 6:			// white king
				return 6;
			case -1:		// black pawn
				return 7;
			case -2:		// black rook
				return 8;
			case -3:		// black knight
				return 9;
			case -4:		// black bishop
				return 10;
			case -5:		// black queen
				return 11;
			case -6:		// black king
				return 12;
			default:
				// shouldn't hit this
				return 0;
		}
	}

    // Update is called once per frame
    void Update()
    {
		// call the boss
		GameObject boss = GameObject.Find("Boss");
		BossScript bossScript = boss.GetComponent<BossScript>();
		
		// figure out where you are
        place = (int)((transform.position.x) + 27 + 8*((transform.position.y)));
		
		// change your costume to the right piece
		int arrayValue = bossScript.GetPositionArray(place);
		int costumeNumber = GetCostumeNumber(arrayValue);
		spriteRenderer.sprite = spriteArray[costumeNumber];
    }
}
