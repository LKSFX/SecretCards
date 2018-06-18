using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PeekButton : MonoBehaviour {

    public GameObject deckTable;
    bool isPressed;
    Button bt;
    DeckBaseScript deck;

	// Use this for initialization
	void Start () {
        bt = GetComponent<Button>();
        bt.onClick.AddListener(clicked);
        bt.interactable = false;
        deck = deckTable.GetComponent<DeckBaseScript>();

    }

    void Update() {
        bt.interactable = !deck.isClickLocked;
    }

    void clicked() {
        isPressed = !isPressed; // inverte estado
        transform.Find("Text1").gameObject.SetActive(isPressed);
        transform.Find("Text2").gameObject.SetActive(!isPressed);
        deck.peekCards(isPressed);
    } 
}
