using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DeckBaseScript : MonoBehaviour {

    public ClickManager clickManager;
    public GameObject testCanvas;
    public GameObject sortingAlert;
    public bool isDebug = false;
    public bool isTestCanvasOn = false;
    public bool isClickLocked { get { return clickManager.isClickLocked; } set { } }
    CardScript[] deck;
    List<CardScript> unrevealedCards;
    private CardScript lastCardChoosed;
    private bool isOnInterval = true; // indica 'true' enquanto as cartas estiverem em modo de amostra
    private int wildcardId = -1;
    private int presentationMode = 3; // modo de exibição das cartas vigente
    private int pairs = 0; // registro de pares formados
    private int score = 3; // o jogo começa com três vidas
    private int isDeckPeekActive = -1; // evita que a função de teste 'show' seja chamada duas vezes seguidas

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
            StartCoroutine(HideCardsAndUnlockDelay(2f, card, lastCardChoosed));
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
                // cartas compõem o par
                Debug.Log("Correct pair. Point!");
                pairs += 1; // o par é registrado
                lastCardChoosed = null; // desassocia última escolha
            } else {
                // cartas não formam o par
                Debug.Log("Incorrect pair. Lost one point!");
                score -= 1; // perde-se um ponto
                // cartas são ocultadas novamente após intervalo
                lockClick();
                StartCoroutine(HideCardsAndUnlockDelay(2f, card, lastCardChoosed));
            }
            updateScore();
        }
    }

	// Update is called once per frame
	void Update () {
		
	}

    // esta função, que chamará o inicio de uma nova, deve ser invocada no termino da rodada
    public void resetDeck() {
        // chamar esse método ao fim da rodada
        lastCardChoosed = null;
        hideCards(true, false); // esconde todas as cartas
        lockClick();
        Invoke("sort", 1);
    }

    // usado para testes
    public void gameReset() {
        // quando o botão reset é clicado 
        score = 3;
        resetDeck();
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

    private void updateScore() {
        // atualiza score no canvas
        // chamar essa função sempre que haja mudança na pontuação
        if (pairs == 3) {
            // aqui todos os pares foram formados
            // jogador vence e ganha uma vida, uma nova partida deverá iniciar
            score += 1; // ganha um ponto ganho pela rodada
            lockClick();
            Invoke("resetDeck", 2);
        }
        if (testCanvas != null && testCanvas.activeInHierarchy) { // atualiza score no canvas de teste
            testCanvas.transform.Find("Scores/Points").GetComponent<Text>().text = score.ToString();
        }
    }

    public void updateAlertTestCanvas(bool active, string text) {
        if (isTestCanvasOn && sortingAlert != null) {
            // apenas em teste mostra aviso
            sortingAlert.GetComponent<Text>().text = text;
            sortingAlert.gameObject.SetActive(active);
        }
    }

    IEnumerator HideCardsAndUnlockDelay(float delay, params CardScript[] cards) {
        yield return new WaitForSeconds(delay);
        foreach (CardScript c in cards)
            if (c != null) c.flipCard(false);
        Invoke("unlockClick", 0.5f);
        lastCardChoosed = null; // não é mais necessária essa referência
    }

    IEnumerator ShowDeckMode(int mode, float after) {
        yield return new WaitForSeconds(after);
        switch (mode) {
            case 0: // Show Full House
                StartCoroutine(ShowFullHouse(3));
                break;
            case 1: // Show Flush
                StartCoroutine(ShowFlush(.8f));
                break;
            case 2: // Show Three of a Kind
                StartCoroutine(ShowThreeOfAKind(3));
                break;
            case 3:
                StartCoroutine(ShowRandonFour(.8f));
                break;
        }
    }

    IEnumerator ShowFullHouse(float duration) {
        updateAlertTestCanvas(true, "Full House");
        showCards();
        yield return new WaitForSeconds(duration);
        hideCards();
        updateAlertTestCanvas(false, "");
    }

    IEnumerator ShowFlush(float delay) {
        // Mostra todas as cartas, porém uma de cada vez
        updateAlertTestCanvas(true, "Flush");
        foreach (CardScript card in deck) {
            card.flipCard(true);
            yield return new WaitForSeconds(delay + .4f);
            card.flipCard(false);
            yield return new WaitForSeconds(.4f);
        }
        Invoke("unlockClick", 0.2f); // desbloqueia tela para cliques
        updateAlertTestCanvas(false, "");
    }

    IEnumerator ShowThreeOfAKind(float duration) {
        updateAlertTestCanvas(true, "Three of a Kind");
        // seleciona uma carta de cada par
        List<CardScript> parSelection = obtainOneFromAPair();
        // vira cartas obtidas dos pares
        foreach (CardScript card in parSelection) {
            card.flipCard(true);
        }
        yield return new WaitForSeconds(duration + .4f);
        // desvira cartas
        hideCards(false, true); // esconde cartas, desbloqueia tela
        yield return new WaitForSeconds(.4f);
        updateAlertTestCanvas(false, "");
    }

    IEnumerator ShowRandonFour(float delay) {
        // mostra quatro cartas selecionadas aleatoriamente
        updateAlertTestCanvas(true, "Random 4");
        CardScript card;
        for (int i = 0; i < 4; i++) {
            card = deck[UnityEngine.Random.Range(0, deck.Length)];
            card.flipCard(true);
            yield return new WaitForSeconds(delay + .4f);
            card.flipCard(false);
            yield return new WaitForSeconds(.4f);
        }
        Invoke("unlockClick", 0.2f); // desbloqueia tela para cliques
        updateAlertTestCanvas(false, "");
    }

    #region hideAndShow

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
        hideCards(true, true); // esconde obrigatoriamente todas as cartas.
    }

    /// <summary>
    /// Esconde todas as cartas ao mesmo tempo
    /// </summary>
    /// <param name="forceAll">Se 'true', todas as cartas serão escondidas.</param>
    /// <param name="unlockAfter">Se 'true', destrava o touch após virar as cartas.</param>
    public void hideCards(bool forceAll, bool unlockAfter) {
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
        if (unlockAfter)
            Invoke("unlockClick", 0.2f);
    }

    /// <summary>
    /// Permite espiar as cartas, útil para a fase de testes
    /// </summary>
    /// <param name="active">se está ou não ativo o modo peeking</param>
    public void peekCards(bool active) {
        if (active) {
            // espiar cartas
            if (isDeckPeekActive < 1) {
                showCards(true);
                isDeckPeekActive = 1;
            }
        }
        else {
            // termina de espiar cartas
            if (isDeckPeekActive > 0) {
                hideCards(false, true);
                isDeckPeekActive = 0;
            }
        }
    }


    #endregion

    // define e embaralha as cartas, esta função deve ser chamada ao iniciar a rodada.
    public void sort() {
        pairs = 0; // registro de pares é zerado
        updateScore(); // reinicia placar
        updateAlertTestCanvas(true, "Sorting...");
        // as cartas são definidas aleatóriamente aqui
        int[] Nipes = obtainNipes(4, 0, 10);
        int[] NipesDuplicated = obtainDuplicatedExceptOne(Nipes);
        List<int> cardList = new List<int>(NipesDuplicated);
        foreach (CardScript card in deck) {
            // randomiza posição das cartas pela última vez
            int randomVal = UnityEngine.Random.Range(0, cardList.Count);
            int chooseCard = cardList[randomVal];
            card.setCard(chooseCard);
            cardList.RemoveAt(randomVal);
            // ---------
            //Debug.Log("choose is " + ": " + chooseCard);
        }
        // após definidas, as cartas são mostradas em um dos 6 modos
        StartCoroutine(ShowDeckMode(presentationMode, 2));
    }

    #region hud

    public void onDropdownUpdate(Dropdown target) {
        Debug.Log("Dropdown changed: " + target.value);
        presentationMode = target.value;
    }

    #endregion

    #region elemental sorting functions

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

    // obtém uma lista com uma carta de cada par
    List<CardScript> obtainOneFromAPair() {
        List<CardScript> parSelection = new List<CardScript>();
        List<int> ids = new List<int>();
        int currentId;
        foreach (CardScript c in deck) {
            currentId = c.numberId;
            if (currentId != wildcardId && !ids.Contains(currentId)) {
                // quando a carta não for o curinga
                parSelection.Add(c);
                ids.Add(currentId);
            }
        }
        return parSelection;
    }

    int[] obtainDuplicatedExceptOne(int[] entry) {
        int[] values = new int[(entry.Length*2)-1];
        int wildcard = entry[UnityEngine.Random.Range(0, entry.Length)]; //escolhe um dos números da lista para ser o curinga
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
    #endregion
}
