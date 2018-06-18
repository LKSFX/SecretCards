using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class CardScript : MonoBehaviour {

    Animator animator;
    DeckBaseScript deck; // referência ao deck
    public bool isFliped { get { return animator.GetBool("Flipped"); } } // descobre se esta carta está virada
    static string[] suitList = new string[] { "Sun", "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn",
        "Uranus", "Neptune", "Pluto", "Moon" };
    public int numberId { get; private set; }

    // Isto ocorre antes do Start
    void Awake () {
        animator = GetComponent<Animator>();
    }

    // Isto ocorre após o Awake
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void flipCard() {
        // inverte posição da carta
        flipCard(!animator.GetBool("Flipped")); 
    }

    // Vira a carta para cima quando true, ou vice-versa.
    public void flipCard(bool stat) {
        flipCard(stat, false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="valid">Verdadeiro se essa virada converte ponto.</param>
    public void flipCard(bool stat, bool valid) {
        // evita chamada desnecessária quando a carta já se encontra no estado requerido
        if (animator.GetBool("Flipped") == stat)
            return; 
        animator.SetBool("Flipped", stat);
        StartCoroutine(ShowInfoWithDelay(stat, valid, stat ? 0.45f : 0.1f));
    }

    IEnumerator ShowInfoWithDelay(bool show, bool valid, float delay) {
        yield return new WaitForSeconds(delay);
        transform.Find("Info").gameObject.SetActive(show);
        if (show && valid) deck.checkForPoint(this); // verifica se um ponto foi convertido
    }

    // definir a carta pelo nome
    public void setCard(string name) { }

    // Associa deck para futura chamadas 
    public void setDeck(DeckBaseScript mainDeck) {
        deck = mainDeck;
    }

    // definir a carta pelo número
    public void setCard(int value) {
        if (value >= suitList.Length || value < 0) return; // evita valor inexistente
        //if (animator == null) animator = GetComponent<Animator>();
        string thisCardName = suitList[value];
        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("Animator/" + thisCardName);
        animator.runtimeAnimatorController = controller;
        Text numberLabel = transform.Find("Info/Number").GetComponent<Text>();
        Text nameLabel = transform.Find("Info/Name").GetComponent<Text>();
        numberLabel.text = value.ToString();
        nameLabel.text = thisCardName;
        numberId = value; // para futuras checagens
    }
}
