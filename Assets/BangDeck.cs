using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BangDeck : MonoBehaviour
{
    public GameObject cardPrefab;

    // Start is called before the first frame update
    void Start()
    {
        CardLoader.loadCards();

        List<Card> shuffledCards = new List<Card>(CardLoader.deckBang);
        DeckShuffler.Shuffle(shuffledCards);

        float z = 0.0f;
        foreach (Card card in shuffledCards) {
            GameObject obj = Instantiate(cardPrefab);
            obj.transform.parent = transform;
            obj.transform.position = new Vector3(0.0f, 0.0f, z);
            CardSprite cardSprite = obj.GetComponent<CardSprite>();
            cardSprite.card = card;
            z += 0.015f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
