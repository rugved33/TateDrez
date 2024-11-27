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
}