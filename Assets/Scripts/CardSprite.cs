using UnityEngine;
using System.Collections.Generic;

public class CardSprite : MonoBehaviour {
    private static Dictionary<string, Sprite> signSprites;

    private static void loadSprites() {
        if (signSprites == null) {
            signSprites = new Dictionary<string, Sprite>();
            string[] names = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "cuori", "quadri", "fiori", "picche"};
            foreach(string name in names) {
                Texture2D texture = Resources.Load("cards/signs/sign_" + name) as Texture2D;
                signSprites[name] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }

    public static Sprite getNumSprite(string sign) {
        loadSprites();
        return signSprites[sign.Substring(0, sign.Length - 1)];
    }
    public static Sprite getSuitSprite(string sign) {
        loadSprites();
        switch (sign[sign.Length - 1]) {
        case 'C':
            return signSprites["cuori"];
        case 'Q':
            return signSprites["quadri"];
        case 'F':
            return signSprites["fiori"];
        case 'P':
            return signSprites["picche"];
        default:
            return null;
        }
    }

    private Card _card;
    public Card card {
        get { return _card; }
        set {
            _card = value;
            setSprites();
        }
    }

    private bool _flipped;
    public bool flipped {
        get { return _flipped; }
        set {
            _flipped = value;
            setSprites();
        }
    }

    private void setSprites() {
        if (card == null || flipped) {
            cardRenderer.sprite = backfaceSprite;
            numRenderer.sprite = null;
            suitRenderer.sprite = null;
        } else {
            cardRenderer.sprite = card.sprite;
            if (typeof(BangCard).IsInstanceOfType(card)) {
                numRenderer.sprite = getNumSprite(((BangCard)card).sign);
                suitRenderer.sprite = getSuitSprite(((BangCard)card).sign);
            } else {
                numRenderer.sprite = null;
                suitRenderer.sprite = null;
            }
        }
    }
    public Sprite backfaceSprite;
    public SpriteRenderer cardRenderer;
    public SpriteRenderer numRenderer;
    public SpriteRenderer suitRenderer;
}
