using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void updateLives(int lives) {
        Transform bar = transform.Find("StatsNonStatic/Bar");
        Animator animator;
        int totalGauges = bar.childCount;
        for (int i = 0; i < totalGauges; i++) {
            animator = bar.Find("Life" + i).GetComponent<Animator>();
            animator.SetBool("Active", i < lives);
        }
    }
}
