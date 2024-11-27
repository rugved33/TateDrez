using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Tatedrez.Model;
using DG.Tweening;

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

        [Space(15)]
        [SerializeField] private float pieceSpawnBounceDuration = 0.5f;
        [SerializeField] private float pieceSelectedScale = 1.2f;
        [SerializeField] private float pieceNormalScale = 1.0f;

        [Space(15)]
        [SerializeField] private float buttonBounceDuration = 0.2f;
        [SerializeField] private float buttonBounceScale = 1.2f;
        [SerializeField] private float buttonOriginalScale = 1.0f;

    
        private Dictionary<PieceType, Button> pieceButtons;
        private const float PieceAnimDuration = 0.5f;
        private bool gameOver;

        private void Start()
        {
            DOTween.Init();
        }
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
            knightButton.onClick.AddListener(() => HandleButtonClick(knightButton, PieceType.Knight, onPieceTypeSelected));
            rookButton.onClick.AddListener(() => HandleButtonClick(rookButton, PieceType.Rook, onPieceTypeSelected));
            bishopButton.onClick.AddListener(() => HandleButtonClick(bishopButton, PieceType.Bishop, onPieceTypeSelected));
        }

        private void HandleButtonClick(Button button, PieceType pieceType, System.Action<PieceType> onPieceTypeSelected)
        {
            button.transform.DOScale(buttonBounceScale, buttonBounceDuration)
                .SetEase(Ease.OutBounce)
                .OnComplete(() =>
                {
                    button.transform.DOScale(buttonOriginalScale, buttonBounceDuration).SetEase(Ease.InOutBounce);
                    onPieceTypeSelected(pieceType); 
                });
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

        public void UpdateBoard(IBoard board, GameState.State gameState)
        {
            if(gameState == GameState.State.Completed)
            {
                gameOver = true;
            }

            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    var piece = board.GetPiece(x, y);
                    UpdateCellVisual(x, y, piece);
                }
            }
        }

        public void UpdateCurrentPlayerView(IPlayer currentPlayer)
        {
            foreach (var pair in pieceButtons)
            {
                var pieceType = pair.Key;  
                var button = pair.Value;

                var sprite = pieceSpriteData.GetSprite(pieceType, currentPlayer.Color);

                var buttonImage = button.transform.GetChild(1).GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.sprite = sprite;
                }
                bool hasPieceOfType = currentPlayer.Pieces.Exists(piece => piece.GetPieceType() == pieceType);

                button.interactable = !hasPieceOfType;
            }
        }


        public void HighlightCell(int x, int y, bool highlight)
        {
            var cell = boardCells[x, y];
            cell.GetComponent<Cell>().Highlight(highlight);
        }
        public void RemoveHighlightCell(int x, int y)
        {
            var cell = boardCells[x, y];
            cell.GetComponent<Cell>().Highlight(false);
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
            ShowPiecesSelectionButtons(true);
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

            pieceObject.transform.localScale = Vector3.zero;
            pieceObject.transform.DOScale(Vector3.one,  pieceSpawnBounceDuration).SetEase(Ease.OutBounce);
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
            RemoveHighlightCell((int)startPosition.x, (int)startPosition.y);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                pieceObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
                yield return null;
            }

            pieceObject.transform.position = targetPosition;

            if(!gameOver)
            {
                ClearHighlights();
            }
        }

        public void HighlightPiece(Piece piece, bool highlight)
        {
            if (pieceGameObjects.TryGetValue(piece, out var pieceObject))
            {
                if (highlight)
                {
                    pieceObject.transform.DOScale(pieceSelectedScale, PieceAnimDuration).SetEase(Ease.OutBounce); // Scale up
                }
                else
                {
                    pieceObject.transform.DOScale(pieceNormalScale, PieceAnimDuration).SetEase(Ease.InOutBounce); // Scale back to normal
                }
            }
        }

        public void ShowPiecesSelectionButtons(bool canShow)
        {
            knightButton.gameObject.SetActive(canShow);
            rookButton.gameObject.SetActive(canShow);
            bishopButton.gameObject.SetActive(canShow);
        }
    }
}
