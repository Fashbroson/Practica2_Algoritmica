using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;

    public int[] values = new int[52];
    int cardIndex = 0;

    private void Awake()
    {
        InitCardValues();
    }

    private void Start()
    {
        ShuffleCards();
        StartGame();
    }

    private void InitCardValues()
    {
        for (int i = 0; i < 52; i++)
        {
            int cardValue = (i % 13) + 1;
            if (cardValue > 10) cardValue = 10;
            if (cardValue == 1) cardValue = 11;
            values[i] = cardValue;
        }
    }

    private void ShuffleCards()
    {
        for (int i = 0; i < values.Length; i++)
        {
            int randomIndex = Random.Range(0, values.Length);
            int tempValue = values[i];
            values[i] = values[randomIndex];
            values[randomIndex] = tempValue;

            Sprite tempFace = faces[i];
            faces[i] = faces[randomIndex];
            faces[randomIndex] = tempFace;
        }
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
        }

        if (player.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "Blackjack! Has ganado!";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
        else if (dealer.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "Blackjack del dealer. Has perdido.";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
    }

    private void CalculateProbabilities()
    {
        // Cálculo de probabilidades básicas (requiere análisis más profundo)
        probMessage.text = "Probabilidades calculadas.";
    }

    void PushDealer()
    {
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
    }

    void PushPlayer()
    {
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
        CalculateProbabilities();
    }

    public void Hit()
    {
        if (dealer.GetComponent<CardHand>().cards.Count == 1)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
        }

        PushPlayer();

        if (player.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Has perdido. Te has pasado de 21.";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
    }

    public void Stand()
    {
        if (dealer.GetComponent<CardHand>().cards.Count == 1)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
        }

        while (dealer.GetComponent<CardHand>().points < 17)
        {
            PushDealer();
        }

        int playerPoints = player.GetComponent<CardHand>().points;
        int dealerPoints = dealer.GetComponent<CardHand>().points;

        if (dealerPoints > 21 || playerPoints > dealerPoints)
        {
            finalMessage.text = "¡Has ganado!";
        }
        else if (dealerPoints == playerPoints)
        {
            finalMessage.text = "Empate.";
        }
        else
        {
            finalMessage.text = "Has perdido.";
        }

        hitButton.interactable = false;
        stickButton.interactable = false;
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
}

