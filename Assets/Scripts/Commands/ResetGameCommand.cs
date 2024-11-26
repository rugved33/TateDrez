using Game.Tatedrez.Model;

namespace Game.Tatedrez.Commands
{
    public class ResetGameCommand : ICommand
    {
        private readonly GameState gameState;

        public ResetGameCommand(GameState gameState)
        {
            this.gameState = gameState;
        }

        public bool Execute()
        {
            gameState.ResetGame();
            return true;
        }
    }
}
