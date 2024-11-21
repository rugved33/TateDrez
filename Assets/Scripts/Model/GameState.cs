using UnityEngine;


namespace Game.Tatedrez.Model
{
    public class GameState
    {
        public enum State
        {
            PlacementPhase,
            DynamicPhase,
            Completed
        }

        public State CurrentState { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public Player OpponentPlayer { get; private set; }
        public IBoard Board { get; private set; }
        public bool GameOver => CurrentState == State.Completed;

        private const int MaxPieces = 3;

        public GameState(Player player1, Player player2, IBoard board)
        {
            CurrentPlayer = player1;
            OpponentPlayer = player2;
            Board = board;
            CurrentState = State.PlacementPhase;
        }

        // Places a piece during the placement phase
        public bool PlacePiece(Piece piece, int x, int y)
        {
            if (CurrentState != State.PlacementPhase) return false;

            if (Board.PlacePiece(piece, x, y))
            {
                CurrentPlayer.AddPiece(piece); // Ensure the piece is tracked by the player

                if (Board.CheckForTicTacToe(CurrentPlayer.Color))
                {
                    CurrentState = State.Completed;
                    Debug.Log($"{CurrentPlayer.Color} wins during the placement phase!");
                    return true;
                }

                // Check if both players have placed all their pieces
                if (CurrentPlayer.Pieces.Count == MaxPieces && OpponentPlayer.Pieces.Count == MaxPieces)
                {
                    CurrentState = State.DynamicPhase;
                    Debug.Log("Switching to Dynamic Phase.");
                }

                SwitchTurn();
                return true;
            }

            return false;
        }


        // Moves a piece during the dynamic phase
        public bool MovePiece(int fromX, int fromY, int toX, int toY)
        {
            if (CurrentState != State.DynamicPhase) return false;

            if (Board.MovePiece(fromX, fromY, toX, toY))
            {
                if (Board.CheckForTicTacToe(CurrentPlayer.Color))
                {
                    CurrentState = State.Completed;
                    Debug.Log($"{CurrentPlayer.Color} wins during the dynamic phase!");
                    return true;
                }

                SwitchTurn();
                return true;
            }

            return false;
        }

        // Switches turn between players
        private void SwitchTurn()
        {
            (CurrentPlayer, OpponentPlayer) = (OpponentPlayer, CurrentPlayer);
        }

        // Resets the game to its initial state
        public void ResetGame()
        {
            CurrentState = State.PlacementPhase;
            CurrentPlayer.Pieces.Clear();
            OpponentPlayer.Pieces.Clear();

            for (int x = 0; x < Board.Width; x++)
            {
                for (int y = 0; y < Board.Height; y++)
                {
                    Board.PlacePiece(null, x, y);
                }
            }

            CurrentPlayer.IsTurn = true;
            OpponentPlayer.IsTurn = false;

            Debug.Log("Game reset to initial state.");
        }
    }
}
