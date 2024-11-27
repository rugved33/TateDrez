using UnityEngine;
using System.Linq;
using System.Collections.Generic;


namespace Game.Tatedrez.Model
{
    public interface IPlayer
    {
        PlayerColor Color { get; set; }
        List<Piece> Pieces { get;  set; }
        bool IsTurn { get; set; }
        void InitPlayerPieces(Dictionary<PieceType, int> Pieces);
        void AddPiece(Piece piece);
        bool RemovePiece(Piece piece);
        bool CanMove(IBoard board);
        bool DeductAvailablePiece(PieceType pieceType);
        int GetAvailablePieceCount(PieceType pieceType);
    }
    public class Player : IPlayer
    {
        public PlayerColor Color { get; set; }
        public List<Piece> Pieces { get;  set; }
        public bool IsTurn { get; set; }
        private Dictionary<PieceType, int> availablePieces;
        private const int MaxLimit = 3;

        private int[,] knightOffsets = new int[,]
        {
            { 2, 1 }, { 1, 2 }, { -1, 2 }, { -2, 1 },
            { -2, -1 }, { -1, -2 }, { 1, -2 }, { 2, -1 }
        };
        private int[,] directions = new int[,]
        {
            { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 }
        };

        public Player(PlayerColor color)
        {
            Color = color;
            Pieces = new List<Piece>();
            IsTurn = color == PlayerColor.Black;
        }

        public void InitPlayerPieces(Dictionary<PieceType, int> Pieces)
        {
            availablePieces = new Dictionary<PieceType, int>();
            availablePieces = Pieces;
        }

        public void AddPiece(Piece piece)
        {
            if (Pieces.Count < MaxLimit)
            {
                Pieces.Add(piece);
                Debug.Log($"Added piece: {piece.GetType().Name}. Total pieces: {Pieces.Count}");
            }
            else
            {
                Debug.Log($"Cannot add piece: {piece.GetType().Name}. Max limit reached.");
            }
        }
        public bool RemovePiece(Piece piece)
        {
            if (Pieces.Contains(piece))
            {
                Pieces.Remove(piece);
                return true;
            }
            return false;
        }

        public bool CanMove(IBoard board)
        {
            foreach (var piece in Pieces)
            {
                var (currentX, currentY) = GetPiecePosition(piece, board);


                foreach (var (x, y) in GetPotentialMoves(piece, currentX, currentY, board))
                {
                    if (piece.IsValidMove(currentX, currentY, x, y, board))
                        return true;
                }
            }
            return false; 
        }
        private IEnumerable<(int x, int y)> GetPotentialMoves(Piece piece, int currentX, int currentY, IBoard board)
        {
            switch (piece.GetPieceType())
            {
                case PieceType.Knight:
                    return GetKnightMoves(currentX, currentY, board);

                case PieceType.Rook:
                    return GetRookMoves(currentX, currentY, board);

                case PieceType.Bishop:
                    return GetBishopMoves(currentX, currentY, board);

                default:
                    return Enumerable.Empty<(int x, int y)>();
            }
        }
        private IEnumerable<(int x, int y)> GetKnightMoves(int currentX, int currentY, IBoard board)
        {

            for (int i = 0; i < knightOffsets.GetLength(0); i++)
            {
                int targetX = currentX + knightOffsets[i, 0];
                int targetY = currentY + knightOffsets[i, 1];

                if (board.IsWithinBounds(targetX, targetY))
                {
                    yield return (targetX, targetY);
                }
            }
        }
        private IEnumerable<(int x, int y)> GetRookMoves(int currentX, int currentY, IBoard board)
        {

            for (int x = 0; x < board.Width; x++)
            {
                if (x != currentX) yield return (x, currentY);
            }
            for (int y = 0; y < board.Height; y++)
            {
                if (y != currentY) yield return (currentX, y);
            }
        }
        private IEnumerable<(int x, int y)> GetBishopMoves(int currentX, int currentY, IBoard board)
        {
            for (int dir = 0; dir < directions.GetLength(0); dir++)
            {
                int dx = directions[dir, 0];
                int dy = directions[dir, 1];

                int x = currentX + dx, y = currentY + dy;

                while (board.IsWithinBounds(x, y))
                {
                    yield return (x, y);

                    x += dx;
                    y += dy;
                }
            }
        }

        // Decrements the count of an available piece type
        public bool DeductAvailablePiece(PieceType pieceType)
        {
            if (availablePieces.ContainsKey(pieceType) && availablePieces[pieceType] > 0)
            {
                availablePieces[pieceType]--;
                return true;
            }
            return false;
        }

        // Returns the count of available pieces for a specific type
        public int GetAvailablePieceCount(PieceType pieceType)
        {
            return availablePieces.ContainsKey(pieceType) ? availablePieces[pieceType] : 0;
        }

        private (int x, int y) GetPiecePosition(Piece piece, IBoard board)
        {
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    if (board.GetPiece(x, y) == piece)
                        return (x, y);
                }
            }
            return (-1, -1);
        }
    }
}