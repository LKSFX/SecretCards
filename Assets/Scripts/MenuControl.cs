using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour {

    DeckBaseScript deck;

    // Use this for initialization
    void Start() {
        GameManager.Instance.setMenuControl(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.Instance.IsMenuPresent = false;
        updateHighscoreIcon();
    }

    // Update is called once per frame
    void Update() {

    }

    public void onPlayGame() {
        // ao clicar PLAY
        SceneManager.LoadScene("Scenes/GameScene", LoadSceneMode.Additive);
        Debug.Log("Clicked on play!");
        GetComponent<CanvasGroup>().blocksRaycasts = false; // não detectar mais cliques
    }

    void hideMenuAndStartGame() {
        StartCoroutine(Fade(2));
    }

    void showMenuAndEndGame() { } // função deverá ser construída

    IEnumerator Fade(float fadeDuration) {
        CanvasGroup group = GetComponent<CanvasGroup>();
        float alpha = group.alpha; // opacidade
        float time = 0;
        while (alpha > 0) {
            time += Time.deltaTime;
            alpha = 1 - (1 * (time/fadeDuration));
            group.alpha = alpha;
            yield return null;
        }
        deck.gameBegin(); // inicia jogo
        alpha = 0;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // quando a cena carregar
        if (scene.name == "GameScene") {
            hideMenuAndStartGame();
        }
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
