using System;
using Game.Tatedrez.Model;
using System.Collections.Generic;

namespace Game.Tatedrez.Factory
{
    public class PieceFactory : IPieceFactory
    {
        public Piece CreatePiece(PieceType pieceType, PlayerColor owner)
        {
            return pieceType switch
            {
                PieceType.Knight => new Knight(owner),
                PieceType.Rook => new Rook(owner),
                PieceType.Bishop => new Bishop(owner),
                _ => throw new ArgumentException($"Invalid piece type: {pieceType}")
            };
        }

        public Dictionary<PieceType, int> CreateDefaultPieces()
        {
            return new Dictionary<PieceType, int>
            {
                { PieceType.Knight, 1 },
                { PieceType.Rook, 1 },
                { PieceType.Bishop, 1 }
            };
        }
    }
}
