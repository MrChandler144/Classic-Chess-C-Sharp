using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// these are just small ui elements I added to hopefully make the controls a bit more intuitive
// this is probably the best candidate for future expansion

// costume			here
// blank			0
// w/s & up/dowm	1

public class UiScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
	public Sprite[] spriteArray;
	
	int costume;
	float timer = 0.0f;

	
	// Start is called before the first frame update
    void Start()
    {
		// centre on the camera
		transform.position = new Vector3(0.5f,0.5f,-8f);
        costume=0;
    }

    // Update is called once per frame
    void Update()
    {
        // call the boss
		GameObject boss = GameObject.Find("Boss");
		BossScript bossScript = boss.GetComponent<BossScript>();
		
		// call the startmenu (one word)
		GameObject startmenu = GameObject.Find("Startmenu");
		StartmenuScript startmenuScript = startmenu.GetComponent<StartmenuScript>();
		
		// default is that we don't see it
		costume = 0;
		
		// show the wsud on the start screen
		if ((bossScript.GetGameState() == -1) && (startmenuScript.GetCostume() <= 3))
		{
			// we are on the home page
			// show the w/s & up/down
			costume = 1;
		}
		
		// show the r on the game over screen after a small delay
		if (bossScript.GetGameState() >= 4)
		{
			// game is over, count down to show the R button
			timer += Time.deltaTime;
		} else {
			timer=0;
		}
		
		if (timer > 2.45)
		{
			costume = 2;
		}
		
		// update the costume
		spriteRenderer.sprite = spriteArray[costume];
	}
}