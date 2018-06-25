using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckMotionScript : MonoBehaviour {

    DeckBaseScript deck;
    public float yy;
    public float yl = 0.07f;
    public float speed = 0.1f;
    int direction = 1;

	// Use this for initialization
	void Start () {
        deck = GetComponent<DeckBaseScript>();
    }
	
	// Update is called once per frame
	void Update () {
        if (deck.isClickLocked) return; // só há movimento quando as cartas estiverem liberadas
        if (yy > yl || yy < -yl) {
            yy = yy > yl ? yl : -yl; // evita o enrosco da variável acima ou abaixo do limite estabelecido
            direction = -direction; // inverte direção do movimento
        }
        yy += Time.deltaTime * direction * speed;
        CardScript card;
        for (int i = 0; i < transform.childCount; i++) {
            card = transform.GetChild(i).GetChild(0).GetComponent<CardScript>();
            if (card.canMove) // carda só se move quando liberada para isso
                card.transform.localPosition = new Vector3(0, yy * (i % 2 == 0 ? 1 : -1), 0);
        }
	}
}
