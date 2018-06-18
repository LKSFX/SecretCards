using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeckBaseScript : MonoBehaviour {

    public ClickManager clickManager;
    public bool isDebug = false;
    CardScript[] deck;
    List<CardScript> unrevealedCards;
    private CardScript lastCardChoosed;
    private bool isOnInterval = true; // indica 'true' enquanto as cartas estiverem em modo de amostra
    private int wildcardId = -1;
    private int score = 3; // o jogo começa com três vidas

	// Use this for initialization
	void Start () {
        // todas as cartas ficam neste array
        deck = GetComponentsInChildren<CardScript>();
        foreach (CardScript card in deck)
            card.setDeck(this); // Para futuras chamadas, associa este deck à todas as cartas
        sort();
	}

    public void checkForPoint(CardScript card) {
        Debug.Log("Checking table for a point.");
        if (card.numberId == wildcardId) {
            // caso o curinga seja selecionado, perde-se ponto
            Debug.Log("You selected a forbidden card! Lost one point!");
            score -= 1;
            updateScore();
            // carta é ocultada novamente após intervalo
            lockClick();
            StartCoroutine(FlipCardsAndUnlockDelay(2f, card));
            return;
        }
        if (lastCardChoosed == null) {
            // nenhuma carta selecionada anteriormente
            // salva a carta para futura checagem
            lastCardChoosed = card;
        }
        else {
            // já há uma carta anteriomente selecionada
            // checa a validade da escolha
            if (lastCardChoosed.numberId == card.numberId) {
                Debug.Log("Correct pair. Point!");
                score += 1; // um ponto ganho pelo acerto
            } else {
                // cartas não compoem o par
                Debug.Log("Incorrect pair. Lost one point!");
                score -= 1; // perde-se um ponto
                // cartas são ocultadas novamente após intervalo
                lockClick();
                StartCoroutine(FlipCardsAndUnlockDelay(2f, card, lastCardChoosed));
            }
            lastCardChoosed = null; // não é mais necessária essa referência
            updateScore();
        }
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void resetDeck() {
        // chamar esse método ao fim da rodada
        lockClick();
    }

    public void lockClick() {
        // não permite que as cartas sejam clicadas
        isOnInterval = true;
        clickManager.setClickLock(isOnInterval);
    }

    public void unlockClick() {
        // se ainda não, libera cartas para serem clicadas
        if (isOnInterval) {
            isOnInterval = false;
            clickManager.setClickLock(isOnInterval);
        }
    }

    IEnumerator FlipCardsAndUnlockDelay(float delay, params CardScript[] cards) {
        yield return new WaitForSeconds(delay);
        foreach (CardScript c in cards)
            c.flipCard(false);
        Invoke("unlockClick", 0.5f);
    }

    private void updateScore() {
        // atualiza score no canvas
        // chamar essa função sempre que haja mudança na pontuação
    }

    // define e embaralha as cartas
    public void sort() {
        // as cartas são definidas aleatóriamente aqui
        int[] Nipes = obtainNipes(4, 0, 10);
        int[] NipesDuplicated = obtainDuplicatedExceptOne(Nipes);
        List<int> cardList = new List<int>(NipesDuplicated);
        foreach (CardScript card in deck) {
            // randomiza posição das cartas pela última vez
            int randomVal = UnityEngine.Random.Range(0, cardList.Count - 1);
            int chooseCard = cardList[randomVal];
            card.setCard(chooseCard);
            cardList.RemoveAt(randomVal);
            // ---------
            //Debug.Log("choose is " + ": " + chooseCard);
        }
        // após definidas, as cartas são mostradas em um dos 6 modos
        StartCoroutine(ShowDeckMode(0, 2));
    }
    
    IEnumerator ShowDeckMode(int mode, float after) {
        yield return new WaitForSeconds(after);
        StartCoroutine(ShowFullHouse(3));
        Debug.Log("Show Deck Start");
    }

    IEnumerator ShowFullHouse(float duration) {
        showCards();
        yield return new WaitForSeconds(duration);
        hideCards();
        Debug.Log("Show Deck End");
    }

    public void showCards() {
        showCards(false); // mostra todas as cartas e não salva estado delas
    }

    /// <summary>
    /// Revela todas as cartas simultaneamente 
    /// </summary>
    /// <param name="saveDelta">se verdadeiro, salva as cartas que foram mostradas para virá-las posteriormente.</param>
    public void showCards(bool saveDelta) {
        if (saveDelta) unrevealedCards = new List<CardScript>();
        foreach (CardScript card in deck) {
            if (saveDelta && !card.isFliped) {
                unrevealedCards.Add(card);
            }
            card.flipCard(true);
        }
    }

    public void hideCards() {
        hideCards(true); // esconde obrigatoriamente todas as cartas.
    }

    /// <summary>
    /// Esconde todas as cartas ao mesmo tempo
    /// </summary>
    /// <param name="forceAll">Se ativado, todas as cartas serão escondidas.</param>
    public void hideCards(bool forceAll) {
        // esta função pode ser reduzida, e muito!
        if (!forceAll && (unrevealedCards != null && unrevealedCards.Count > 0)) {
            // se a carta já foi revelada pelo clique válido do mouse, não será escondida novamente.
            // isto ocorre apenas se 'forceAll' for falso
            foreach (CardScript card in unrevealedCards) {
                card.flipCard(false);
            }
        }
        else { 
            // isto ocorre quando nenhuma carta foi revelada ainda ou 'forceAll' for verdadeiro.
            foreach (CardScript card in deck) {
                card.flipCard(false);
            }
        }
        Invoke("unlockClick", 0.2f);
    }

    // retorna 3 inteiros não repetidos entre determinados valores
    int[] obtainNipes(int size, int min, int max) {
        if (min > max) return new int[0]; // mínimo não pode ser maior que o máximo
        max = Mathf.Max(size, max); // tamanho máximo não pode ser menor que o mínimo de valores retornados
        int[] values = new int[size];
        // evita que o array inicie com valores zero, o que inviabilizaria a seleção desse valor
        for (int i = 0; i < values.Length; i++) { values[i] = -1; }
        // se o tamanho for igual ao número máximo, retorna sequencial
        if (size == max) {
            for (int i = 0; i < values.Length; i++) {
                values[i] = i;
            }
        }
        // se o tamanho for menor que o número máximo, sorteia aleatoriamente
        else {
            int currentVal;
            List<int> valuesList = new List<int>(values);
            for (int i = 0; i < valuesList.Count; i++) {
                currentVal = UnityEngine.Random.Range(min, max);
                // Este ciclo ocorre enquanto o valor aleatório já estiver na lista, visto que só pode ocorrer um de cada. 
                while (valuesList.Contains(currentVal)) {
                    currentVal++; // se o valor escolhido aleatoriamente já estiver contido na lista, passa para o próximo
                    if (currentVal > max) currentVal = min;
                }
                valuesList[i] = currentVal;
                if (isDebug) Debug.Log("Nipe " + i + ": " + currentVal);
            }
            values = valuesList.ToArray();
        }
        return shuffle(values); // retornar valores embaralhados
    }

    int[] obtainDuplicatedExceptOne(int[] entry) {
        int[] values = new int[(entry.Length*2)-1];
        int wildcard = entry[UnityEngine.Random.Range(0, entry.Length-1)]; //escolhe um dos números da lista para ser o curinga
        for (int i = 0, z = 0; i < entry.Length; i++) { // duplica os valores que não forem o curinga
            values[z++] = entry[i];
            if (entry[i] == wildcard) continue;
            values[z++] = entry[i];
        }
        if (isDebug) Debug.Log("NipeDuplicated " + String.Join("", new List<int>(values).ConvertAll(i => i.ToString()).ToArray()));
        // salva o valor do curinga na variável 'wildcardId' para checagem durante o processo de checagem de escolhas
        wildcardId = wildcard;
        return values;
    }

    int[] shuffle(int[] entry) {
        List<int> list = new List<int>(entry);
        int[] values = new int[entry.Length];
        int choose;
        for (int i = 0; i < entry.Length; i++) {
            choose = UnityEngine.Random.Range(0, list.Count);
            values[i] = list[choose];
            list.RemoveAt(choose);
        }
        return values;
    }
}
