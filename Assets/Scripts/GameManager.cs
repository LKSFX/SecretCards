using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    protected GameManager() { }

    private bool _isMusicOn;
    private bool _isSoundOn;
    private readonly string cfgPath = "Canvas/WindowDynamic";
    public bool IsMenuPresent { get { return MainMenu != null; } }
    public readonly string HighscoreIndex = "PlayerHighscore0";
    public MenuControl MainMenu { get; private set; }

	// Use this for initialization
	void Awake () {
		
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
