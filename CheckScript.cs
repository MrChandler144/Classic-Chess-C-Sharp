using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is the cute crosshairs the king makes when in check

public class CheckScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
	public Sprite[] spriteArray;
    
	int place;
	int isWhiteAttacking;
	int isBlackAttacking;

    // Update is called once per frame
    void Update()
    {
		// call the boss
		GameObject boss = GameObject.Find("Boss");
		BossScript bossScript = boss.GetComponent<BossScript>();
		
        // figure out where you are
		place = (int)((transform.position.x) + 27 + 8*(transform.position.y));
		
		// access the variable stored in Boss
		isWhiteAttacking = bossScript.GetWhiteAttacks(place);
		isBlackAttacking = bossScript.GetBlackAttacks(place);
		
		// set the correct costume
		if ((isWhiteAttacking == 1) && (bossScript.GetPositionArray(place) == -6))
		{
			// the black king is here and he is in check
			spriteRenderer.sprite = spriteArray[1];
			return;
		}
		if ((isBlackAttacking == 1) && (bossScript.GetPositionArray(place) == 6))
		{
			// the white king is here and he is in check
			spriteRenderer.sprite = spriteArray[1];
			return;
		}
		// if we got here there's no king in check here, do nothing
		spriteRenderer.sprite = spriteArray[0];
    }
}
