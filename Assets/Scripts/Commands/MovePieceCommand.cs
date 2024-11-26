using Game.Tatedrez.Model;

namespace Game.Tatedrez.Commands
{
    public class MovePieceCommand : ICommand
    {
        private readonly GameState gameState;
        private readonly int fromX;
        private readonly int fromY;
        private readonly int toX;
        private readonly int toY;

        public MovePieceCommand(GameState gameState, int fromX, int fromY, int toX, int toY)
        {
            this.gameState = gameState;
            this.fromX = fromX;
            this.fromY = fromY;
            this.toX = toX;
            this.toY = toY;
        }

        public bool Execute()
        {
            return gameState.MovePiece(fromX, fromY, toX, toY);
        }
    }
}
