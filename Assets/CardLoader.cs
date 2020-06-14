using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class CardItem {
    public string name;
    public string image;
    public string expansion;
}

[Serializable]
class BangCardItem : CardItem {
    public string[] signs;
    public string color;
    public string type;
    public int cube_cost;
}

[Serializable]
class CharacterCardItem : CardItem {
    public int hp;
}

[Serializable]
class ExpansionCardItem : CardItem {
    public bool last_card;
}

[Serializable]
class GoldrushCardItem : CardItem {
    public string color;
    public string type;
    public int gold_cost;
    public int count;
}

[Serializable]
class RootObject {
    public BangCardItem[] cards;
    public CharacterCardItem[] characters;
    public CardItem[] roles;
    public ExpansionCardItem[] fistfulofcards;
    public ExpansionCardItem[] highnoon;
    public ExpansionCardItem[] wildwestshow;
    public GoldrushCardItem[] goldrush;
}

public class CardLoader
{
    public static List<Card> deckBang = new List<Card>();
    public static List<Card> deckCharacters = new List<Card>();
    public static List<Card> deckRoles = new List<Card>();
    public static List<Card> deckFistfulofcards = new List<Card>();
    public static List<Card> deckHighnoon = new List<Card>();
    public static List<Card> deckWildwestshow = new List<Card>();
    public static List<Card> deckGoldrush = new List<Card>();

    public static void loadCards() {
        RootObject obj = JsonUtility.FromJson<RootObject>(Resources.Load<TextAsset>("bang_cards").ToString());

        foreach (BangCardItem item in obj.cards) {
            foreach (string sign in item.signs) {
                BangCard card = new BangCard();
                card.name = item.name;
                card.expansion = item.expansion;
                card.loadTexture("cards/" + item.image);
                card.sign = sign;
                card.color = item.color;
                card.type = item.type;
                card.cube_cost = item.cube_cost;
                deckBang.Add(card);
            }
        }

        foreach (CharacterCardItem item in obj.characters) {
            CharacterCard card = new CharacterCard();
            card.name = item.name;
            card.expansion = item.expansion;
            card.loadTexture("characters/" + item.image);
            card.hp = item.hp;
            deckCharacters.Add(card);
        }

        foreach (CardItem item in obj.roles) {
            Card card = new Card();
            card.name = item.name;
            card.loadTexture("roles/" + item.image);
            deckRoles.Add(card);
        }

        foreach (ExpansionCardItem item in obj.fistfulofcards) {
            ExpansionCard card = new ExpansionCard();
            card.name = item.name;
            card.expansion = "fistfulofcards";
            card.loadTexture("fistfulofcards/" + item.image);
            card.last_card = item.last_card;
            deckFistfulofcards.Add(card);
        }

        foreach (ExpansionCardItem item in obj.highnoon) {
            ExpansionCard card = new ExpansionCard();
            card.name = item.name;
            card.expansion = "highnoon";
            card.loadTexture("highnoon/" + item.image);
            card.last_card = item.last_card;
            deckHighnoon.Add(card);
        }

        foreach (ExpansionCardItem item in obj.wildwestshow) {
            ExpansionCard card = new ExpansionCard();
            card.name = item.name;
            card.expansion = "wildwestshow";
            card.loadTexture("wildwestshow/" + item.image);
            card.last_card = item.last_card;
            deckWildwestshow.Add(card);
        }

        foreach (GoldrushCardItem item in obj.goldrush) {
            GoldrushCard card = new GoldrushCard();
            card.name = item.name;
            card.expansion = "goldrush";
            card.loadTexture("goldrush/" + item.image);
            card.color = item.color;
            card.type = item.type;
            card.gold_cost = item.gold_cost;
            for (int i=0; i<item.count; ++i) {
                deckGoldrush.Add(card);
            }
        }
    }

}
