using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class CardItem {
    public string name;
    public string image;
    public string expansion;

    public bool disabled;
}

[Serializable]
class BangCardItem : CardItem {
    public string[] signs;
    public string color;
    public string type;
    public string target;
    public string equip_target;
    public int distance;
    public string effect;
    public int cube_cost;
    public string cube_effect;
    public string cube_target;
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

    public string target;
    public string equip_target;
    public string effect;
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
            if (!item.disabled) {
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
        }

        foreach (CharacterCardItem item in obj.characters) {
            if (!item.disabled) {
                CharacterCard card = new CharacterCard();
                card.name = item.name;
                card.expansion = item.expansion;
                card.loadTexture("characters/" + item.image);
                card.hp = item.hp;
                deckCharacters.Add(card);
            }
        }

        foreach (CardItem item in obj.roles) {
            Card card = new Card();
            card.name = item.name;
            card.loadTexture("roles/" + item.image);
            deckRoles.Add(card);
        }

        foreach (ExpansionCardItem item in obj.fistfulofcards) {
            if (!item.disabled) {
                ExpansionCard card = new ExpansionCard();
                card.name = item.name;
                card.expansion = "fistfulofcards";
                card.loadTexture("fistfulofcards/" + item.image);
                card.last_card = item.last_card;
                deckFistfulofcards.Add(card);
            }
        }

        foreach (ExpansionCardItem item in obj.highnoon) {
            if (!item.disabled) {
                ExpansionCard card = new ExpansionCard();
                card.name = item.name;
                card.expansion = "highnoon";
                card.loadTexture("highnoon/" + item.image);
                card.last_card = item.last_card;
                deckHighnoon.Add(card);
            }
        }

        foreach (ExpansionCardItem item in obj.wildwestshow) {
            if (!item.disabled) {
                ExpansionCard card = new ExpansionCard();
                card.name = item.name;
                card.expansion = "wildwestshow";
                card.loadTexture("wildwestshow/" + item.image);
                card.last_card = item.last_card;
                deckWildwestshow.Add(card);
            }
        }

        foreach (GoldrushCardItem item in obj.goldrush) {
            if (!item.disabled) {
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

}
