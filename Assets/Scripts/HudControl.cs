using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // define a porção da barra de vida a ser mostrada
    public void updateLives(int lives) {
        Transform bar = transform.Find("StatsNonStatic/Bar");
        Animator animator;
        int totalGauges = bar.childCount;
        for (int i = 0; i < totalGauges; i++) {
            animator = bar.Find("Life" + i).GetComponent<Animator>();
            animator.SetBool("Active", i < lives);
        }
    }

    // define o SCORE a ser mostrado na barra
    public void updateScore(int score) {
        transform.Find("StatsStatic/Bar/ScoreLabel/Text").GetComponent<Text>().text = score.ToString();
    }
}
