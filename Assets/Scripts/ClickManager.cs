using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour {

    public GameObject deckTable;
    public bool isClickLocked { get; private set; } // ao iniciar o level, virar a carta é proibido

	// Use this for initialization
	void Start () {
        isClickLocked = true;
	}
	
    public void setClickLock(bool stat) {
        isClickLocked = stat;
    }

	// Update is called once per frame
	void Update () {
        // checa por clique em carta, se permitido
        if (!isClickLocked && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0)) {
            //Debug.Log("CLICK DETECTED");
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null) {
                //Debug.Log(hit.collider.gameObject.name + " was hit!");
                CardScript card = hit.collider.gameObject.GetComponent<CardScript>();
                card.flipCard(true, true);
            }
        }
        // atalhos para espiar cartas, útil em testes 
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (Input.GetKey(KeyCode.LeftControl)) {// esconde todas as cartas
                deckTable.GetComponent<DeckBaseScript>().peekCards(false);
            }
            else {// mostra todas as cartas
                deckTable.GetComponent<DeckBaseScript>().peekCards(true);
            }
        }
	}

    
}
