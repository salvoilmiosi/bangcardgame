using UnityEngine;
using System.Collections.Generic;

class SignSprites {
    private static Dictionary<string, Sprite> sprites;

    private static void loadSprites() {
        if (sprites == null) {
            sprites = new Dictionary<string, Sprite>();
            string[] names = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "cuori", "quadri", "fiori", "picche"};
            foreach(string name in names) {
                Texture2D texture = Resources.Load("cards/signs/sign_" + name) as Texture2D;
                sprites[name] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }

    public static Sprite getNum(string name) {
        loadSprites();
        return sprites[name];
    }
    public static Sprite getSuit(char name) {
        loadSprites();
        switch (name) {
        case 'C':
            return sprites["cuori"];
        case 'Q':
            return sprites["quadri"];
        case 'F':
            return sprites["fiori"];
        case 'P':
            return sprites["picche"];
        default:
            return null;
        }
    }
}

public class CardSprite : MonoBehaviour {
    private Card _card;
    public Card card {
        get { return _card; }
        set {
            _card = value;
            if (cardRenderer == null) return;
            if (value == null) return;

            cardRenderer.sprite = _card.sprite;
            if (typeof(BangCard).IsInstanceOfType(_card)) {
                string sign = ((BangCard)_card).sign;
                char suit = sign[sign.Length - 1];
                string num = sign.Substring(0, sign.Length - 1);

                numRenderer.sprite = SignSprites.getNum(num);
                suitRenderer.sprite = SignSprites.getSuit(suit);
            } else {
                numRenderer.sprite = null;
                suitRenderer.sprite = null;
            }
        }
    }

    public SpriteRenderer cardRenderer;
    public SpriteRenderer numRenderer;
    public SpriteRenderer suitRenderer;
}
