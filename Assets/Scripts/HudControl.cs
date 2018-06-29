using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudControl : MonoBehaviour {

    int localHighscore;
    int deltaHighscore;

	// Use this for initialization
	void Start () {
        // obtém último highscore válido salvo
        deltaHighscore = PlayerPrefs.GetInt(GameManager.Instance.HighscoreIndex, 0);
        updateHighscore(deltaHighscore);
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
        updateHighscore(score);
    }

    public void updateHighscore(int score) {
        if (score <= localHighscore) return; // highscore não superado
        localHighscore = score;
        transform.Find("StatsStatic/Bar/Highscore/Counter").GetComponent<Text>().text = "<b>" + localHighscore + "</b>";
    }

    public void saveHighscore() {
        if (localHighscore > deltaHighscore) {
            PlayerPrefs.SetInt(GameManager.Instance.HighscoreIndex, localHighscore);
            PlayerPrefs.Save();
        }
    }

    private void OnDestroy() {
        // Salva o highscore ao sair ou fechar a cena ou jogo
        saveHighscore();
    }

    private void OnApplicationPause(bool pauseStatus) {
        // Salva o highscore ao sair ou fechar a cena ou jogo
        // OBRIGATÓRIO no ANDROID
        if (pauseStatus) saveHighscore();
    }
}
