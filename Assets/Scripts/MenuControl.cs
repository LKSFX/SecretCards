using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour {

    public Transform configMenu;
    DeckBaseScript deck;
    Scene sceneGame;
    private float windowHidePos = 1500f;
    private float windowAlphaMeta = .5f;
    private int windowDropSpeed = 2300;

    // Use this for initialization
    void Start() {
        GameManager.Instance.setMenuControl(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
        updateHighscoreIcon();
        StartCoroutine(Intro());
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
        StartCoroutine(OpenConfigMenu());
    }

    public void onCloseConfigMenu() {
        StartCoroutine(CloseConfigMenu());
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
        transform.Find("Buttons/BtContainer/Scores/Counter").GetComponent<Text>().text = "<b>" + PlayerPrefs.GetInt(GameManager.Instance.HighscoreIndex, 0) + "</b>";
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

    IEnumerator Intro() {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        Image title = transform.Find("Static/Title").GetComponent<Image>();
        title.color = new Color(1, 1, 1, 0);
        float yy = -1000;
        RectTransform container = (RectTransform)transform.Find("Buttons/BtContainer");
        container.anchoredPosition = Vector2.up * yy;
        while (container.anchoredPosition.y < 0) {
            container.anchoredPosition = container.anchoredPosition + Vector2.up * Time.deltaTime * windowDropSpeed * .3f;
            title.color = new Color(1, 1, 1, (-yy + container.anchoredPosition.y)/-yy );
            yield return null;
        }
        container.anchoredPosition = Vector2.zero;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    #region ConfigMenu

    IEnumerator OpenConfigMenu() {
        if (configMenu != null) {
            configMenu.gameObject.SetActive(true);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        RectTransform window = (RectTransform)configMenu.Find("Canvas/Window");
        Debug.Log(window.name);
        window.anchoredPosition = Vector2.up * windowHidePos;
        yield return ShowConfigWindow(window);
        yield return null;
    }

    IEnumerator ShowConfigWindow(RectTransform window) {
        Image background = window.parent.Find("Background").GetComponent<Image>();
        background.color = new Color(20, 34, 59, 0);
        float dropTime = windowHidePos / windowDropSpeed;
        float time = 0;
        Vector2 init = window.anchoredPosition;
        while (window.anchoredPosition.y > 0) {
            window.anchoredPosition = Vector2.Lerp(init, Vector2.zero, (time += Time.deltaTime)/dropTime);
            background.color = new Color(20/255f, 34/255f, 59/255f, (windowHidePos - window.anchoredPosition.y)/windowHidePos * windowAlphaMeta);
            yield return null;
        }
        window.anchoredPosition = Vector2.zero;
    }

    IEnumerator CloseConfigMenu() {
        RectTransform window = (RectTransform)configMenu.Find("Canvas/Window");
        Debug.Log(window.name);
        yield return HideConfigWindow(window);
        if (configMenu != null) {
            configMenu.gameObject.SetActive(false);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    IEnumerator HideConfigWindow(RectTransform window) {
        Image background = window.parent.Find("Background").GetComponent<Image>();
        while (window.anchoredPosition.y < windowHidePos) {
            window.anchoredPosition = window.anchoredPosition + (Vector2.up * Time.deltaTime * windowDropSpeed);
            background.color = new Color(20 / 255f, 34 / 255f, 59 / 255f, windowAlphaMeta - ((window.anchoredPosition.y / windowHidePos) * windowAlphaMeta));
            yield return null;
        }
    }

    #endregion
}
