using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class CardScript : MonoBehaviour {

    Animator animator;
    static string[] suitList = new string[] { "Sun", "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn",
        "Uranus", "Neptune", "Pluto", "Moon" };

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
        animator.SetBool("Flipped", stat);
        StartCoroutine(ShowInfoWithDelay(stat, stat ? 0.45f : 0.1f));
    }

    IEnumerator ShowInfoWithDelay(bool show, float delay) {
        yield return new WaitForSeconds(delay);
        transform.Find("Info").gameObject.SetActive(show);
    }

    // definir a carta pelo nome
    public void setCard(string name) { }

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
    }
}
