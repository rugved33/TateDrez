using UnityEngine;

namespace Game.Tatedrez.View
{
    public class Cell : MonoBehaviour
    {
        public int X { get; private set; } // X-coordinate of the cell on the board
        public int Y { get; private set; } // Y-coordinate of the cell on the board

        private SpriteRenderer cellRenderer;
        private Color assignedColor;
        private const float ScaleFactor = 0.9f;

        public void Initialize(int x, int y, BoardData boardData)
        {
            X = x;
            Y = y;

            cellRenderer = gameObject.AddComponent<SpriteRenderer>();
            cellRenderer.sprite = boardData.square;
            bool isDarkTile = (x + y) % 2 == 1;
            cellRenderer.color = isDarkTile ? boardData.darkTileColor : boardData.lightTileColor;
            assignedColor = cellRenderer.color;

            transform.position = new Vector3(X, Y, 0);
            transform.localScale = Vector3.one * ScaleFactor; 
        }

        public void Highlight(bool highlight)
        {
            if (cellRenderer != null)
            {
                cellRenderer.color = highlight ? Color.yellow : assignedColor;
            }
        }
    }
}
