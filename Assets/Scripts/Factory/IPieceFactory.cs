using System.Collections.Generic;
using Game.Tatedrez.Model;

namespace Game.Tatedrez.Factory
{
    public interface IPieceFactory
    {
        Piece CreatePiece(PieceType pieceType, PlayerColor owner); 
        Dictionary<PieceType,int> CreateDefaultPieces();
    }
}
