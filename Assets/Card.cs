using UnityEngine;

public class Card {
    public string name;
    public string expansion;

    public Sprite sprite;

    public void loadTexture(string asset_path) {
        Texture2D texture = Resources.Load("cards/" + asset_path) as Texture2D;
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}

public class BangCard : Card {
    public string sign;
    public string color;
    public string type;
    public int cube_cost;
}

public class CharacterCard : Card {
    public int hp;
}

public class ExpansionCard : Card {
    public bool last_card;
}

public class GoldrushCard : Card {
    public string color;
    public string type;
    public int gold_cost;
}