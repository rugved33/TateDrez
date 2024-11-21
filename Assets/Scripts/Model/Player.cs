using UnityEngine;
using System.Collections.Generic;

namespace Game.Tatedrez.Model
{
    public class Player
    {
        public PlayerColor Color { get; private set; }
        public List<Piece> Pieces { get; private set; }
        public bool IsTurn { get; set; }

        private const int MaxLimit = 3;

        public Player(PlayerColor color)
        {
            Color = color;
            Pieces = new List<Piece>();
            IsTurn = color == PlayerColor.Black;
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
                for (int x = 0; x < board.Width; x++)
                {
                    for (int y = 0; y < board.Height; y++)
                    {
                        if (piece.IsValidMove(GetPiecePosition(piece, board).x, GetPiecePosition(piece, board).y, x, y, board))
                            return true;
                    }
                }
            }
            return false;
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