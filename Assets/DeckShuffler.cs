using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckShuffler : MonoBehaviour
{
    private List<Card> shuffledCards;

    private int selectedCard = 0;
    private CardSprite sprite;

    public static void Shuffle<T>(IList<T> list) {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = (int)(Random.value * n);  
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }  
    }

    void Start()
    {
        CardLoader.loadCards();

        shuffledCards = new List<Card>(CardLoader.deckCharacters);
        Shuffle(shuffledCards);

        sprite = gameObject.GetComponent<CardSprite>();
    }

    void OnMouseDown()
    {
        sprite.card = shuffledCards[selectedCard];
        ++selectedCard;
        if (selectedCard == shuffledCards.Count) {
            selectedCard = 0;
        }
    }
}
