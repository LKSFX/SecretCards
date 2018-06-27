using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour {

    public GameObject deckTable;
    public bool isClickLocked { get; private set; } // ao iniciar o level, virar a carta é proibido
    int fingerId = -1;
    float clickDelta = 0;
    float clickInterval = 0.5f; // tempo mínimo entre cliques

    // Use this for initialization
    void Start () {
        isClickLocked = true;
#if !UNITY_EDITOR
        fingerId = 0;
#endif
    }

    public void setClickLock(bool stat) {
        isClickLocked = stat;
    }

	// Update is called once per frame
	void Update () {
        if (clickDelta > clickInterval) { // DELAY ENTRE CLIQUES
            // checa por clique em carta, se permitido
            if (!isClickLocked && !EventSystem.current.IsPointerOverGameObject(fingerId) && Input.GetMouseButtonDown(0)) {
                //Debug.Log("CLICK DETECTED");
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null) {
                    //Debug.Log(hit.collider.gameObject.name + " was hit!");
                    CardScript card = hit.collider.gameObject.GetComponent<CardScript>();
                    card.flipCard(true, true);
                    if (CardScript.validCardsFlipped > 1) clickDelta = 0;
                }
            }
        }
        clickDelta += Time.deltaTime;
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
