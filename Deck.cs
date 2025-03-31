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
        if (faces == null || dealer == null || player == null || hitButton == null || stickButton == null || playAgainButton == null || finalMessage == null || probMessage == null)
        {
            Debug.LogError("Uno o más componentes no están asignados en el inspector.");
            return;
        }
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
        int remainingCards = 52 - cardIndex;
        int dealerPoints = dealer.GetComponent<CardHand>().points;
        int playerPoints = player.GetComponent<CardHand>().points;

        int countHigherThanPlayer = 0;
        int countBetween17And21 = 0;
        int countAbove21 = 0;

        for (int i = cardIndex; i < 52; i++)
        {
            int newCardValue = values[i];
            int newPlayerPoints = playerPoints + newCardValue;

            if (dealerPoints > playerPoints)
            {
                countHigherThanPlayer++;
            }
            if (newPlayerPoints >= 17 && newPlayerPoints <= 21)
            {
                countBetween17And21++;
            }
            if (newPlayerPoints > 21)
            {
                countAbove21++;
            }
        }

        float probHigherThanPlayer = (float)countHigherThanPlayer / remainingCards * 100;
        float probBetween17And21 = (float)countBetween17And21 / remainingCards * 100;
        float probAbove21 = (float)countAbove21 / remainingCards * 100;

        probMessage.text = $"Prob. dealer > jugador: {probHigherThanPlayer:F2}%\n" +
                           $"Prob. 17-21: {probBetween17And21:F2}%\n" +
                           $"Prob. >21: {probAbove21:F2}%";
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
        probMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
}
