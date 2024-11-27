using UnityEngine;

namespace Game.Tatedrez.Model
{
    public class Board : IBoard
    {
        private Piece[,] boardState;
        public int Width => boardState.GetLength(0);
        public int Height => boardState.GetLength(1);

        public Board(int width, int height)
        {
            boardState = new Piece[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PlacePiece(null, x, y);
                }
            }
        }

        public Piece GetPiece(int x, int y) => IsWithinBounds(x, y) ? boardState[x, y] : null;

        public bool PlacePiece(Piece piece, int x, int y)
        {
            if (!IsWithinBounds(x, y) || boardState[x, y] != null) return false;

            if (boardState[x, y] != null)
            {
                Debug.LogWarning($"Cannot place piece at ({x}, {y}): Cell is already occupied by {boardState[x, y].GetPieceType()}.");
                return false;
            }
            boardState[x, y] = piece;
            return true;
        }

        public bool MovePiece(int fromX, int fromY, int toX, int toY)
        {
            if (!IsWithinBounds(fromX, fromY) || !IsWithinBounds(toX, toY)) return false;

            Piece piece = boardState[fromX, fromY];
            if (piece == null || boardState[toX, toY] != null) return false;

            if (piece.IsValidMove(fromX, fromY, toX, toY, this))
            {
                boardState[toX, toY] = piece;
                boardState[fromX, fromY] = null;
                return true;
            }

            return false;
        }

        public bool CheckForTicTacToe(PlayerColor playerColor)
        {
            // Check rows
            for (int row = 0; row < Height; row++)
            {
                if (IsLine(playerColor, boardState[row, 0], boardState[row, 1], boardState[row, 2]))
                    return true;
            }

            // Check columns
            for (int col = 0; col < Width; col++)
            {
                if (IsLine(playerColor, boardState[0, col], boardState[1, col], boardState[2, col]))
                    return true;
            }

            // Check diagonals
            if (IsLine(playerColor, boardState[0, 0], boardState[1, 1], boardState[2, 2]))
                return true;

            if (IsLine(playerColor, boardState[0, 2], boardState[1, 1], boardState[2, 0]))
                return true;

            return false;
        }

        // Helper method to check if all pieces in a line belong to the same player
        private bool IsLine(PlayerColor playerColor, Piece p1, Piece p2, Piece p3)
        {
            return p1?.Owner == playerColor && p2?.Owner == playerColor && p3?.Owner == playerColor;
        }
        public string GetDetailedBoardState()
        {
            var boardString = new System.Text.StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var piece = GetPiece(x, y);
                    string pieceInfo = piece != null
                        ? $"{piece.GetPieceType()} ({piece.Owner})"
                        : "Empty";

                    boardString.AppendLine($"Position ({x}, {y}): {pieceInfo}");
                }
            }

            return boardString.ToString();
        }

        public bool IsWithinBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
    }
}