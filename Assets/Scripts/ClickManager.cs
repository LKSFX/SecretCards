using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {

    public GameObject deckTable;
    bool isClickLocked = true; // ao iniciar o level, virar a carta é proibido
    int isDeckPeekActive = -1; // evita que a função de teste 'show' seja chamada duas vezes seguidas

	// Use this for initialization
	void Start () {
		
	}
	
    public void setClickLock(bool stat) {
        isClickLocked = stat;
    }

	// Update is called once per frame
	void Update () {
        // checa por clique em carta, se permitido
        if (!isClickLocked && Input.GetMouseButtonDown(0)) {
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
        // espiar cartas, útil em testes 
        if (Input.GetKeyDown(KeyCode.Space)) {

            if (Input.GetKey(KeyCode.LeftControl)) {// esconde todas as cartas
                if (isDeckPeekActive > 0) {
                    deckTable.GetComponent<DeckBaseScript>().hideCards(false);
                    isDeckPeekActive = 0;
                }
            }
            else {// mostra todas as cartas
                if (isDeckPeekActive < 1) {
                    deckTable.GetComponent<DeckBaseScript>().showCards(true);
                    isDeckPeekActive = 1;
                }
            }
        }
	}
}
