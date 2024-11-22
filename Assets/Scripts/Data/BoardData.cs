using UnityEngine;
using Game.Tatedrez.Model;

[CreateAssetMenu(fileName = "BoardData", menuName = "Tatedrez/BoardData", order = 51)]
public class BoardData : ScriptableObject
{
    public Sprite square;
    public Color darkTileColor;
    public Color lightTileColor;
}

