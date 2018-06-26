﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    protected GameManager() { }

    public bool IsMenuPresent = true;
    public MenuControl MainMenu { get; private set; }

	// Use this for initialization
	void Awake () {
		
	}

    public void setMenuControl(MenuControl controller) {
        MainMenu = controller;
    }
}
