using Game.Tatedrez.Model;

namespace Game.Tatedrez.Factory
{
    public interface IPieceFactory
    {
        Piece CreatePiece(PieceType pieceType, PlayerColor owner); 
    }
}
