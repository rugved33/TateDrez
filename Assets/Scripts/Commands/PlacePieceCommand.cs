using Game.Tatedrez.Model;

namespace Game.Tatedrez.Commands
{
    public class PlacePieceCommand : ICommand
    {
        private readonly GameState gameState;
        private readonly Piece piece;
        private readonly int x;
        private readonly int y;

        public PlacePieceCommand(GameState gameState, Piece piece, int x, int y)
        {
            this.gameState = gameState;
            this.piece = piece;
            this.x = x;
            this.y = y;
        }

        public bool Execute()
        {
            var currentPlayer = gameState.CurrentPlayer;
            var piecePlaceSuccessfully =  gameState.PlacePiece(piece, x, y);

            if(piecePlaceSuccessfully)
            {
                currentPlayer.DeductAvailablePiece(piece.GetPieceType());
            }
            return piecePlaceSuccessfully;
        }
    }
}
