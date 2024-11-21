
namespace Game.Tatedrez.Model
{
    public interface IBoard
    {
        int Width { get; }
        int Height { get; }
        Piece GetPiece(int x, int y);
        bool PlacePiece(Piece piece, int x, int y);
        bool MovePiece(int fromX, int fromY, int toX, int toY);
        bool CheckForTicTacToe(PlayerColor playerColor);
    }
}