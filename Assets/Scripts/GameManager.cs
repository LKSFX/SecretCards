using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    protected GameManager() { }

    private bool _isMusicOn;
    private bool _isSoundOn;
    private readonly string cfgPath = "Canvas/Window/WindowDynamic";
    public bool IsMenuPresent { get { return MainMenu != null; } }
    public readonly string HighscoreIndex = "PlayerHighscore0";
    public MenuControl MainMenu { get; private set; }

    private float escTime;
    private float escWait = .5f;
    private int exitAsks;

    public SoundManager Sound { get; private set; }

	// Use this for initialization
	void Awake () {
        Sound = gameObject.AddComponent<SoundManager>();
        transform.position = Vector3.up * 10000;
	}

    void Update() {
        // QUIT: verifica clique sobre botão return 
        escTime += Time.deltaTime;
        if (exitAsks > 0 && escTime > escWait)
            exitAsks = 0; // tempo transpassado, zera pedido
        if (Input.GetKeyDown(KeyCode.Escape)) {
            exitAsks++;
            if (exitAsks > 1 && escTime < escWait) {
                exitAsks = 0; // DOIS cliques sobre ESCAPE/RETURN, jogo fecha
                Application.Quit();
                Debug.Log("application quitting");
            }
            escTime = 0;
        }
    }

    public void setMenuControl(MenuControl controller) {
        MainMenu = controller;
    }

    public bool IsMusicOn {
        get { return _isMusicOn; }
        set {
            _isMusicOn = value;
            if (IsMenuPresent) {
                UnityEngine.UI.Toggle toggle = MainMenu.getComponentInConfigWindow<UnityEngine.UI.Toggle>
                    (cfgPath + "/MusicToggle/" + (value ? "ToggleOn" : "ToggleOff"));
                if (!toggle.isOn) { // certifica que o menu mostre corretamente o seletor
                    toggle.isOn = true;
                }
            }
        }
    }
    public bool IsSoundOn { get { return _isSoundOn; }
        set {
            if (IsMenuPresent) {
                _isSoundOn = value;
                UnityEngine.UI.Toggle toggle = MainMenu.getComponentInConfigWindow<UnityEngine.UI.Toggle>
                    (cfgPath + "/SoundToggle/" + (value ? "ToggleOn" : "ToggleOff"));
                if (!toggle.isOn) { // certifica que o menu mostre corretamente o seletor
                    Debug.Log("toggle check");
                    toggle.isOn = true;
                }
            }
        }
    }

}
