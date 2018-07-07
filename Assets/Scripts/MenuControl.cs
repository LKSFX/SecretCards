using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour {

    public Transform configMenu;
    DeckBaseScript deck;
    Scene sceneGame;

    // Use this for initialization
    void Start() {
        GameManager.Instance.setMenuControl(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
        updateHighscoreIcon();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {
            GameManager.Instance.IsMusicOn = !GameManager.Instance.IsMusicOn;
        }
        if (Input.GetKeyDown(KeyCode.J)) {
            GameManager.Instance.IsSoundOn = !GameManager.Instance.IsSoundOn;
        }
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

    public void onOpenConfigMenu() {
        if (configMenu != null) {
            configMenu.gameObject.SetActive(true);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void onCloseConfigMenu() {
        Debug.Log("Config. Window close call!");
        if (configMenu != null) {
            configMenu.gameObject.SetActive(false);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    void hideMenuAndStartGame() {
        StartCoroutine(FadeOut(2));
        // ativa o canvas de teste
        foreach (GameObject go in sceneGame.GetRootGameObjects())
            if (go.name == "TestCanvas1") 
                go.SetActive(true);
    }
    
    public void showMenu() {
        StartCoroutine(FadeIn(1));
        // desativa o canvas de teste
        foreach (GameObject go in sceneGame.GetRootGameObjects())
            if (go.name == "TestCanvas1")
                go.SetActive(false);
    }

    void showMenuAndEndGame() { } // função deverá ser construída

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
            StartCoroutine(GameBegin());
        }
        Debug.Log("GameScene Loaded!");
    }

    public void setDeck(DeckBaseScript deckController) {
        deck = deckController;
    }

    // configurações 

    public void setMusic(bool on) {
        GameManager.Instance.IsMusicOn = on;
    }

    public void setSound(bool on) {
        GameManager.Instance.IsSoundOn = on;
    }

    public void updateHighscoreIcon() {
        // precisa ser corretamente implementado
        //TODO
        transform.Find("Buttons/Scores/Counter").GetComponent<Text>().text = "<b>" + PlayerPrefs.GetInt(GameManager.Instance.HighscoreIndex, 0) + "</b>";
    }

    public T getComponentInConfigWindow<T>(string path) where T : Component {
        return configMenu.Find(path).GetComponent<T>();
    }

    IEnumerator FadeOut(float fadeDuration) {
        CanvasGroup group = GetComponent<CanvasGroup>();
        float alpha = group.alpha; // opacidade
        float time = 0;
        while (alpha > 0) {
            time += Time.deltaTime;
            alpha = 1 - (1 * (time / fadeDuration));
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
}
