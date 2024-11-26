using UnityEngine;
using Game.Tatedrez.Model;
using Game.Tatedrez.View;
using Game.Tatedrez.Factory;
using Game.Tatedrez.Commands;

namespace Game.Tatedrez.Presenter
{
    public class TatedrezPresenter : MonoBehaviour
    {
        [SerializeField] private TatedrezView view; // Reference to the View (UI/Board)
        private GameState gameState;

        private Player player1;
        private Player player2;
        private IBoard board;

        private Piece selectedPiece;
        private (int x, int y) selectedPosition; 
        private PieceType selectedPieceType;
        private IPieceFactory pieceFactory;
        private const int BoardWidth = 3;
        private const int BoardHeight = 3;

        private void Start()
        {
            pieceFactory = new PieceFactory();
            InitializeGame();
        }

        private void Update()
        {
            HandleInput();
        }

        private void InitializeGame()
        {
            player1 = new Player(PlayerColor.White);
            player2 = new Player(PlayerColor.Black);

            player1.InitPlayerPieces(pieceFactory.CreateDefaultPieces());
            player2.InitPlayerPieces(pieceFactory.CreateDefaultPieces());

            board = new Board(BoardWidth, BoardHeight);
            gameState = new GameState(player1, player2, board);

            view.InitializeBoard(BoardWidth, BoardHeight);
            view.BindPieceTypeSelection(SelectPieceType); 
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                var boardPosition = view.GetBoardPositionFromMouse(Input.mousePosition);
                if (boardPosition.HasValue)
                {
                    int x = boardPosition.Value.x;
                    int y = boardPosition.Value.y;

                    if (gameState.CurrentState == GameState.State.PlacementPhase)
                    {
                        HandlePlacementPhase(x, y);
                    }
                    else if (gameState.CurrentState == GameState.State.DynamicPhase)
                    {
                        HandleDynamicPhase(x, y);
                    }
                }
            }
        }

        
        private void HandlePlacementPhase(int x, int y)
        {
            if (selectedPieceType == PieceType.None)
            {
                Debug.Log("Please select a piece type before placing.");
                return;
            }

            if (gameState.CurrentPlayer.GetAvailablePieceCount(selectedPieceType) <= 0)
            {
                Debug.Log($"No {selectedPieceType} pieces available for {gameState.CurrentPlayer.Color}");
                return;
            }
            Debug.Log($"current player is {gameState.CurrentPlayer.Color}");

            var piece = pieceFactory.CreatePiece(selectedPieceType, gameState.CurrentPlayer.Color);

            var command = new PlacePieceCommand(gameState, piece, x, y);

            if (command.Execute())
            {
                view.UpdateBoard(board); 
                selectedPieceType = PieceType.None; 
                Debug.Log($"{piece.GetType().Name} placed at ({x}, {y})");
            }
            else
            {
                Debug.Log("Invalid placement position.");
            }

            Debug.Log($"current player after placement is {gameState.CurrentPlayer.Color}");
            UpdateView();
        }

        
        private void HandleDynamicPhase(int x, int y)
        {
            var clickedPiece = board.GetPiece(x, y);

            if(selectedPiece == clickedPiece)
            {
                selectedPiece = null;
                view.HighlightCell(x,y,false);
                Debug.Log($"Deselecting piece at ({x},{y})");
                return;
            }

            if (selectedPiece == null)
            {
                
                if (clickedPiece != null && clickedPiece.Owner == gameState.CurrentPlayer.Color)
                {
                    selectedPiece = clickedPiece;
                    selectedPosition = (x, y);
                    view.HighlightCell(x, y, true); 
                    Debug.Log($"Selected piece at ({x}, {y})");
                }
                else
                {
                    Debug.Log("No piece selected or piece does not belong to the current player.");
                }
            }
            else
            {
                var command = new MovePieceCommand(gameState, selectedPosition.x, selectedPosition.y, x, y);

                if (command.Execute())
                {
                    view.UpdateBoard(board); 
                    view.ClearHighlights();
                    selectedPiece = null; 
                    Debug.Log($"Moved piece to ({x}, {y})");
                }
                else
                {
                    Debug.Log("Invalid move. Try again.");
                }
            }
            UpdateView();
        }

        public void UpdateView()
        {
            Debug.Log($"updating view current player after placement is {gameState.CurrentPlayer.Color}");

            int totalMoves = gameState.TotalMoves;
            view.UpdateHUD(totalMoves);

            var currentPlayer = gameState.CurrentPlayer;
            view.UpdateCurrentPlayerView(currentPlayer);
        }

        public void SelectPieceType(PieceType pieceType)
        {
            if (gameState.CurrentState != GameState.State.PlacementPhase)
            {
                Debug.Log("Cannot select a piece type outside the placement phase.");
                return;
            }

            if (gameState.CurrentPlayer.GetAvailablePieceCount(pieceType) <= 0)
            {
                Debug.Log($"No {pieceType} pieces available for {gameState.CurrentPlayer.Color}");
                return;
            }

            selectedPieceType = pieceType;
            Debug.Log($"{pieceType} selected for placement.");
        }
        
        public void ResetGame()
        {
            var command = new ResetGameCommand(gameState);

            if(command.Execute())
            {
                view.ResetView();
                InitializeGame();
            }
        }
    }
}
