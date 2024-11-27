using UnityEngine;
using System;


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
        public int TotalMoves { get; private set; }
        public bool GameOver => CurrentState == State.Completed;
        public event Action OnPlayerSwitched;

        private const int MaxPieces = 3;

        private Player BonusTurnPlayer;
        private const int MaxTurns = 1;
        private int currentTurns = 0;

        public GameState(Player player1, Player player2, IBoard board)
        {
            CurrentPlayer = player1;
            OpponentPlayer = player2;
            Board = board;
            CurrentState = State.PlacementPhase;
            TotalMoves = 0;
        }

        // Places a piece during the placement phase
        public bool PlacePiece(Piece piece, int x, int y)
        {
            if (CurrentState != State.PlacementPhase) return false;

            if (Board.PlacePiece(piece, x, y))
            {
                CurrentPlayer.AddPiece(piece); // Ensure the piece is tracked by the player
                IncrementMoves();
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

        private void IncrementMoves()
        {
            TotalMoves += 1;
        }

        private void CheckExtraTurns()
        {
            if(BonusTurnPlayer != CurrentPlayer && BonusTurnPlayer != null)
            {
                (CurrentPlayer, OpponentPlayer) = (OpponentPlayer, CurrentPlayer);
                currentTurns--;

                if(currentTurns <= 0)
                {
                    BonusTurnPlayer = null;
                }
            }
        }

        // Moves a piece during the dynamic phase  
        public bool MovePiece(int fromX, int fromY, int toX, int toY)
        {
            if (CurrentState != State.DynamicPhase)
            {
                Debug.LogWarning("Cannot move pieces outside of the Dynamic Phase.");
                Debug.Log(Board.GetDetailedBoardState());
                return false;
            }

            // Check if the current player is stuck
            if (!CurrentPlayer.CanMove(Board))
            {
                Debug.LogWarning($"{CurrentPlayer.Color} has no valid moves. Switching turn to {OpponentPlayer.Color}.");
                SwitchTurn();
                BonusTurnPlayer = CurrentPlayer;
                currentTurns = MaxTurns;


                // Check if the opponent is also stuck
                if (!CurrentPlayer.CanMove(Board))
                {
                    Debug.Log("Both players are stuck. The game ends in a draw.");
                    CurrentState = State.Completed;
                    return false;
                }

                return false; // Turn switches, but no move is made by the stuck player
            }


            if (Board.MovePiece(fromX, fromY, toX, toY))
            {
                IncrementMoves();
                if (Board.CheckForTicTacToe(CurrentPlayer.Color))
                {
                    CurrentState = State.Completed;
                    Debug.Log($"{CurrentPlayer.Color} wins during the dynamic phase!");
                    return true;
                }

                SwitchTurn();
                return true;
            }

            Debug.LogWarning($"Move failed unexpectedly from ({fromX}, {fromY}) to ({toX}, {toY}).");
            Debug.Log(Board.GetDetailedBoardState());
            return false;
        }

        // Switches turn between players
        private void SwitchTurn()
        {
            (CurrentPlayer, OpponentPlayer) = (OpponentPlayer, CurrentPlayer);
            CheckExtraTurns();
            OnPlayerSwitched?.Invoke();
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
            TotalMoves = 0;

            Debug.Log("Game reset to initial state.");
        }
    }
}
