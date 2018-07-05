﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour {

    DeckBaseScript deck;
    Scene sceneGame;

    // Use this for initialization
    void Start() {
        GameManager.Instance.setMenuControl(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.Instance.IsMenuPresent = true;
        updateHighscoreIcon();
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void onPlayGame() {
        // ao clicar PLAY
        if (sceneGame.name == "GameScene")
            StartCoroutine(GameBegin());
        else {
            SceneManager.LoadScene("Scenes/GameScene", LoadSceneMode.Additive);
        }
        Debug.Log("Clicked on play!");
    }

    void hideMenuAndStartGame() {
        StartCoroutine(FadeOut(2));
    }
    
    public void showMenu() {
        StartCoroutine(FadeIn(1));
    }

    void showMenuAndEndGame() { } // função deverá ser construída

    IEnumerator FadeOut(float fadeDuration) {
        CanvasGroup group = GetComponent<CanvasGroup>();
        float alpha = group.alpha; // opacidade
        float time = 0;
        while (alpha > 0) {
            time += Time.deltaTime;
            alpha = 1 - (1 * (time/fadeDuration));
            group.alpha = alpha;
            yield return null;
        }
        alpha = 0;
    }

    IEnumerator FadeIn(float fadeDuration) {
        CanvasGroup group = GetComponent<CanvasGroup>();
        float alpha = group.alpha; // opacidade
        float time = 0;
        while (alpha < 1) {
            time += Time.deltaTime;
            alpha = (1 * (time / fadeDuration));
            group.alpha = alpha;
            yield return null;
        }
        alpha = 1;
        GetComponent<CanvasGroup>().blocksRaycasts = true; // detecta cliques no menu após FADEIN
    }

    IEnumerator GameBegin() {
        GetComponent<CanvasGroup>().blocksRaycasts = false; // não detectar mais cliques
        yield return new WaitForEndOfFrame();
        deck.gameBegin(); // inicia jogo
        hideMenuAndStartGame();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // quando a cena carregar
        if (scene.name == "GameScene") {
            sceneGame = scene;
            StartCoroutine("GameBegin");
        }
        Debug.Log("GameScene Loaded!");
    }

    public void setDeck(DeckBaseScript deckController) {
        deck = deckController;
    }

    public void updateHighscoreIcon() {
        // precisa ser corretamente implementado
        //TODO
        transform.Find("Buttons/Scores/Counter").GetComponent<Text>().text = "<b>" + PlayerPrefs.GetInt(GameManager.Instance.HighscoreIndex, 0) + "</b>";
    }
}
