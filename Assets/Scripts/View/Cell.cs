using UnityEngine;

namespace Game.Tatedrez.View
{
    public class Cell : MonoBehaviour
    {
        public int X { get; private set; } // X-coordinate of the cell on the board
        public int Y { get; private set; } // Y-coordinate of the cell on the board

        private Renderer cellRenderer;

        public void Initialize(int x, int y)
        {
            X = x;
            Y = y;

            // Add a visual representation for the cell 
            cellRenderer = gameObject.AddComponent<SpriteRenderer>();

            // Adjust the transform for better visuals
            transform.position = new Vector3(X, Y, 0); // Place cell at grid position
            transform.localScale = Vector3.one * 0.9f; // Scale down slightly for gaps
        }

        // Highlight the cell (for selection or valid moves)
        public void Highlight(bool highlight)
        {
            if (cellRenderer != null)
            {
                cellRenderer.material.color = highlight ? Color.yellow : Color.white;
            }
        }
    }
}
