using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Tatedrez.Model;
using TMPro;

namespace Game.Tatedrez.View
{
    public class TatedrezView : MonoBehaviour
    {
        private GameObject[,] boardCells;
        private Dictionary<Piece, GameObject> pieceGameObjects; 

        [SerializeField] private BoardData boardData;
        [SerializeField] private PieceSpriteData pieceSpriteData;
        [SerializeField] private Button knightButton;
        [SerializeField] private Button rookButton;
        [SerializeField] private Button bishopButton;
        [SerializeField] private TextMeshProUGUI currentPlayerText;
    
        private Dictionary<PieceType, Button> pieceButtons;
        private const float PieceAnimDuration = 0.5f;

        public void InitializeBoard(int width, int height)
        {
            boardCells = new GameObject[width, height];
            pieceGameObjects = new Dictionary<Piece, GameObject>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = CreateCell(x, y);
                    boardCells[x, y] = cell;
                }
            }

            // Map piece types to buttons
            pieceButtons = new Dictionary<PieceType, Button>
            {
                { PieceType.Knight, knightButton },
                { PieceType.Rook, rookButton },
                { PieceType.Bishop, bishopButton }
            };
        }

        public void BindPieceTypeSelection(System.Action<PieceType> onPieceTypeSelected)
        {
            knightButton.onClick.AddListener(() => onPieceTypeSelected(PieceType.Knight));
            rookButton.onClick.AddListener(() => onPieceTypeSelected(PieceType.Rook));
            bishopButton.onClick.AddListener(() => onPieceTypeSelected(PieceType.Bishop));
        }

        public Vector2Int? GetBoardPositionFromMouse(Vector3 mousePosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var cell = hit.collider.gameObject.GetComponent<Cell>();
                return cell != null ? new Vector2Int(cell.X, cell.Y) : null;
            }
            return null;
        }

        public void UpdateBoard(IBoard board)
        {
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    var piece = board.GetPiece(x, y);
                    UpdateCellVisual(x, y, piece);
                }
            }
        }

        public void UpdateCurrentPlayerView(Player currentPlayer)
        {
            currentPlayerText.text = $"Current Player: {currentPlayer.Color}";

            foreach (var pair in pieceButtons)
            {
                var pieceType = pair.Key;  
                var button = pair.Value;  
                bool hasPieceOfType = currentPlayer.Pieces.Exists(piece => piece.GetPieceType() == pieceType);

                button.interactable = !hasPieceOfType;
            }
        }


        public void HighlightCell(int x, int y, bool highlight)
        {
            var cell = boardCells[x, y];
            cell.GetComponent<Cell>().Highlight(highlight);
        }

        public void ClearHighlights()
        {
            foreach (var cell in boardCells)
            {
                cell.GetComponent<Cell>().Highlight(false);
            }
        }

        public void ResetView()
        {
            foreach (var cell in boardCells)
            {
                Destroy(cell);
            }

            foreach (var pieceObject in pieceGameObjects.Values)
            {
                Destroy(pieceObject);
            }

            boardCells = null;
            pieceGameObjects.Clear();
        }

        private GameObject CreateCell(int x, int y)
        {
            var cellObject = new GameObject($"Cell_{x}_{y}");
            var cell = cellObject.AddComponent<Cell>();
            cell.Initialize(x, y, boardData);

            // Add a collider to detect mouse clicks
            var collider = cellObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(1, 1, 1);

            return cellObject;
        }

        private void UpdateCellVisual(int x, int y, Piece piece)
        {
            if (piece == null) return;

            // If the piece is not yet spawned, create it
            if (!pieceGameObjects.ContainsKey(piece))
            {
                Debug.Log($"spawning piece {piece.GetPieceType()}");
                SpawnPiece(piece, x, y);
            }
            else
            {
                // If the piece is already spawned, animate it to the new position
                MovePiece(piece, x, y);
            }
        }

        private void SpawnPiece(Piece piece, int x, int y)
        {
            var sprite = pieceSpriteData.GetSprite(piece.GetPieceType(), piece.Owner);

            // Create a new GameObject for the piece
            var pieceObject = new GameObject($"Piece_{piece.GetPieceType()}_{piece.Owner}");
            var spriteRenderer = pieceObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = 1;

            pieceObject.transform.position = GetCellWorldPosition(x, y);
            pieceObject.transform.SetParent(transform);

            // Track the spawned piece
            pieceGameObjects[piece] = pieceObject;
        }

        private void MovePiece(Piece piece, int x, int y)
        {
            var pieceObject = pieceGameObjects[piece];
            if (pieceObject != null)
            {
                // Animate movement to the target cell
                var targetPosition = GetCellWorldPosition(x, y);
                StartCoroutine(AnimatePieceMovement(pieceObject, targetPosition));
            }
        }

        private Vector3 GetCellWorldPosition(int x, int y)
        {
            return new Vector3(x, y, 0); 
        }

        private System.Collections.IEnumerator AnimatePieceMovement(GameObject pieceObject, Vector3 targetPosition)
        {
            float duration = PieceAnimDuration; 
            float elapsed = 0f;

            Vector3 startPosition = pieceObject.transform.position;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                pieceObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
                yield return null;
            }

            pieceObject.transform.position = targetPosition;
        }
    }
}