using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is the square outline that shows the squares you can move to

public class AuthScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
	public Sprite[] spriteArray;
    
	int place;			// which array spot it correlates to
	int arrayValue;		// the number you're meant to be, pulled from Boss

    // Update is called once per frame
    void Update()
    {
        // figure out where you are
		place = (int)((transform.position.x) + 27 + 8*(transform.position.y));
		
		// give BossScript a call
		GameObject boss = GameObject.Find("Boss");
		BossScript bossScript = boss.GetComponent<BossScript>();
		
		// set the right costume
		arrayValue = bossScript.GetValidPosition(place);
		spriteRenderer.sprite = spriteArray[arrayValue];
    }
}
