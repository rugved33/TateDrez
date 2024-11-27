using System;

namespace Game.Tatedrez.Model
{
    public enum PlayerColor { Black, White }
    public abstract class Piece
    {
        public PlayerColor Owner { get; private set; }

        protected Piece(PlayerColor owner)
        {
            Owner = owner;
        }
        public abstract PieceType GetPieceType();
        public abstract bool IsValidMove(int fromX, int fromY, int toX, int toY, IBoard board);
    }

    public class Knight : Piece
    {
        public Knight(PlayerColor owner) : base(owner) { }

        public override PieceType GetPieceType()
        {
            return PieceType.Knight;
        }
        public override bool IsValidMove(int fromX, int fromY, int toX, int toY, IBoard board)
        {
            int dx = Math.Abs(toX - fromX);
            int dy = Math.Abs(toY - fromY);

            // Check if the move follows the L-shape
            if (!((dx == 2 && dy == 1) || (dx == 1 && dy == 2)))
                return false;

            // Ensure destination is empty
            if (board.GetPiece(toX, toY) != null)
                return false;

            return true;
        }
    }

    public class Rook : Piece
    {
        public Rook(PlayerColor owner) : base(owner) { }
        public override PieceType GetPieceType()
        {
            return PieceType.Rook;
        }
        public override bool IsValidMove(int fromX, int fromY, int toX, int toY, IBoard board)
        {
            if (fromX != toX && fromY != toY) return false;

            int dx = toX > fromX ? 1 : (toX < fromX ? -1 : 0);
            int dy = toY > fromY ? 1 : (toY < fromY ? -1 : 0);

            int x = fromX + dx, y = fromY + dy;

            while (x != toX || y != toY)
            {
                if (board.GetPiece(x, y) != null) return false;
                x += dx;
                y += dy;
            }

            return board.GetPiece(toX, toY) == null;
        }
    }

    public class Bishop : Piece
    {
        public Bishop(PlayerColor owner) : base(owner) { }

        public override PieceType GetPieceType()
        {
            return PieceType.Bishop;
        }

        public override bool IsValidMove(int fromX, int fromY, int toX, int toY, IBoard board)
        {
            if (Math.Abs(toX - fromX) != Math.Abs(toY - fromY)) return false;

            int dx = toX > fromX ? 1 : -1;
            int dy = toY > fromY ? 1 : -1;

            int x = fromX + dx, y = fromY + dy;

            while (x != toX || y != toY)
            {
                if (board.GetPiece(x, y) != null) return false;
                x += dx;
                y += dy;
            }

            return board.GetPiece(toX, toY) == null;
        }
    }
}