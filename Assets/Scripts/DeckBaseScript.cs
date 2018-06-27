using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckBaseScript : MonoBehaviour {

    public ClickManager clickManager;
    public HudControl hud;
    public GameObject testCanvas;
    public GameObject sortingAlert;
    public bool isDebug = false;
    public bool isTestCanvasOn = false;
    public bool isClickLocked { get { return clickManager.isClickLocked; } set { } }
    CardScript[] deck;
    List<CardScript> unrevealedCards;
    private CardScript lastCardChoosed;
    private bool isOnInterval = true; // indica 'true' enquanto as cartas estiverem em modo de amostra
    private bool isFirstTurn = true; // verdadeiro se for o primeiro turno jogado
    private int wildcardId = -1;
    private int presentationMode = 0; // modo de exibição das cartas vigente
    private int pairs = 0; // registro de pares formados
    private int lives = 3; // o jogo começa com três vidas
    private int score = 0;
    private int isDeckPeekActive = -1; // evita que a função de teste 'show' seja chamada duas vezes seguidas

    // Use this for initialization
    void Start () {
        // todas as cartas ficam neste array
        deck = GetComponentsInChildren<CardScript>();
        foreach (CardScript card in deck)
            card.setDeck(this); // Para futuras chamadas, associa este deck à todas as cartas
        // Inicia o jogo imediatamente se o menu não estiver aberto
        if (GameManager.Instance.IsMenuPresent) {
            gameBegin();
        } else { // manda referência para o main menu controlar este deck
            GameManager.Instance.MainMenu.setDeck(this);
            GameObject.Find("Background").SetActive(false); // desativa o background presente no MENU
        }
	}

    public void gameBegin() {
        CardScript.resetValidFlippedCardCount(); // Zera lista de cartas viradas
        pairs = 0; // registro de pares é zerado
        updateScore(); // reinicia placares
        sort();
        // após definidas, as cartas são mostradas em um dos 6 modos
        int showMode = presentationMode-1; // quando o dropdown menu estiver na opção ZERO estaremos na -1
        showMode = showMode == -1 ? obtainPresentationId() : showMode; // então o jogo se comportará normalmente
        // para testes o showMode pode não respeitar a lógica
        StartCoroutine(ShowDeckMode(showMode, 2));
        isFirstTurn = false; // fim do primeiro TURNO
    }

    /// <summary>
    /// Usado para testes. Esta função já chama gameBegin 
    /// </summary>
    public void gameReset() {
        // quando o botão reset é clicado 
        lives = 3;
        score = 0; // reinicia contagem da pontuação
        isFirstTurn = true; // reseta TURNO
        lastCardChoosed = null;
        wildcardId = -1;
        isDeckPeekActive = -1;
        updateAlertTestCanvas(true, "Restarting...");
        resetDeck();
    }

    IEnumerator GameEnd() {
        // vidas terminaram
        lockClick();
        updateScore();
        updateAlertTestCanvas(true, "Game Over");
        yield return new WaitForSeconds(1f);
        hideCards(true, false);
        yield return new WaitForSeconds(2f);
        foreach (CardScript card in deck)
            card.setDestroy();
        yield return new WaitForSeconds(3f);
        // reinicia cartas
        foreach (CardScript card in deck)
            card.setResetCard();
        // reinicia variáveis e inicia jogo
        gameReset();
    }

    // obtém apresentação baseando-se no número de vidas
    int obtainPresentationId() {
        if (isFirstTurn || lives == 1) return 0; // apresentação flush com UMA vida ou primeiro turno jogado
        // possibilidades a partir de DUAS vidas
        List<int> possibilities = new List<int>();
        possibilities.Add(0);
        if (lives > 1) possibilities.Add(1);
        if (lives > 2) {
            // TRÊS vidas > 1, 2, 3
            possibilities.Remove(0);
            possibilities.Add(2);
            possibilities.Add(3);
        }
        if (lives > 3) {
            // QUATRO vidas > 2, 3
            possibilities.Remove(1);
        }
        if (lives > 4) {
            // CINCO vidas > 3, 4
            possibilities.Remove(2);
            possibilities.Add(4);
        }
        if (lives > 5) {
            // SEIS vidas > 4, 5
            possibilities.Remove(3);
            possibilities.Add(5);
        }
        return possibilities[UnityEngine.Random.Range(0, possibilities.Count)];
    }

    public void checkForPoint(CardScript card) {
        Debug.Log("Checking table for a point.");
        if (card.numberId == wildcardId) {
            // caso o curinga seja selecionado em teste, recebe-se aviso
            Debug.Log("You selected a forbidden card! Will lose point!");
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
                CardScript.resetValidFlippedCardCount(); // ZERA lista de cartas viradas válidas
                // gera efeito de match em ambas as cartas assinalando PAR completo
                lastCardChoosed.setMatch();
                card.setMatch();
                lastCardChoosed = null; // desassocia última escolha
            } else {
                // cartas não formam o par
                Debug.Log("Incorrect pair. Lost one point!");
                lives -= 1; // perde-se um ponto
                if (hud != null && hud.isActiveAndEnabled) {
                    // se o HUD estiver presente
                    hud.updateLives(lives);
                }
                if (lives < 1) {
                    // GAME OVER 
                    // jogador ficou sem vida
                    StartCoroutine("GameEnd");
                    return;
                }
                // cartas são ocultadas novamente após intervalo
                lockClick();
                StartCoroutine(HideCardsAndUnlockDelay(2f, card, lastCardChoosed));
            }
            updateScore();
        }
    }

    public bool isACardWaitingToBeMatched() {
        return lastCardChoosed != null;
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
        Invoke("gameBegin", 1);
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
            // VENCEU
            // aqui todos os pares foram formados
            // jogador vence e ganha uma vida, uma nova partida deverá iniciar
            score += lives; // vidas remanescentes tornam-se pontos
            lives += 1; // ganha um ponto ganho pela rodada
            updateAlertTestCanvas(true, "You Win");
            showCards(); // revela a carta curinga 
            lockClick();
            Invoke("resetDeck", 2);
            testCanvas.transform.Find("Scores/Points").GetComponent<Text>().text = score.ToString();
        }
        if (testCanvas != null && testCanvas.activeInHierarchy) { // atualiza VIDAS no canvas de teste
            testCanvas.transform.Find("Scores/Lives").GetComponent<Text>().text = lives.ToString();
        }
        if (hud != null && hud.isActiveAndEnabled) {
            // se o HUD estiver presente
            hud.updateLives(lives);
            hud.updateScore(score);
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
            if (c != null) c.flipCard(false, true);
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
            case 3: // Show Random 4
                StartCoroutine(ShowRandonFour(.8f));
                break;
            case 4: // Show Joker
                StartCoroutine(ShowJoker(1.5f));
                break;
            case 5: // Show Bad Game
                StartCoroutine(ShowBadGame());
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
        foreach (CardScript card in parSelection) {
            card.flipCard(false); // esconde cartas
        }
        yield return new WaitForSeconds(.4f);
        //desbloqueia tela
        unlockClick();
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
            yield return new WaitForSeconds(.6f);
        }
        Invoke("unlockClick", 0.2f); // desbloqueia tela para cliques
        updateAlertTestCanvas(false, "");
    }

    IEnumerator ShowJoker(float duration) {
        // mostra apenas o curinga
        updateAlertTestCanvas(true, "Joker");
        foreach (CardScript card in deck) {
            if (card.numberId == wildcardId) {
                card.flipCard(true);
                yield return new WaitForSeconds(duration + .4f);
                card.flipCard(false);
                yield return new WaitForSeconds(.4f);
                break;
            }
        }
        Invoke("unlockClick", 0.2f); // desbloqueia tela para cliques
        updateAlertTestCanvas(false, "");
    }

    IEnumerator ShowBadGame() {
        updateAlertTestCanvas(true, "Bad Game");
        yield return new WaitForSeconds(2f);
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
        // =============
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
        if (isDebug) Debug.Log("NipeDuplicated " + values.ToString());
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
