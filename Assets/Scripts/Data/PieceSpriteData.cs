using UnityEngine;
using Game.Tatedrez.Model;

[CreateAssetMenu(fileName = "PieceSpriteData", menuName = "Tatedrez/PieceSpriteData", order = 51)]
public class PieceSpriteData : ScriptableObject
{
    [System.Serializable]
    public class PieceSprite
    {
        public PieceType type;
        public PlayerColor color;
        public Sprite sprite;
    }

    public PieceSprite[] pieces;

    // Method to get the sprite for a specific piece type and color
    public Sprite GetSprite(PieceType type, PlayerColor color)
    {
        foreach (var piece in pieces)
        {
            if (piece.type == type && piece.color == color)
                return piece.sprite;
        }
        Debug.LogError($"Sprite not found for PieceType: {type} and PlayerColor: {color}");
        return null;
    }
}

