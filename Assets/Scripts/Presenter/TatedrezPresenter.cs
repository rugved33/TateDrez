using UnityEngine;
using Game.Tatedrez.Model;
using Game.Tatedrez.View;
using Game.Tatedrez.Factory;
using Game.Tatedrez.Commands;
using DependencyInjection;


namespace Game.Tatedrez.Presenter
{
    public class TatedrezPresenter : MonoBehaviour
    {
        [SerializeField] private TatedrezView view; // Reference to the View (UI/Board)
        [SerializeField] private GameUIView gameUIView; 
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

        [Inject]
        private void Init(IPieceFactory pieceFactory)
        {
            this.pieceFactory = pieceFactory;
        }

        private void Start()
        {
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
            gameState.OnPlayerSwitched += DeselectPiece;
            gameState.OnPlayerSwitched += view.ClearHighlights;

            view.InitializeBoard(BoardWidth, BoardHeight);
            view.BindPieceTypeSelection(SelectPieceType); 

            UpdateGameUIView();
            gameUIView.BindRetryButton(ResetGame);
        }

        private void HandleInput()
        {
#if UNITY_EDITOR || UNITY_STANDALONE

            if (Input.GetMouseButtonDown(0)) 
            {
                HandleBoardInteraction(Input.mousePosition);
            }
#elif UNITY_IOS || UNITY_ANDROID

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended) 
                {
                    HandleBoardInteraction(touch.position);
                }
            }
#endif
        }

        private void HandleBoardInteraction(Vector3 inputPosition)
        {
            var boardPosition = view.GetBoardPositionFromMouse(inputPosition);
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
                view.UpdateBoard(board, gameState.CurrentState); 
                selectedPieceType = PieceType.None; 
                Debug.Log($"{piece.GetType().Name} placed at ({x}, {y})");

                if (gameState.CurrentState == GameState.State.DynamicPhase)
                {
                    view.ShowPiecesSelectionButtons(false); 
                    Debug.Log("Dynamic phase started. Disabling piece selection buttons.");
                }

                if (gameState.GameOver)
                {
                    HandleGameOver(gameState.CurrentPlayer);
                }
            }
            else
            {
                Debug.Log("Invalid placement position.");
            }

            Debug.Log($"current player after placement is {gameState.CurrentPlayer.Color}");
            UpdateGameUIView();
        }

        
        private void HandleDynamicPhase(int x, int y)
        {
            var clickedPiece = board.GetPiece(x, y);

            if(selectedPiece == clickedPiece)
            {
                DeselectPiece();
                return;
            }

            if (selectedPiece == null)
            {
                
                if (clickedPiece != null && clickedPiece.Owner == gameState.CurrentPlayer.Color)
                {
                    SelectPiece(clickedPiece,x,y);
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
                    view.UpdateBoard(board, gameState.CurrentState); 
                    view.HighlightCell(x,y,true);
                    selectedPiece = null; 
                    Debug.Log($"Moved piece to ({x}, {y})");

                    if (gameState.GameOver)
                    {
                        HandleGameOver(gameState.CurrentPlayer);
                    }
                }
                else
                {
                    Debug.Log("Invalid move. Try again.");
                }
            }
            UpdateGameUIView();
        }

        private void SelectPiece(Piece piece, int x, int y)
        {
            selectedPiece = piece;
            selectedPosition = (x, y);
            view.HighlightPiece(piece, true); // Highlight the selected piece
            view.HighlightCell(x,y,true);
            Debug.Log($"Selected piece at ({x}, {y})");
        }

        private void DeselectPiece()
        {
            if (selectedPiece != null)
            {
                view.HighlightPiece(selectedPiece, false); // Remove highlight
                view.HighlightCell(selectedPosition.x,selectedPosition.y,false);
                selectedPiece = null;
                Debug.Log("Piece deselected.");
            }
        }

        public void UpdateGameUIView()
        {
            Debug.Log($"updating view current player after placement is {gameState.CurrentPlayer.Color}");
            view.UpdateCurrentPlayerView(gameState.CurrentPlayer);
            gameUIView.UpdateHUD(gameState.TotalMoves, gameState.CurrentPlayer);
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

        private void HandleGameOver(Player winner)
        {
            if (winner != null)
            {
                var winningCells = gameState.GetWinningCells();

                foreach (var (x, y) in winningCells)
                {
                    view.HighlightCell(x, y, true); 
                }
                gameUIView.ShowWinScreen(winner);
            }
        }
    }
}
