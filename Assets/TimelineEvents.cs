using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineEvents : MonoBehaviour {

    public DeckBaseScript deck;

    private void OnEnable() {
        Debug.Log(name);
        if (name == "GameBegin") {
            deck.gameStart();
        }
    }

}
